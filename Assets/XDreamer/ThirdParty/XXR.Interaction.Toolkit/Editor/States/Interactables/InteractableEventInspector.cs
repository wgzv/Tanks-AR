using UnityEditor;
using XCSJ.EditorSMS.States.Base;
using XCSJ.PluginXXR.Interaction.Toolkit.States.Interactables;

namespace XCSJ.EditorXXR.Interaction.Toolkit.States.Interactables
{
    /// <summary>
    /// 可交互组件事件检查器
    /// </summary>
    [CustomEditor(typeof(InteractableEvent))]
    public class InteractableEventInspector : TriggerInspector<InteractableEvent>
    {
    }
}
