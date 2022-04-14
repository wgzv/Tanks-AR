using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;

namespace XCSJ.Extension.GenericStandards.Gif
{
    [RequireComponent(typeof(Image))]
    [DisallowMultipleComponent]
    [Serializable]
    [Name("Gif纹理渲染到图像")]
    [XCSJ.Attributes.Icon(EIcon.GIF)]
    public class GifTextureRenderToImage : GifTextureRenderToTarget<Image>
    {
        protected override void OnUpdateGifTexture(Texture2D texture2D)
        {
            if (!texture2D || !targetObject) return;
            //Debug.Log(texture2D);
            //targetObject.material.mainTexture = texture2D;
            targetObject.overrideSprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2());
        }
    }
}
