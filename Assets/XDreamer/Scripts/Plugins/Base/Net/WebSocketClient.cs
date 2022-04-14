using System;
using System.Collections;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Net;

namespace XCSJ.Extension.Base.Net
{
    /// <summary>
    /// WebSocket�ͻ���
    /// </summary>
    public class WebSocketClient : BaseClient
    {
        /// <summary>
        /// Զ�̵�ַ
        /// </summary>
        public override Address remoteAddress => new Address(url, port);

        /// <summary>
        /// ���ص�ַ
        /// </summary>
        public override Address localAddress => new Address();

        /// <summary>
        /// URL
        /// </summary>
        public string url = "";

        /// <summary>
        /// �˿�
        /// </summary>
        public int port = 0;

        /// <summary>
        /// WebSocket����
        /// </summary>
        public WebSocket sock { get; private set; }

        /// <summary>
        /// ·��
        /// </summary>
        public string path { get; protected set; } = "";

        /// <summary>
        /// �ر�
        /// </summary>
        public override void Close()
        {
            try
            {
                clientState = EClientState.Closing;
                if (sock != null)
                {
                    sock.Close();
                    sock = null;
                }
            }
            finally
            {
                clientState = EClientState.Closed;
            }            
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <returns></returns>
        public override bool Connect()
        {
            if (IsConnected()) return true;

            clientState = EClientState.WillConnect;
            sock = new WebSocket(WebSocket.ToUri(url, port, false, path));
            sock.Connect();
            clientState = EClientState.Connecting;
            return true;
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="serverAddress"></param>
        /// <returns></returns>
        public override bool Connect(IAddress serverAddress)
        {
            if (serverAddress == null) return false;
            if (IsConnected()) return true;
            this.url = serverAddress.address;
            this.port = serverAddress.port;
            return Connect();
        }

        /// <summary>
        /// �Ƿ�������
        /// </summary>
        /// <returns></returns>
        public override bool IsConnected()
        {
            if (sock == null) return false;
            if (sock.Connected)
            {
                clientState = EClientState.Connected;
                return true;
            }
            else
            {
                if (!string.IsNullOrEmpty(sock.Error))
                {
                    Close();
                    return false;
                }
                if (clientState == EClientState.Connecting) return true;
            }
            return false;
        }        

        /// <summary>
        /// д��
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override int Write(string data)
        {
            if (!IsConnected())
            {
                return 0;
            }
            if (clientState == EClientState.Connecting)
            {
                //Debug.Log("Write 1");
                return 0;
            }
            return sock.SendString(data);
        }

        /// <summary>
        /// ��ȡ
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override int Read(out string data)
        {
            if (!IsConnected() || clientState == EClientState.Connecting)
            {
                data = null;
                return 0;
            }
            return sock.RecvString(out data);
        }
    }
}