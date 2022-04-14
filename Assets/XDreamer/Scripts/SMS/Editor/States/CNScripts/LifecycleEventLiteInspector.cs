using UnityEditor;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginSMS.CNScripts;
using XCSJ.PluginSMS.States.CNScripts;

namespace XCSJ.EditorSMS.States.CNScripts
{
    [CustomEditor(typeof(LifecycleEventLite))]
    public class LifecycleEventLiteInspector : StateScriptComponentInspector<LifecycleEventLite, ELifecycleEventLite, LifecycleEventLiteSet>
    {
    }
}
