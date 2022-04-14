using UnityEditor;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginSMS.CNScripts;
using XCSJ.PluginSMS.States.CNScripts;

namespace XCSJ.EditorSMS.States.CNScripts
{
    [CustomEditor(typeof(LifecycleEvent))]
    public class LifecycleEventInspector : StateScriptComponentInspector<LifecycleEvent, ELifecycleEvent, LifecycleEventSet>
    {
    }
}
