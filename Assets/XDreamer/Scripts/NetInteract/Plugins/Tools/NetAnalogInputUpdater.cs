using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Inputs;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginNetInteract.Base;
using XCSJ.PluginXGUI.Views.Inputs;

namespace XCSJ.PluginNetInteract.Tools
{
    /// <summary>
    /// 网络模拟输入更新器
    /// </summary>
    [Name("网络模拟输入更新器")]
    [RequireManager(typeof(NetInteractManager))]
    [Tool(NetInteractHelper.Title, nameof(NetInteractManager))]
    [XCSJ.Attributes.Icon(EIcon.JoyStick)]
    public class NetAnalogInputUpdater : BaseAnalogInputUpdater
    {
        /// <summary>
        /// 网络端规则
        /// </summary>
        public enum ENetEndRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 客户端
            /// </summary>
            [Name("客户端")]
            Client,

            /// <summary>
            /// 服务器
            /// </summary>
            [Name("服务器")]
            Server,

            /// <summary>
            /// 两者
            /// </summary>
            [Name("两者")]
            Both,
        }

        /// <summary>
        /// 网络端规则
        /// </summary>
        [Name("网络端规则")]
        [EnumPopup]
        public ENetEndRule _netEndRule = ENetEndRule.Client;

        /// <summary>
        /// 客户端列表
        /// </summary>
        [Name("客户端列表")]
        public List<Client> _clients = new List<Client>();

        /// <summary>
        /// 服务器列表
        /// </summary>
        [Name("服务器列表")]
        public List<Server> _servers = new List<Server>();

        protected void Reset()
        {
            AutoAddNetObject(_netEndRule);
        }

        /// <summary>
        /// 仅用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            AutoAddNetObject(_netEndRule);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
        }

        private void AutoAddNetObject(ENetEndRule netEndRule)
        {
            switch (netEndRule)
            {
                case ENetEndRule.Client:
                    {
                        if (_clients.Count==0 || !_clients.Exists(c => c))
                        {
                            var clientArray = FindObjectsOfType(typeof(Client));
                            if (clientArray.Length > 0)
                            {
                                this.XModifyProperty(() => _clients.AddRange(clientArray.Cast<Client>()));
                            }
                        }
                        break;
                    }
                case ENetEndRule.Server:
                    {
                        if (_servers.Count == 0 || !_servers.Exists(s => s))
                        {
                            var serverArray = FindObjectsOfType(typeof(Server));
                            if (serverArray.Length > 0)
                            {
                                this.XModifyProperty(() => _servers.AddRange(serverArray.Cast<Server>()));
                            }
                        }
                        break;
                    }
                case ENetEndRule.Both:
                    {
                        AutoAddNetObject(ENetEndRule.Client);
                        AutoAddNetObject(ENetEndRule.Server);
                        break;
                    }
            }
        }

        private void ClientSend(NetAnalogInput netAnalogInput)
        {
            foreach(var c in _clients)
            {
                if (c) c.Send(netAnalogInput);
            }
        }

        private void ServertSend(NetAnalogInput netAnalogInput)
        {
            foreach (var s in _servers)
            {
                if (s) s.Broadcast(netAnalogInput);
            }
        }

        /// <summary>
        /// 更新轴
        /// </summary>
        /// <param name="input"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public override void UpdateAxis(IInput input, string name, float value)
        {
            NetAnalogInput netAnalogInput = new NetAnalogInput() { input = input.input, name = name, value = value, inputUpdateType = NetAnalogInput.EInputUpdateType.Axis };
            switch (_netEndRule)
            {
                case ENetEndRule.Client:
                    {
                        ClientSend(netAnalogInput);
                        break;
                    }
                case ENetEndRule.Server:
                    {
                        ServertSend(netAnalogInput);
                        break;
                    }
                case ENetEndRule.Both:
                    {
                        ClientSend(netAnalogInput);
                        ServertSend(netAnalogInput);
                        break;
                    }
            }
        }

        /// <summary>
        /// 更新按钮
        /// </summary>
        /// <param name="input"></param>
        /// <param name="name"></param>
        /// <param name="downOrUp"></param>
        public override void UpdateButton(IInput input, string name, bool downOrUp)
        {
            NetAnalogInput netAnalogInput = new NetAnalogInput() { input = input.input, name = name, downOrUp = downOrUp, inputUpdateType = NetAnalogInput.EInputUpdateType.Button };
            switch (_netEndRule)
            {
                case ENetEndRule.Client:
                    {
                        ClientSend(netAnalogInput);
                        break;
                    }
                case ENetEndRule.Server:
                    {
                        ServertSend(netAnalogInput);
                        break;
                    }
                case ENetEndRule.Both:
                    {
                        ClientSend(netAnalogInput);
                        ServertSend(netAnalogInput);
                        break;
                    }
            }
        }
    }

    /// <summary>
    /// 网络模拟输入
    /// </summary>
    [Serializable]
    [Import]
    public class NetAnalogInput
    {
        /// <summary>
        /// 输入
        /// </summary>
        public EInput input { get; set; } = default;

        /// <summary>
        /// 输入更新类型
        /// </summary>
        public enum EInputUpdateType
        {
            /// <summary>
            /// 无
            /// </summary>
            None,

            /// <summary>
            /// 轴
            /// </summary>
            Axis,

            /// <summary>
            /// 按钮
            /// </summary>
            Button,
        }

        /// <summary>
        /// 输入更新类型
        /// </summary>
        public EInputUpdateType inputUpdateType { get; set; } = EInputUpdateType.None;

        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; } = "";

        /// <summary>
        /// 值
        /// </summary>
        public float value { get; set; } = 0;

        /// <summary>
        /// 按下或弹起
        /// </summary>
        public bool downOrUp { get; set; } = false;

        /// <summary>
        /// 转网络模拟输入问题
        /// </summary>
        /// <returns></returns>
        public NetAnalogInputQuestion ToNetQuestion()
        {
            var question = new NetAnalogInputQuestion() { questionCode = EQuestionCode.Valid };
            question.netAnalogInput = this;
            return question;
        }

        /// <summary>
        /// 隐式转换为网络模拟输入问题
        /// </summary>
        /// <param name="netAnalogInput"></param>
        public static implicit operator NetAnalogInputQuestion(NetAnalogInput netAnalogInput) => netAnalogInput?.ToNetQuestion();

        /// <summary>
        /// 转网络模拟输入答案
        /// </summary>
        /// <returns></returns>
        public NetAnalogInputAnswer ToNetAnswer()
        {
            var answer = new NetAnalogInputAnswer() { answerCode = EAnswerCode.Valid };
            answer.netAnalogInput = this;
            return answer;
        }

        /// <summary>
        /// 隐式转换为网络模拟输入答案
        /// </summary>
        /// <param name="netAnalogInput"></param>
        public static implicit operator NetAnalogInputAnswer(NetAnalogInput netAnalogInput) => netAnalogInput?.ToNetAnswer();

        /// <summary>
        /// 处理
        /// </summary>
        public void Handle()
        {
            //Debug.LogFormat("{0}: {1}: {2}: {3}: {4}", input, inputUpdateType, name, value, downOrUp);
            switch (inputUpdateType)
            {
                case EInputUpdateType.Axis:
                    {
                        input.UpdateAxis(name, value);
                        break;
                    }
                case EInputUpdateType.Button:
                    {
                        input.UpdateButton(name, downOrUp);
                        break;
                    }
            }
        }
    }

    /// <summary>
    /// 网络模拟输入问题
    /// </summary>
    [Import]
    public class NetAnalogInputQuestion : NetQuestion
    {
        /// <summary>
        /// 网络模拟输入
        /// </summary>
        public NetAnalogInput netAnalogInput { get; set; } = new NetAnalogInput();
    }

    /// <summary>
    /// 网路模拟输入答案
    /// </summary>
    [Import]
    public class NetAnalogInputAnswer : NetAnswer
    {
        /// <summary>
        /// 网络模拟输入
        /// </summary>
        public NetAnalogInput netAnalogInput { get; set; } = new NetAnalogInput();
    }
}
