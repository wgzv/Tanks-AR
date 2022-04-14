using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;

namespace XCSJ.PluginTools.SelectionUtils
{
    /// <summary>
    /// 选择集轮廓线渲染器
    /// </summary>
    [DisallowMultipleComponent]
    [Name("选择集轮廓线渲染器")]
    public class SelectionOutlineRenderer : SelectionRender<SelectionOutlineRenderer>, IOutlineData
    {
        /// <summary>
        /// 轮廓线颜色
        /// </summary>
        [Name("轮廓线颜色")]
        public Color _color = Color.green;

        /// <summary>
        /// 轮廓线颜色
        /// </summary>
        [Name("轮廓线宽度")]
        [Min(0)]
        public float _width = 2;

        /// <summary>
        /// 无遮挡
        /// </summary>
        [Name("无遮挡")]
        public bool _overlay = true;

        public Color color => _color;

        public float width => _width;

        bool IOutlineData.overlay => _overlay;

        /// <summary>
        /// 选择集变化
        /// </summary>
        /// <param name="oldSelections"></param>
        /// <param name="flag"></param>
        protected override void OnSelectionChanged(GameObject[] oldSelections, bool flag)
        {
            // 取消选中轮廓线效果
            foreach (var go in oldSelections)
            {
                if (go)
                {
                    UnSelect(go);
                }
            }

            // 设置选中轮廓线效果
            foreach (var go in Selection.selections)
            {
                if (go)
                {
                    Select(go);
                }
            }
        }

        private void Select(GameObject go)
        {
            var hh = CommonFun.GetOrAddComponent<OutlineController>(go);
            hh.StartDisplay(this);
        }

        private void UnSelect(GameObject go)
        {
            var hh = go.GetComponent<OutlineController>();
            if (hh)
            {
                hh.StopDisplay();
            }
        }
    }
}
