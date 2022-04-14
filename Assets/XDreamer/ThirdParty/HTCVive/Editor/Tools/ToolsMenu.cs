using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorTools;
using XCSJ.EditorXXR.Interaction.Toolkit.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginHTCVive;
using XCSJ.PluginHTCVive.Tools;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginXRSpaceSolution;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.PluginXXR.Interaction.Toolkit.Base;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;

namespace XCSJ.EditorHTCVive
{
    /// <summary>
    /// 工具菜单
    /// </summary>
    public static class ToolsMenu
    {
        const string XRSpaceTitle = "HTC Vive型XR空间";

        /// <summary>
        /// HTC Vive型XR空间:创建XR装备/原点，由头盔与左右两个手柄构成；
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(XRITHelper.SpaceSolution)]
        [Tool(HTCViveManager.Title)]
        [Name(XRSpaceTitle)]
        [Tip("创建XR装备/原点，由头盔与左右两个手柄构成；")]
        [XCSJ.Attributes.Icon(EIcon.State)]
        [RequireManager(typeof(HTCViveManager), typeof(XXRInteractionToolkitManager))]
        public static void CreateHTCViveXRSpace(ToolContext toolContext)
        {
            CreateXRSpace();
        }

        /// <summary>
        /// 创建HTC Vive型XR空间
        /// </summary>
        /// <returns></returns>
        public static GameObject CreateXRSpace()
        {
            if (!CheckPackage()) return null;

            var (xrSpaceGO, xrSpace, spaceOffset) = XRSpaceSolutionHelper.CreateXRSpaceRoot(XRSpaceTitle);
            xrSpace.XSetEnable(false);

            var origin = XCSJ.EditorXXR.Interaction.Toolkit.Tools.ToolsMenu.Create(out CameraController hmd, out Transform leftHand, out Transform rightHand, out var locomotionSystem, null, EXRInputType.DeviceBased);
            if (!origin) return null;

            origin.gameObject.XSetParent(spaceOffset);
            origin.transform.XResetLocalPRS();

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
            origin.gameObject.XSetUniqueName("XR原点 - HTC Vive");
#else
            origin.gameObject.XSetUniqueName("XR装备 - HTC Vive");
#endif
            if (hmd)
            {
                hmd.cameraTransformer.XGetOrAddComponent<CameraTransformByXRHMDDevice>();
            }

            if (leftHand)
            {
                var obj = leftHand.XAddComponent<InteractIOByVive>();
                if (obj) obj._viveControllerInteractAxisAndButton._handleType = EHandRule.Left;

                var pose = leftHand.XAddComponent<PoseIOByXRHandDevice>();
                if (pose) pose._deviceType = EVRDeviceType.Left;
            }

            if (rightHand)
            {
                var obj = rightHand.XAddComponent<InteractIOByVive>();
                if (obj) obj._viveControllerInteractAxisAndButton._handleType = EHandRule.Right;

                var pose = rightHand.XAddComponent<PoseIOByXRHandDevice>();
                if (pose) pose._deviceType = EVRDeviceType.Right;
            }

            if (locomotionSystem)
            {
                var mover = locomotionSystem.XAddComponent<TransformByVive>();
                if (mover)
                {
                    mover._targetTransform = origin.transform; 
                    mover.SetDefaultMove();
                }

                var rotater = locomotionSystem.XAddComponent<TransformByVive>();
                if (rotater)
                {
                    rotater._targetTransform = origin.transform;
                    rotater.SetDefaultRotateWorldY();
                }
            }

            return origin.gameObject;
        }

        /// <summary>
        /// 检查XR交互工具包是否存在
        /// </summary>
        /// <returns></returns>
        private static bool CheckPackage()
        {
            if (!XRITHelper.CheckPackage())
            {
                return false;
            }
#if XDREAMER_STEAMVR
            return true;
#else
            Debug.LogWarning("插件[" + HTCViveManager.Title + "]依赖库缺失,无法创建！");
            return false;
#endif
        }
    }
}
