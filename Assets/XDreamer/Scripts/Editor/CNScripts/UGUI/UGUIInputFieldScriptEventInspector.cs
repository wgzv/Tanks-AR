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
    /// UGUI文本输入框脚本事件检查器
    /// </summary>
    [CanEditMultipleObjects, CustomEditor(typeof(UGUIInputFieldScriptEvent))]
    public class UGUIInputFieldScriptEventInspector : BaseScriptEventInspector<UGUIInputFieldScriptEvent, EUGUIInputFieldScriptEventType, UGUIInputFieldScriptEventSet>
    {
        [MenuItem(EditorScriptHelper.UGUIMenu + UGUIInputFieldScriptEvent.Title, false)]
        public static void CreateScriptEvent()
        {
            CreateComponentInternal();
        }

        [MenuItem(EditorScriptHelper.UGUIMenu + UGUIInputFieldScriptEvent.Title, true)]
        public static bool ValidateCreateScriptEvent()
        {
            return ValidateCreateComponentWithRequireInternal<InputField>();
        }
    }
}
