using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.GenericStandards.Gif
{
    [Serializable]
    [DisallowMultipleComponent]
    public abstract class GifTextureRenderToTargetBatch<T> : GifTexturePlayer where T : Component
    {
        [Name("目标对象")]
        [Tip("Gif纹理渲染时的目标组件对象;默认为当前游戏对象的可支持Gif纹理渲染的组件对象；")]
        public List<T> targetObjects;

        protected void Awake()
        {
            targetObjects.RemoveAll(obj => !obj);
            targetObjects.Distinct();
        }
    }
}
