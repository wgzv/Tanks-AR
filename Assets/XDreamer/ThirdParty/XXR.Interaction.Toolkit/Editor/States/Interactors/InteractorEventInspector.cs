using UnityEditor;
using XCSJ.EditorSMS.States.Base;
using XCSJ.PluginXXR.Interaction.Toolkit.States.Interactors;

namespace XCSJ.EditorXXR.Interaction.Toolkit.States.Interactors
{
    /// <summary>
    /// 交互器事件检查器
    /// </summary>
    [CustomEditor(typeof(InteractorEvent))]
    public class InteractorEventInspector : TriggerInspector<InteractorEvent>
    {
    }
}
