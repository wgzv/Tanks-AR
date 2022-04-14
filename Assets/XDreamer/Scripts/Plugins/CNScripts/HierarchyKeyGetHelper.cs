using XCSJ.Attributes;
using XCSJ.LitJson;
using XCSJ.Scripts;

namespace XCSJ.Extension.CNScripts
{
    /// <summary>
    /// 层级键获取组手：层级变量中的层级键扩展获取机制
    /// </summary>
    public static class HierarchyKeyGetHelper
    {
        /// <summary>
        /// 数量
        /// </summary>
        /// <param name="varContext"></param>
        /// <param name="hierarchyVar"></param>
        /// <param name="extensionHierarchyKey"></param>
        /// <returns></returns>
        [Name("数量")]
        [Tip("尝试获取子级变量数目：针对列表（数组）、字典（对象）类型时返回其元素数量；针对字符串，返回字符串的长度信息；")]
        [HierarchyKey(EHierarchyKeyMode.Get, "数量", nameof(Count))]
        public static string Count(IVarContext varContext, IHierarchyVar hierarchyVar, string extensionHierarchyKey)
        {
            switch (hierarchyVar.GetJsonType())
            {
                case JsonType.Array:
                case JsonType.Object:
                    {
                        return hierarchyVar.Count.ToString();
                    }
                case JsonType.String:
                    {
                        return hierarchyVar.GetString().Length.ToString();
                    }
            }
            return null;
        }
    }
}
