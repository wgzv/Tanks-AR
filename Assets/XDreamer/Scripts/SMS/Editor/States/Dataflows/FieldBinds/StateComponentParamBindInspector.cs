using UnityEditor;
using XCSJ.PluginSMS.States.Dataflows.FieldBinds;

namespace XCSJ.EditorSMS.States.Dataflows.FieldBinds
{
    /// <summary>
    /// 状态组件参数绑定检查器
    /// </summary>
    [CustomEditor(typeof(StateComponentParamBind))]
    public class StateComponentParamBindInspector : FieldBindOfVirtualInspector<StateComponentParamBind>
    {

    }
}
