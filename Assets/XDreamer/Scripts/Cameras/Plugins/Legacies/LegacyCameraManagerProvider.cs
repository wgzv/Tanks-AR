using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Scripts;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCamera.LegacyCameras;
using XCSJ.Attributes;
using XCSJ.Helper;
using UnityEngine;
using XCSJ.PluginCommonUtils.Base.Kernel;

namespace XCSJ.PluginsCameras.Legacies
{
    /// <summary>
    /// 旧版相机管理器提供者
    /// </summary>
    [Name("旧版相机管理器提供者")]
    public class LegacyCameraManagerProvider : BaseLegacyCameraManagerProvider
    {
#pragma warning disable CS0618 // 类型或成员已过时

        /// <summary>
        /// 初始化相机
        /// </summary>
        [Name("初始相机")]
        [Tip("如果初始相机无效，会将全局查找到的第一个相机作为初始相机；")]
        public BaseCamera _initCamera;

        /// <summary>
        /// 初始相机
        /// </summary>
        public BaseCamera initCamera
        {
            get => _initCamera;
            set
            {
                this.XModifyProperty(() => _initCamera = value);
            }
        }

        [Name("仅初始相机激活")]
        [Tip("会将除初始相机以外的所有相机设置为非激活状态；")]
        public bool onlyInitCameraActive = true;

        private BaseCamera m_CurrentCamera;

        /// <summary>
        /// 当前相机
        /// </summary>
        public override BaseCamera currentCamera
        {
            get { return m_CurrentCamera; }
            set
            {
                onWillChangeCurrentCamera?.Invoke();
                if (m_CurrentCamera) m_CurrentCamera.isCurrentCamera = false;
                m_CurrentCamera = value;
                if (m_CurrentCamera) m_CurrentCamera.isCurrentCamera = true;
                onChangedCurrentCamera?.Invoke();
            }
        }

        /// <summary>
        /// 上一个相机
        /// </summary>
        public BaseCamera lastCamera { get; private set; }

        /// <summary>
        /// 目标相机
        /// </summary>
        public BaseCamera targetCamera { get; private set; }

        /// <summary>
        /// 过渡相机,相机在做转场时使用的临时相机
        /// </summary>
        public BaseCamera tempCamera { get; private set; }

        /// <summary>
        /// 切换相机时的补间对象
        /// </summary>
        public object switchTweener { get; private set; }

        /// <summary>
        /// 当将要切换当前相机之前的回调事件
        /// </summary>
        public static event Action onWillChangeCurrentCamera;

        /// <summary>
        /// 当已切换当前相机之后回调的事件
        /// </summary>
        public static event Action onChangedCurrentCamera;

        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="cameraManager"></param>
        public override void CMStart(CameraManager cameraManager)
        {
            CameraCache();
            var bcs = GetAllCameras();
            if (onlyInitCameraActive)
            {
                foreach (var bc in bcs) bc.SetCameraActive(false);
            }
            //如果初始相机无效，尝试使用第一个相机做初始相机
            if (!_initCamera && bcs.Length > 0) _initCamera = bcs[0];
            //如果初始相机仍然无效~报错
            if (_initCamera)
            {
                //部分信息初始化
                currentCamera = _initCamera;
                lastCamera = currentCamera;
                if (!currentCamera) Debug.LogWarning("初始相机未包含BaseCamera的子类组件!");
                else currentCamera.SetCameraActive(true);
            }
        }

        public void OnDestroy()
        {
            ClearCameraCache();
        }

        /// <summary>
        /// 尝试将相机设置为当前相机：如果已经存在当前相机，那么设置失败！
        /// </summary>
        /// <param name="bc"></param>
        public static void TrySetCurrentCamera(Component component)
        {
            var bc = component as BaseCamera;
            if (!bc) return;
            var manager = CameraManager.instance;
            if (manager && !manager.GetCurrentCamera())
            {
                manager.SetCurrentCamera(bc);
            }
        }

        public void SetInitCamera(BaseCamera bc)
        {
            if (!bc || currentCamera == bc) return;
            _initCamera = bc;
            currentCamera = bc;
        }

        public bool SwitchCamera(GameObject target, float duration, string funcCallback, bool mustSwitch)
        {
            if (!target) return false;
            return SwitchCamera(target.GetComponent<BaseCamera>(), duration, funcCallback, mustSwitch);
        }

        public bool SwitchCamera(BaseCamera target, float duration, string funcCallback, bool mustSwitch)
        {
            if (target == null || target == currentCamera) return false;

            //时间过短，立即切换，不再补间
            var directSwitch = duration < 0.01f;

            if (tempCamera)//已经有一个正在切换中 -- 肯定说明当前相机是有效的
            {
                //不强制切换，那么直接返回
                if (!mustSwitch) return false;

                //强制切换~移除当前正在执行的切换补间
                TweenHandler.Kill(switchTweener);

                //继续执行相机切换操作
            }
            else if (!currentCamera)//当前相机无效
            {
                //将补间时间设置为0，后续逻辑将执行立即切换
                duration = 0;
                directSwitch = true;
            }
            else if (!directSwitch)//需要补间，那么创建临时相机
            {
                //创建中间的过渡相机
                var go = Instantiate(currentCamera.gameObject, currentCamera.transform.position, currentCamera.transform.rotation) as GameObject;
                tempCamera = go.GetComponent<BaseCamera>();
                tempCamera.enabled = false;
                tempCamera.transform.parent = gameObject.transform;

                //将当前相机标记为上一次相机
                lastCamera = currentCamera;

                //将临时相机 设置为 当前相机，并添加必要的AudioListener组件
                currentCamera = tempCamera;
            }

            //设置新的目标相机
            targetCamera = target;

            if (directSwitch)
            {
                //直接执行完成后回调函数
                OnCompleteOfSwitchCamera(funcCallback);
            }
            else//需要补间
            {
                //执行补间切换
                switchTweener = TweenHandler.To(currentCamera.transform, target.transform.position, target.transform.rotation, duration, (o, os) =>
                {
                    if (os != null && os.Length > 0 && os[0] is string funcName)
                    {
                        OnCompleteOfSwitchCamera(funcName);
                    }
                }, funcCallback);
            }
            return true;
        }

        public bool SwitchCameraByName(string cameraName, float duration, string funcCallback, bool mustSwitch)
        {
            return SwitchCamera(GetCameraByName(cameraName), duration, funcCallback, mustSwitch);
        }

        private void OnCompleteOfSwitchCamera(string funcCallback)
        {
            //移除中间对象
            if (tempCamera) Destroy(tempCamera.gameObject);
            tempCamera = null;
            switchTweener = null;
            //将目标相机设置为当前相机
            currentCamera = targetCamera;
            //执行事件回调
            ScriptManager.CallUDF(funcCallback);
        }

        /// <summary>
        /// 根据名称查找相机，全局范围内查找
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BaseCamera GetCameraByName(string name)
        {
            foreach (var bc in GetAllCameras())
            {
                if (bc && bc.name == name) return bc;
            }
            return null;
        }

        /// <summary>
        /// 设置当前相机移动速度
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        public bool SetCurrentCameraMoveSpeed(float speed)
        {
            return SetCameraMoveSpeed(currentCamera, speed);
        }

        /// <summary>
        /// 设置指定相机的移动速度
        /// </summary>
        /// <param name="targetCamera"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public bool SetCameraMoveSpeed(GameObject targetCamera, float speed)
        {
            if (!targetCamera) return false;
            return SetCameraMoveSpeed(targetCamera.GetComponent<BaseCamera>(), speed);
        }

        /// <summary>
        /// 设置相机移动速度
        /// </summary>
        /// <param name="bc">相机基类</param>
        /// <param name="speed">速度</param>
        /// <returns></returns>
        public bool SetCameraMoveSpeed(BaseCamera bc, float speed)
        {
            if (!bc) return false;
            bc.moveSpeed.baseValue = speed;
            return true;
        }

        /// <summary>
        /// 设置当前相机旋转速度
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        public bool SetCurrentCameraRotateSpeed(float speed)
        {
            return SetCameraRotateSpeed(currentCamera, speed);
        }

        /// <summary>
        /// 设置指定相机的旋转速度
        /// </summary>
        /// <param name="targetCamera"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public bool SetCameraRotateSpeed(GameObject targetCamera, float speed)
        {
            if (!targetCamera) return false;
            return SetCameraRotateSpeed(targetCamera.GetComponent<BaseCamera>(), speed);
        }

        /// <summary>
        /// 设置相机旋转速度
        /// </summary>
        /// <param name="bc">相机基类</param>
        /// <param name="speed">速度</param>
        /// <returns></returns>
        public bool SetCameraRotateSpeed(BaseCamera bc, float speed)
        {
            if (!bc) return false;
            bc.rotateSpeed.baseValue = speed;
            return true;
        }

        /// <summary>
        /// 修改当前相机的近裁截面
        /// </summary>
        /// <param name="nearClipPlane"></param>
        /// <returns></returns>
        public bool SetCurrentCameraNearClipPlane(float nearClipPlane)
        {
            return SetCameraNearClipPlane(currentCamera, nearClipPlane);
        }

        /// <summary>
        /// 修改指定相机的近裁截面
        /// </summary>
        /// <param name="targetCamera"></param>
        /// <param name="nearClipPlane"></param>
        /// <returns></returns>
        public bool SetCameraNearClipPlane(GameObject targetCamera, float nearClipPlane)
        {
            if (!targetCamera) return false;
            return SetCameraNearClipPlane(targetCamera.GetComponent<BaseCamera>(), nearClipPlane);
        }

        public bool SetCameraNearClipPlane(BaseCamera bc, float nearClipPlane)
        {
            if (!bc) return false;
            bc.nearClipPlane = nearClipPlane;
            return true;
        }

        /// <summary>
        /// 修改当前相机的远裁截面
        /// </summary>
        /// <param name="farClipPlane">设定的远裁截面的值</param>
        /// <returns></returns>
        public bool SetCurrentCameraFarClipPlane(float farClipPlane)
        {
            return SetCameraFarClipPlane(currentCamera, farClipPlane);
        }

        /// <summary>
        /// 修改指定相机的远裁截面
        /// </summary>
        /// <param name="targetCamera">目标相机</param>
        /// <param name="farClipPlane">设定的远裁截面的值</param>
        /// <returns></returns>
        public bool SetCameraFarClipPlane(GameObject targetCamera, float farClipPlane)
        {
            if (!targetCamera) return false;
            return SetCameraFarClipPlane(targetCamera.GetComponent<BaseCamera>(), farClipPlane);
        }

        public bool SetCameraFarClipPlane(BaseCamera bc, float farClipPlane)
        {
            if (!bc) return false;
            bc.farClipPlane = farClipPlane;
            return true;
        }

        /// <summary>
        /// 修改当前相机的视角的缩放系数
        /// </summary>
        /// <param name="fieldOfView">设定的远裁截面的值</param>
        /// <returns></returns>
        public bool SetCurrentCameraFieldOfView(float fieldOfView)
        {
            return SetCameraFieldOfView(currentCamera, fieldOfView);
        }

        /// <summary>
        /// 修改指定相机的视角的缩放系数
        /// </summary>
        /// <param name="targetCamera">目标相机</param>
        /// <param name="fieldOfView">设定的远裁截面的值</param>
        /// <returns></returns>
        public bool SetCameraFieldOfView(GameObject targetCamera, float fieldOfView)
        {
            if (!targetCamera) return false;
            return SetCameraFieldOfView(targetCamera.GetComponent<BaseCamera>(), fieldOfView);
        }

        public bool SetCameraFieldOfView(BaseCamera bc, float fieldOfView)
        {
            if (!bc) return false;
            bc.fieldOfView = fieldOfView;
            return true;
        }

        public bool MoveCamera(GameObject go, Vector3 dir)
        {
            return MoveCamera(go.GetComponent<BaseCamera>(), dir);
        }

        public bool MoveCamera(BaseCamera bc, Vector3 dir)
        {
            if (!bc) return false;
            bc.Move(dir);
            return true;
        }

        public void ResetCurrentCamera(float duration, string fun = "", string param = "")
        {
            ResetCamera(currentCamera, duration, fun, param);
        }

        public void ResetCamera(GameObject go, float duration, string fun = "", string param = "")
        {
            if (!go) return;
            ResetCamera(go.GetComponent<BaseCamera>(), duration, fun, param);
        }

        public void ResetCamera(BaseCamera bc, float duration, string fun = "", string param = "")
        {
            if (!bc) return;
            bc.ResetCamera(duration, fun, param);
        }

        public string GetNewCameraDefaultName(string defaultName)
        {
            string baseName = StringHelper.StringTransform(defaultName);
            string retName;
            int i = 0;
            do
            {
                retName = string.Format("{0} ({1})", baseName, i++);
            } while (transform.Find(retName));
            return retName;
        }

        public BaseCamera CreateCamera<T>() where T : BaseCamera
        {
            return CreateCamera(typeof(T));
        }

        public BaseCamera CreateCamera(Type t)
        {
            //确保是 BaseCamera 的子类
            if (t == null || !t.IsSubclassOf(typeof(BaseCamera))) return null;
            GameObject camera = UnityObjectHelper.CreateGameObject(GetNewCameraDefaultName(CommonFun.Name(t)));
            camera.XSetParent(transform);
            camera.XModifyProperty(() =>
            {
                camera.tag = CameraHelperExtension.DefaultMainCameraTag;
            });
            BaseCamera bc = camera.XAddComponent(t) as BaseCamera;
            if (bc)
            {
                if (currentCamera && currentCamera != bc)
                {
                    bc.XModifyProperty(() => { bc.SetCameraActive(false); });                    
                }
                else
                {
                    currentCamera = bc;
                    lastCamera = currentCamera;
                }
                if (!initCamera)
                {
                    initCamera = bc;
                    CameraHelperExtension.HandleDefaultMainCamera();
                }
            }

            return bc;
        }

        public Type GetCameraType(string typeName)
        {
            foreach (var t in TypeHelper.FindTypeInAppWithClass(typeof(BaseCamera)))
            {
                if (t.Name == typeName) return t;
            }
            return null;
        }

        /// <summary>
        /// 程序运行期间使用的相机
        /// </summary>
        internal static HashSet<BaseCamera> cameras = new HashSet<BaseCamera>();

        internal static void ClearCameraCache()
        {
            cameras.Clear();
        }

        private void CameraCache()
        {
            try
            {
                ClearCameraCache();
                foreach (var bc in CommonFun.GetComponentsInChildren<BaseCamera>(true))
                {
                    cameras.Add(bc);
                }
            }
            catch { ClearCameraCache(); }
        }

        /// <summary>
        /// 当前场景的所有相机，包括不再相机管理类下的相机,该相机组件依赖的游戏对象可能处于非激活状态；
        /// </summary>
        /// <returns></returns>
        public BaseCamera[] GetAllCameras()
        {
            if (cameras.Count == 0) CameraCache();
            return cameras.ToArray();
        }

        public List<string> GetCameras()
        {
            List<string> ret = new List<string>();
            foreach (var bc in GetAllCameras())
            {
                ret.Add(bc.name);
            }
            return ret;
        }

#pragma warning restore CS0618 // 类型或成员已过时
    }
}
