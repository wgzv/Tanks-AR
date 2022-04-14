using UnityEditor;
using XCSJ.Algorithms;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Controls;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorExtension.Base.Controllers;
using XCSJ.EditorTools;
using XCSJ.Extension.Characters;

namespace XCSJ.EditorExtension.Characters
{
    /// <summary>
    /// 角色控制器检查器
    /// </summary>
    [CustomEditor(typeof(XCharacterController), true)]
    public class XCharacterControllerInspector : BaseMainControllerInspector<XCharacterController>
    {
        /// <summary>
        /// 目录列表
        /// </summary>
        public static XObject<CategoryList> categoryList { get; } = new XObject<CategoryList>(cl => cl != null, x => EditorToolsHelper.GetWithPurposes(nameof(XCharacterController)));

        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();
            CategoryListExtension.DrawVertical(categoryList);
        }
    }
}
