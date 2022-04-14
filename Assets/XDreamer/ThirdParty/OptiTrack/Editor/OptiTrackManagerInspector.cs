using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginOptiTrack;
using XCSJ.PluginOptiTrack.Base;
using XCSJ.PluginOptiTrack.Tools;

namespace XCSJ.EditorOptiTrack
{
    /// <summary>
    /// OptiTrack管理器检查器
    /// </summary>
    [CustomEditor(typeof(OptiTrackManager))]
    public class OptiTrackManagerInspector : BaseManagerInspector<OptiTrackManager>
    {
        #region 编译宏

        /// <summary>
        /// 宏
        /// </summary>
        public static readonly Macro XDREAMER_OPTITRACK = new Macro(nameof(XDREAMER_OPTITRACK), BuildTargetGroup.Standalone);

        /// <summary>
        /// 初始化宏
        /// </summary>
        [Macro]
        public static void InitMacro()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (TypeHelper.Exists("NaturalPoint.NatNetLib.NatNetClient"))
            {
                XDREAMER_OPTITRACK.DefineIfNoDefined();
            }
            else
#endif
            {
                XDREAMER_OPTITRACK.UndefineWithSelectedBuildTargetGroup();
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
                if (t == typeof(OptiTrackManager))
                {
                    EditorHelper.ImportPackageIfNeedWithDialog(XDREAMER_OPTITRACK, UnityPackagePath);
                }
            };

            EditorSceneManager.sceneOpened += (scene, mode) =>
            {
                UICommonFun.DelayCall(() =>
                {
                    if (OptiTrackManager.instance)
                    {
                        EditorHelper.ImportPackageIfNeedWithDialog(XDREAMER_OPTITRACK, UnityPackagePath);
                    }
                });
            };
        }

        private const string UnityPackagePath = "Assets/XDreamer-ThirdPartyUnityPackage/XDreamer_Pure_OptiTrack_Unity_Plugin_1.4.0.unitypackage";

        /// <summary>
        /// 启用时
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
#if XDREAMER_OPTITRACK
            var manager = this.manager;
            if (manager && manager.hasAccess)
            {
                manager.XGetOrAddComponent<OptitrackStreamingClient>();
            }
#endif
        }

        /// <summary>
        /// 当纵向绘制之前回调
        /// </summary>
        public override void OnBeforeVertical()
        {
            #region 检测是否需要导入UnityPackage

            EditorHelper.ImportPackageIfNeedWithButton(XDREAMER_OPTITRACK, UnityPackagePath);

            #endregion
            base.OnBeforeVertical();
        }

        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

#if XDREAMER_OPTITRACK

            if(GUILayout.Button("创建[OptiTrack流客户端]"))
            {
                manager.XGetOrAddComponent<OptitrackStreamingClient>();
            }
#else
            UICommonFun.RichHelpBox("当前工程未导入OptiTrack插件包！", MessageType.Warning);
#endif

            DrawDetailInfos();
        }

        /// <summary>
        /// OptiTrack刚体关联列表
        /// </summary>
        [Name("OptiTrack刚体关联列表")]
        [Tip("当前场景中所有与OptiTrack刚体关联的对象")]
        private static bool _display = true;

        private void DrawDetailInfos()
        {
            _display = UICommonFun.Foldout(_display, CommonFun.NameTip(GetType(), nameof(_display)));
            if (!_display) return;

            CommonFun.BeginLayout();

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("OptiTrack关联对象", "OptiTrack刚体关联的组件；本项只读；"));
            GUILayout.Label(CommonFun.TempContent("关联对象拥有者", "OptiTrack刚体关联对象拥有者所在的游戏对象；本项只读；"));
            GUILayout.Label(CommonFun.TempContent("激活启用", "OptiTrack刚体关联对象是否处于激活并启用的状态；本项只读；"), UICommonOption.Width60);
            GUILayout.Label(CommonFun.TempContent("刚体ID", "用于与Motive软件进行数据流通信的刚体ID；"), UICommonOption.Width60);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var cache = ComponentCache.Get(typeof(IOptiTrackObject), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as MonoBehaviour;
                var link = component as IOptiTrackObject;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //OptiTrack关联对象
                var gameObject = component.gameObject;
                EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

                //OptiTrack关联对象
                var owner = component.GetOptiTrackObjectOwnerGameObject();
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
