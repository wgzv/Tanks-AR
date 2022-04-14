using System;
using XCSJ.Extension.Base.Dataflows.Models;

namespace XCSJ.PluginDataflows.Models
{
    /// <summary>
    /// 普通可观察对象 ：用于可主动触发属性变化的类型
    /// </summary>
    public class ObservableObject : ISendPropertyChangeEvent
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
}
