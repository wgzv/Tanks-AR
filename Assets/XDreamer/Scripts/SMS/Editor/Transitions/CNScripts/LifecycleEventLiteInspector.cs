using UnityEditor;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginSMS.CNScripts;
using XCSJ.PluginSMS.Transitions.CNScripts;

namespace XCSJ.EditorSMS.Transitions.CNScripts
{
    [CustomEditor(typeof(LifecycleEventLite))]
    public class LifecycleEventLiteInspector : TransitionScriptComponentInspector<LifecycleEventLite, ELifecycleEventLite, LifecycleEventLiteSet>
    {
    }
}
