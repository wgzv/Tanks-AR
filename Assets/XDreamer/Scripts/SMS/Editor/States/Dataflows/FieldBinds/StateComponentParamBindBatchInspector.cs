using UnityEditor;
using XCSJ.PluginSMS.States.Dataflows.FieldBinds;

namespace XCSJ.EditorSMS.States.Dataflows.FieldBinds
{
    /// <summary>
    /// 状态组件参数绑定批量检查器
    /// </summary>
    [CustomEditor(typeof(StateComponentParamBindBatch))]
    public class StateComponentParamBindBatchInspector : FieldBindBatchOfVirtualInspector<StateComponentParamBindBatch, StateComponentBindInfo>
    {

    }
}
