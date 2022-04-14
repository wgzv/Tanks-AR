using XCSJ.Attributes;
using XCSJ.EditorTools.Windows.CodeCreaters.Base;
using XCSJ.EditorTools.Windows.CodeCreaters.States;

namespace XCSJ.EditorTools.Windows.CodeCreaters
{
    /// <summary>
    /// 代码生成器类型
    /// </summary>
    [Name("代码生成器类型")]
    public enum ECodeCreaterType
    {
        /// <summary>
        /// 属性设置
        /// </summary>
        [Name("属性设置")]
        [CodeCreaterType(typeof(PropertySetCodeCreater))]
        PropertySet,

        /// <summary>
        /// 属性设置
        /// </summary>
        [Name("属性获取")]
        [CodeCreaterType(typeof(PropertyGetCodeCreater))]
        PropertyGet,

        /// <summary>
        /// 枚举型属性值
        /// </summary>
        [Name("枚举型属性值")]
        [CodeCreaterType(typeof(EnumPropertyValueCodeCreater))]
        EnumPropertyValue,
    }
}
