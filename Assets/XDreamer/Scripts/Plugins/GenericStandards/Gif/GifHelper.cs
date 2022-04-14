using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.GenericStandards.Gif
{
    public class GifHelper
    {
        public static void Init()
        {
            UnityAssetObjectManager.instance.AddBufferType<GifTextureAsset>((buffer, name, ab, bytes) => {
                if (ab)
                {
                    GifTexture gif = GifTexture.LoadImage(ab.LoadAsset<TextAsset>(name));
                    if (gif != null) return gif.gifTextureAsset;
                }
                if (bytes != null)
                {
                    //尝试加载gif图片
                    GifTexture gif = GifTexture.LoadImage(bytes, System.IO.Path.GetFileNameWithoutExtension(name));
                    if (gif != null) return gif.gifTextureAsset;
                }
                return null;
            });
        }
    }
}
