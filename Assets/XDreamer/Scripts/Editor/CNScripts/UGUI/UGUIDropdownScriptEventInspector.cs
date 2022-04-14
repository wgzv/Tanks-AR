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
    /// UGUI下拉选择框脚本事件检查器
    /// </summary>
    [CanEditMultipleObjects, CustomEditor(typeof(UGUIDropdownScriptEvent))]
    public class UGUIDropdownScriptEventInspector : BaseScriptEventInspector<UGUIDropdownScriptEvent, EUGUIDropdownScriptEventType, UGUIDropdownScriptEventSet>
    {
        [MenuItem(EditorScriptHelper.UGUIMenu + UGUIDropdownScriptEvent.Title, false)]
        public static void CreateScriptEvent()
        {
            CreateComponentInternal();
        }

        [MenuItem(EditorScriptHelper.UGUIMenu + UGUIDropdownScriptEvent.Title, true)]
        public static bool ValidateCreateScriptEvent()
        {
            return ValidateCreateComponentWithRequireInternal<Dropdown>();
        }
    }
}
