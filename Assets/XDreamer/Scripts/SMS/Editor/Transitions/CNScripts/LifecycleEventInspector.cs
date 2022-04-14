using UnityEditor;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginSMS.CNScripts;
using XCSJ.PluginSMS.Transitions.CNScripts;

namespace XCSJ.EditorSMS.Transitions.CNScripts
{
    [CustomEditor(typeof(LifecycleEvent))]
    public class LifecycleEventInspector : TransitionScriptComponentInspector<LifecycleEvent, ELifecycleEvent, LifecycleEventSet>
    {
    }
}
