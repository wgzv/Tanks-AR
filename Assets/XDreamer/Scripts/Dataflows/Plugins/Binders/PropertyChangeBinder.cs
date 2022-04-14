using XCSJ.Extension.Base.Dataflows.DataBinders;
using XCSJ.Extension.Base.Dataflows.Models;
using XCSJ.PluginDataflows.Models;

namespace XCSJ.PluginDataflows.Binders
{
    /// <summary>
    /// 绑定对象为可观察对象
    /// </summary>
    [DataBinder(typeof(ObservableMB))]
    [DataBinder(typeof(ObservableObject))]
    [DataBinder(typeof(ObservableSO))]
    public class PropertyChangeBinder : TypeMemberDataBinder<ISendPropertyChangeEvent>
    {
        /// <summary>
        /// 绑定主体对象
        /// </summary>
        protected override void BindMainObjectSendEvent()
        {
            if (target != null)
            {
                target.sendEvent += Transfer;
            }
        }

        /// <summary>
        /// 解绑主体对象
        /// </summary>
        protected override void UnbindMainObjectSendEvent()
        {
            if (target != null)
            {
                target.sendEvent -= Transfer;
            }
        }

        /// <summary>
        /// 中转发送，将发送者转为包装类对象
        /// 过滤发送参数：发送属性名称匹配当前记录的属性名称，才进行转发，防止出现通知对象混乱的状态
        /// </summary>
        /// <param name="eventArg"></param>
        protected void Transfer(XValueEventArg eventArg)
        {
            if (eventArg is XPropertyChangeEventArgs arg && typeMemberBinder != null && typeMemberBinder.memberInfo.Name == arg.propertyName)
            {
                eventArg.sender = this;
                SendEvent(arg);
            }
        }
    }
}
