using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginNetInteract.Tools;

namespace XCSJ.PluginNetInteract.UI
{
    /// <summary>
    /// 客户端配置视图
    /// </summary>
    [Name("客户端配置视图")]
    [RequireManager(typeof(NetInteractManager))]
    public class ClientConfigView : MB
    {
        /// <summary>
        /// 客户端
        /// </summary>
        [Name("客户端")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Client _client;

        /// <summary>
        /// 客户端
        /// </summary>
        public Client client => this.XGetComponentInChildrenOrGlobal(ref _client);

        [Name("连接状态")]
        [Tip("客户端是否已连接服务器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Text _stateConnectionText;

        [Name("服务器IP")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public InputField _serverIPInputField;

        [Name("服务器端口")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public InputField _serverPortInputField;

        [Name("连接按钮")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Button _connectButton;

        [Name("断开按钮")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Button _disconnectButton;


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

            InitClientIPAndPort();
            UpdateUI();

            timeCounter = 0;

            if (_connectButton)
            {
                _connectButton.onClick.AddListener(OnConnectButtonClick);
            }
            if (_disconnectButton)
            {
                _disconnectButton.onClick.AddListener(OnDisconnectButtonClick);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();

            if (_connectButton)
            {
                _connectButton.onClick.RemoveListener(OnConnectButtonClick);
            }
            if (_disconnectButton)
            {
                _disconnectButton.onClick.RemoveListener(OnDisconnectButtonClick);
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

        private void OnConnectButtonClick()
        {
            if (client)
            {
                if (client.inConnectAsync || client.isConnected)
                {
                    client.CloseAndSyncObject();
                }

                if (_serverIPInputField)
                {
                    client._connectServerConfig._address = _serverIPInputField.text;
                }
                if (_serverPortInputField)
                {
                    if (int.TryParse(_serverPortInputField.text, out var port) && port > 0)
                    {
                        client._connectServerConfig._port = port;
                    }
                }
                client.ConnectAndTrySyncObject();
            }
        }

        private void OnDisconnectButtonClick()
        {
            if (client)
            {
                client.CloseAndSyncObject();
            }
        }

        private void InitClientIPAndPort()
        {
            if (_serverIPInputField)
            {
                _serverIPInputField.text = client._connectServerConfig._address;
            }
            if (_serverPortInputField)
            {
                _serverPortInputField.text = client._connectServerConfig._port.ToString();
            }
        }

        private void UpdateUI()
        {
            if (!client) return;

            UpdateClientConnectionState();
            UpdateButton();
        }

        private void UpdateClientConnectionState()
        {
            if (_stateConnectionText)
            {
                if (client.inConnectAsync)
                {
                    _stateConnectionText.text = "连接中";
                }
                else
                {
                    _stateConnectionText.text = client.isConnected ? "已连接" : "未连接";
                }
            }
        }

        private void UpdateButton()
        {
            var isConnected = client.isConnected;
            if (_connectButton)
            {
                _connectButton.interactable = !isConnected;
            }
            if (_disconnectButton)
            {
                _disconnectButton.interactable = isConnected;
            }
        }
    }
}