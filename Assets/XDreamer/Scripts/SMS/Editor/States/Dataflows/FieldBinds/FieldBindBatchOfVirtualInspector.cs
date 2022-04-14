using XCSJ.PluginSMS.States.Dataflows.FieldBinds;

namespace XCSJ.EditorSMS.States.Dataflows.FieldBinds
{
    /// <summary>
    /// 抽象型批量字段绑定绘制器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TBindInfo"></typeparam>
    public class FieldBindBatchOfVirtualInspector<T, TBindInfo> : FieldBindBatchInspector<T, TBindInfo>
        where T : FieldBindBatchOfVirtual<T, TBindInfo>
        where TBindInfo : BindInfoOfVirtual
    {
    }
}
