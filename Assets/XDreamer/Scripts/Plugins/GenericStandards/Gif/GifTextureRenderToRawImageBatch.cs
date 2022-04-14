using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.Tools;

namespace XCSJ.Extension.GenericStandards.Gif
{
    [Name("Gif纹理渲染到原始图像")]
    [XCSJ.Attributes.Icon(EIcon.GIF)]
    [Tool("多媒体", rootType = typeof(ToolsManager), groupRule = EToolGroupRule.None)]
    public class GifTextureRenderToRawImageBatch : GifTextureRenderToTargetBatch<RawImage>
    {
        protected override void OnUpdateGifTexture(Texture2D texture2D)
        {
            if (!texture2D) return;
            targetObjects.ForEach(img=>
            {
                if (img && img.isActiveAndEnabled)
                {
                    img.texture = texture2D;
                }
            });
        }
    }
}
