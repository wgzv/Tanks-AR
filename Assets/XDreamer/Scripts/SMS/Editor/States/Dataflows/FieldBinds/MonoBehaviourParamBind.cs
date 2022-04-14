using UnityEditor;
using XCSJ.PluginSMS.States.Dataflows.FieldBinds;

namespace XCSJ.EditorSMS.States.Dataflows.FieldBinds
{
    /// <summary>
    /// MonoBehaviour参数绑定检查器
    /// </summary>
    [CustomEditor(typeof(MonoBehaviourParamBind))]
    public class MonoBehaviourParamBindInspector : FieldBindInspector<MonoBehaviourParamBind>
    {

    }
}
