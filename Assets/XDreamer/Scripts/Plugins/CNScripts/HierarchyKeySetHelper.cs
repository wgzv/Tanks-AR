using XCSJ.Attributes;
using XCSJ.LitJson;
using XCSJ.Scripts;

namespace XCSJ.Extension.CNScripts
{
    /// <summary>
    /// 层级键设置组手：层级变量中的层级键扩展设置机制
    /// </summary>
    public static class HierarchyKeySetHelper
    {
        /// <summary>
        /// 数量
        /// </summary>
        /// <param name="varContext"></param>
        /// <param name="hierarchyVar"></param>
        /// <param name="extensionHierarchyKey"></param>
        /// <returns></returns>
        [Name("数量")]
        [Tip("尝试设置子级变量数目：仅针对列表（数组）类型时有效;末尾操作，多删除，少补空字符串；")]
        [HierarchyKey(EHierarchyKeyMode.Set, "数量", nameof(Count))]
        public static bool Count(IVarContext varContext, IHierarchyVar hierarchyVar, string extensionHierarchyKey, string varValue)
        {
            return int.TryParse(varValue, out var count) && hierarchyVar.TrySetChildCount(count);
        }
    }
}
