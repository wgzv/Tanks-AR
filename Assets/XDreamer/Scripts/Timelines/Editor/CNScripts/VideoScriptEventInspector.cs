using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginTimelines.CNScripts;

namespace XCSJ.EditorTimelines.CNScripts
{
    /// <summary>
    /// 视频脚本事件检查器
    /// </summary>
    [CustomEditor(typeof(VideoScriptEvent), true)]
    public class VideoScriptEventInspector : BaseScriptEventInspector<VideoScriptEvent, EVideoScriptEventType, VideoScriptEventSet>
    {
        
    }
}
