using System;
using UnityEngine;
using XCSJ.Attributes;

namespace XCSJ.Extension.GenericStandards.Gif
{
    /// <summary>
    /// Gif纹理渲染到渲染器
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    [DisallowMultipleComponent]
    [Serializable]
    [Name("Gif纹理渲染到渲染器")]
    [XCSJ.Attributes.Icon(EIcon.GIF)]
    public class GifTextureRenderToRenderer : GifTextureRenderToTarget<Renderer>
    {
        protected override void OnUpdateGifTexture(Texture2D texture2D)
        {
            if (!texture2D || !targetObject || !targetObject.material) return;
            targetObject.material.mainTexture = texture2D;
        }
    }
}
