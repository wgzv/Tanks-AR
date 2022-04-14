using XCSJ.PluginSMS.States.Dataflows.FieldBinds;

namespace XCSJ.EditorSMS.States.Dataflows.FieldBinds
{
    /// <summary>
    /// 抽象型字段绑定检查器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FieldBindOfVirtualInspector<T> : FieldBindInspector<T> where T : FieldBindOfVirtual<T>
    {
    }
}