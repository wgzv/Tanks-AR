using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.PluginTimelines;

namespace XCSJ.EditorTimelines
{
    /// <summary>
    /// 时间轴检查器
    /// </summary>
    [CustomEditor(typeof(TimelineManager), true)]
    public class TimelineManagerInspector : BaseManagerInspector<TimelineManager>
    {
        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            InitCategoryList();
        }

        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            CategoryListExtension.DrawVertical(categoryList);

            base.OnAfterVertical();
        }

        private static CategoryList categoryList = null;

        private void InitCategoryList()
        {
            if (categoryList == null)
            {
                categoryList = EditorToolsHelper.GetWithPurposes(nameof(TimelineManager));
            }
        }
    }
}
