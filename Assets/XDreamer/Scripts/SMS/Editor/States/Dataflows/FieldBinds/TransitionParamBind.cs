using UnityEditor;
using XCSJ.PluginSMS.States.Dataflows.FieldBinds;

namespace XCSJ.EditorSMS.States.Dataflows.FieldBinds
{
    /// <summary>
    /// 跳转参数绑定检查器
    /// </summary>
    [CustomEditor(typeof(TransitionParamBind))]
    public class TransitionParamBindInspector : FieldBindOfVirtualInspector<TransitionParamBind>
    {

    }
}
