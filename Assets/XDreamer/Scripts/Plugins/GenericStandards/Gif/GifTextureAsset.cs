using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;

namespace XCSJ.Extension.GenericStandards.Gif
{
    [Serializable]
    [Name("Gif纹理资源")]
    public class GifTextureAsset : ScriptableObject
    {
        public GifTexture gifTexture { get; protected set; }

        internal void SetGifTexture(GifTexture gifTexture)
        {
            this.gifTexture = gifTexture;
            this.name = gifTexture.name;
            //Debug.Log("SetGifTexture gifTexture.name:" + gifTexture.name);
        }
    }
}
