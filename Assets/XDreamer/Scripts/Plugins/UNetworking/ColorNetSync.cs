using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class ColorNetSync : BaseVarNetSync
    {
#if !UNITY_2019_1_OR_NEWER
        [SyncVar]
#endif
        [HideInInspector]
        public Color color;

        [Name("渲染器")]
        public Renderer m_Renderer = null;

        void Awake()
        {
            if (!m_Renderer)
            {
                m_Renderer = gameObject.GetComponent<Renderer>();
            }
            if (m_Renderer)
            {
                color = m_Renderer.material.color;
            }
        }

        protected override bool IsVarChanged()
        {
            return true;
        }

        protected override void DataToVar()
        {
            color = m_Renderer.material.color;
        }

        protected override void VarToData()
        {
            m_Renderer.material.color = color;
        }
    }
}
