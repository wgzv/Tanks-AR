using System;
using UnityEditor;
using UnityEngine.UI;
using XCSJ.EditorExtension.CNScripts;
using XCSJ.EditorExtension.CNScripts.UGUI;
using XCSJ.PluginsCameras.Legacies.CNScripts;

namespace XCSJ.EditorCameras.Legacies.CNScripts
{
    /// <summary>
    /// UGUI操纵杆按钮(相机专用)脚本事件检查器
    /// </summary>
    [CustomEditor(typeof(UGUIJoystickButtonForCameraScriptEvent))]
    [Obsolete("产品功能升级，不推荐用户再使用本功能组件，请使用新的功能组件替代！", false)]
    public class UGUIJoystickButtonForCameraScriptEventInspector : UGUIJoystickButtonScriptEventInspector
    {
        /// <summary>
        /// 创建脚本事件
        /// </summary>
        [MenuItem(EditorScriptHelper.UGUIMenu + UGUIJoystickButtonForCameraScriptEvent.Title, false)]
        public new static void CreateScriptEvent()
        {
            CreateComponentInternal<UGUIJoystickButtonForCameraScriptEvent>();
        }

        /// <summary>
        /// 验证创建脚本事件
        /// </summary>
        /// <returns></returns>
        [MenuItem(EditorScriptHelper.UGUIMenu + UGUIJoystickButtonForCameraScriptEvent.Title, true)]
        public new static bool ValidateCreateScriptEvent()
        {
            return ValidateCreateComponentWithRequireInternal<UGUIJoystickButtonForCameraScriptEvent, Button>();
        }
    }
}
