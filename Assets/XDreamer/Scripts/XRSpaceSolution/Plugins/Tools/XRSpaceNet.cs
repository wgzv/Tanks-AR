using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Maths;
using XCSJ.Net;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Safety.XR;
using XCSJ.PluginsCameras;
using XCSJ.PluginStereoView.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginTools.Renderers;
using XCSJ.PluginXRSpaceSolution.Base;

namespace XCSJ.PluginXRSpaceSolution.Tools
{
    /// <summary>
    /// XR空间网络:用于通过网络构建单机单（多）通道环境的XR空间网络功能组件
    /// </summary>
    [Name("XR空间网络")]
    [RequireManager(typeof(XRSpaceSolutionManager))]
    public class XRSpaceNet : ToolMB
    {
        /// <summary>
        /// XR空间
        /// </summary>
        [Name("XR空间")]
        public XRSpace _xrSpace;

        /// <summary>
        /// XR空间
        /// </summary>
        public XRSpace xrSpace => this.XGetComponentInParent(ref _xrSpace);

        /// <summary>
        /// 当启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            XDreamerEvents.onXRAnswerReceived += OnXRNetMsgReceived;
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            XDreamerEvents.onXRAnswerReceived -= OnXRNetMsgReceived;
        }

        /// <summary>
        /// 重置
        /// </summary>
        private void Reset()
        {
            if (xrSpace) { }
        }

        private void OnXRNetMsgReceived(XRAnswer answer)
        {
            Debug.Log(answer.ToJson());
            if (answer is XRSpaceConfigA xRSpaceConfigA)
            {
                OnHandle(xRSpaceConfigA);
            }
        }

        private void OnHandle(XRSpaceConfigA xRSpaceConfigA)
        {
            //XR控件
            var xrSpace = this.xrSpace;
            if (!xrSpace) return;

            //空间偏移
            var spaceOffset = xrSpace.spaceOffset;
            if (spaceOffset)
            {
                spaceOffset.XSetLocalPosition(xRSpaceConfigA.spaceOffset.position);
                spaceOffset.XSetLocalRotation(xRSpaceConfigA.spaceOffset.rotation);
            }

            //屏幕组
            var screenGroup = xrSpace.screenGroup;
            if (!screenGroup) return;

            //屏幕组偏移
            {
                var screenGroupTransform = screenGroup.transform;
                screenGroupTransform.XSetLocalPosition(xRSpaceConfigA.screenGroupOffset.position);
                screenGroupTransform.XSetLocalRotation(xRSpaceConfigA.screenGroupOffset.rotation);
            }

            //装备拥有者
            var rigOwner = xrSpace.rigOwner;
            if (!rigOwner) return;

            //相机偏移：装备偏移
            var cameraOffset = rigOwner.cameraOffset;
            if (cameraOffset)
            {
                cameraOffset.XSetLocalPosition(xRSpaceConfigA.cameraOffset.position);
                cameraOffset.XSetLocalRotation(xRSpaceConfigA.cameraOffset.rotation);
            }

            #region 屏幕

            var screenCount = xRSpaceConfigA.screens.Count;
            var screens = screenGroup.screens;//当前已经有的屏幕组

            for (int i = screenCount; i < screens.Length; i++)//将可能多出的屏幕隐藏
            {
                var s = screens[i];
                if (s)
                {
                    s.gameObject.XSetActive(false);
                    s.XSetName(i.ToString() + "___已禁用");
                }
            }
            Dictionary<string, BaseScreen> tempScreens = new Dictionary<string, BaseScreen>();
            List<(BaseScreen, ScreenInfo)> anchorLinkScreens = new List<(BaseScreen, ScreenInfo)>();

            BaseScreen defaultStandardScreen = default;
            for (int i = 0; i < screenCount; i++)
            {
                var screenInfo = xRSpaceConfigA.screens[i];
                var screen = i < screens.Length ? screens[i] : VirtualScreen.CreateScreen(screenInfo.name, screenGroup.transform);

                screen.gameObject.XSetActive(true);//保证游戏对象激活
                screen.XSetName(screenInfo.name);
                screen.screenSize = screenInfo.screenSize.ToVector3();

                if (!tempScreens.ContainsKey(screenInfo.name))
                {
                    tempScreens.Add(screenInfo.name, screen);
                }

                var renderer = screen.XGetOrAddComponent<GizmoRenderer>();
                renderer.text = screenInfo.displayName;

                switch (screenInfo.screenPoseMode)
                {
                    case ScreenPoseMode.ScreenPose:
                        {
                            if (!defaultStandardScreen) defaultStandardScreen = screen;//至少有一个默认的标准屏幕

                            var screenTransform = screen.transform;
                            screenTransform.XSetLocalPosition(screenInfo.screenPose.position);
                            screenTransform.XSetLocalRotation(screenInfo.screenPose.rotation);

                            var screenAnchorLink = screen.GetComponent<ScreenAnchorLink>();
                            if (screenAnchorLink)
                            {
                                screenAnchorLink.XSetEnable(false);
                            }
                            break;
                        }
                    case ScreenPoseMode.AnchorLink:
                        {
                            anchorLinkScreens.Add((screen, screenInfo));
                            break;
                        }
                }
            }

            if (!defaultStandardScreen)
            {
                Debug.LogError("未找到有效的标准屏幕!");
            }
            foreach (var kv in anchorLinkScreens)
            {
                var screenInfo = kv.Item2;
                var screen = kv.Item1;
                var screenAnchorLink = screen.XGetOrAddComponent<ScreenAnchorLink>();
                var screenAnchorLinkInfo = screenInfo.screenAnchorLinkInfo;
                if (tempScreens.TryGetValue(screenAnchorLinkInfo.standardScreen, out var standardScreen))
                {
                    screenAnchorLink.standardScreen = standardScreen;
                }
                else
                {
                    Debug.LogErrorFormat("屏幕[{0}]依赖锚点关联时未找到基准屏幕[{1}],使用[{2}]替代",
                        screenInfo.name,
                        screenAnchorLinkInfo.standardScreen,
                        defaultStandardScreen ? defaultStandardScreen.name : "");
                }

                screenAnchorLink.standardScreenAnchor = screenAnchorLinkInfo.standardScreenAnchor;
                screenAnchorLink._standardScreenAnchorOffset = screenAnchorLinkInfo.standardScreenAnchorOffset.ToVector3();
                screenAnchorLink._standardScreenAnchorOffsetSpaceType = screenAnchorLinkInfo.standardScreenAnchorOffsetSpaceType == 0 ? ESpaceType.World : ESpaceType.Local;

                screenAnchorLink.screenAnchor = screenAnchorLinkInfo.screenAnchor;
                screenAnchorLink._screenAnchorOffset = screenAnchorLinkInfo.screenAnchorOffset.ToVector3();
                screenAnchorLink._screenAnchorOffsetSpaceType = screenAnchorLinkInfo.screenAnchorOffsetSpaceType == 0 ? ESpaceType.World : ESpaceType.Local;

                screenAnchorLink.linkRotation = screenAnchorLinkInfo.linkRotation.ToVector3();
            }

            #endregion

            #region 相机

            var cameraCount = xRSpaceConfigA.cameras.Count;
            var cameras = rigOwner.hmd.cameraEntityController.cameras;//当前已经有的相机组
            for (int i = cameraCount; i < cameras.Length; i++)//将可能多出的相机隐藏
            {
                var c = cameras[i];
                if (c)
                {
                    c.gameObject.XSetActive(false);
                    c.XSetName(i.ToString() + "___已禁用");
                }
            }

            var parent = rigOwner.hmd.cameraEntityController.transform;
            for (int i = 0; i < cameraCount; i++)
            {
                var cameraInfo = xRSpaceConfigA.cameras[i];
                if (!tempScreens.TryGetValue(cameraInfo.screen, out var screen) || !(screen is VirtualScreen virtualScreen) || !virtualScreen)
                {
                    Debug.LogError("未找到屏幕:" + cameraInfo.screen);
                    continue;
                }

                var camera = i < cameras.Length ? cameras[i] : CameraHelperExtension.CreateCamera(parent);

                camera.gameObject.XSetActive(true);//保证游戏对象激活
                camera.XSetName(cameraInfo.name);

                var cameraTransform = camera.transform;
                //cameraTransform.XResetLocalPRS();
                cameraTransform.XSetLocalPosition(cameraInfo.cameraOffset_Position);
                cameraTransform.XSetLocalRotation(cameraInfo.cameraOffset_Rotation);

                camera.XModifyProperty(() =>
                {
                    camera.rect = cameraInfo.viewportRect.ToRect();
                });

                var cameraProjection = camera.XGetOrAddComponent<CameraProjection>();
                cameraProjection.screen = virtualScreen;
            }

            rigOwner.hmd.cameraEntityController.UpdateCamears();//更新相机组

            #endregion
        }
    }
}
