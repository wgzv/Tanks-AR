using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginZVR;
using XCSJ.PluginZVR.Base;
using XCSJ.PluginZVR.Tools;

namespace XCSJ.EditorZVR
{
    /// <summary>
    /// ZVR管理器检查器
    /// </summary>
    [CustomEditor(typeof(ZVRManager))]
    public class ZVRManagerInspector : BaseManagerInspector<ZVRManager>
    {
        #region 编译宏

        /// <summary>
        /// 宏
        /// </summary>
        public static readonly Macro XDREAMER_ZVR = new Macro(nameof(XDREAMER_ZVR), BuildTargetGroup.Standalone);

        /// <summary>
        /// 初始化宏
        /// </summary>
        [Macro]
        public static void InitMacro()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (TypeHelper.Exists("ActiveCenter.GokuLib.GokuClient"))
            {
                XDREAMER_ZVR.DefineIfNoDefined();
            }
            else
#endif
            {
                XDREAMER_ZVR.UndefineWithSelectedBuildTargetGroup();
            }
        }

        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        [InitializeOnLoadMethod]
        public static void Init()
        {
            InitMacro();
            PluginCommonUtilsRootInspector.onCreatedManager += (t) =>
            {
                if (t == typeof(ZVRManager))
                {
                    EditorHelper.ImportPackageIfNeedWithDialog(XDREAMER_ZVR, UnityPackagePath);
                }
            };

            EditorSceneManager.sceneOpened += (scene, mode) =>
            {
                UICommonFun.DelayCall(() =>
                {
                    if (ZVRManager.instance)
                    {
                        EditorHelper.ImportPackageIfNeedWithDialog(XDREAMER_ZVR, UnityPackagePath);
                    }
                });
            };
        }

        private const string UnityPackagePath = "Assets/XDreamer-ThirdPartyUnityPackage/zvrgoku_unity_plugin_v1.4.4.28.unitypackage";

        /// <summary>
        /// 启用时
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
#if XDREAMER_ZVR
            var manager = this.manager;
            if (manager && manager.hasAccess)
            {
                manager.XGetOrAddComponent<ZvrGokuStreamClient>();
            }
#endif
        }

        /// <summary>
        /// 当纵向绘制之前回调
        /// </summary>
        public override void OnBeforeVertical()
        {
            #region 检测是否需要导入UnityPackage

            EditorHelper.ImportPackageIfNeedWithButton(XDREAMER_ZVR, UnityPackagePath);

            #endregion
            base.OnBeforeVertical();
        }

        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

#if XDREAMER_ZVR

            if (GUILayout.Button("创建[ZVRGoku流客户端]"))
            {
                manager.XGetOrAddComponent<ZvrGokuStreamClient>();
            }
#else
            UICommonFun.RichHelpBox("当前工程未导入ZVR插件包！", MessageType.Warning);
#endif

            DrawDetailInfos();
        }

        /// <summary>
        /// ZVR刚体关联列表
        /// </summary>
        [Name("ZVR刚体关联列表")]
        [Tip("当前场景中所有与ZVR刚体关联的对象")]
        private static bool _display = true;

        private void DrawDetailInfos()
        {
            _display = UICommonFun.Foldout(_display, CommonFun.NameTip(GetType(), nameof(_display)));
            if (!_display) return;

            CommonFun.BeginLayout();

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("ZVR关联对象", "ZVR刚体关联的组件；本项只读；"));
            GUILayout.Label(CommonFun.TempContent("关联对象拥有者", "ZVR刚体关联对象拥有者所在的游戏对象；本项只读；"));
            GUILayout.Label(CommonFun.TempContent("激活启用", "ZVR刚体关联对象是否处于激活并启用的状态；本项只读；"), UICommonOption.Width60);
            GUILayout.Label(CommonFun.TempContent("刚体ID", "用于与Motive软件进行数据流通信的刚体ID；"), UICommonOption.Width60);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var cache = ComponentCache.Get(typeof(IZVRObject), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as MonoBehaviour;
                var link = component as IZVRObject;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //ZVR关联对象
                var gameObject = component.gameObject;
                EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

                //ZVR关联对象
                var owner = component.GetZVRObjectOwnerGameObject();
                EditorGUILayout.ObjectField(owner, typeof(GameObject), true);

                //激活启用
                EditorGUILayout.Toggle(component.isActiveAndEnabled, UICommonOption.Width60);

                //刚体ID
                EditorGUI.BeginChangeCheck();
                var rigidBodyID = EditorGUILayout.DelayedIntField(link.rigidBodyID, UICommonOption.Width60);
                if (EditorGUI.EndChangeCheck())
                {
                    link.rigidBodyID = rigidBodyID;
                }

                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }
    }
}
