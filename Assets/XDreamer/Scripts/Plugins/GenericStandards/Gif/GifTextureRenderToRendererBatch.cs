using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;

namespace XCSJ.Extension.GenericStandards.Gif
{
    /// <summary>
    /// Gif纹理渲染到渲染器
    /// </summary>
    [Name("Gif纹理渲染到渲染器")]
    [XCSJ.Attributes.Icon(EIcon.GIF)]
    [Tool("多媒体", rootType = typeof(ToolsManager), groupRule = EToolGroupRule.None)]
    public class GifTextureRenderToRendererBatch : GifTextureRenderToTargetBatch<Renderer>
    {
        protected override void OnUpdateGifTexture(Texture2D texture2D)
        {
            if (!texture2D) return;

            targetObjects.ForEach(render =>
            {
                if (render && render.enabled && render.material)
                {
                    render.material.mainTexture = texture2D;
                }
            });
        }
    }
}
