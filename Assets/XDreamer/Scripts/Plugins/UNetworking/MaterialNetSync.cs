using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.PluginCommonUtils;
using UnityEngine.Networking;
using System;
using XCSJ.Attributes;

namespace XCSJ.UNetworking
{
#if UNITY_2019_1_OR_NEWER    
    [Obsolete("因Unity中高级API类NetworkBehaviour已被移除，所以本类不推荐使用!", true)]
#elif UNITY_2018_3_OR_NEWER
    [Obsolete("因Unity中高级API类NetworkBehaviour已被标记过时，所以本类不推荐使用!")]
    [NetworkSettings(sendInterval = 0.2f)]
#endif
    public class MaterialNetSync : BaseVarNetSync
    {
#if !UNITY_2019_1_OR_NEWER
        [SyncVar]
#endif
        [HideInInspector]
        public string materialName = "";

        [Name("渲染器")]
        public Renderer m_Renderer = null;

        private string lastName = "";
        
        void Awake()
        {
            if (!m_Renderer)
            {
                m_Renderer = gameObject.GetComponent<Renderer>();
            }
            if(m_Renderer)
            {
                // 记录以前的材质
                UnityAssetObjectManager.instance.Add(m_Renderer.material);
                lastName = materialName = m_Renderer.material.name;
            }
        }

        protected override bool IsVarChanged()
        {
            return lastName != materialName;
        }

        protected override void DataToVar()
        {
            if (m_Renderer)
            {
                materialName = m_Renderer.material.name;
            }
        }

        protected override void VarToData()
        {
            if (m_Renderer)
            {
                string matName = materialName;

                if (!SetRenderMaterial(matName))
                {
                    int length = matName.LastIndexOf(" (Instance)");
                    if(length >= 0)
                    {
                        SetRenderMaterial(matName.Substring(0, length));
                    }                    
                }

                lastName = materialName;
            }
        }

        private bool SetRenderMaterial(string matName)
        {
            Material newMat = UnityAssetObjectManager.instance.GetObject<Material>(matName);
            if (newMat)
            {
                m_Renderer.material = newMat;
                return true;
            }
            return false;
        }
    }
}

