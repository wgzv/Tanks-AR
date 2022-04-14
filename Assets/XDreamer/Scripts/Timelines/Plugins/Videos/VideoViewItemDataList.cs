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
    /// 视频项列表
    /// </summary>
    [Name("视频项列表")]
    [RequireManager(typeof(TimelineManager))]
    public class VideoViewItemDataList : ViewItemDataCollectionMB
    {
        /// <summary>
        /// 视频播放器
        /// </summary>
        [Name("视频播放器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Readonly(EEditorMode.Runtime)]
        public VideoPlayer _videoPlayer = null;

        /// <summary>
        /// 视频视图项
        /// </summary>
        [Name("视频剪辑列表")]
        [ValidityCheck(EValidityCheckType.ElementCountGreater, 0)]
        public List<VideoViewItemData> _videoViewItemList = new List<VideoViewItemData>();

        /// <summary>
        /// 视图数据
        /// </summary>
        public override IEnumerable<IViewItemData> datas => _videoViewItemList.Cast<IViewItemData>();

        /// <summary>
        /// 唤醒
        /// </summary>
        protected void Awake()
        {
            if (_videoPlayer)
            {
                _videoViewItemList.ForEach(v => v.InitData(_videoPlayer));
            }
        }
    }
}
