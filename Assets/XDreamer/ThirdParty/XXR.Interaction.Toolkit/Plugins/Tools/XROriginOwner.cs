using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using System.Linq;
using XCSJ.PluginsCameras.Base;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginOptiTrack.Tools;
using XCSJ.PluginOptiTrack.Base;
using XCSJ.PluginZVR.Base;
using XCSJ.PluginART.Base;
using XCSJ.Extension.Base.Extensions;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit;
#endif

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
using Unity.XR.CoreUtils;
#endif

namespace XCSJ.PluginXXR.Interaction.Toolkit.Tools
{
    /// <summary>
    /// XR原点拥有者：用于标识XR装备/原点的拥有者，通常在XR装备/原点所在游戏对象的父级游戏对象上添加；也可用于标识相机控制器拥有者、OptiTrack刚体拥有者；
    /// </summary>
    [Name("XR原点拥有者")]
    [Tip("用于标识XR装备/原点的拥有者，通常在XR装备/原点所在游戏对象的父级游戏对象上添加；也可用于标识相机控制器拥有者、OptiTrack刚体拥有者；")]
    [RequireManager(typeof(XXRInteractionToolkitManager))]
    public class XROriginOwner : MB, IXRRigOwner, ICameraOwner, IOptiTrackObjectOwner, IZVRObjectOwner, IARTObjectOwner
    {
        /// <summary>
        /// 拥有者游戏对象
        /// </summary>
        public GameObject ownerGameObject => gameObject;

#if XDREAMER_XR_INTERACTION_TOOLKIT

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER

        /// <summary>
        /// XR原点
        /// </summary>
        public XROrigin xrRig => this.GetComponent<XROrigin>();

#else
        /// <summary>
        /// XR装备
        /// </summary>
        public XRRig xrRig => this.GetComponent<XRRig>();

#endif

        /// <summary>
        /// 运动系统
        /// </summary>
        public LocomotionSystem locomotionSystem => this.GetComponentInChildren<LocomotionSystem>();

        /// <summary>
        /// 传送提供者
        /// </summary>
        public TeleportationProvider teleportationProvider
        {
            get
            {
                var locomotionSystem = this.locomotionSystem;
                return locomotionSystem ? locomotionSystem.GetComponent<TeleportationProvider>() : null;
            }
        }

#endif

        /// <summary>
        /// 相机偏移
        /// </summary>
        [Name("相机偏移")]
        [Readonly]
        public Transform _cameraOffset;

        /// <summary>
        /// 相机偏移
        /// </summary>
        public Transform cameraOffset
        {
            get
            {
                if (!_cameraOffset)
                {
                    var sot = transform.Find("相机偏移");
                    if (sot)
                    {
                        this.XModifyProperty(ref _cameraOffset, sot);
                    }
                    else if (transform.childCount == 1)//如果只有一个子级对象，直接使用该对象
                    {
                        this.XModifyProperty(ref _cameraOffset, transform.GetChild(0));
                    }
                }
                return _cameraOffset;
            }
            set => this.XModifyProperty(ref _cameraOffset, value);
        }

        /// <summary>
        /// 左手控制器
        /// </summary>
        [Name("左手控制器")]
        [Readonly]
        public Transform _leftController;

        /// <summary>
        /// 左手控制器
        /// </summary>
        public Transform leftController
        {
            get
            {
                if (!_leftController && cameraOffset)
                {
                    var sot = cameraOffset.Find("左手控制器");
                    if (sot)
                    {
                        this.XModifyProperty(ref _leftController, sot);
                    }
                }
                return _leftController;
            }
            set => this.XModifyProperty(ref _leftController, value);
        }

        /// <summary>
        /// 右手控制器
        /// </summary>
        [Name("左手控制器")]
        [Readonly]
        public Transform _rightController;

        /// <summary>
        /// 右手控制器
        /// </summary>
        public Transform rightController
        {
            get
            {
                if (!_rightController && cameraOffset)
                {
                    var sot = cameraOffset.Find("右手控制器");
                    if (sot)
                    {
                        this.XModifyProperty(ref _rightController, sot);
                    }
                }
                return _rightController;
            }
            set => this.XModifyProperty(ref _rightController, value);
        }

        /// <summary>
        /// HMD
        /// </summary>
        [Name("HMD")]
        [Readonly]
        public BaseCameraMainController _hmd;

        /// <summary>
        /// HMD
        /// </summary>
        public BaseCameraMainController hmd => this.XGetComponentInChildren(ref _hmd);

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            if (cameraOffset) { }
            if (leftController) { }
            if (rightController) { }
            if (hmd) { }
        }
    }

    /// <summary>
    /// XR装备拥有者接口
    /// </summary>
    public interface IXRRigOwner : IComponentOwner { }

    /// <summary>
    /// XR装备扩展类
    /// </summary>
    public static class XRRigExtension
    {
#if XDREAMER_XR_INTERACTION_TOOLKIT

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER

        /// <summary>
        /// 获取XR原点拥有者
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public static XROriginOwner GetOwner(this XROrigin origin) => origin.GetParentOwner<XROriginOwner>();

#else

        /// <summary>
        /// 获取XR装备拥有者
        /// </summary>
        /// <param name="rig"></param>
        /// <returns></returns>
        public static XROriginOwner GetOwner(this XRRig rig) => rig.GetParentOwner<XROriginOwner>();

#endif

#endif
    }
}
