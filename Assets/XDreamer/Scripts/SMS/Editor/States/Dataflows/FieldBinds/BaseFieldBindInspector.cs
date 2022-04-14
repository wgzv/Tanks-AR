using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginSMS.States.Dataflows.FieldBinds;

namespace XCSJ.EditorSMS.States.Dataflows.FieldBinds
{
    /// <summary>
    /// 基础字段绑定检查器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseFieldBindInspector<T> : StateComponentInspector<T> where T : BaseFieldBind<T>
    {
    }
}
