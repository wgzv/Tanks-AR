using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Net;
using XCSJ.Net.Tcp;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginNetInteract.Base;
using XCSJ.PluginNetInteract.CNScripts;
using XCSJ.PluginNetInteract.Tools;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginNetInteract.States
{
    /// <summary>
    /// 服务器发送消息:服务器发送消息到客户端
    /// </summary>
    [Name(Title)]
    [Tip("服务器发送消息到客户端")]
    [ComponentMenu(NetInteractHelper.Server + "/" + Title, typeof(NetInteractManager))]
    [XCSJ.Attributes.Icon(EIcon.Export)]
    public class ServerSendMsg : LifecycleExecutor<ServerSendMsg>, IGetScriptSet
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "服务器发送消息";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(NetInteractHelper.Server, typeof(NetInteractManager))]
        [StateComponentMenu(NetInteractHelper.Server + "/" + Title, typeof(NetInteractManager))]
        [Name(Title, nameof(ServerSendMsg))]
        [Tip("服务器发送消息到客户端")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 服务器
        /// </summary>
        [Name("服务器")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Server _server;

        /// <summary>
        /// 服务器
        /// </summary>
        public Server server => this.XGetComponentInGlobal(ref _server);

        /// <summary>
        /// 网络消息类型
        /// </summary>
        [Name("网络消息类型")]
        [EnumPopup]
        public ENetMsgType _netMsgType = ENetMsgType.Msg;

        /// <summary>
        /// 网络消息
        /// </summary>
        [Name("网络消息")]
        [HideInSuperInspector(nameof(_netMsgType), EValidityCheckType.NotEqual, ENetMsgType.Msg)]
        public NetMsg _netMsg = new NetMsg();

        /// <summary>
        /// 网络中文脚本
        /// </summary>
        [Name("网络中文脚本")]
        [HideInSuperInspector(nameof(_netMsgType), EValidityCheckType.NotEqual, ENetMsgType.CNScript)]
        public NetCNScript _netCNScript = new NetCNScript();

        /// <summary>
        /// 发送规则
        /// </summary>
        public enum ESendRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 广播:将网络消息广播到所有已连接到服务器的客户端；
            /// </summary>
            [Name("广播")]
            [Tip("将网络消息广播到所有已连接到服务器的客户端")]
            Broadcast,
        }

        /// <summary>
        /// 发送规则
        /// </summary>
        [Name("发送规则")]
        [EnumPopup]
        public ESendRule _sendRule = ESendRule.Broadcast;

        /// <summary>
        /// 获取网络问题
        /// </summary>
        /// <returns></returns>
        public NetAnswer GetNetAnswer()
        {
            switch (_netMsgType)
            {
                case ENetMsgType.CNScript: return _netCNScript;
                case ENetMsgType.Msg: return _netMsg;
                default: return NetAnswer.HeatBeatAnswer;
            }
        }

        private IDataValidity GetDataValidityObject()
        {
            switch (_netMsgType)
            {
                case ENetMsgType.CNScript: return _netCNScript;
                case ENetMsgType.Msg: return _netMsg;
                default: return NetAnswer.HeatBeatAnswer;
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            var server = this.server;
            if (!server) return;
            switch(_sendRule)
            {
                case ESendRule.Broadcast:
                    {
                        server.Broadcast(GetNetAnswer());
                        break;
                    }
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            if (!_server || !GetDataValidityObject().DataValidity()) return false;
            return base.DataValidity();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (!server) { }
        }

        ScriptSet IGetScriptSet.GetScriptSet(string propertyPath) => _netCNScript._scriptSet._value;

        /// <summary>
        /// 提示
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            switch (_netMsgType)
            {
                case ENetMsgType.Msg:
                    {
                        return _netMsg.GetTip();
                    }
                case ENetMsgType.CNScript:
                    {
                        return CommonFun.Name(_netMsgType);
                    }
            }
            return "";
        }
    }
}
