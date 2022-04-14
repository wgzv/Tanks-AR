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
    /// 服务器中文脚本处理器:用于服务器处理中文脚本命令或其返回值的处理
    /// </summary>
    [Name("服务器中文脚本处理器")]
    [Tip("用于服务器处理中文脚本命令或其返回值的处理")]
    [Tool(NetInteractHelper.Title, nameof(NetInteractManager), nameof(Server), rootType = typeof(Server), groupRule = EToolGroupRule.None)]
    [RequireManager(typeof(NetInteractManager))]
    [XCSJ.Attributes.Icon(EIcon.CNScript)]
    public class ServerCNScriptProcessor : ToolMB
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
            if (!(netQuestion is NetCNScriptQuestion question)) return;
            if (!ValidServer(server)) return;
            var package = question.scriptPackage.Handle();
            switch (package.packageType)
            {
                case ENetCNScriptPackageType.ReturnSuccess:
                case ENetCNScriptPackageType.ReturnFail:
                    {
                        netQuestion.client?.Send(package);
                        break;
                    }
            }
        }
    }

    /// <summary>
    /// 服务器信息
    /// </summary>
    [Serializable]
    public class ServerInfo
    {
        /// <summary>
        /// 服务器规则
        /// </summary>
        public enum EServerRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 任意
            /// </summary>
            [Name("任意")]
            Any,

            /// <summary>
            /// 列表中
            /// </summary>
            [Name("列表中")]
            InList,

            /// <summary>
            /// 不在列表中
            /// </summary>
            [Name("不在列表中")]
            NotInList,
        }

        /// <summary>
        /// 服务器规则
        /// </summary>
        [Name("服务器规则")]
        [EnumPopup]
        public EServerRule _serverRule = EServerRule.Any;

        /// <summary>
        /// 服务器列表
        /// </summary>
        [Name("服务器列表")]
        public List<Server> _servers = new List<Server>();

        /// <summary>
        /// 有效服务器
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public bool ValidServer(Server server)
        {
            if (!server) return false;
            switch (_serverRule)
            {
                case EServerRule.Any: return true;
                case EServerRule.InList: return _servers.Contains(server);
                case EServerRule.NotInList: return !_servers.Contains(server);
                default: return false;
            }
        }
    }
}
