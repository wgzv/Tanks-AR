using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.CNScripts;
using XCSJ.Extension.CNScripts.UGUI;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.CNScripts.UGUI
{
    /// <summary>
    /// UGUI触发脚本事件检查器
    /// </summary>
    [CanEditMultipleObjects, CustomEditor(typeof(UGUITriggerScriptEvent))]
    public class UGUITriggerScriptEventInspector : BaseScriptEventInspector<UGUITriggerScriptEvent, EUGUITriggerScriptEventType, UGUITriggerScriptEventSet>
    {
        [MenuItem(EditorScriptHelper.UGUIMenu + UGUITriggerScriptEvent.Title, false)]
        public static void CreateScriptEvent()
        {
            CreateComponentInternal();
        }

        [MenuItem(EditorScriptHelper.UGUIMenu + UGUITriggerScriptEvent.Title, true)]
        public static bool ValidateCreateScriptEvent()
        {
            return ValidateCreateComponentWithRequireInternal<RectTransform>();
        }
    }
}
