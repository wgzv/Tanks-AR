using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorTools;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTimelines;
using XCSJ.PluginTools;

namespace XCSJ.EditorTimelines
{
    public static class ToolsMenu
    {
        /// <summary>
        /// 视频播放器界面
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool("多媒体", nameof(TimelineManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Tool("常用", nameof(TimelineManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Name("视频播放器")]
        [XCSJ.Attributes.Icon(EIcon.Video)]
        [RequireManager(typeof(TimelineManager))]
        public static void CreateVideoPlayer(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath("时间轴/视频播放窗口.prefab"));
        }

        /// <summary>
        /// 视频列表播放器
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool("多媒体", nameof(TimelineManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Name("视频列表播放器")]
        [XCSJ.Attributes.Icon(EIcon.Video)]
        [RequireManager(typeof(TimelineManager))]
        public static void CreateVideoList(ToolContext toolContext)
        {
            var go = EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath("时间轴/视频列表播放器.prefab");
            if (go)
            {
                EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
                var rectTransform = go.GetComponent<RectTransform>();
                if (rectTransform)
                {
                    rectTransform.XStretchHV();
                }
            }
        }
    }
}
