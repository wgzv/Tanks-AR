using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.CNScripts.Base;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.CNScripts.Base
{
    /// <summary>
    /// MonoBehaviour脚本事件检查器
    /// </summary>
    [CanEditMultipleObjects, CustomEditor(typeof(MonoBehaviourScriptEvent))]
    public class MonoBehaviourScriptEventInspector : BaseScriptEventInspector<MonoBehaviourScriptEvent, EMonoBehaviourScriptEventType, MonoBehaviourScriptEventSet>
    {
        [MenuItem(XDreamerMenu.ScriptEvent + MonoBehaviourScriptEvent.Title, false)]
        public static void CreateScriptEvent()
        {
            CreateComponentInternal();
        }

        [MenuItem(XDreamerMenu.ScriptEvent + MonoBehaviourScriptEvent.Title, true)]
        public static bool ValidateCreateScriptEvent()
        {
            return ValidateCreateComponentInternal();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(new GUIContent("过多使用本组件，影响效率(推荐使用简版)！请谨慎使用^_^", "过多使用本组件，影响效率！请谨慎使用^_^"), UICommonOption.lableYellowBG, GUILayout.ExpandWidth(true));
            base.OnInspectorGUI();
        }
    }
}
