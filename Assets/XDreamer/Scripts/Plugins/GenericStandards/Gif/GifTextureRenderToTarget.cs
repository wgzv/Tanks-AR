using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.GenericStandards.Gif
{
    [Serializable]
    [DisallowMultipleComponent]
    public abstract class GifTextureRenderToTarget<T> : GifTexturePlayer
       where T : Component
    {
        //[HideInSuperInspector]
        [Name("目标对象")]
        [Tip("Gif纹理渲染时的目标组件对象;默认为当前游戏对象的可支持Gif纹理渲染的组件对象；")]
        public T targetObject;

        public override void OnEnable()
        {
            //Debug.Log("Start");
            if (!targetObject) targetObject = this.GetComponent<T>();

            base.OnEnable();
        }

        public virtual void Reset()
        {
            //Debug.Log("Reset");
            targetObject = this.GetComponent<T>();
            playOnStart = true;
            playLoopCount = 0;
            pauseAnimation = false;
        }
    }
}
