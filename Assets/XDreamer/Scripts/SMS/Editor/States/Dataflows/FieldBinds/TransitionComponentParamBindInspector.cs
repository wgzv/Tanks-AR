using UnityEditor;
using XCSJ.PluginSMS.States.Dataflows.FieldBinds;

namespace XCSJ.EditorSMS.States.Dataflows.FieldBinds
{
    /// <summary>
    /// 跳转组件参数绑定检查器
    /// </summary>
    [CustomEditor(typeof(TransitionComponentParamBind))]
    public class TransitionComponentParamBindInspector : FieldBindOfVirtualInspector<TransitionComponentParamBind>
    {

    }
}
