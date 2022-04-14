using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.CNScripts.Base;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.CNScripts.Base
{
    [CustomEditor(typeof(AnimationScriptEvent))]
    public class AnimationScriptEventInspector : BaseScriptEventInspector<AnimationScriptEvent, EAnimationScriptEventType, AnimationScriptEventSet>
    {
        [MenuItem(XDreamerMenu.ScriptEvent + AnimationScriptEvent.Title, false)]
        public static void CreateScriptEvent()
        {
            CreateComponentInternal();
        }

        [MenuItem(XDreamerMenu.ScriptEvent + AnimationScriptEvent.Title, true)]
        public static bool ValidateCreateScriptEvent()
        {
            return ValidateCreateComponentInternal();
        }
    }
}
