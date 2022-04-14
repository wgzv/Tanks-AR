using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginTools.Renderers;
using XCSJ.Tools;

namespace XCSJ.PluginTools.SelectionUtils
{
    /// <summary>
    /// 选择集颜色渲染器
    /// </summary>
    [DisallowMultipleComponent]
    [Name("选择集颜色渲染器")]
    public class SelectionColorRenderer : SelectionRecorderRenderer<SelectionColorRenderer>
    {
        /// <summary>
        /// 颜色
        /// </summary>
        [Name("颜色")]
        public Color color = Color.green;

        /// <summary>
        /// 更新渲染器
        /// </summary>
        protected override void UpdateRenderer()
        {
            foreach (var rendererRecorderInfo in rendererRecorder._records)
            {
                rendererRecorderInfo.SetColor(color);
            }
        }
    }
}
