using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;

namespace XCSJ.Extension.GenericStandards.Gif
{
    [RequireComponent(typeof(RawImage))]
    [DisallowMultipleComponent]
    [Serializable]
    [Name("Gif纹理渲染到原始图像")]
    [XCSJ.Attributes.Icon(EIcon.GIF)]
    public class GifTextureRenderToRawImage : GifTextureRenderToTarget<RawImage>
    {
        protected override void OnUpdateGifTexture(Texture2D texture2D)
        {
            if (!texture2D || !targetObject) return;
            targetObject.texture = texture2D;
        }
    }
}
