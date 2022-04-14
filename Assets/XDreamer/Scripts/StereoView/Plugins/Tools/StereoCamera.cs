using UnityEngine;
using System.Collections;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.Extension.Base.Dataflows.Base;
using System;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCamera;
using XCSJ.PluginTools;

namespace XCSJ.PluginStereoView.Tools
{
    /// <summary>
    /// 立体相机
    /// </summary>
    [Name(Title)]
    [RequireManager(typeof(StereoViewManager), typeof(CameraManager))]
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public class StereoCamera : ToolMB
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "立体相机";

        /// <summary>
        /// 立体模式
        /// </summary>
        [Name("立体模式")]
        [Tip("立体模式")]
        [EnumPopup]
        public EStereoMode1 _stereoMode = EStereoMode1.ActiveStereo;

        /// <summary>
        /// 立体模式
        /// </summary>
        public EStereoMode1 stereoMode
        {
            get => _stereoMode;
            set
            {
                this.XModifyProperty(ref _stereoMode, value);
                UpdateStereoIfNeed();
            }
        }

        /// <summary>
        /// 禁用时立体模式
        /// </summary>
        [Name("禁用时立体模式")]
        [EnumPopup]
        public EStereoMode1 _stereoModeOnDisable = EStereoMode1.None;

        /// <summary>
        /// 左眼
        /// </summary>
        [Name("左眼")]
        [HideInSuperInspector(nameof(_stereoMode),EValidityCheckType.NotEqual, EStereoMode1.LeftRight)]
        public Camera _leftEye;

        /// <summary>
        /// 左眼
        /// </summary>
        public Camera leftEye
        {
            get => _leftEye;
            private set => this.XModifyProperty(ref _leftEye, value);
        }

        /// <summary>
        /// 右眼
        /// </summary>
        [Name("右眼")]
        [HideInSuperInspector(nameof(_stereoMode), EValidityCheckType.NotEqual, EStereoMode1.LeftRight)]
        public Camera _rightEye;

        /// <summary>
        /// 右眼
        /// </summary>
        public Camera rightEye
        {
            get => _rightEye;
            private set => this.XModifyProperty(ref _rightEye, value);
        }

        /// <summary>
        /// 相机透视
        /// </summary>
        public CameraProjection cameraProjection => this.GetComponent<CameraProjection>();

        private Camera _thisCamera;

        /// <summary>
        /// 当前相机：与当前组件在同一游戏对象上的相机组件对象
        /// </summary>
        public Camera thisCamera
        {
            get
            {
                if(!_thisCamera) _thisCamera = GetComponent<Camera>();
                return _thisCamera;
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            UpdateStereo(_stereoMode);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            UpdateStereo(_stereoModeOnDisable);
        }

        /// <summary>
        /// 验证
        /// </summary>
        public void OnValidate()
        {
            UpdateStereoIfNeed();
        }

        /// <summary>
        /// 如果需要更新立体
        /// </summary>
        public void UpdateStereoIfNeed()
        {
#if UNITY_EDITOR
            //编辑器中非运行态，不更新立体
            if (!Application.isPlaying) return;
#endif
            if (isActiveAndEnabled) UpdateStereo(_stereoMode);
        }

        /// <summary>
        /// 当前的立体模式
        /// </summary>
        public EStereoMode1 currentStereoMode { get; private set; } = EStereoMode1.None;

        private void UpdateStereo(EStereoMode1 _stereoMode)
        {
            currentStereoMode = _stereoMode;
            switch (_stereoMode)
            {
                case EStereoMode1.LeftRight:
                    {
                        SetLeftRight(true);
                        break;
                    }
                default:
                    {
                        SetLeftRight(false);
                        break;
                    }
            }
        }

        private void CreateIfActive(ref Camera eye, bool active, string name, Vector3 localPosition)
        {
            if (active)
            {
                Create(ref eye, name, localPosition);
            }
            if (eye)
            {
                leftEye.gameObject.XSetActive(active);
            }
        }

        private void Create(ref Camera eye, string name, Vector3 localPosition)
        {
            if (!eye)
            {
                this.XModifyProperty(ref eye, NewChildCamera(name, localPosition));
            }
        }

        /// <summary>
        /// 创建左右眼：激活并且对应相机无效时才执行创建；
        /// </summary>
        /// <param name="active">激活：用于设置左右眼游戏对象激活性</param>
        public void CreateLeftRight(bool active = true)
        {
            CreateIfActive(ref _leftEye, active, "LeftEye", new Vector3(-0.03f, 0, 0));
            CreateIfActive(ref _rightEye, active, "RightEye", new Vector3(0.03f, 0, 0));
        }

        /// <summary>
        /// 设置左右眼
        /// </summary>
        /// <param name="active"></param>
        private void SetLeftRight(bool active)
        {
            CreateLeftRight(active);
            if (active)
            {
                thisCamera.XSetEnable(false);
                var cameraProjection = this.cameraProjection;
                if (cameraProjection)
                {
                    cameraProjection.enabled = false;
                }
            }
            else
            {
                thisCamera.XSetEnable(true);
                var cameraProjection = this.cameraProjection;
                if (cameraProjection)
                {
                    cameraProjection.enabled = true;
                }
            }
        }

        private Camera NewChildCamera(string name, Vector3 localPosition)
        {
            var newCamera = this.XCreateChild<Camera>(name);
            newCamera.transform.XSetLocalPosition(localPosition);
            newCamera.XModifyProperty(() =>
            {
                newCamera.nearClipPlane = thisCamera.nearClipPlane;
                newCamera.farClipPlane = thisCamera.farClipPlane;
            });
            var cameraProjection = this.cameraProjection;
            if (cameraProjection)
            {
                var newCameraProjection = newCamera.XAddComponent<CameraProjection>();
                newCameraProjection.CopyDataFrom(cameraProjection);
            }
            return newCamera;
        }
    }

    /// <summary>
    /// 立体模式
    /// </summary>
    public enum EStereoMode1
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        [Tip("")]
        None,

        /// <summary>
        /// 主动立体
        /// </summary>
        [Name("主动立体")]
        ActiveStereo,

        /// <summary>
        /// 左右眼:如左右眼相机不存在，会尝试自动创建左右眼相机游戏对象；
        /// </summary>
        [Name("左右眼")]
        [Tip("如左右眼相机不存在，会尝试自动创建左右眼相机游戏对象；")]
        LeftRight,
    }
}
