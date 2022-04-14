using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;

namespace XCSJ.Extension.GenericStandards.Gif
{
    [Serializable]
    [Name("帧纹理")]
    public class FrameTexture
    {
        [Name("2D纹理")]
        [Tip("序列帧纹理中的单帧2D纹理对象；")]
        public Texture2D texture2d;

        [Name("延时秒数")]
        [Tip("延时播放下一帧纹理的时间，单位为秒；")]
        public float delaySec;

        public bool canUse
        {
            get
            {
                return texture2d != null;
            }
        }

        public FrameTexture(Texture2D texture2d, float delaySec)
        {
            this.texture2d = texture2d;
            this.delaySec = delaySec;
        }

        public static FrameTexture Create(UniGif.GifTexture gif)
        {
            return new FrameTexture(gif.texture2d, gif.delaySec);
        }

        public override string ToString()
        {
            return texture2d ? (texture2d.name + " , " + delaySec.ToString()) : ("None (" + texture2d.GetType() + ")");
            //return base.ToString();
        }

        public static implicit operator string(FrameTexture ft)
        {
            return ft.ToString();
        }
    }
}
