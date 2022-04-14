using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginStereoView;
using XCSJ.PluginStereoView.Tools;

namespace XCSJ.EditorStereoView
{
    /// <summary>
    /// 立体显示管理器检查器
    /// </summary>
    [CustomEditor(typeof(StereoViewManager))]
    public class StereoViewManagerInspector : BaseManagerInspector<StereoViewManager>
    {
        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            CheckActiveStereoConfig();

            DrawDetailInfos();
            DrawStereoCameraDetailInfos();
        }

        string warnLog = "";

        private void CheckActiveStereoConfig()
        {
            if (GUILayout.Button("一键设置主动立体配置"))
            {
                SetConfig();
            }
            if(GUILayout.Button("一键设置主动立体配置-高级（*慎用*）"))
            {
                SetConfigHigh();
            }
            if (!string.IsNullOrEmpty(warnLog))
            {
                EditorGUILayout.HelpBox(warnLog, MessageType.Warning);
            }
        }

        private void SetConfig()
        {
            warnLog = "";
            if (EditorUserBuildSettings.selectedBuildTargetGroup != BuildTargetGroup.Standalone)
            {
                warnLog = "主动立体仅可在[" + BuildTargetGroup.Standalone + "]平台使用!";
                return;
            }
            PlayerSettings.fullScreenMode = FullScreenMode.ExclusiveFullScreen;//独占全屏模式
            PlayerSettings.forceSingleInstance = true;//强制单例
        }

        private const string VRSDK_Key = "stereo";//对应界面显示"Stereo Display (non head-mounted)"

        private void SetConfigHigh()
        {
            warnLog = "";
            if (EditorUserBuildSettings.selectedBuildTargetGroup != BuildTargetGroup.Standalone)
            {
                warnLog = "主动立体仅可在[" + BuildTargetGroup.Standalone + "]平台使用!";
                return;
            }
            PlayerSettings.fullScreenMode = FullScreenMode.ExclusiveFullScreen;//独占全屏模式
            PlayerSettings.forceSingleInstance = true;//强制单例
            {//设置首选图形API为OpenGLCore
                var apis64 = PlayerSettings.GetGraphicsAPIs(BuildTarget.StandaloneWindows64);
                if (apis64.IndexOf(GraphicsDeviceType.OpenGLCore) < 0)
                {
                    var list = apis64.ToList();
                    list.Insert(0, GraphicsDeviceType.OpenGLCore);
                    PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64, list.ToArray());
                }
            }
            {//设置首选图形API为OpenGLCore
                var apis = PlayerSettings.GetGraphicsAPIs(BuildTarget.StandaloneWindows);
                if (apis.IndexOf(GraphicsDeviceType.OpenGLCore) < 0)
                {
                    var list = apis.ToList();
                    list.Insert(0, GraphicsDeviceType.OpenGLCore);
                    PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows, list.ToArray());
                }
            }
            PlayerSettings.stereoRenderingPath = StereoRenderingPath.SinglePass;//单通道立体绘制模式
#if !UNITY_2020_1_OR_NEWER
            PlayerSettings.SetVirtualRealitySupported(BuildTargetGroup.Standalone, true);
            {//设置VR SDK
                var sdks = PlayerSettings.GetVirtualRealitySDKs(BuildTargetGroup.Standalone);
                if (sdks.IndexOf(VRSDK_Key) < 0)
                {
                    var list = sdks.ToList();
                    list.Insert(0, VRSDK_Key);
                    PlayerSettings.SetVirtualRealitySDKs(BuildTargetGroup.Standalone, list.ToArray());
                }
            }
#endif
            {//设置相机参数
                foreach (var camera in ComponentCache.GetComponents<Camera>())
                {
                    camera.renderingPath = RenderingPath.Forward;
                    camera.allowHDR = false;
                    camera.allowMSAA = false;
                }
            }
        }

        /// <summary>
        /// 虚拟屏幕列表
        /// </summary>
        [Name("虚拟屏幕列表")]
        [Tip("当前场景中所有的虚拟屏幕对象")]
        private static bool _display = true;

        private void DrawDetailInfos()
        {
            _display = UICommonFun.Foldout(_display, CommonFun.NameTip(GetType(), nameof(_display)));
            if (!_display) return;

            CommonFun.BeginLayout();

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("虚拟屏幕", "虚拟屏幕所在的游戏对象；本项只读；"), UICommonOption.Width200);
            GUILayout.Label(CommonFun.TempContent("屏幕尺寸", "虚拟屏幕的真实物理尺寸；X为宽，Y为高,Z为厚度；单位：米；"));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var cache = ComponentCache.Get(typeof(VirtualScreen), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as VirtualScreen;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //虚拟屏幕
                var gameObject = component.gameObject;
                EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true, UICommonOption.Width200);

                //屏幕尺寸
                EditorGUI.BeginChangeCheck();
                var screenSize = EditorGUILayout.Vector3Field("", component.screenSize);
                if (EditorGUI.EndChangeCheck())
                {
                    component.screenSize = screenSize;
                }

                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }

        /// <summary>
        /// 立体相机列表
        /// </summary>
        [Name("立体相机列表")]
        [Tip("当前场景中所有的立体相机组件对象")]
        private static bool _displayStereoCameras = true;

        private void DrawStereoCameraDetailInfos()
        {
            _displayStereoCameras = UICommonFun.Foldout(_displayStereoCameras, CommonFun.NameTip(GetType(), nameof(_displayStereoCameras)));
            if (!_displayStereoCameras) return;

            CommonFun.BeginLayout();

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("立体相机", "立体相机所在的游戏对象；本项只读；"));
            GUILayout.Label(CommonFun.TempContent("立体模式", "立体相机实现的立体模式；"), UICommonOption.Width120);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var hasActiveStereo = false;
            var cache = ComponentCache.Get(typeof(StereoCamera), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as StereoCamera;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //虚拟屏幕
                var gameObject = component.gameObject;
                EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

                //屏幕尺寸
                EditorGUI.BeginChangeCheck();
                var stereoMode = UICommonFun.EnumPopup(component.stereoMode, UICommonOption.Width120);
                if (EditorGUI.EndChangeCheck())
                {
                    component.stereoMode = (EStereoMode1)stereoMode;
                }
                hasActiveStereo = hasActiveStereo || component.stereoMode == EStereoMode1.ActiveStereo;

                UICommonFun.EndHorizontal();
            }

#if UNITY_STANDALONE
            if (!PlayerSettings.forceSingleInstance && hasActiveStereo)
            {
                UICommonFun.RichHelpBox("播放器设置[forceSingleInstance](强制单实例)必须为true时，才能支持主动立体！", MessageType.Warning);
                if (GUILayout.Button("设置[强制单实例]"))
                {
                    PlayerSettings.forceSingleInstance = true;
                }
            }
#endif

            CommonFun.EndLayout();
        }
    }
}
