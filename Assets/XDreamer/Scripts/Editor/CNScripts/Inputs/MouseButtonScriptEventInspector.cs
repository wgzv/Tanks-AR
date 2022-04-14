using System;
using UnityEditor;
using UnityEngine;
using XCSJ.PluginCommonUtils;
using XCSJ.Extension.CNScripts.Inputs;
using XCSJ.EditorCommonUtils;

namespace XCSJ.EditorExtension.CNScripts.Inputs
{
    /// <summary>
    /// 鼠标按钮脚本事件检查器
    /// </summary>
    [CanEditMultipleObjects, CustomEditor(typeof(MouseButtonScriptEvent))]
    public class MouseButtonScriptEventInspector : BaseScriptEventInspector<MouseButtonScriptEvent, EMouseButtonScriptEventType, MouseButtonScriptEventSet>
    {
        [MenuItem(EditorScriptHelper.InputMenu + MouseButtonScriptEvent.Title, false)]
        public static void CreateScriptEvent()
        {
            CreateComponentInternal();
        }

        [MenuItem(EditorScriptHelper.InputMenu + MouseButtonScriptEvent.Title, true)]
        public static bool ValidateCreateScriptEvent()
        {
            return ValidateCreateComponentInternal();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(new GUIContent("过多使用本组件，影响效率！请谨慎使用^_^", "过多使用本组件，影响效率！请谨慎使用^_^"), UICommonOption.lableYellowBG, GUILayout.ExpandWidth(true));
            base.OnInspectorGUI();
        }
    }
}
