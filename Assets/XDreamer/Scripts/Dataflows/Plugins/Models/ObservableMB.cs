using System;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Models;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginDataflows.Models
{
    /// <summary>
    /// 可观察MB对象 ：用于可主动触发属性变化的类型
    /// </summary>
    [Name("可观察")]
    public class ObservableMB : BaseDataflowMB, ISendPropertyChangeEvent
    {
        /// <summary>
        /// 发送值变化事件
        /// </summary>
        private event Action<XValueEventArg> _sendEvent;

        /// <summary>
        /// 发送值变化事件
        /// </summary>
        public event Action<XValueEventArg> sendEvent
        {
            add => _sendEvent += value;
            remove => _sendEvent -= value;
        }

        /// <summary>
        /// 值将改变事件
        /// </summary>
        /// <param name="xEventArg"></param>
        public void SendPropertyWillChange(XPropertyChangeEventArgs xEventArg) => _sendEvent?.Invoke(xEventArg);

        /// <summary>
        /// 值已改变事件
        /// </summary>
        /// <param name="xEventArg"></param>
        public void SendPropertyChanged(XPropertyChangeEventArgs xEventArg) => _sendEvent?.Invoke(xEventArg);
    }

    /// <summary>
    /// 基础数据流MB
    /// </summary>
    [RequireManager(typeof(DataflowManager))]
    public abstract class BaseDataflowMB : MB { }
}
