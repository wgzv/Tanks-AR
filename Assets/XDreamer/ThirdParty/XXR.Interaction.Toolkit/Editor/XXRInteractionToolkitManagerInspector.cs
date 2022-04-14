using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.XR;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorExtension.Base.XUnityEditor.PackageManager;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXXR.Interaction.Toolkit;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
#endif

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
using Unity.XR.CoreUtils;
#endif

namespace XCSJ.EditorXXR.Interaction.Toolkit
{
    /// <summary>
    /// XR交互工具包检查器
    /// </summary>
    [CustomEditor(typeof(XXRInteractionToolkitManager))]
    public class XXRInteractionToolkitManagerInspector : BaseManagerInspector<XXRInteractionToolkitManager>
    {
        /// <summary>
        /// 需要的所有依赖包;需要调用包管理器
        /// </summary>
        public const string PackageName = "com.unity.xr.interaction.toolkit";

        /// <summary>
        /// 初始化
        /// </summary>
        [InitializeOnLoadMethod]
        public static void Init()
        {
            InitMacro();
            PluginCommonUtilsRootInspector.onCreatedManager += (t) =>
            {
                if (t == typeof(XXRInteractionToolkitManager))
                {
                    EditorHelper.OpenPackageManagerIfNeedWithDialog(XDREAMER_XR_INTERACTION_TOOLKIT, PackageName);
                }
            };

            EditorSceneManager.sceneOpened += (scene, mode) =>
            {
                UICommonFun.DelayCall(() =>
                {
                    if (XXRInteractionToolkitManager.instance)
                    {
                        EditorHelper.OpenPackageManagerIfNeedWithDialog(XDREAMER_XR_INTERACTION_TOOLKIT, PackageName);
                    }
                });
            };
        }

        #region 编译宏

        /// <summary>
        /// 宏
        /// </summary>
        public static readonly Macro XDREAMER_XR_INTERACTION_TOOLKIT = new Macro(nameof(XDREAMER_XR_INTERACTION_TOOLKIT), BuildTargetGroup.Standalone, BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.WSA);
        public static readonly Macro XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER = new Macro(nameof(XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER), BuildTargetGroup.Standalone, BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.WSA);

        /// <summary>
        /// 初始化宏
        /// </summary>
        [Macro]
        public static void InitMacro()
        {
#if UNITY_2019_4_OR_NEWER
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
            if (TypeHelper.Exists("UnityEngine.XR.Interaction.Toolkit.XRInteractionManager")
                && PackageInfo_LinkType.GetPackageInfoByPackageName(PackageName) is UnityEditor.PackageManager.PackageInfo packageInfo)
            {
                if(UICommonFun.NaturalCompare(packageInfo.version, "1.0.0") >= 0)
                {
                    XDREAMER_XR_INTERACTION_TOOLKIT.DefineIfNoDefined();
                }
                if (UICommonFun.NaturalCompare(packageInfo.version, "2.0.0") >= 0)
                {
                    XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER.DefineIfNoDefined();
                }
                else
                {
                    XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER.UndefineWithSelectedBuildTargetGroup();
                }
            }
            else
#endif
#endif
            {
                XDREAMER_XR_INTERACTION_TOOLKIT.UndefineWithSelectedBuildTargetGroup();
                XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER.UndefineWithSelectedBuildTargetGroup();
            }
        }

#endregion

        /// <summary>
        /// 当纵向绘制之前回调
        /// </summary>
        public override void OnBeforeVertical()
        {
            InstallXRInteractionToolkitPackage();

            base.OnBeforeVertical();
        }

        /// <summary>
        /// 检测是否需要安装XR打开包管理器
        /// </summary>
        public static void InstallXRInteractionToolkitPackage()
        {
#if UNITY_2019_4_OR_NEWER
            EditorHelper.OpenPackageManagerIfNeedWithButton(XDREAMER_XR_INTERACTION_TOOLKIT, PackageName);
#if !XDREAMER_XR_INTERACTION_TOOLKIT
            UICommonFun.RichHelpBox("<color=red>请安装(或更新)到Unity的[" + XRITHelper.Title + "]包[1.0.0](含)或更高版本！</color>", MessageType.Warning);
#endif
#else
            UICommonFun.RichHelpBox("<color=red>" + Product.Name + "提供的基于[" + XRITHelper.Title + "]包的插件扩展，仅支持[Unity2019.4.0](含)或更高版本中使用！</color>", MessageType.Warning);
#endif
        }

        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            EditorXRITHelper.DrawOpenXRInteractionDebugger();

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
            DrawXROriginDetailInfos();
#else
            DrawXRRigDetailInfos();
#endif
            DrawTrackedDeviceGraphicRaycasterDetailInfos();
            DrawTeleportationDetailInfos();
        }

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER

        /// <summary>
        /// XR原点列表
        /// </summary>
        [Name("XR原点列表")]
        [Tip("当前场景中所有的XR原点对象")]
        private static bool _displayXROrigins = true;
        
        private void DrawXROriginDetailInfos()
        {
            _displayXROrigins = UICommonFun.Foldout(_displayXROrigins, CommonFun.NameTip(GetType(), nameof(_displayXROrigins)));
            if (!_displayXROrigins) return;

            CommonFun.BeginLayout();

#region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("XR原点对象", "XR原点备所在的游戏对象；本项只读；"));
            GUILayout.Label(CommonFun.TempContent("跟踪原点模式标志", "跟踪原点模式标志；"), UICommonOption.Width120);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

#endregion

#if XDREAMER_XR_INTERACTION_TOOLKIT

            var cache = ComponentCache.Get(typeof(XROrigin), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as XROrigin;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //XR装备对象
                var gameObject = component.gameObject;
                EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

                //跟踪原点模式标志
                EditorGUI.BeginChangeCheck();
                var trackingOriginMode = EditorGUILayout.EnumPopup(component.RequestedTrackingOriginMode, UICommonOption.Width120);
                if (EditorGUI.EndChangeCheck())
                {
                    component.XModifyProperty(() => component.RequestedTrackingOriginMode = (XROrigin.TrackingOriginMode)trackingOriginMode);
                }

                UICommonFun.EndHorizontal();
            }

#endif

            CommonFun.EndLayout();
        }

#else

        /// <summary>
        /// XR装备列表
        /// </summary>
        [Name("XR装备列表")]
        [Tip("当前场景中所有的XR装备对象")]
        private static bool _displayXRRigs = true;

        private void DrawXRRigDetailInfos()
        {
            _displayXRRigs = UICommonFun.Foldout(_displayXRRigs, CommonFun.NameTip(GetType(), nameof(_displayXRRigs)));
            if (!_displayXRRigs) return;

            CommonFun.BeginLayout();

#region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("XR装备对象", "XR装备所在的游戏对象；本项只读；"));
            GUILayout.Label(CommonFun.TempContent("跟踪原点模式标志", "跟踪原点模式标志；"), UICommonOption.Width120);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

#endregion

#if XDREAMER_XR_INTERACTION_TOOLKIT

            var cache = ComponentCache.Get(typeof(XRRig), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as XRRig;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //XR装备对象
                var gameObject = component.gameObject;
                EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

                //跟踪原点模式标志
                EditorGUI.BeginChangeCheck();
                var trackingOriginMode = EditorGUILayout.EnumPopup(component.requestedTrackingOriginMode, UICommonOption.Width120);
                if (EditorGUI.EndChangeCheck())
                {
                    component.XModifyProperty(() => component.requestedTrackingOriginMode = (UnityEngine.XR.Interaction.Toolkit.XRRig.TrackingOriginMode)trackingOriginMode);
                }

                UICommonFun.EndHorizontal();
            }

#endif

            CommonFun.EndLayout();
        }

#endif

        /// <summary>
        /// 跟踪设备图形射线检测器列表
        /// </summary>
        [Name("跟踪设备图形射线检测器列表")]
        [Tip("当前场景中所有的跟踪设备图形射线检测器对象")]
        private static bool _displayTrackedDeviceGraphicRaycasters = false;

        private void DrawTrackedDeviceGraphicRaycasterDetailInfos()
        {
            _displayTrackedDeviceGraphicRaycasters = UICommonFun.Foldout(_displayTrackedDeviceGraphicRaycasters, CommonFun.NameTip(GetType(), nameof(_displayTrackedDeviceGraphicRaycasters)));
            if (!_displayTrackedDeviceGraphicRaycasters) return;

            CommonFun.BeginLayout();

#region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("射线检测器", "跟踪设备图形射线检测器所在的游戏对象；本项只读；"));
            GUILayout.Label(CommonFun.TempContent("事件相机", "跟踪设备图形射线检测器所在的游戏对象上的画布在世界空间绘制模式时的事件相机；"));
            GUILayout.Label(CommonFun.TempContent("绘制模式", "跟踪设备图形射线检测器所在的游戏对象上的画布的绘制模式；"), UICommonOption.Width120);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

#endregion

#if XDREAMER_XR_INTERACTION_TOOLKIT

            var cache = ComponentCache.Get(typeof(TrackedDeviceGraphicRaycaster), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as TrackedDeviceGraphicRaycaster;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //射线检测器
                var gameObject = component.gameObject;
                EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

                //事件相机
                var canvas = component.GetComponent<Canvas>();
                EditorGUI.BeginChangeCheck();
                var worldCamera = EditorGUILayout.ObjectField(canvas.worldCamera, typeof(Camera), true);
                if (EditorGUI.EndChangeCheck())
                {
                    canvas.XModifyProperty(() => canvas.worldCamera = worldCamera as Camera);
                }

                //绘制模式
                EditorGUI.BeginChangeCheck();
                var renderMode = EditorGUILayout.EnumPopup(canvas.renderMode, UICommonOption.Width120);
                if (EditorGUI.EndChangeCheck())
                {
                    canvas.XModifyProperty(() => canvas.renderMode = (RenderMode)renderMode);
                }

                UICommonFun.EndHorizontal();
            }
#endif

            CommonFun.EndLayout();
        }

        /// <summary>
        /// 传送锚点与区域列表
        /// </summary>
        [Name("传送锚点与区域列表")]
        [Tip("当前场景中所有的传送锚点与区域对象")]
        private static bool _displayTeleportations = false;

        private void DrawTeleportationDetailInfos()
        {
            _displayTeleportations = UICommonFun.Foldout(_displayTeleportations, CommonFun.NameTip(GetType(), nameof(_displayTeleportations)));
            if (!_displayTeleportations) return;

            CommonFun.BeginLayout();

#region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("传送对象", "传送锚点与区域组件对象；本项只读；"));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

#endregion

#if XDREAMER_XR_INTERACTION_TOOLKIT

            var cache = ComponentCache.Get(typeof(BaseTeleportationInteractable), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as BaseTeleportationInteractable;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //传送对象
                EditorGUILayout.ObjectField(component, component.GetType(), true);

                UICommonFun.EndHorizontal();
            }
#endif

            CommonFun.EndLayout();
        }
    }
}
