using System;
using UnityEngine;
using UnityEngine.Video;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.CNScripts;

namespace XCSJ.PluginTimelines.CNScripts
{
    /// <summary>
    /// 视频脚本事件
    /// </summary>
    public enum EVideoScriptEventType
    {
        [Name("视频开始时 执行")]
        VideoStarted = 0,

        [Name("播放结束时 执行")]
        videoEnded,
    }

    /// <summary>
    /// 视频脚本事件集
    /// </summary>
    [Serializable]
    public class VideoScriptEventSet : BaseScriptEventSet<EVideoScriptEventType>
    {
        //
    }

    /// <summary>
    /// 视频脚本事件
    /// </summary>
    [Name("视频脚本事件")]
    [Serializable]
    [RequireComponent(typeof(VideoPlayer))]
    [DisallowMultipleComponent]
    public class VideoScriptEvent : BaseScriptEvent<EVideoScriptEventType, VideoScriptEventSet>
    {
        private VideoPlayer _videoPlayer;

        /// <summary>
        /// 启动
        /// </summary>
        public override void Start()
        {
            _videoPlayer = GetComponent<VideoPlayer>();
            if (_videoPlayer)
            {
                _videoPlayer.started += VideoStarted;
                _videoPlayer.loopPointReached += VideoEnded;
            }
            else
            {
                Log.Error(string.Format("{0}不包含VideoPlayer组件", gameObject.name));
            }
        }

        void VideoStarted(VideoPlayer source)
        {
            RunScriptEvent(EVideoScriptEventType.VideoStarted);
        }

        void VideoEnded(VideoPlayer source)
        {
            RunScriptEvent(EVideoScriptEventType.videoEnded);
        }
    }
}
