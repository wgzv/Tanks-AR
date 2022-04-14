using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Events;
using XCSJ.Extension.Base.Inputs;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.AI.Autos
{
    /// <summary>
    /// 自动等待
    /// </summary>
    [Tool("AI")]
    [Name("自动等待")]
    [RequireManager(typeof(ToolsManager))]
    public abstract class AutoWait : ToolMB, IAwake, IOnEnable, IOnDisable, IUpdate, IInputEventSender
    {
        /// <summary>
        /// 启用后等待时间:组件启用后，等待指定时间，才执行对应的自动逻辑；单位：秒；
        /// </summary>
        [Group("时间")]
        [Name("启用后等待时间")]
        [Tip("组件启用后，等待指定时间，才执行对应的自动逻辑；单位：秒；")]
        [Range(0, 600)]
        public float waitTimeOfEnable = 3;

        /// <summary>
        /// 输入等待时间:无输入操作后，等待指定时间，才执行对应的自动逻辑；单位：秒；
        /// </summary>
        [Name("输入等待时间")]
        [Tip("无输入操作后，等待指定时间，才执行对应的自动逻辑；单位：秒；")]
        [Range(0, 600)]
        [EndGroup(true)]
        public float waitTimeOfInput = 3;

        private float waitedTime = 0;

        /// <summary>
        /// 标识能否更新
        /// </summary>
        public bool canUpdate => wait == EWait.Update;

        /// <summary>
        /// 等待标记量
        /// </summary>
        public EWait wait { get; private set; } = EWait.None;

        /// <summary>
        /// 等待枚举
        /// </summary>
        public enum EWait
        {
            /// <summary>
            /// 无
            /// </summary>
            None,

            /// <summary>
            /// 等待启用
            /// </summary>
            WaitForEnable,

            /// <summary>
            /// 等待输入
            /// </summary>
            WaitForInput,

            /// <summary>
            /// 更新
            /// </summary>
            Update,
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        public virtual void Awake()
        {
            this.Find(handlers);
        }

        /// <summary>
        /// 启用时
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            wait = EWait.WaitForEnable;
            waitedTime = 0;
        }

        /// <summary>
        /// 更新
        /// </summary>
        public virtual void Update()
        {
            //优先检测输入事件
            if (Input.anyKey)
            {
                wait = EWait.WaitForInput;
                waitedTime = 0;
            }

            switch (wait)
            {
                case EWait.WaitForEnable:
                    {
                        waitedTime += Time.deltaTime;
                        if (waitedTime >= waitTimeOfEnable)
                        {
                            wait = EWait.Update;
                        }
                        break;
                    }
                case EWait.WaitForInput:
                    {
                        waitedTime += Time.deltaTime;
                        if (waitedTime >= waitTimeOfInput)
                        {
                            wait = EWait.Update;
                        }
                        break;
                    }
            }           
        }

        #region IInputEventSender

        /// <summary>
        /// 事件处理器列表
        /// </summary>
        public List<IEventHandler> handlers { get; } = new List<IEventHandler>();

        /// <summary>
        /// 标识是否启用发送
        /// </summary>
        public bool enableSend { get; set; } = true;

        /// <summary>
        /// 标识是否启用输入
        /// </summary>
        public bool enableInput { get => enabled; set => enabled = value; }

        #endregion
    }
}
