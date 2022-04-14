using System;
using UnityEditor;
using UnityEngine;
using XCSJ.PluginCommonUtils;
using XCSJ.Extension.CNScripts.Inputs;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.CNScripts.Base;

namespace XCSJ.EditorExtension.CNScripts.Base
{
    /// <summary>
    /// MonoBehaviour脚本事件简版检查器
    /// </summary>
    [CanEditMultipleObjects, CustomEditor(typeof(MonoBehaviourScriptEventLite))]
    public class MonoBehaviourScriptEventLiteInspector : BaseScriptEventInspector<MonoBehaviourScriptEventLite, EMonoBehaviourScriptEventLiteType, MonoBehaviourScriptEventLiteSet>
    {
        [MenuItem(XDreamerMenu.ScriptEvent + MonoBehaviourScriptEventLite.Title, false)]
        public static void CreateScriptEvent()
        {
            CreateComponentInternal();
        }

        [MenuItem(XDreamerMenu.ScriptEvent + MonoBehaviourScriptEventLite.Title, true)]
        public static bool ValidateCreateScriptEvent()
        {
            return ValidateCreateComponentInternal();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Separator();
            base.OnInspectorGUI();
        }
    }
}
