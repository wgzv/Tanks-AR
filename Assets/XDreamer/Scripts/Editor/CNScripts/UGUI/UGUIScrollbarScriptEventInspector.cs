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
    /// UGUI滚动条脚本事件检查器
    /// </summary>
    [CanEditMultipleObjects, CustomEditor(typeof(UGUIScrollbarScriptEvent))]
    public class UGUIScrollbarScriptEventInspector : BaseScriptEventInspector<UGUIScrollbarScriptEvent, EUGUIScrollbarScriptEventType, UGUIScrollbarScriptEventSet>
    {
        [MenuItem(EditorScriptHelper.UGUIMenu + UGUIScrollbarScriptEvent.Title, false)]
        public static void CreateScriptEvent()
        {
            CreateComponentInternal();
        }

        [MenuItem(EditorScriptHelper.UGUIMenu + UGUIScrollbarScriptEvent.Title, true)]
        public static bool ValidateCreateScriptEvent()
        {
            return ValidateCreateComponentWithRequireInternal<Scrollbar>();
        }
    }
}
