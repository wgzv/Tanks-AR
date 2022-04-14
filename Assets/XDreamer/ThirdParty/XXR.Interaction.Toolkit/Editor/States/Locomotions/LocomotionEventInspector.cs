using UnityEditor;
using XCSJ.EditorSMS.States.Base;
using XCSJ.PluginXXR.Interaction.Toolkit.States.Locomotions;

namespace XCSJ.EditorXXR.Interaction.Toolkit.States.Locomotions
{
    /// <summary>
    /// 运动事件检查器
    /// </summary>
    [CustomEditor(typeof(LocomotionEvent))]
    public class LocomotionEventInspector : TriggerInspector<LocomotionEvent>
    {
    }
}
