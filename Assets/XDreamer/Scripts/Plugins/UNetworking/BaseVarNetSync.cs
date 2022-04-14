using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.UNetworking
{
#if UNITY_2019_1_OR_NEWER    
    [Obsolete("因Unity中高级API类NetworkBehaviour已被移除，所以本类不推荐使用!", true)]
    public abstract class BaseVarNetSync : MB
#elif UNITY_2018_3_OR_NEWER
    [Obsolete("因Unity中高级API类NetworkBehaviour已被标记过时，所以本类不推荐使用!")]
    public abstract class BaseVarNetSync : NetworkBehaviour
#else
    public abstract class BaseVarNetSync : NetworkBehaviour
#endif
    {
        [Name("同步类型")]
        [HideInInspector]
        [EnumPopup]
        public ENetSyncType netSyncType = ENetSyncType.ServerToClient;

        protected abstract bool IsVarChanged();

        protected abstract void DataToVar();

        protected abstract void VarToData();

        protected virtual void Update()
        {
            UpdateVar();

            if (IsVarChanged())
            {
                UpdateData();
            }
        }

        private void UpdateVar()
        {
#if !UNITY_2019_1_OR_NEWER
            switch (netSyncType)
            {
                case ENetSyncType.ServerToClient:
                    {
                        if (isServer)
                        {
                            DataToVar();
                        }
                        break;
                    }
                case ENetSyncType.ClientToServer:
                    {
                        if (isClient)
                        {
                            DataToVar();
                        }
                    }
                    break;
                case ENetSyncType.Both:
                    {
                        DataToVar();
                        break;
                    }
                case ENetSyncType.None:
                default:
                    break;
            }
#endif
        }

        private void UpdateData()
        {
#if !UNITY_2019_1_OR_NEWER
            switch (netSyncType)
            {
                case ENetSyncType.ServerToClient:
                    {
                        if (isClient)
                        {
                            VarToData();
                        }
                        break;
                    }
                case ENetSyncType.ClientToServer:
                    {
                        if (isServer)
                        {
                            VarToData();
                        }
                    }
                    break;
                case ENetSyncType.Both:
                    {
                        VarToData();
                        break;
                    }
                case ENetSyncType.None:
                default:
                    break;
            }
#endif
        }
    }

    public enum ENetSyncType
    {
        [Name("无")]
        None,

        [Name("服务器到客户端")]
        ServerToClient,

        [Name("客户端到服务器")]
        ClientToServer,

        [Name("双向")]
        Both
    }
}
