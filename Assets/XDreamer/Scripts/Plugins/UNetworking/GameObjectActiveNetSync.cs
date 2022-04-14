using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using XCSJ.Attributes;

namespace XCSJ.UNetworking
{
#if UNITY_2019_1_OR_NEWER    
    [Obsolete("因Unity中高级API类NetworkBehaviour已被移除，所以本类不推荐使用!", true)]
#elif UNITY_2018_3_OR_NEWER
    [Obsolete("因Unity中高级API类NetworkBehaviour已被标记过时，所以本类不推荐使用!")]
#endif
    public class GameObjectActiveNetSync : BaseVarNetSync
    {
#if !UNITY_2019_1_OR_NEWER
        [SyncVar]
#endif
        [HideInInspector]
        public bool activeState = false;

        [Name("游戏对象")]
        public GameObject go = null;

        private bool lastActiveState = false;

        void Awake()
        {
            if (!go) go = gameObject;
            lastActiveState = activeState = go.activeSelf;
        }

        protected override bool IsVarChanged()
        {
            return lastActiveState != activeState;
        }

        protected override void DataToVar()
        {
            if(go)
            {
                activeState = go.activeSelf;
            }
        }

        protected override void VarToData()
        {
            if (go)
            {
                go.SetActive(activeState);
                lastActiveState = activeState;
            }
        }
    }
}

