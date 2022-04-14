using System;
using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.NodeKit;
using XCSJ.EditorSMS.States.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.MultiMedia;

namespace XCSJ.EditorSMS.States.MultiMedia
{
    /// <summary>
    /// 视频检查器
    /// </summary>
    [CustomEditor(typeof(Video))]
    class VideoInspector : WorkClipInspector<Video>
    {
    }
}
