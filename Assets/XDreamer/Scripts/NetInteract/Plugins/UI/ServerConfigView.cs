using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginNetInteract.Tools;

namespace XCSJ.PluginNetInteract.UI
{
    /// <summary>
    /// 服务器配置视图
    /// </summary>
    [Name("服务器配置视图")]
    [RequireManager(typeof(NetInteractManager))]
    public class ServerConfigView : MB
    {
        /// <summary>
        /// 服务器
        /// </summary>
        [Name("服务器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Server _server;

        /// <summary>
        /// 服务器
        /// </summary>
        public Server server => this.XGetComponentInChildrenOrGlobal(ref _server);

        /// <summary>
        /// 服务器状态
        /// </summary>
        [Name("服务器状态")]
        [Tip("服务器是否在线")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Text _onLineText;

        /// <summary>
        /// 连入客户端信息
        /// </summary>
        [Name("连入客户端信息")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Text _clientInfoText;

        /// <summary>
        /// IP类型
        /// </summary>
        [Name("IP类型")]
        public EIPType _ipType = EIPType.HostAddresseIPV4;

        /// <summary>
        /// IP
        /// </summary>
        [Name("IP")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Text _ipText;

        public enum EIPType
        {
            [Name("主机IP地址")]
            HostAddresse,

            [Name("主机IPV4地址")]
            HostAddresseIPV4,

            [Name("主机IPV6地址")]
            HostAddresseIPV6
        }

        /// <summary>
        /// 端口
        /// </summary>
        [Name("端口")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public InputField _portInputField;

        /// <summary>
        /// 启用按钮
        /// </summary>
        [Name("启用按钮")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Readonly(EEditorMode.Runtime)]
        public Button _startButton;

        /// <summary>
        /// 停止按钮
        /// </summary>
        [Name("停止按钮")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Readonly(EEditorMode.Runtime)]
        public Button _stopButton;

        /// <summary>
        /// 定时更新
        /// </summary>
        [Name("定时更新")]
        public bool _updateOnTimer = true;

        /// <summary>
        /// 更新时间间隔
        /// </summary>
        [Name("更新时间间隔")]
        [Min(0)]
        [HideInSuperInspector(nameof(_updateOnTimer), EValidityCheckType.False)]
        public float _updateTime = 3;

        private float timeCounter = 0;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            UpdateUI();

            timeCounter = 0;

            if (_startButton)
            {
                _startButton.onClick.AddListener(OnStartButtonClick);
            }
            if (_stopButton)
            {
                _stopButton.onClick.AddListener(OnStopButtonClick);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();

            if (_startButton)
            {
                _startButton.onClick.RemoveListener(OnStartButtonClick);
            }
            if (_stopButton)
            {
                _stopButton.onClick.RemoveListener(OnStopButtonClick);
            }
        }

        private void Update()
        {
            if (!_updateOnTimer) return;

            if (timeCounter < _updateTime)
            {
                timeCounter += Time.deltaTime;
            }
            else
            {
                timeCounter = 0;
                UpdateUI();
            }
        }

        private void OnStartButtonClick()
        {
            if (server)
            {
                if (_portInputField)
                {
                    if (int.TryParse(_portInputField.text, out var port) && port > 0)
                    {
                        server.port = port;
                    }
                }
                server.StartServerAndTrySyncObject();
            }
        }

        private void OnStopButtonClick()
        {
            if (server)
            {
                server.StopServerAndSyncObject();
            }
        }

        private void UpdateUI()
        {
            if (!server) return;

            UpdateServerState();
            UpdateClientInfo();
            UpdateIPAndPort();
            UpdateButton();
        }

        private void UpdateServerState()
        {
            if (_onLineText)
            {
                _onLineText.text = server.isOnLine ? "在线" : "离线";
            }
        }

        private void UpdateClientInfo()
        {
            if (_clientInfoText)
            {
                _clientInfoText.text = server._server.clients.count.ToString();
            }
        }

        private void UpdateIPAndPort()
        {
            if (_ipText)
            {
                _ipText.text = GetHostIP(_ipType);
            }
            if (_portInputField && string.IsNullOrEmpty(_portInputField.text))
            {
                _portInputField.text = server.port.ToString();
            }
        }

        private string GetHostIP(EIPType ipType)
        {
            var hostName = Dns.GetHostName();
            switch (ipType)
            {
                case EIPType.HostAddresse:
                    {
                        return Dns.GetHostAddresses(hostName).ToStringDirect();
                    }
                case EIPType.HostAddresseIPV4:
                    {
                        return Dns.GetHostAddresses(hostName).Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToStringDirect();
                    }
                case EIPType.HostAddresseIPV6:
                    {
                        return Dns.GetHostAddresses(hostName).Where(ip => ip.AddressFamily == AddressFamily.InterNetworkV6).ToStringDirect();
                    }
                default: return "";
            }
        }

        private void UpdateButton()
        {
            var isOnLine = server.isOnLine;
            if (_startButton)
            {
                _startButton.interactable = !isOnLine;
            }
            if (_stopButton)
            {
                _stopButton.interactable = isOnLine;
            }
        }
    }
}
