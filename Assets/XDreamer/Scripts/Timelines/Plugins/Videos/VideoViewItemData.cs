using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Views.ScrollViews;

namespace XCSJ.PluginTimelines.Videos
{
    /// <summary>
    /// 视频视图项
    /// </summary>
    [Serializable]
    [Name("视频视图项")]
    public class VideoViewItemData : ViewItemData
    {
        /// <summary>
        /// 视频剪辑
        /// </summary>
        [Name("视频剪辑")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public VideoClip _videoClip = null;

        /// <summary>
        /// 描述
        /// </summary>
        [Name("描述")]
        public string _description;

        /// <summary>
        /// 封面图片
        /// </summary>
        [Name("封面图片")]
        public Sprite _cover;

        /// <summary>
        /// 描述
        /// </summary>
        public override string description { get => _description; set => _description = value; }

        /// <summary>
        /// 选中
        /// </summary>
        public override bool selected
        {
            get
            {
                if (videoPlayer && _videoClip)
                {
                    base.selected = videoPlayer.clip == _videoClip;
                }
                return base.selected;
            }
        }

        private VideoPlayer videoPlayer = null;

        /// <summary>
        /// 初始化视频播放器
        /// </summary>
        /// <param name="videoPlayer"></param>
        public void InitData(VideoPlayer videoPlayer)
        {
            this.videoPlayer = videoPlayer;
            sprite = _cover;
        }

        /// <summary>
        /// 点击
        /// </summary>
        public override void OnClick()
        {
            base.OnClick();

            if (videoPlayer && _videoClip)
            {
                videoPlayer.Stop();
                videoPlayer.clip = _videoClip;
                videoPlayer.Play();
            }
        }
    }
}
