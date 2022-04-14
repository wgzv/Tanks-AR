using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.LineNotes
{
    /// <summary>
    /// 线样式
    /// </summary>
    [Name("线样式")]
    public class LineStyle : ToolMB
    {
        [Name("宽度", "width")]
        [Range(0, 100)]
        [Tip("=0为系统细线")]
        public float width = 0;

        [Name("颜色", "color")]
        public Color color = Color.green;

        [Name("材质", "mat")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Material mat;

        [Name("遮挡", "occlusion")]
        public bool occlusion = true;
    }
}
