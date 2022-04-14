using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace XCSJ.UNetworking
{
#if UNITY_2019_1_OR_NEWER    
    [Obsolete("因Unity中高级API类NetworkBehaviour已被移除，所以本类不推荐使用!", true)]
#elif UNITY_2018_3_OR_NEWER
    [Obsolete("因Unity中高级API类NetworkBehaviour已被标记过时，所以本类不推荐使用!")]
#endif
    public class TransformNetSyncManager : MonoBehaviour
    {
        public bool hiddenObjectsBeforeNetSync = false;

        protected void Start()
        {
            if (hiddenObjectsBeforeNetSync)
            {
                SetChildrenStartSyn(false);
            }
        }

        public void OnNetSyncStart()
        {
            SetChildrenStartSyn(true);
        }

        private void SetChildrenStartSyn(bool flag)
        {
            transform.GetComponentsInChildren<TransformNetSync>().ToList().ForEach(tns => tns.startSyn = flag);
        }
    }
}
