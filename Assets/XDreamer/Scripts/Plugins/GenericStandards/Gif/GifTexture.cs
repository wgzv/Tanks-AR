using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.GenericStandards.Gif
{
    [Serializable]
    [Name("Gif纹理")]
    public class GifTexture : IObject
    {
        /// <summary>
        /// 运行态创建的对象，用于运行态的搜索与查找；
        /// </summary>
        [Name("Gif纹理资源")]
        [Tip("Gif纹理资源抽象类对象；抽象对象，用于抽象Gif纹理或自定义的序列帧纹理；")]
        [Readonly(false)]
        public GifTextureAsset gifTextureAsset;

        /// <summary>
        /// 在初始化时，如果本对象不为空，会以gif字节流的方式解析并拆分为序列帧纹理；
        /// 如果本对象为空，则不处理；
        /// </summary>
        [Name("Gif纹理资源数据流")]
        [Tip("Gif纹理资源数据流文件;即将GIF文件修改为bytes后缀的文件；在非运行态，本对象不为空时，序列帧纹理队列必须为空；")]
        public TextAsset gifTextureAssetBytes;

        [Name("过滤模式")]
        [Tip("Gif纹理资源数据流转化为序列帧纹理时，每帧纹理的过滤模式；")]
        public FilterMode filterMode = FilterMode.Point;

        [Name("纹理循环模式")]
        [Tip("Gif纹理资源数据流转化为序列帧纹理时，每帧纹理的纹理循环模式；")]
        public TextureWrapMode wrapMode = TextureWrapMode.Clamp;

        [Name("是否输出调试信息")]
        [Tip("Gif纹理资源数据流转化为序列帧纹理时，是否输出转化过程中的一些调试信息；错误信息直接输出，不受此项限制；")]
        public bool outputDebugLog = false;

        /// <summary>
        /// 在编辑器状态设定的；之后无法修改；
        /// </summary>
        [Name("资源名")]
        [Tip("Gif纹理资源无效(即无法通过Gif纹理资源数据流转化)无效或者是用户自定义的序列帧纹理时，Gif纹理的名称为本名称；")]
        [HideInSuperInspector(nameof(gifTextureAssetBytes), EValidityCheckType.NotNull)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty | EValidityCheckType.And, null, nameof(gifTextureAssetBytes), EValidityCheckType.Null, invalidExplanation = "当[Gif纹理资源数据流]对象无效时本名称不可为空!")]
        public string assetName = nameof(GifTexture);

        [Name("序列帧纹理队列")]
        [Tip("用于存储Gif纹理资源数据流转化为序列帧纹理队列，此时本队列非运行态为空，运行态动态填充；如果是用户自定义的序列帧纹理队列，本队列非运行态不为空，但Gif纹理资源数据流必须为空；")]
        [HideInSuperInspector(nameof(assetName), EValidityCheckType.Edit | EValidityCheckType.And, null, nameof(gifTextureAssetBytes), EValidityCheckType.NotNull)]
        public List<FrameTexture> textureList = new List<FrameTexture>();

        public int width { get; private set; }

        public int height { get; private set; }

        public string name
        {
            get
            {
                //内存状态的对象，可能名称为空
                if (gifTextureAsset && !string.IsNullOrEmpty(gifTextureAsset.name)) return gifTextureAsset.name;
                //有实体的资源对象是，使用实体资源对象的名字；
                if (gifTextureAssetBytes) return gifTextureAssetBytes.name;
                //使用编辑器状态编辑好的名称；
                return assetName;
            }
            set
            {
                if (gifTextureAsset) gifTextureAsset.name = value;
            }
        }

        public int count
        {
            get
            {
                return textureList.Count;
            }
        }

        public FrameTexture[] textures
        {
            get
            {
                return textureList.ToArray();
            }
        }

        public bool canUse
        {
            get
            {
                //Debug.LogWarning(textures != null ? textures.Length.ToString() : "null");
                return (textures != null && textures.Length > 0 && textures[0].canUse);
            }
        }

        public static GifTexture LoadImage(TextAsset text)
        {
            if (!text) return null;
            GifTexture gif = new GifTexture();
            gif.gifTextureAssetBytes = text;
            gif.Init();
            if (gif.Check()) return gif;
            UnityEngine.Object.Destroy(gif.gifTextureAsset);
            return null;
        }

        public static GifTexture LoadImage(byte[] data, string name)
        {
            GifTexture gif = new GifTexture();
            gif.Init();
            gif.name = name;
            if (UpdateGifTexture(data, gif)) return gif;
            UnityEngine.Object.Destroy(gif.gifTextureAsset);
            return null;
        }

        public static bool UpdateGifTexture(byte[] data, GifTexture gif)
        {
            if (gif == null) return false;
            int loop, w, h;
            var list = UniGif.GetTextureList(data, out loop, out w, out h, gif.filterMode, gif.wrapMode, gif.outputDebugLog);
            if (list == null || list.Count == 0) return false;
            gif.Release();
            gif.width = w;
            gif.height = h;
            for (int i = 0; i < list.Count; ++i)
            {
                UniGif.GifTexture tex = list[i];
                FrameTexture frameTexture = FrameTexture.Create(tex);
                frameTexture.texture2d.name = string.Format("{0}_{1}", gif.name, (i + 1).ToString());
                gif.textureList.Add(frameTexture);
            }
            return true;
        }

        public static bool UpdateGifTexture(TextAsset text, GifTexture gif)
        {
            if (!text) return false;
            //Debug.Log("UpdateGifTexture TextAsset: " + text.name);
            bool ret = UpdateGifTexture(text.bytes, gif);
            if (ret)
            {
                gif.gifTextureAssetBytes = text;
                gif.InitGifTextureAsset();
            }
            return ret;
        }

        private void InitGifTextureAsset()
        {
            if (!gifTextureAsset) gifTextureAsset = ScriptableObject.CreateInstance<GifTextureAsset>();
            if (gifTextureAsset)
            {
                gifTextureAsset.SetGifTexture(this);
            }
        }

        /// <summary>
        /// 将数据信息进行初始化；使其符合运行态执行的要求；
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool Init()
        {
            if (gifTextureAssetBytes)
            {
                Release();
                UpdateGifTexture(gifTextureAssetBytes, this);
            }
            InitGifTextureAsset();
            return true;
        }

        /// <summary>
        /// 项序列帧纹理等信息销毁；
        /// </summary>
        public void Release()
        {
            //             foreach (var t in textureList)
            //             {
            //                 //UnityEngine.Object.Destroy(t.texture2d);
            //                 UnityEngine.Object.DestroyImmediate(t.texture2d, true);
            //             }
            textureList.Clear();
            width = height = 0;
        }

        /// <summary>
        /// 将本类还原到刚创建为对象的默认状态；
        /// </summary>
        public void Reset()
        {
            Release();
            filterMode = FilterMode.Point;
            wrapMode = TextureWrapMode.Clamp;
            outputDebugLog = false;
        }

        /// <summary>
        /// 检查gif具有纹理并且每帧纹理尺寸相同
        /// </summary>
        public bool Check()
        {
            if (!canUse)
            {
                //Debug.LogWarning("Gif纹理为空!");
                return false;
            }
            int w = textures[0].texture2d.width;
            int h = textures[0].texture2d.height;
            foreach (var t in textures)
            {
                if (t == null || t.texture2d == null)
                {
                    Debug.LogWarning(string.Format("Gif纹理中帧纹理无效,即存在引用为 null 的纹理!"));
                    return false;
                }
                if (t.texture2d.width != w || t.texture2d.height != h)
                {
                    Debug.LogWarning(string.Format("Gif纹理中帧纹理[{0}]与首帧纹理尺寸不统一!", t.texture2d.name));
                    return false;
                }
            }
            return true;
        }
    }
}
