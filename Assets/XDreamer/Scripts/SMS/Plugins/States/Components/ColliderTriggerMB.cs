using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;

namespace XCSJ.PluginSMS.States.Components
{
    /// <summary>
    /// 碰撞体触发器组件
    /// </summary>
    [Name("碰撞体触发器组件")]
    [RequireManager(typeof(SMSManager))]
    public class ColliderTriggerMB : ToolMB, IOnTrigger
    {
        /// <summary>
        /// 当触发器进入
        /// </summary>
        public event Action<ColliderTriggerMB, Collider> onTriggerEnter;

        /// <summary>
        /// 当触发器保持
        /// </summary>
        public event Action<ColliderTriggerMB, Collider> onTriggerStay;

        /// <summary>
        /// 当触发器退出
        /// </summary>
        public event Action<ColliderTriggerMB, Collider> onTriggerExit;

        /// <summary>
        /// 当触发器进入
        /// </summary>
        /// <param name="collider"></param>
        public void OnTriggerEnter(Collider collider)
        {
            onTriggerEnter?.Invoke(this, collider);
        }

        /// <summary>
        /// 当触发器退出
        /// </summary>
        /// <param name="collider"></param>
        public void OnTriggerExit(Collider collider)
        {
            onTriggerExit?.Invoke(this, collider);
        }

        /// <summary>
        /// 当触发器保持
        /// </summary>
        /// <param name="collider"></param>
        public void OnTriggerStay(Collider collider)
        {
            onTriggerStay?.Invoke(this, collider);
        }
    }
}
