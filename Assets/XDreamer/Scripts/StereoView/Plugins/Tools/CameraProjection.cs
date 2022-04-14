using UnityEngine;
using System.Collections;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginsCameras;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Tweens;

namespace XCSJ.PluginStereoView.Tools
{
    /// <summary>
    /// 相机透视:根据屏幕与相机位置实时更新相机透视矩阵的工具组件
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [Name(Title)]
    [Tip("根据屏幕与相机位置实时更新相机透视矩阵的工具组件")]
    [RequireManager(typeof(StereoViewManager), typeof(CameraManager))]
    [DisallowMultipleComponent]
    public class CameraProjection : MB
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "相机透视";

        /// <summary>
        /// 屏幕
        /// </summary>
        [Name("屏幕")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public VirtualScreen _screen;

        /// <summary>
        /// 屏幕
        /// </summary>
        public VirtualScreen screen
        {
            get => this.XGetComponentInChildrenOrGlobal(ref _screen);
            set => this.XModifyProperty(ref _screen, value);
        }

        /// <summary>
        /// 预估视锥:使相机总是朝向屏幕中心，并且根据相机位置与屏幕纵横比动态调整相机的FOV；
        /// </summary>
        [Name("预估视锥")]
        [Tip("使相机总是朝向屏幕中心，并且根据相机位置与屏幕纵横比动态调整相机的FOV；")]
        public bool _estimateViewFrustum = true;

        /// <summary>
        /// 禁用时设置矩阵规则
        /// </summary>
        public enum ESetMatrixRuleOnDisable
        {
            /// <summary>
            /// 无:不做任何处理
            /// </summary>
            [Name("无")]
            [Tip("不做任何处理")]
            None,

            /// <summary>
            /// 重置:重置所有的矩阵信息；
            /// </summary>
            [Name("重置")]
            [Tip("重置所有的矩阵信息；")]
            Reset,

            /// <summary>
            /// 到启用:设置为启用时记录的矩阵信息；
            /// </summary>
            [Name("到启用")]
            [Tip("设置为启用时记录的矩阵信息；")]
            ToEnable,
        }

        /// <summary>
        /// 禁用时设置矩阵规则
        /// </summary>
        [Name("禁用时设置矩阵规则")]
        [EnumPopup]
        public ESetMatrixRuleOnDisable _setMatrixRuleOnDisable = ESetMatrixRuleOnDisable.Reset;

        /// <summary>
        /// 屏幕尺寸
        /// </summary>
        [Name("屏幕尺寸")]
        public Vector2 screenSize
        {
            get => _screen.screenSize;
            set => _screen.screenSize = value;
        }

        /// <summary>
        /// 半屏幕尺寸
        /// </summary>
        [Name("半屏幕尺寸")]
        public Vector2 halfScreenSize => _screen.screenSize * 0.5f;

        /// <summary>
        /// 当前相机：与当前组件在同一游戏对象上
        /// </summary>
        private Camera thisCamera;

        /// <summary>
        /// 当前变换：当前组件（相机）所在游戏对象的变换组件
        /// </summary>
        private Transform thisTransform;

        /// <summary>
        /// 屏幕左下角世界坐标
        /// </summary>
        Vector3 pa;

        /// <summary>
        /// 屏幕右下角世界坐标
        /// </summary>
        Vector3 pb;

        /// <summary>
        /// 屏幕左上角世界坐标
        /// </summary>
        Vector3 pc;

        /// <summary>
        /// 近裁剪
        /// </summary>
        float n;

        /// <summary>
        /// 远裁剪
        /// </summary>
        float f;

        /// <summary>
        /// 唤醒初始化
        /// </summary>
        public void Awake()
        {
            thisTransform = this.transform;
            thisCamera = GetComponent<Camera>();
        }

        private Matrix4x4 projectionMatrixBak;
        private Matrix4x4 worldToCameraMatrixBak;
        private float fieldOfViewBak;
        private Quaternion rotationBak;

        /// <summary>
        /// 启动
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            if (!screen)
            {
                Debug.LogWarningFormat("当前场景无有效屏幕,游戏对象[{0}]上功能组件[{1}]({2})无法有效工作！", CommonFun.GameObjectToStringWithoutAlias(gameObject), CommonFun.Name(GetType()), GetType().Name);
            }
            if (thisCamera)
            {
                projectionMatrixBak = thisCamera.projectionMatrix;
                worldToCameraMatrixBak = thisCamera.worldToCameraMatrix;
                fieldOfViewBak = thisCamera.fieldOfView;
                rotationBak = thisCamera.transform.rotation;
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            switch(_setMatrixRuleOnDisable)
            {
                case ESetMatrixRuleOnDisable.ToEnable:
                    {
                        if (thisCamera)
                        {
                            thisCamera.projectionMatrix = projectionMatrixBak;
                            thisCamera.worldToCameraMatrix = worldToCameraMatrixBak;
                            thisCamera.fieldOfView = fieldOfViewBak;
                            thisCamera.transform.rotation = rotationBak;
                        }
                        break;
                    }
                case ESetMatrixRuleOnDisable.Reset:
                    {
                        if (thisCamera)
                        {
                            thisCamera.ResetProjectionMatrix();
                            thisCamera.ResetWorldToCameraMatrix();
                            thisCamera.fieldOfView = 60;
                            thisCamera.transform.rotation = Quaternion.identity;
                        }
                        break;
                    }
            }
            
        }

        /// <summary>
        /// 延后更新
        /// </summary>
        public void LateUpdate()
        {
            if (!_screen || !thisCamera) return;
            {
                var halfScreenSize = this.halfScreenSize;
                var halfWidth = halfScreenSize.x;
                var halfHeight = halfScreenSize.y;

                //如果屏幕尺寸近似为0，不做处理
                if (Mathf.Approximately(halfWidth, 0) || Mathf.Approximately(halfHeight, 0)) return;

                var screenTransform = _screen.transform;
                var mat = Matrix4x4.TRS(screenTransform.position, screenTransform.rotation, Vector3.one);

                //屏幕左下角世界坐标
                //pa = screenTransform.TransformPoint(new Vector3(-halfWidth, -halfHeight, 0.0f));
                pa = mat.MultiplyPoint(new Vector3(-halfWidth, -halfHeight, 0.0f));

                //屏幕右下角世界坐标
                //pb = screenTransform.TransformPoint(new Vector3(halfWidth, -halfHeight, 0.0f));
                pb = mat.MultiplyPoint(new Vector3(halfWidth, -halfHeight, 0.0f));

                //屏幕左上角世界坐标
                //pc = screenTransform.TransformPoint(new Vector3(-halfWidth, halfHeight, 0.0f));
                pc = mat.MultiplyPoint(new Vector3(-halfWidth, halfHeight, 0.0f));

                //近裁剪
                n = thisCamera.nearClipPlane;

                //远裁剪
                f = thisCamera.farClipPlane;
            }

            {
                //相机位置-眼睛位置
                Vector3 pe = thisTransform.position;

                Vector3 va; // from pe to pa
                Vector3 vb; // from pe to pb 
                Vector3 vc; // from pe to pc
                Vector3 vr; // right axis of screen
                Vector3 vu; // up axis of screen
                Vector3 vn; // normal vector of screen

                float l; // distance to left screen edge
                float r; // distance to right screen edge
                float b; // distance to bottom screen edge
                float t; // distance to top screen edge
                float d; // distance from eye to screen 

                vr = pb - pa;
                vu = pc - pa;
                vr.Normalize();
                vu.Normalize();
                vn = -Vector3.Cross(vr, vu);
                // we need the minus sign because Unity 
                // uses a left-handed coordinate system
                vn.Normalize();

                va = pa - pe;
                vb = pb - pe;
                vc = pc - pe;

                d = -Vector3.Dot(va, vn);
                l = Vector3.Dot(vr, va) * n / d;
                r = Vector3.Dot(vr, vb) * n / d;
                b = Vector3.Dot(vu, va) * n / d;
                t = Vector3.Dot(vu, vc) * n / d;

                Matrix4x4 p = new Matrix4x4(); // projection matrix 
                p[0, 0] = 2.0f * n / (r - l);
                p[0, 1] = 0.0f;
                p[0, 2] = (r + l) / (r - l);
                p[0, 3] = 0.0f;

                p[1, 0] = 0.0f;
                p[1, 1] = 2.0f * n / (t - b);
                p[1, 2] = (t + b) / (t - b);
                p[1, 3] = 0.0f;

                p[2, 0] = 0.0f;
                p[2, 1] = 0.0f;
                p[2, 2] = (f + n) / (n - f);
                p[2, 3] = 2.0f * f * n / (n - f);

                p[3, 0] = 0.0f;
                p[3, 1] = 0.0f;
                p[3, 2] = -1.0f;
                p[3, 3] = 0.0f;

                Matrix4x4 rm = new Matrix4x4(); // rotation matrix;
                rm[0, 0] = vr.x;
                rm[0, 1] = vr.y;
                rm[0, 2] = vr.z;
                rm[0, 3] = 0.0f;

                rm[1, 0] = vu.x;
                rm[1, 1] = vu.y;
                rm[1, 2] = vu.z;
                rm[1, 3] = 0.0f;

                rm[2, 0] = vn.x;
                rm[2, 1] = vn.y;
                rm[2, 2] = vn.z;
                rm[2, 3] = 0.0f;

                rm[3, 0] = 0.0f;
                rm[3, 1] = 0.0f;
                rm[3, 2] = 0.0f;
                rm[3, 3] = 1.0f;

                Matrix4x4 tm = new Matrix4x4(); // translation matrix;
                tm[0, 0] = 1.0f;
                tm[0, 1] = 0.0f;
                tm[0, 2] = 0.0f;
                tm[0, 3] = -pe.x;

                tm[1, 0] = 0.0f;
                tm[1, 1] = 1.0f;
                tm[1, 2] = 0.0f;
                tm[1, 3] = -pe.y;

                tm[2, 0] = 0.0f;
                tm[2, 1] = 0.0f;
                tm[2, 2] = 1.0f;
                tm[2, 3] = -pe.z;

                tm[3, 0] = 0.0f;
                tm[3, 1] = 0.0f;
                tm[3, 2] = 0.0f;
                tm[3, 3] = 1.0f;

                // set matrices
                thisCamera.projectionMatrix = p;
                thisCamera.worldToCameraMatrix = rm * tm;
                // The original paper puts everything into the projection 
                // matrix (i.e. sets it to p * rm * tm and the other 
                // matrix to the identity), but this doesn't appear to 
                // work with Unity's shadow maps.

                if (_estimateViewFrustum)
                {
                    // rotate camera to screen for culling to work
                    Quaternion q = new Quaternion();
                    q.SetLookRotation((0.5f * (pb + pc) - pe), vu);
                    // look at center of screen
                    thisTransform.rotation = q;

                    // set fieldOfView to a conservative estimate 
                    // to make frustum tall enough
                    if (thisCamera.aspect >= 1.0)
                    {
                        thisCamera.fieldOfView = Mathf.Rad2Deg *
                           Mathf.Atan(((pb - pa).magnitude + (pc - pa).magnitude)
                           / va.magnitude);
                    }
                    else
                    {
                        // take the camera aspect into account to 
                        // make the frustum wide enough 
                        thisCamera.fieldOfView =
                           Mathf.Rad2Deg / thisCamera.aspect *
                           Mathf.Atan(((pb - pa).magnitude + (pc - pa).magnitude)
                           / va.magnitude);
                    }
                }
            }
        }

        /// <summary>
        /// 选中时绘制Gizmos
        /// </summary>
        public void OnDrawGizmosSelected()
        {
            if (screen)
            {
                XGizmos.DrawPath(Color.blue, thisTransform.position, screen.transform.position);
            }
        }

        /// <summary>
        /// 从指定相机透视拷贝数据
        /// </summary>
        /// <param name="cameraProjection"></param>
        public void CopyDataFrom(CameraProjection cameraProjection)
        {
            if (!cameraProjection) return;
            this.XModifyProperty(() =>
            {
                this._screen = cameraProjection._screen;
                this._estimateViewFrustum = cameraProjection._estimateViewFrustum;
                this._setMatrixRuleOnDisable = cameraProjection._setMatrixRuleOnDisable;
            });
        }
    }
}
