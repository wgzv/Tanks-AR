using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.PluginCamera;
using XCSJ.PluginCamera.Cameras;
using XCSJ.PluginCamera.Kernel;
using XCSJ.PluginCamera.LegacyCameras;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Legacies;
using XCSJ.PluginsCameras.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.PluginsCameras.Kernel
{
    /// <summary>
    /// 默认相机处理器
    /// </summary>
    public class DefaultCameraHandler : InstanceClass<DefaultCameraHandler>, ICameraHandler
    {
        /// <summary>
        /// 初始化
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            CameraHandler.handler = instance;
        }

        /// <summary>
        /// 获取脚本
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public List<Script> GetScripts(CameraManager manager) => Script.GetScriptsOfEnum<ECameraScriptID>(manager);

        /// <summary>
        /// 运行脚本
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public ReturnValue RunScript(CameraManager manager, int id, ScriptParamList param)
        {
            switch ((ECameraScriptID)id)
            {
                #region 旧版相机

#pragma warning disable CS0618 // 类型或成员已过时

                case ECameraScriptID.SwitchCamera:
                    {
                        var go = param[1] as GameObject;
                        float duration = (float)param[2];
                        string funcCallback = param[3] as string;
                        return ReturnValue.Create(manager.SwitchCamera(go, duration, funcCallback, ((EBool)(param[4])) == EBool.Yes));
                    }
                case ECameraScriptID.SwtichCameraByName:
                    {
                        //Log.Debug(param);
                        string name = param[1] as string;
                        float duration = (float)param[2];
                        string funcCallback = param[3] as string;
                        return ReturnValue.Create(manager.SwitchCameraByName(name, duration, funcCallback, ((EBool)(param[4])) == EBool.Yes));
                    }
                case ECameraScriptID.SetCameraSpeed:
                    {
                        var speedType = param[3] as string;
                        if (string.IsNullOrEmpty(speedType) || speedType == "移动")
                        {
                            return ReturnValue.Create(manager.SetCameraMoveSpeed(param[1] as GameObject, (float)param[2]));
                        }
                        else if (speedType == "旋转")
                        {
                            return ReturnValue.Create(manager.SetCameraRotateSpeed(param[1] as GameObject, (float)param[2]));
                        }
                        else
                        {
                            return ReturnValue.False();
                        }
                    }
                case ECameraScriptID.SetCurrentCameraSpeed:
                    {
                        var speedType = param[3] as string;
                        if (string.IsNullOrEmpty(speedType) || speedType == "移动")
                        {
                            return ReturnValue.Create(manager.SetCurrentCameraMoveSpeed((float)param[2]));
                        }
                        else if (speedType == "旋转")
                        {
                            return ReturnValue.Create(manager.SetCurrentCameraRotateSpeed((float)param[2]));
                        }
                        else
                        {
                            return ReturnValue.False();
                        }
                    }
                case ECameraScriptID.SetCameraNearClipPlane:
                    {
                        return ReturnValue.Create(manager.SetCameraNearClipPlane(param[1] as GameObject, (float)param[2]));
                    }
                case ECameraScriptID.SetCurrentCameraNearClipPlane:
                    {
                        return ReturnValue.Create(manager.SetCurrentCameraNearClipPlane((float)param[2]));
                    }
                case ECameraScriptID.SetCameraFarClipPlane:
                    {
                        return ReturnValue.Create(manager.SetCameraFarClipPlane(param[1] as GameObject, (float)param[2]));
                    }
                case ECameraScriptID.SetCurrentCameraFarClipPlane:
                    {
                        return ReturnValue.Create(manager.SetCurrentCameraFarClipPlane((float)param[2]));
                    }
                case ECameraScriptID.SetCameraFieldOfView:
                    {
                        return ReturnValue.Create(manager.SetCameraFieldOfView(param[1] as GameObject, (int)param[2]));
                    }
                case ECameraScriptID.SetCurrentCameraFieldOfView:
                    {
                        return ReturnValue.Create(manager.SetCurrentCameraFieldOfView((int)param[2]));
                    }
                case ECameraScriptID.SetCameraMove:
                    {
                        bool ret = false;
                        float dis = (float)param[3];
                        switch (param[2] as string)
                        {
                            case "前进":
                                {
                                    ret = manager.MoveCamera(param[1] as GameObject, dis * Vector3.forward);
                                    break;
                                }
                            case "后退":
                                {
                                    ret = manager.MoveCamera(param[1] as GameObject, dis * Vector3.back);
                                    break;
                                }
                            case "左移":
                                {
                                    ret = manager.MoveCamera(param[1] as GameObject, dis * Vector3.left);
                                    break;
                                }
                            case "右移":
                                {
                                    ret = manager.MoveCamera(param[1] as GameObject, dis * Vector3.right);
                                    break;
                                }
                            case "上移":
                                {
                                    ret = manager.MoveCamera(param[1] as GameObject, dis * Vector3.up);
                                    break;
                                }
                            case "下移":
                                {
                                    ret = manager.MoveCamera(param[1] as GameObject, dis * Vector3.down);
                                    break;
                                }
                        }
                        return ReturnValue.Create(ret);
                    }
                case ECameraScriptID.SetCurrentCameraMove:
                    {
                        #region SetCurrentCameraMove
                        var currentCamera = manager.GetCurrentCamera();
                        bool ret = false;
                        float dis = (float)param[3];
                        switch (param[2] as string)
                        {
                            case "前进":
                                {
                                    ret = manager.MoveCamera(currentCamera, dis * Vector3.forward);
                                    break;
                                }
                            case "后退":
                                {
                                    ret = manager.MoveCamera(currentCamera, dis * Vector3.back);
                                    break;
                                }
                            case "左移":
                                {
                                    ret = manager.MoveCamera(currentCamera, dis * Vector3.left);
                                    break;
                                }
                            case "右移":
                                {
                                    ret = manager.MoveCamera(currentCamera, dis * Vector3.right);
                                    break;
                                }
                            case "上移":
                                {
                                    ret = manager.MoveCamera(currentCamera, dis * Vector3.up);
                                    break;
                                }
                            case "下移":
                                {
                                    ret = manager.MoveCamera(currentCamera, dis * Vector3.down);
                                    break;
                                }
                        }
                        return ReturnValue.Create(ret);
                        #endregion
                    }
                case ECameraScriptID.SetCameraRotate:
                    {
                        var go = param[1] as GameObject;
                        if (!go) break;
                        var bc = go.GetComponent<BaseCamera>();
                        if (!bc) break;
                        float rotateAngle = (float)param[3];
                        switch (param[2] as string)
                        {
                            case "X轴":
                                {
                                    bc.Rotate(new Vector3(rotateAngle, 0, 0));
                                    return ReturnValue.Yes;
                                }
                            case "Y轴":
                                {
                                    bc.Rotate(new Vector3(0, rotateAngle, 0));
                                    return ReturnValue.Yes;
                                }
                        }
                        break;
                    }
                case ECameraScriptID.SetCurrentCameraRotate:
                    {
                        if (!manager.GetCurrentCamera()) break;
                        float rotateAngle = (float)param[3];
                        switch (param[2] as string)
                        {
                            case "X轴":
                                {
                                    manager.GetCurrentCamera().Rotate(new Vector3(rotateAngle, 0, 0));
                                    return ReturnValue.Yes;
                                }
                            case "Y轴":
                                {
                                    manager.GetCurrentCamera().Rotate(new Vector3(0, rotateAngle, 0));
                                    return ReturnValue.Yes;
                                }
                        }
                        break;
                    }
                case ECameraScriptID.SetCameraTargetRotate:
                    {
                        var go = param[1] as GameObject;
                        if (!go) break;
                        var bc = go.GetComponent<BaseCamera>();
                        if (!bc) break;
                        float rotateAngle = (float)param[3];
                        switch (param[2] as string)
                        {
                            case "X轴":
                                {
                                    bc.TargetRotate(new Vector3(rotateAngle, 0, 0));
                                    return ReturnValue.Yes;
                                }
                            case "Y轴":
                                {
                                    bc.TargetRotate(new Vector3(0, rotateAngle, 0));
                                    return ReturnValue.Yes;
                                }
                        }
                        break;
                    }
                case ECameraScriptID.SetCurrentCameraTargetRotate:
                    {
                        if (!manager.GetCurrentCamera()) break;
                        float rotateAngle = (float)param[3];
                        switch (param[2] as string)
                        {
                            case "X轴":
                                {
                                    manager.GetCurrentCamera().TargetRotate(new Vector3(rotateAngle, 0, 0));
                                    return ReturnValue.Yes;
                                }
                            case "Y轴":
                                {
                                    manager.GetCurrentCamera().TargetRotate(new Vector3(0, rotateAngle, 0));
                                    return ReturnValue.Yes;
                                }
                        }
                        break;
                    }
                case ECameraScriptID.ResetCurrentCamera:
                    {
                        manager.ResetCurrentCamera((float)param[2], param[3] as string, param[4] as string);
                        return ReturnValue.Yes;
                    }
                case ECameraScriptID.LockCurrentCamera:
                    {
                        if (!manager.GetCurrentCamera()) break;
                        manager.GetCurrentCamera().lockCamera = CommonFun.BoolChange(manager.GetCurrentCamera().lockCamera, (EBool)param[2]);
                        return ReturnValue.No;
                    }
                case ECameraScriptID.GetCurrentCameraInfo:
                    {
                        if (!manager.GetCurrentCamera()) break;
                        switch (param[2] as string)
                        {
                            case "游戏对象":
                                {
                                    return ReturnValue.True(CommonFun.GameObjectToString(manager.GetCurrentCamera().gameObject));
                                }
                            case "游戏对象组件":
                                {
                                    return ReturnValue.True(CommonFun.GameObjectComponentToString(manager.GetCurrentCamera()));
                                }
                            case "组件类型":
                                {
                                    return ReturnValue.True(CommonFun.ComponentTypeToString(manager.GetCurrentCamera().GetType()));
                                }
                            case "目标游戏对象":
                                {
                                    return ReturnValue.True(CommonFun.GameObjectToString(manager.GetCurrentCamera().GetTarget()));
                                }
                        }
                        break;
                    }
                case ECameraScriptID.OperatorCurrentCamera:
                    {
                        if (!manager.GetCurrentCamera()) break;
                        switch (param[2] as string)
                        {
                            case "刷新信息":
                                {
                                    manager.GetCurrentCamera().RefreshInfo();
                                    break;
                                }
                            case "锁定":
                                {
                                    manager.GetCurrentCamera().lockCamera = true;
                                    break;
                                }
                            case "解除锁定":
                                {
                                    manager.GetCurrentCamera().lockCamera = false;
                                    break;
                                }
                            case "跳跃":
                                {
                                    manager.GetCurrentCamera().Jump();
                                    break;
                                }
                            case "显示速度信息":
                                {
                                    manager.GetCurrentCamera().ShowCameraInfo();
                                    break;
                                }
                            case "移动速度自动增加":
                                {
                                    manager.GetCurrentCamera().AutoSetMoveSpeed(true);
                                    break;
                                }
                            case "移动速度自动减少":
                                {
                                    manager.GetCurrentCamera().AutoSetMoveSpeed(false);
                                    break;
                                }
                            case "旋转速度自动增加":
                                {
                                    manager.GetCurrentCamera().AutoSetRotateSpeed(true);
                                    break;
                                }
                            case "旋转速度自动减少":
                                {
                                    manager.GetCurrentCamera().AutoSetRotateSpeed(false);
                                    break;
                                }
                            default:
                                {
                                    return ReturnValue.No;
                                }
                        }
                        return ReturnValue.Yes;
                    }
                case ECameraScriptID.SetCameraTargetGameObject:
                    {
                        var go = param[1] as GameObject;
                        if (!go) break;
                        var bc = go.GetComponent<BaseCamera>();
                        if (!bc) break;
                        bc.SetTarget(param[2] as GameObject);
                        return ReturnValue.Yes;
                    }
                case ECameraScriptID.SetCurrentCameraTargetGameObject:
                    {
                        if (!manager.GetCurrentCamera()) break;
                        manager.GetCurrentCamera().SetTarget(param[2] as GameObject);
                        return ReturnValue.Yes;
                    }
                case ECameraScriptID.CameraFocusTarget:
                    {
                        var go = param[1] as GameObject;
                        if (go) return ReturnValue.Create(go.GetComponent<BaseCamera>().FocusTarget(param[3] as GameObject, (EBoundsAnchor)param[2], (float)param[4]));

                        if (!manager.GetCurrentCamera()) break;
                        return ReturnValue.Create(manager.GetCurrentCamera().FocusTarget(param[3] as GameObject, (EBoundsAnchor)param[2], (float)param[4]));
                    }
                case ECameraScriptID.CameraFocusSelection:
                    {
                        var go = param[1] as GameObject;
                        if (go) return ReturnValue.Create(go.GetComponent<BaseCamera>().FocusTarget(Selection.selection, (EBoundsAnchor)param[2], (float)param[4]));

                        if (!manager.GetCurrentCamera()) break;
                        return ReturnValue.Create(manager.GetCurrentCamera().FocusTarget(Selection.selection, (EBoundsAnchor)param[2], (float)param[4]));
                    }
                case ECameraScriptID.SetCameraViewportRectangle:
                    {
                        Camera cam = param[1] as Camera;
                        if (cam != null)
                        {
                            cam.rect = (Rect)param[2];
                            return new ReturnValue(true);
                        }
                        break;
                    }

#pragma warning restore CS0618 // 类型或成员已过时

                #endregion

                #region 新版相机

                case ECameraScriptID.SwitchCameraController:
                    {
                        var go = param[1] as GameObject;
                        float duration = (float)param[2];
                        string funcCallback = param[3] as string;
                        System.Action onComplete = () => ScriptManager.CallUDF(funcCallback);
                        var muse = ((EBool)(param[4])) == EBool.Yes;
                        var switchRule = param[5] as string;
                        bool returnValue = false;
                        switch (switchRule)
                        {
                            case "使用目标相机控制器对象":
                                {
                                    returnValue = manager.SwitchCameraController(go, duration, onComplete, muse);
                                    break;
                                }
                            case "上一个相机":
                                {
                                    returnValue = CameraHelperExtension.SwitchPreviousCamera(duration, onComplete, muse);
                                    break;
                                }
                            case "下一个相机":
                                {
                                    returnValue = CameraHelperExtension.SwitchNextCamera(duration, onComplete, muse);
                                    break;
                                }
                        }
                        return returnValue ? ReturnValue.Yes : ReturnValue.No;
                    }
                case ECameraScriptID.SetCameraControllerMainTarget:
                    {
                        var cameraController = GetCameraController(manager, param[10] as string, param[1] as GameObject);
                        if (!cameraController) break;
                        var mainTarget = param[2] as GameObject;
                        if (!mainTarget) break;
                        cameraController.mainTarget = mainTarget.transform;
                        return ReturnValue.Yes;
                    }
                case ECameraScriptID.SetCameraControllerSpeedCoefficient:
                    {
                        var cameraController = GetCameraController(manager, param[10] as string, param[1] as GameObject);
                        if (!cameraController) break;
                        switch (param[3] as string)
                        {
                            case "移动":
                                {
                                    cameraController.moveSpeedCoefficient = (Vector3)param[2];
                                    return ReturnValue.Yes;
                                }
                            case "旋转":
                                {
                                    cameraController.rotateSpeedCoefficient = (Vector3)param[2];
                                    return ReturnValue.Yes;
                                }
                        }
                        break;
                    }
                case ECameraScriptID.SetCameraControllerDampingCoefficient:
                    {
                        var cameraController = GetCameraController(manager, param[10] as string, param[1] as GameObject);
                        if (!cameraController) break;
                        switch (param[3] as string)
                        {
                            case "移动":
                                {
                                    cameraController.moveDampingCoefficient = (float)param[2];
                                    return ReturnValue.Yes;
                                }
                            case "旋转":
                                {
                                    cameraController.rotateDampingCoefficient = (float)param[2];
                                    return ReturnValue.Yes;
                                }
                        }
                        break;
                    }
                case ECameraScriptID.RecoverCameraControllerState:
                    {
                        var cameraController = GetCameraController(manager, param[10] as string, param[1] as GameObject);
                        if (!cameraController) break;
                        switch (param[2] as string)
                        {
                            case "原始状态":
                                {
                                    cameraController.cameraTransformer.RecoverToOriginal();
                                    return ReturnValue.Yes;
                                }
                            case "上一次状态":
                                {
                                    cameraController.cameraTransformer.RecoverToLast();
                                    return ReturnValue.Yes;
                                }
                        }
                        break;
                    }

                    #endregion
            }
            return ReturnValue.No;
        }

        private BaseCameraMainController GetCameraController(CameraManager manager, string rule,GameObject gameObject)
        {
            switch(rule)
            {
                case "指定":
                    {
                        if (!gameObject) break;
                        return gameObject.GetComponent<BaseCameraMainController>();
                    }
                case "当前":
                    {
                        return manager.GetProvider().currentCameraController;
                    }
            }
            return default;
        }

        /// <summary>
        /// 获取旧版相机管理器提供者
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public BaseLegacyCameraManagerProvider GetLegacyCameraManagerProvider(CameraManager manager)
        {
            return manager.XAddComponent<LegacyCameraManagerProvider>();
        }

        /// <summary>
        /// 获取相机管理器提供者
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public BaseCameraManagerProvider GetCameraManagerProvider(CameraManager manager)
        {
            return manager.XAddComponent<CameraManagerProvider>();
        }

        /// <summary>
        /// 尝试设置为当前相机
        /// </summary>
        /// <param name="component"></param>
        public void TrySetCurrentCamera(Component component)
        {
            LegacyCameraManagerProvider.TrySetCurrentCamera(component);
        }

        /// <summary>
        /// 清空相机缓存
        /// </summary>
        public void ClearCameraCache()
        {
            LegacyCameraManagerProvider.ClearCameraCache();
        }
    }
}
