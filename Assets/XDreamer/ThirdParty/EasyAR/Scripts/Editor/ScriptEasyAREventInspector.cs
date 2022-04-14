using UnityEngine;
using System.Collections;
using UnityEditor;
using XCSJ.PluginEasyAR;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
#if XDREAMER_EASYAR_4_0_1 || XDREAMER_EASYAR_3_0_1
using easyar;
#elif XDREAMER_EASYAR_2_3_0
using EasyAR;
#endif

namespace XCSJ.EditorEasyAR
{
    [CustomEditor(typeof(ScriptEasyAREvent))]
    public class ScriptEasyAREventInspector : BaseScriptEventInspector<ScriptEasyAREvent, EScriptEasyAREventType, ScriptEasyAREventSet>
    {
        [MenuItem(XDreamerMenu.ScriptEvent + "EasyAR/脚本EasyAR事件", false)]
        public static void CreateScriptEvent()
        {
            CreateComponentInternal();
        }

        [MenuItem(XDreamerMenu.ScriptEvent + "EasyAR/脚本EasyAR事件", true)]
        public static bool ValidateCreateScriptEvent()
        {
            return ValidateCreateComponentInternal() && EasyARManager.instance;
        }

        public override void OnInspectorGUI()
        {
#if XDREAMER_EASYAR_2_3_0
            base.OnInspectorGUI();
#else
            EditorGUILayout.HelpBox("当前版本不支持该组件。", MessageType.Error);
#endif
        }
    }
}
