using UnityEditor;
using XCSJ.Algorithms;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Controls;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorExtension.Characters.Base;
using XCSJ.Extension.Characters;
using XCSJ.EditorTools;

namespace XCSJ.EditorExtension.Characters
{
    /// <summary>
    /// 角色控制器检查器
    /// </summary>
    [CustomEditor(typeof(CharacterMover), true)]
    public class CharacterMoverInspector : BaseCharacterMoverInspector<CharacterMover>
    {
        /// <summary>
        /// 目录列表
        /// </summary>
        public static XObject<CategoryList> categoryList { get; } = new XObject<CategoryList>(cl => cl != null, x => EditorToolsHelper.GetWithPurposes(nameof(CharacterMover)));

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
