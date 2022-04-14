using System.Reflection;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Dataflows.FieldBinds
{
    /// <summary>
    /// 抽象型批量字段绑定
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TBindInfo"></typeparam>
    public abstract class FieldBindBatchOfVirtual<T, TBindInfo> : FieldBindBatch<T, TBindInfo>
        where T : FieldBindBatchOfVirtual<T, TBindInfo>
        where TBindInfo : BindInfoOfVirtual
    {

    }

    /// <summary>
    /// 抽象型字段信息
    /// </summary>
    public abstract class BindInfoOfVirtual : BindInfo
    {
        /// <summary>
        /// 能否绑定
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public override bool CanBind(FieldInfo fieldInfo)
        {
            switch (fieldInfo.Name)
            {
                case "_" + nameof(IVirtual.enable):
                case "_" + nameof(IVirtual.parent):
                case nameof(WorkClip.workRange):
                    {
                        return false;
                    }
            }
            return base.CanBind(fieldInfo);
        }
    }

}

