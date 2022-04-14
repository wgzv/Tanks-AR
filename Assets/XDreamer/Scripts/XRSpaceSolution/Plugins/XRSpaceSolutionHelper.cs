using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginStereoView;
using XCSJ.PluginStereoView.Tools;
using XCSJ.PluginXBox.Base;
using XCSJ.PluginXBox.Tools;
using XCSJ.PluginXRSpaceSolution.Tools;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools;

namespace XCSJ.PluginXRSpaceSolution
{
    /// <summary>
    /// XR空间解决方案组手
    /// </summary>
    public static class XRSpaceSolutionHelper
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "XR空间解决方案";

        /// <summary>
        /// 创建HMD飞行相机
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static CameraController CreateHMDFlyCamera<T>(string name = "HMD-飞行相机") where T : Component
        {
            var cameraController = CameraHelperExtension.CreateFlyCamera(name);
            cameraController.cameraTransformer.XAddComponent<T>();

            var mainCamera = cameraController.cameraEntityController.mainCamera;
            mainCamera.XAddComponent<CameraProjection>();
            mainCamera.XAddComponent<StereoCamera>();
            return cameraController;
        }

        /// <summary>
        /// 创建带XBox的XR空间
        /// </summary>
        /// <param name="screenCount"></param>
        /// <param name="onScreenCreated"></param>
        /// <param name="createXROriginFunc"></param>
        /// <param name="onCameraLinkedToScreen"></param>
        /// <returns></returns>
        public static XRSpace CreateXRSpace_XBox(int screenCount, Action<ScreenGroup, List<VirtualScreen>> onScreenCreated, Func<(MonoBehaviour, CameraController, Transform, Transform)> createXROriginFunc, Action<int, Camera, VirtualScreen> onCameraLinkedToScreen, string xrSpaceName)
        {
            var xrSpace = CreateXRSpace(screenCount, onScreenCreated, () =>
            {
                var (origin, hmd, leftHand, rightHand) = createXROriginFunc();
                if (origin)//XRRig->XROrigin
                {
                    if (leftHand)
                    {
                        var interact = leftHand.XGetOrAddComponent<InteractIOByXBox>();
                        if (interact)
                        {
                        }
                    }

                    if (rightHand)
                    {
                        var interact = rightHand.XGetOrAddComponent<InteractIOByXBox>();
                        if (interact)
                        {
                            interact.buttonOfActivate = EXBoxAxisAndButton.RightTrigger;
                            interact.buttonOfSelect = EXBoxAxisAndButton.RightBumper;
                            interact.buttonOfUI = EXBoxAxisAndButton.RightTrigger;
                        }
                    }
                }
                return (origin, hmd, leftHand, rightHand);
            }, onCameraLinkedToScreen, xrSpaceName);
            if (xrSpace)
            {
                var move = xrSpace.XAddComponent<TransformByXBox>();
                move.SetDefaultMove();

                var rotate = xrSpace.XAddComponent<TransformByXBox>();
                rotate.SetDefaultRotateWorldY();
            }
            return xrSpace;
        }

        /// <summary>
        /// 创建带XBox的XR空间
        /// </summary>
        /// <param name="screenCount"></param>
        /// <param name="onScreenCreated"></param>
        /// <param name="createXROriginFunc"></param>
        /// <param name="onCameraLinkedToScreen"></param>
        /// <returns></returns>
        public static XRSpace CreateXRSpace(int screenCount, Action<ScreenGroup, List<VirtualScreen>> onScreenCreated, Func<(MonoBehaviour, CameraController, Transform, Transform)> createXROriginFunc, Action<int, Camera, VirtualScreen> onCameraLinkedToScreen, string xrSpaceName)
        {
            if (screenCount < 1)
            {
                Debug.LogWarning("创建XR空间时，构建的屏幕数" + screenCount.ToString() + "不能低于1个！");
                return null;
            }
            if (createXROriginFunc == null)
            {
                Debug.LogWarning("创建XR空间时，构建XR装备的方法不能为空！");
                return null;
            }

#if !XDREAMER_XR_INTERACTION_TOOLKIT
            Debug.LogWarning("插件[" + XRITHelper.Title + "]依赖库缺失,无法创建！");
            return default;
#else
            var (xrSpaceGO, xrSpace, spaceOffset) = CreateXRSpaceRoot(xrSpaceName);

            #region 屏幕组

            List<VirtualScreen> screens = new List<VirtualScreen>();
            var screenGroup = spaceOffset.XCreateChild<ScreenGroup>("屏幕组");
            if (screenGroup)
            {
                var screenGroupTransform = screenGroup.transform;
                for (int i = 0; i < screenCount; i++)
                {
                    screens.Add(VirtualScreen.CreateScreen("屏幕" + i.ToString(), screenGroupTransform));
                }
                screenGroupTransform.XSetLocalPosition(new Vector3(0, 1, 0));
                onScreenCreated?.Invoke(screenGroup, screens);
            }

            #endregion

            #region XR装备/原点

            var (origin, hmd, leftHand, rightHand) = createXROriginFunc();
            if (origin)
            {
                var rigTransform = origin.transform;
                rigTransform.XSetTransformParent(spaceOffset);
                rigTransform.XSetLocalPosition(new Vector3(0, 0, -2));
               
            }

            #endregion

            #region 相机与虚拟屏幕关联

            if (hmd)
            {
                var cameraParent = hmd.cameraEntityController;
                var cameraParentTransform = cameraParent.transform;

                var camera0 = cameraParent.mainCamera;
                var screen0 = screens[0];

                camera0.XSetName("相机0");
                var cameraProjection1 = camera0.XGetOrAddComponent<CameraProjection>();
                cameraProjection1.screen = screen0;
                onCameraLinkedToScreen?.Invoke(0, camera0, screen0);

                for (int i = 1; i < screenCount; i++)
                {
                    var camera = camera0.gameObject.XCloneObject().GetComponent<Camera>();//通过组件所在游戏对象完成克隆，才能支持撤销

                    camera.XSetName("相机" + i.ToString());
                    camera.transform.XSetTransformParent(cameraParentTransform);
                    camera.transform.XResetLocalPRS();

                    var screen = screens[i];
                    camera.XGetOrAddComponent<CameraProjection>().screen = screen;
                    onCameraLinkedToScreen?.Invoke(i, camera, screen);
                }

                //刷新相机实体控制器的相机列表
                cameraParent.UpdateCamears();
            }

            #endregion

            return xrSpace;
#endif
        }

        /// <summary>
        /// 创建XR空间根
        /// </summary>
        /// <param name="xrSpaceName"></param>
        /// <returns></returns>
        public static (GameObject xrSpaceGO, XRSpace xrSpace, Transform spaceOffset) CreateXRSpaceRoot(string xrSpaceName)
        {
            var xrSpaceGO = UnityObjectHelper.CreateGameObject("");
            var xrSpace = xrSpaceGO.XAddComponent<XRSpace>();
            xrSpaceGO.XSetUniqueName(xrSpaceName ?? "XR空间");
            var spaceOffset = xrSpaceGO.XCreateChild<Transform>("空间偏移");
            xrSpace.Reset();
            xrSpace.spaceOffset = spaceOffset;
            return (xrSpaceGO, xrSpace, spaceOffset);
        }

        /// <summary>
        /// 创建XD空间网络版
        /// </summary>
        /// <param name="xrSpaceName"></param>
        /// <returns></returns>
        public static XRSpace CreateXRSpaceNet(string xrSpaceName)
        {
            var (xrSpaceGO, xrSpace, spaceOffset) = CreateXRSpaceRoot(xrSpaceName);
            xrSpace.XAddComponent<XRSpaceNet>();//默认添加网络组件

            #region 屏幕组

            List<VirtualScreen> screens = new List<VirtualScreen>();
            var screenGroup = spaceOffset.XCreateChild<ScreenGroup>("屏幕组");
            if (screenGroup)
            {
                var screenGroupTransform = screenGroup.transform;
                VirtualScreen.CreateScreen("屏幕", screenGroupTransform);
                screenGroupTransform.XSetLocalPosition(new Vector3(0, 1, 0));
            }

            #endregion

            #region XR装备/原点

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
            var origin = spaceOffset.XCreateChild<XROriginOwner>("XR原点");
#else
            var origin = spaceOffset.XCreateChild<XROriginOwner>("XR装备");
#endif
            origin.transform.XResetLocalPRS();

            var cameraOffsetTransform = origin.XCreateChild<Transform>("相机偏移");
            cameraOffsetTransform.XResetLocalPRS();

            //删除默认主相机
            CameraHelperExtension.HandleDefaultMainCamera();

            //左手
            var leftHand = cameraOffsetTransform.XCreateChild<Transform>("左手控制器");

            //右手
            var rightHand = cameraOffsetTransform.XCreateChild<Transform>("左手控制器");

            //HMD
            var hmd = CameraHelperExtension.CreateFlyCamera("HMD");
            hmd.transform.XSetTransformParent(cameraOffsetTransform);

#endregion

            return xrSpace;
        }
    }
}
