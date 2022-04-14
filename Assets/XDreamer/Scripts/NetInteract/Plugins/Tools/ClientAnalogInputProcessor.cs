using System;
using System.Collections.Generic;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginNetInteract.Base;
using XCSJ.PluginNetInteract.CNScripts;
using XCSJ.PluginTools;

namespace XCSJ.PluginNetInteract.Tools
{
    /// <summary>
    /// 客户端模拟输入处理器:用于客户端处理模拟输入答案数据包
    /// </summary>
    [Name("客户端模拟输入处理器")]
    [Tip("用于客户端处理模拟输入答案数据包")]
    [Tool(NetInteractHelper.Title, nameof(NetInteractManager), nameof(Client), rootType = typeof(Client), groupRule = EToolGroupRule.None)]
    [RequireManager(typeof(NetInteractManager))]
    [XCSJ.Attributes.Icon(EIcon.JoyStick)]
    public class ClientAnalogInputProcessor : ToolMB
    {
        /// <summary>
        /// 客户端信息
        /// </summary>
        [Name("客户端信息")]
        public ClientInfo _clientInfo = new ClientInfo();

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            Client.onReceived += OnReceived;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            Client.onReceived -= OnReceived;
        }

        private bool ValidClient(Client client) => _clientInfo.ValidClient(client);

        private void OnReceived(Client client, NetAnswer netAnswer)
        {
            if (!(netAnswer is NetAnalogInputAnswer answer)) return;
            if (!ValidClient(client)) return;
            answer.netAnalogInput.Handle();
        }
    }
}
