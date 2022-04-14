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
    /// 服务器模拟输入处理器:用于服务器处理模拟输入问题数据包
    /// </summary>
    [Name("服务器模拟输入处理器")]
    [Tip("用于服务器处理模拟输入问题数据包")]
    [Tool(NetInteractHelper.Title, nameof(NetInteractManager), nameof(Server), rootType = typeof(Server), groupRule = EToolGroupRule.None)]
    [RequireManager(typeof(NetInteractManager))]
    [XCSJ.Attributes.Icon(EIcon.JoyStick)]
    public class ServerAnalogInputProcessor : ToolMB
    {
        /// <summary>
        /// 服务器信息
        /// </summary>
        [Name("服务器信息")]
        public ServerInfo serverInfo = new ServerInfo();

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            Server.onReceived += OnReceived;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            Server.onReceived -= OnReceived;
        }

        private bool ValidServer(Server server) => serverInfo.ValidServer(server);

        private void OnReceived(Server server, NetQuestion netQuestion)
        {
            if (!(netQuestion is NetAnalogInputQuestion question)) return;
            if (!ValidServer(server)) return;
            question.netAnalogInput.Handle();
        }
    }
}
