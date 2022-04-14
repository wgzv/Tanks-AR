using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorTools;
using XCSJ.EditorXXR.Interaction.Toolkit;
using XCSJ.EditorXXR.Interaction.Toolkit.Tools;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginOptiTrack;
using XCSJ.PluginOptiTrack.Tools;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginStereoView;
using XCSJ.PluginStereoView.Tools;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.LocomotionProviders;
using XCSJ.PluginXBox;
using XCSJ.PluginXBox.Tools;
using XCSJ.PluginXBox.Base;
using System.Collections.Generic;
using XCSJ.EditorCommonUtils;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.PluginXRSpaceSolution.Tools;
using XCSJ.PluginXRSpaceSolution;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit;
#endif

namespace XCSJ.EditorOptiTrack.Tools
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    public class ToolsMenu
    {
        /// <summary>
        /// HMD-OptiTrack型飞行相机
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(OptiTrackHelper.Title, nameof(OptiTrackManager), rootType = typeof(OptiTrackManager), groupRule = EToolGroupRule.None)]
        [Name("HMD-OptiTrack型飞行相机")]
        [XCSJ.Attributes.Icon(EIcon.FlyCamera)]
        [RequireManager(typeof(OptiTrackManager), typeof(StereoViewManager))]
        public static void CreateOptiTrackCamera(ToolContext toolContext)
        {
            var cameraController = CreateOptiTrackCamera();
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, cameraController.gameObject);
        }

        private static CameraController CreateOptiTrackCamera() => XRSpaceSolutionHelper.CreateHMDFlyCamera<CameraTransformByOptiTrack>("HMD-OptiTrack型飞行相机");

        /// <summary>
        /// OptiTrack型XR原点:创建基于OptiTrack跟踪定位的XR装备/原点
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(OptiTrackHelper.Title, nameof(OptiTrackManager))]
        [Name("OptiTrack型XR原点")]
        [Tip("创建基于OptiTrack跟踪定位的XR装备/原点")]
        [XCSJ.Attributes.Icon(EIcon.State)]
        [RequireManager(typeof(OptiTrackManager), typeof(XXRInteractionToolkitManager), typeof(StereoViewManager))]
        public static void Create(ToolContext toolContext) => Create(out _, out _, out _);

        private static bool CheckPackage()
        {
#if XDREAMER_OPTITRACK
            return true;
#else
            Debug.LogWarning("插件[" + OptiTrackHelper.Title + "]依赖库缺失！");
            return false;
#endif
        }

        /// <summary>
        /// 创建基于OptiTrack跟踪定位的XR装备/原点
        /// </summary>
        /// <returns></returns>
        private static MonoBehaviour Create(out CameraController hmd, out Transform leftHand, out Transform rightHand)
        {
            hmd = default;
            leftHand = default;
            rightHand = default;

            if (!CheckPackage()) return default;

            var origin = XCSJ.EditorXXR.Interaction.Toolkit.Tools.ToolsMenu.Create(out hmd, out leftHand, out rightHand, out var locomotionSystem, CreateOptiTrackCamera);
            if (!origin) return origin;
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
            origin.gameObject.XSetUniqueName("XR原点 - OptiTrack");
#else
            origin.gameObject.XSetUniqueName("XR装备 - OptiTrack");
#endif
            if (hmd)
            {
                hmd.cameraTransformer.XGetOrAddComponent<CameraTransformByOptiTrack>();
            }

            if (leftHand)
            {
                var pose = leftHand.XAddComponent<PoseIOByOptiTrack>();
                if (pose) pose.rigidBodyID = 1;
            }

            if (rightHand)
            {
                var pose = rightHand.XAddComponent<PoseIOByOptiTrack>();
                if (pose) pose.rigidBodyID = 2;
            }
            return origin;
        }

        private static XRSpace CreateXRSpace(EScreenLayoutMode screenLayoutMode)
        {
            if (screenLayoutMode == EScreenLayoutMode.None || !CheckPackage()) return default;

            return XRSpaceSolutionHelper.CreateXRSpace_XBox(screenLayoutMode.GetScreenCount(), (screenGroup, screens) =>
            {
                VirtualScreen.LayoutScreen(screens, screenLayoutMode);
            }, () =>
            {
                var origin = Create(out CameraController hmd, out Transform leftHand, out Transform rightHand);
                return (origin, hmd, leftHand, rightHand);
            }, (i, camera, screen) =>
            {
                VirtualScreen.LayoutCamera(i, camera, screenLayoutMode);
            }, XRSpaceTitle);
        }

        const string XRSpaceTitle = "OptiTrack+XBox+单机多通道主动立体型XR空间";

        /// <summary>
        /// 创建XR空间，由OptiTrack型XR装备/原点、XBox、单机多通道动立体虚拟屏幕等工具组件构建的XR交互空间；
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(OptiTrackHelper.Title, nameof(OptiTrackManager))]
        [Tool(XRITHelper.SpaceSolution, nameof(OptiTrackManager))]
        [Name(XRSpaceTitle)]
        [Tip("创建XR空间，由OptiTrack型XR装备/原点、XBox、单机多通道动立体虚拟屏幕等工具组件构建的XR交互空间；")]
        [XCSJ.Attributes.Icon(EIcon.State)]
        [RequireManager(typeof(OptiTrackManager), typeof(XXRInteractionToolkitManager), typeof(StereoViewManager), typeof(XBoxManager))]
        public static void CreateXRSpace_XBox_MultiScreen_ActiveStereo(ToolContext toolContext)
        {
            MenuHelper.DrawMenu(nameof(CreateXRSpace_XBox_MultiScreen_ActiveStereo), m =>
            {
                EnumCache<EScreenLayoutMode>.Array.Foreach(e =>
                {
                    m.AddMenuItem(CommonFun.Name(e), () => CreateXRSpace(e), () => true);
                });
            });
        }
    }
}
