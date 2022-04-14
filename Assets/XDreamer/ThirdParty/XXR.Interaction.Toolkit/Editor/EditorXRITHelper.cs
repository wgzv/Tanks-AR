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
#endif

namespace XCSJ.EditorXXR.Interaction.Toolkit
{
    /// <summary>
    /// 编辑器XRIT辅助类
    /// </summary>
    public static class EditorXRITHelper
    {
        /// <summary>
        /// 绘制打开XR交互调试器
        /// </summary>
        public static void DrawOpenXRInteractionDebugger()
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT

            if (GUILayout.Button("打开[XR交互调试器]"))
            {
                EditorApplication.ExecuteMenuItem("Window/Analysis/XR Interaction Debugger");
            }
#endif
        }

        /// <summary>
        /// 绘制选中X<see cref="XXRInteractionToolkitManager"/>
        /// </summary>
        public static void DrawSelectXRITManager()
        {
            if (GUILayout.Button("选中[" + XRITHelper.Title + "]管理器") && XXRInteractionToolkitManager.instance)
            {
                Selection.activeObject = XXRInteractionToolkitManager.instance;
            }
        }
    }
}
