using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorTools;
using XCSJ.Extension.Base.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.LocomotionProviders;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginsCameras.Controllers;
using System;
using XCSJ.PluginsCameras;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
#endif

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
using Unity.XR.CoreUtils;
#endif

namespace XCSJ.EditorXXR.Interaction.Toolkit.Tools
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    public static class ToolsMenu
    {
        private static T Create<T>(string menuItemPath) where T : MonoBehaviour
        {
            var mbs = UnityEngine.Object.FindObjectsOfType<T>();
            EditorApplication.ExecuteMenuItem(menuItemPath);
            var newMBs = UnityEngine.Object.FindObjectsOfType<T>();
            return newMBs.FirstOrDefault(r => !mbs.Contains(r));
        }

        /// <summary>
        /// 创建基于XR交互工具包框架的XR装备/原点
        /// </summary>
        /// <returns>XR装备/原点组件对象</returns>
        public static MonoBehaviour Create(out CameraController hmd, out Transform leftHand, out Transform rightHand, out Transform locomotionSystem, Func<CameraController> hmdFunc, EXRInputType inputType = EXRInputType.DeviceBased)
        {
            hmd = default;
            leftHand = default;
            rightHand = default;
            locomotionSystem = default;
            if (hmdFunc == null) hmdFunc = () => CameraHelperExtension.CreateFlyCamera();

#if !XDREAMER_XR_INTERACTION_TOOLKIT
            Debug.LogWarning("插件[" + XRITHelper.Title + "]依赖库缺失,无法创建！");
            return default;
#else
            var origin = Create(inputType);
            if (!origin) return origin;
            origin.transform.XResetLocalPRS();
            var cameraOffsetTransform = origin.transform.Find("Camera Offset");
            if (!cameraOffsetTransform) return origin;
            cameraOffsetTransform.XSetName("相机偏移");

            var defaultMainCamera = cameraOffsetTransform.Find("Main Camera");
            if(defaultMainCamera)
            {
                defaultMainCamera.gameObject.XDestoryObject();
            }

            //删除默认主相机
            CameraHelperExtension.HandleDefaultMainCamera();

            //创建HMD飞行相机
            hmd = hmdFunc();
            hmd.transform.XSetTransformParent(cameraOffsetTransform);

            //将相机应用到XR装备/原点
            var mainCamera = hmd.cameraEntityController.mainCamera;
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
            origin.XModifyProperty(() => origin.Camera = mainCamera);
#else
            origin.XModifyProperty(() => origin.cameraGameObject = mainCamera.gameObject);   
#endif


            //创建拥有者
            origin.XAddComponent<XROriginOwner>();

            //修改左手
            leftHand = cameraOffsetTransform.Find("LeftHand Controller");
            if (leftHand)
            {
                leftHand.XSetName("左手控制器");
                var leftController = leftHand.GetComponent<XRBaseController>();
                leftController.XDestoryObject();

                leftController = leftHand.XAddComponent<AnalogController>();                
            }
            else
            {
                var leftController = cameraOffsetTransform.XCreateChild<AnalogController>("左手控制器");
                leftHand = leftController.transform;
            }

            //修改右手
            rightHand = cameraOffsetTransform.Find("RightHand Controller");
            if (rightHand)
            {
                rightHand.XSetName("右手控制器");
                var rightController = rightHand.GetComponent<XRBaseController>();
                rightController.XDestoryObject();

                rightController = rightHand.XAddComponent<AnalogController>();
            }
            else
            {
                var rightController = cameraOffsetTransform.XCreateChild<AnalogController>("右手控制器");
                rightHand = rightController.transform;
            }

            //创建运动系统
            var sys = CreateLocomotionSystem(origin, origin.transform, out AnalogLocomotionProvider analogLocomotionProvider);
            if (sys)
            {
                sys.XSetName("运动系统");
                locomotionSystem = sys.transform;
            }
            return origin;
#endif
        }

#if XDREAMER_XR_INTERACTION_TOOLKIT

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER

        /// <summary>
        /// 创建XR原点
        /// </summary>
        /// <param name="inputType"></param>
        /// <returns></returns>
        public static XROrigin Create(EXRInputType inputType) => Create<XROrigin>(AbbreviationAttribute.GetAbbreviation(inputType));

#else

        /// <summary>
        /// 创建XR装备
        /// </summary>
        /// <param name="inputType"></param>
        /// <returns></returns>
        public static XRRig Create(EXRInputType inputType) => Create<XRRig>(AbbreviationAttribute.GetAbbreviation(inputType));

#endif

        /// <summary>
        /// 创建运动系统
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="parent"></param>
        /// <param name="analogLocomotionProvider"></param>
        /// <returns></returns>
        public static LocomotionSystem CreateLocomotionSystem(MonoBehaviour origin, Transform parent, out AnalogLocomotionProvider analogLocomotionProvider)
        {
            var go = CommonFun.CreateGameObjectWithUniqueName(parent ? parent.gameObject : null, "Locomotion System");
            var system = go.XAddComponent<LocomotionSystem>();
            if (origin)
            {
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
                system.XModifyProperty(() => system.xrOrigin = origin as XROrigin);
#else
                system.XModifyProperty(() => system.xrRig = origin as XRRig);
#endif
            }

            //添加模拟运动提供者
            {
                var provider = go.XAddComponent<AnalogLocomotionProvider>();
                provider.XModifyProperty(() => provider.system = system);
                analogLocomotionProvider = provider;
            }

            //添加传送提供者
            {
                var provider = go.XAddComponent<TeleportationProvider>();
                provider.XModifyProperty(() => provider.system = system);
            }

            return system;
        }

#endif

                #region 空间UI

                /// <summary>
                /// XR空间画布：创建可用于XR空间交互使用的XR空间画布组件对象
                /// </summary>
                /// <param name="toolContext"></param>
                [Tool(XRITHelper.Interact, nameof(XXRInteractionToolkitManager))]
        [Name("XR空间画布")]
        [Tip("创建可用于XR空间交互使用的XR空间画布组件对象")]
        [XCSJ.Attributes.Icon(EIcon.UI)]
        [RequireManager(typeof(XXRInteractionToolkitManager))]
        public static void CreateCanvas(ToolContext toolContext)
        {
#if !XDREAMER_XR_INTERACTION_TOOLKIT
            Debug.LogError(XRITHelper.Title + "环境缺失！无法创建XR空间画布组件对象！");
#else
            EditorToolsHelperExtension.PopupAddComponentMenu(e =>
            {
                switch (e)
                {
                    case EditorToolsHelperExtension.EDefaultToolsMenu.在当前选中游戏对象上添加组件:
                        {
                            var gameObject = Selection.activeGameObject;
                            if (gameObject && gameObject.GetComponent<Canvas>())
                            {
                                gameObject.XGetOrAddComponent<TrackedDeviceGraphicRaycaster>();
                            }
                            break;
                        }
                    case EditorToolsHelperExtension.EDefaultToolsMenu.在当前选中游戏对象下创建游戏对象:
                        {
                            var gameObject = Selection.activeGameObject;
                            var canvas = UnityObjectExtension.CreateCanvas("XR空间画布", gameObject ? gameObject.transform : null);
                            canvas.renderMode = RenderMode.WorldSpace;
                            canvas.XGetOrAddComponent<TrackedDeviceGraphicRaycaster>();
                            break;
                        }
                    case EditorToolsHelperExtension.EDefaultToolsMenu.创建游戏对象:
                        {
                            var canvas = UnityObjectExtension.CreateCanvas("XR空间画布");
                            canvas.renderMode = RenderMode.WorldSpace;
                            canvas.XGetOrAddComponent<TrackedDeviceGraphicRaycaster>();
                            break;
                        }
                }
            }, e =>
            {
                switch (e)
                {
                    case EditorToolsHelperExtension.EDefaultToolsMenu.在当前选中游戏对象上添加组件:
                        {
                            var gameObject = Selection.activeGameObject;
                            return gameObject && gameObject.GetComponent<Canvas>() && !gameObject.GetComponent<TrackedDeviceGraphicRaycaster>();
                        }
                    case EditorToolsHelperExtension.EDefaultToolsMenu.在当前选中游戏对象下创建游戏对象:
                        {
                            return Selection.activeGameObject;
                        }
                    case EditorToolsHelperExtension.EDefaultToolsMenu.创建游戏对象:
                        {
                            return true;
                        }
                }
                return true;
            });
#endif
        }

        #endregion

        #region 可交互对象

        /// <summary>
        /// XR抓取可交互:创建可用于XR空间交互使用的XR抓取可交互组件对象
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(XRITHelper.Interact, nameof(XXRInteractionToolkitManager))]
        [Name("XR抓取可交互")]
        [Tip("创建可用于XR空间交互使用的XR抓取可交互组件对象")]
        [XCSJ.Attributes.Icon(EIcon.Put)]
        [RequireManager(typeof(XXRInteractionToolkitManager))]
        public static void CreateGrabInteractable(ToolContext toolContext)
        {
#if !XDREAMER_XR_INTERACTION_TOOLKIT
            Debug.LogError(XRITHelper.Title + "环境缺失！无法创建XR抓取可交互组件对象！");
#else
            EditorToolsHelperExtension.PopupAddComponentMenu(e =>
            {
                switch (e)
                {
                    case EditorToolsHelperExtension.EDefaultToolsMenu.在当前选中游戏对象上添加组件:
                        {
                            var gameObject = Selection.activeGameObject;
                            if (gameObject && gameObject.GetComponent<Rigidbody>())
                            {
                                gameObject.XGetOrAddComponent<XRGrabInteractable>();
                            }
                            break;
                        }
                    case EditorToolsHelperExtension.EDefaultToolsMenu.在当前选中游戏对象下创建游戏对象:
                        {
                            break;
                        }
                    case EditorToolsHelperExtension.EDefaultToolsMenu.创建游戏对象:
                        {
                            UnityObjectExtension.CreateGameObject<XRGrabInteractable>("XR抓取可交互");
                            break;
                        }
                }
            }, e =>
            {
                switch (e)
                {
                    case EditorToolsHelperExtension.EDefaultToolsMenu.在当前选中游戏对象上添加组件:
                        {
                            var gameObject = Selection.activeGameObject;
                            return gameObject && gameObject.GetComponent<Rigidbody>() && !gameObject.GetComponent<XRGrabInteractable>();
                        }
                    case EditorToolsHelperExtension.EDefaultToolsMenu.在当前选中游戏对象下创建游戏对象:
                        {
                            return false;
                        }
                    case EditorToolsHelperExtension.EDefaultToolsMenu.创建游戏对象:
                        {
                            return true;
                        }
                }
                return true;
            });
#endif
        }

        #endregion

        #region 运动系统-传送

        /// <summary>
        /// 创建传送锚点
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(XRITHelper.Interact, nameof(XXRInteractionToolkitManager))]
        [Name("传送锚点")]
        [Tip("创建一个直径1米的圆形传送锚点")]
        [XCSJ.Attributes.Icon(EIcon.Teleport)]
        [RequireManager(typeof(XXRInteractionToolkitManager))]
        public static void CreateTeleportationAnchor(ToolContext toolContext)
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT

            //创建传送层
            var gameObject = EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath(XRITHelper.CategoryName + "/传送锚点.prefab");
            gameObject.XSetUniqueName("传送锚点");

#else
            Debug.LogFormat("当前工程缺失插件[{0}]所需的包或是版本不匹配！", XRITHelper.Title);
#endif
        }

        /// <summary>
        /// 创建传送区域
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(XRITHelper.Interact, nameof(XXRInteractionToolkitManager))]
        [Name("传送区域")]
        [Tip("创建一个10x10米的方形传送区域")]
        [XCSJ.Attributes.Icon(EIcon.Teleport)]
        [RequireManager(typeof(XXRInteractionToolkitManager))]
        public static void CreateTeleportationArea(ToolContext toolContext)
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT

            //创建传送层            
            var gameObject = EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath(XRITHelper.CategoryName + "/传送区域.prefab");
            gameObject.XSetUniqueName("传送区域");

#else
            Debug.LogFormat("当前工程缺失插件[{0}]所需的包或是版本不匹配！", XRITHelper.Title);
#endif
        }

        /// <summary>
        /// 传送提供者
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Tool(XRITHelper.LocomotionSystem, nameof(AnalogLocomotionProvider))]
        [Name("传送提供者")]
        [Tip("在当前游戏对象上创建一个[传送提供者]组件对象")]
        [XCSJ.Attributes.Icon(EIcon.Teleport)]
        [RequireManager(typeof(XXRInteractionToolkitManager))]
        public static bool CreateTeleportationProvider(ToolContext toolContext) => CreateLocomotionProvider<TeleportationProvider>(toolContext);

        #endregion

        #region 运动系统

        /// <summary>
        /// 基于动作的连续移动提供者
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Tool(XRITHelper.LocomotionSystem, nameof(AnalogLocomotionProvider))]
        [Name("基于动作的连续移动提供者")]
        [Tip("在当前游戏对象上创建一[个基于动作的连续移动提供者]组件对象")]
        [XCSJ.Attributes.Icon(EIcon.Move)]
        [RequireManager(typeof(XXRInteractionToolkitManager))]
        public static bool CreateActionBasedContinuousMoveProvider(ToolContext toolContext) => CreateLocomotionProvider<ActionBasedContinuousMoveProvider>(toolContext);

        /// <summary>
        /// 基于设备的连续移动提供者
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Tool(XRITHelper.LocomotionSystem, nameof(AnalogLocomotionProvider))]
        [Name("基于设备的连续移动提供者")]
        [Tip("在当前游戏对象上创建一个[基于设备的连续移动提供者]组件对象")]
        [XCSJ.Attributes.Icon(EIcon.Move)]
        [RequireManager(typeof(XXRInteractionToolkitManager))]
        public static bool CreateDeviceBasedContinuousMoveProvider(ToolContext toolContext) => CreateLocomotionProvider<DeviceBasedContinuousMoveProvider>(toolContext);

        /// <summary>
        /// 基于动作的连续转弯提供者
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Tool(XRITHelper.LocomotionSystem, nameof(AnalogLocomotionProvider))]
        [Name("基于动作的连续转弯提供者")]
        [Tip("在当前游戏对象上创建一个[基于动作的连续转弯提供者]组件对象")]
        [XCSJ.Attributes.Icon(EIcon.Rotate)]
        [RequireManager(typeof(XXRInteractionToolkitManager))]
        public static bool CreateActionBasedContinuousTurnProvider(ToolContext toolContext) => CreateLocomotionProvider<ActionBasedContinuousTurnProvider>(toolContext);

        /// <summary>
        /// 基于设备的连续转弯提供者
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Tool(XRITHelper.LocomotionSystem, nameof(AnalogLocomotionProvider))]
        [Name("基于设备的连续转弯提供者")]
        [Tip("在当前游戏对象上创建一个[基于设备的连续转弯提供者]组件对象")]
        [XCSJ.Attributes.Icon(EIcon.Rotate)]
        [RequireManager(typeof(XXRInteractionToolkitManager))]
        public static bool CreateDeviceBasedContinuousTurnProvider(ToolContext toolContext) => CreateLocomotionProvider<DeviceBasedContinuousTurnProvider>(toolContext);

        /// <summary>
        /// 基于动作的快速转弯提供者
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Tool(XRITHelper.LocomotionSystem, nameof(AnalogLocomotionProvider))]
        [Name("基于动作的快速转弯提供者")]
        [Tip("在当前游戏对象上创建一个[基于动作的快速转弯提供者]组件对象")]
        [XCSJ.Attributes.Icon(EIcon.Rotate)]
        [RequireManager(typeof(XXRInteractionToolkitManager))]
        public static bool CreateActionBasedSnapTurnProvider(ToolContext toolContext) => CreateLocomotionProvider<ActionBasedSnapTurnProvider>(toolContext);

        /// <summary>
        /// 基于设备的捕捉转弯提供者
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Tool(XRITHelper.LocomotionSystem, nameof(AnalogLocomotionProvider))]
        [Name("基于设备的捕捉转弯提供者")]
        [Tip("在当前游戏对象上创建一个[基于设备的捕捉转弯提供者]组件对象")]
        [XCSJ.Attributes.Icon(EIcon.Rotate)]
        [RequireManager(typeof(XXRInteractionToolkitManager))]
        public static bool CreateDeviceBasedSnapTurnProvider(ToolContext toolContext) => CreateLocomotionProvider<DeviceBasedSnapTurnProvider>(toolContext);

        private static bool CreateLocomotionProvider<T>(ToolContext toolContext) where T: LocomotionProvider
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT
            var go = Selection.activeGameObject;
            if (!go) return false;

            var provider = go.GetComponent<T>();
            if (provider) return false;

            var system = go.GetComponentInParent<LocomotionSystem>();
            if (!system) return false;

            if (toolContext.toolFuncType == EToolFuncType.OnClick)
            {
                provider = go.XAddComponent<T>();
                provider.XModifyProperty(() => provider.system = system);
            }
            return true;
#else
            return false;
#endif
        }

        #endregion


#if !XDREAMER_XR_INTERACTION_TOOLKIT //不存在XR交互工具时的定义类

        #region 运动系统

        /// <summary>
        /// 运动提供者
        /// </summary>
        public class LocomotionProvider { }

        /// <summary>
        /// 基于设备的捕捉转弯提供者
        /// </summary>
        public class DeviceBasedSnapTurnProvider : LocomotionProvider { }

        /// <summary>
        /// 基于设备的连续转弯提供者
        /// </summary>
        public class DeviceBasedContinuousTurnProvider : LocomotionProvider { }

        /// <summary>
        /// 基于设备的连续移动提供者
        /// </summary>
        public class DeviceBasedContinuousMoveProvider : LocomotionProvider { }

        /// <summary>
        /// 基于动作的连续转弯提供者
        /// </summary>
        public class ActionBasedContinuousTurnProvider : LocomotionProvider { }

        /// <summary>
        /// 基于动作的快速转弯提供者
        /// </summary>
        public class ActionBasedSnapTurnProvider : LocomotionProvider { }

        /// <summary>
        /// 基于动作的连续移动提供者
        /// </summary>
        public class ActionBasedContinuousMoveProvider : LocomotionProvider { }

        /// <summary>
        /// 传送提供者
        /// </summary>
        public class TeleportationProvider : LocomotionProvider { }

        #endregion

#endif
    }

    /// <summary>
    /// XR输入类型
    /// </summary>
    public enum EXRInputType
    {
        /// <summary>
        /// 基于动作
        /// </summary>
        [Name("基于动作")]
        [Tip("基于Unity新版输入系统的动作获取输入")]
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
        [Abbreviation("GameObject/XR/XR Origin")]
#else
        [Abbreviation("GameObject/XR/XR Rig")]
#endif
        ActionBased,

        /// <summary>
        /// 基于设备
        /// </summary>
        [Name("基于设备）")]
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
        [Abbreviation("GameObject/XR/Device-based/XR Origin")]
#else
        [Abbreviation("GameObject/XR/Device-based/XR Rig")]
#endif
        DeviceBased,
    }
}
