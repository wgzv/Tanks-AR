using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorExtension.Base.XUnityEditor.PackageManager;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXAR.Foundation;
using XCSJ.PluginXAR.Foundation.Base.Tools;
using static XCSJ.PluginXAR.Foundation.Base.Tools.BaseTracker;

namespace XCSJ.EditorXAR.Foundation
{
    /// <summary>
    /// AR Foundation管理器检查器
    /// </summary>
    [CustomEditor(typeof(XARFoundationManager))]
    public class XARFoundationManagerInspector : BaseManagerInspector<XARFoundationManager>
    {
        /// <summary>
        /// 需要的所有依赖包;需要调用包管理器
        /// </summary>
        public const string PackageName = "com.unity.xr.arfoundation";

        /// <summary>
        /// 初始化
        /// </summary>
        [InitializeOnLoadMethod]
        public static void Init()
        {
            InitMacro();
            PluginCommonUtilsRootInspector.onCreatedManager += (t) =>
            {
                if (t == typeof(XARFoundationManager))
                {
                    EditorHelper.OpenPackageManagerIfNeedWithDialog(XDREAMER_AR_FOUNDATION, PackageName);
                }
            };

            EditorSceneManager.sceneOpened += (scene, mode) =>
            {
                UICommonFun.DelayCall(() =>
                {
                    if (XARFoundationManager.instance)
                    {
                        EditorHelper.OpenPackageManagerIfNeedWithDialog(XDREAMER_AR_FOUNDATION, PackageName);
                    }
                });
            };
        }

        #region 编译宏

        /// <summary>
        /// 宏
        /// </summary>
        public static readonly Macro XDREAMER_AR_FOUNDATION = new Macro(nameof(XDREAMER_AR_FOUNDATION), BuildTargetGroup.Standalone, BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.WSA);

        /// <summary>
        /// 初始化宏
        /// </summary>
        [Macro]
        public static void InitMacro()
        {
#if UNITY_2020_3_OR_NEWER
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
            if (TypeHelper.Exists("UnityEngine.XR.ARFoundation.ARSession")
                && PackageInfo_LinkType.GetPackageInfoByPackageName(PackageName) is UnityEditor.PackageManager.PackageInfo packageInfo
                && UICommonFun.NaturalCompare(packageInfo.version, "4.2.0") >= 0)
            {
                XDREAMER_AR_FOUNDATION.DefineIfNoDefined();
            }
            else
#endif
#endif
            {
                XDREAMER_AR_FOUNDATION.UndefineWithSelectedBuildTargetGroup();
            }
        }

        #endregion

        /// <summary>
        /// 当纵向绘制之前回调
        /// </summary>
        public override void OnBeforeVertical()
        {
            #region 检测是否需要打开包管理器

#if UNITY_2020_3_OR_NEWER
            EditorHelper.OpenPackageManagerIfNeedWithButton(XDREAMER_AR_FOUNDATION, PackageName);

#if !XDREAMER_AR_FOUNDATION
            UICommonFun.RichHelpBox("<color=red>请安装(或更新)到Unity的[" + XARFoundationHelper.Title + "]包[4.2.0](含)或更高版本！</color>", MessageType.Warning);
#endif

#else
            UICommonFun.RichHelpBox("<color=red>" + Product.Name + "提供的基于[" + XARFoundationHelper.Title + "]包的插件扩展，仅支持[Unity2020.3.0](含)或更高版本中使用！</color>", MessageType.Warning);      
#endif

            #endregion

            base.OnBeforeVertical();
        }

        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            #region 一键配置

#if XDREAMER_AR_FOUNDATION

            if (GUILayout.Button("一键配置"))
            {
#if UNITY_ANDROID

                //移除Vulkan
                Debug.Log("因AR Core不支持，移除图形API：" + GraphicsDeviceType.Vulkan);
                var types = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
                if (types.Any(t => t == GraphicsDeviceType.Vulkan))
                {
                    PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, types.Where(t => t != GraphicsDeviceType.Vulkan).ToArray());
                }
#endif
            }

#endif

            #endregion

            DrawDetailInfos();
        }

        /// <summary>
        /// 跟踪器列表
        /// </summary>
        [Name("跟踪器列表")]
        [Tip("当前场景中所有的跟踪器对象")]
        private static bool _display = true;

        private void DrawDetailInfos()
        {
            _display = UICommonFun.Foldout(_display, CommonFun.NameTip(GetType(), nameof(_display)));
            if (!_display) return;

            CommonFun.BeginLayout();

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("跟踪器", "跟踪器所在的游戏对象；本项只读；"));
            GUILayout.Label(CommonFun.TempContent("变换同步规则", "变换同步规则"), UICommonOption.Width120);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var cache = ComponentCache.Get(typeof(BaseTracker), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as BaseTracker;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //虚拟屏幕
                var gameObject = component.gameObject;
                EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

                //屏幕尺寸
                EditorGUI.BeginChangeCheck();
                var rule = UICommonFun.EnumPopup(component._transformSyncRule, UICommonOption.Width120);
                if (EditorGUI.EndChangeCheck())
                {
                    component.transformSyncRule = (ETransformSyncRule)rule;
                }

                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }
    }
}

