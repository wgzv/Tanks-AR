using UnityEditor;
using UnityEditor.SceneManagement;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.Helper;
using XCSJ.PluginVoxelTracker;

namespace XCSJ.EditorVoxelTracker
{
    /// <summary>
    /// VoxelTracker检查器
    /// </summary>
    [CustomEditor(typeof(VoxelTrackerManager))]
    public class VoxelTrackerManagerInspector : BaseManagerInspector<VoxelTrackerManager>
    {
        #region 编译宏

        /// <summary>
        /// 宏
        /// </summary>
        public static readonly Macro XDREAMER_VOXELTRACKER = new Macro(nameof(XDREAMER_VOXELTRACKER), BuildTargetGroup.Standalone);

        /// <summary>
        /// 初始化宏
        /// </summary>
        [Macro]
        public static void InitMacro()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (TypeHelper.Exists("VoxelStationUtil.VoxelCore"))
            {
                XDREAMER_VOXELTRACKER.DefineIfNoDefined();
            }
            else
#endif
            {
                XDREAMER_VOXELTRACKER.UndefineWithSelectedBuildTargetGroup();
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
                if (t == typeof(VoxelTrackerManager))
                {
                    EditorHelper.ImportPackageIfNeedWithDialog(XDREAMER_VOXELTRACKER, UnityPackagePath);
                }
            };

            EditorSceneManager.sceneOpened += (scene, mode) =>
            {
                UICommonFun.DelayCall(() =>
                {
                    if (VoxelTrackerManager.instance)
                    {
                        EditorHelper.ImportPackageIfNeedWithDialog(XDREAMER_VOXELTRACKER, UnityPackagePath);
                    }
                });
            };
        }

        private const string UnityPackagePath = "Assets/XDreamer-ThirdPartyUnityPackage/VoxelSense SDK for Unity v1.4.0.unitypackage";

        /// <summary>
        /// 当纵向绘制之前回调
        /// </summary>
        public override void OnBeforeVertical()
        {
            #region 检测是否需要导入UnityPackage

            EditorHelper.ImportPackageIfNeedWithButton(XDREAMER_VOXELTRACKER, UnityPackagePath);

            #endregion
            base.OnBeforeVertical();
        }

        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

#if XDREAMER_VOXELTRACKER

            
#else
            UICommonFun.RichHelpBox("当前工程未导入VoxelTracker插件包！", MessageType.Warning);
#endif

            //DrawDetailInfos();
        }
    }
}
