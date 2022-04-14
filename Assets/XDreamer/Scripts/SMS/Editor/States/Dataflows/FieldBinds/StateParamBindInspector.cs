using UnityEditor;
using XCSJ.PluginSMS.States.Dataflows.FieldBinds;

namespace XCSJ.EditorSMS.States.Dataflows.FieldBinds
{
    /// <summary>
    /// 状态参数绑定检查器
    /// </summary>
    [CustomEditor(typeof(StateParamBind))]
    public class StateParamBindInspector : FieldBindOfVirtualInspector<StateParamBind>
    {

    }
}
