using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace XCSJ.UNetworking
{
#if UNITY_2019_1_OR_NEWER    
    [Obsolete("因Unity中高级API类NetworkBehaviour已被移除，所以本类不推荐使用!", true)]
#elif UNITY_2018_3_OR_NEWER
    [Obsolete("因Unity中高级API类NetworkBehaviour已被标记过时，所以本类不推荐使用!")]
    [NetworkSettings(sendInterval = 0.03f)]
#endif
    public class TransformNetSync : BaseVarNetSync
    {
#if !UNITY_2019_1_OR_NEWER
        [SyncVar]
#endif
        [HideInInspector]
        private Vector3 offsetPosition = Vector3.zero;

#if !UNITY_2019_1_OR_NEWER
        [SyncVar]
#endif
        [HideInInspector]
        private Quaternion offsetRotation = Quaternion.identity;

#if !UNITY_2019_1_OR_NEWER
        [SyncVar]
#endif
        [HideInInspector]
        private Vector3 offsetScale = Vector3.zero;

        public bool startSyn { get; set; } = false;

        private Vector3 orgPosition = Vector3.zero;

        //private Quaternion orgRotation = Quaternion.Euler(0,0,0);

        private Vector3 orgScale = Vector3.zero;        

        public void Awake()
        {
            orgPosition = transform.localPosition;
            //orgRotation = transform.localRotation;
            orgScale = transform.localScale;
        }

        protected override bool IsVarChanged() => true;

        protected override void DataToVar()
        {
            offsetPosition = transform.localPosition - orgPosition;
            offsetRotation = transform.localRotation;
            offsetScale = transform.localScale - orgScale;
        }

        protected override void VarToData()
        {
            if (startSyn)
            {
                transform.localPosition = orgPosition + offsetPosition;
                transform.localRotation = offsetRotation;
                transform.localScale = orgScale + offsetScale;
            }
        }
    }
}
