using System;
using XCSJ.Extension.Base.Dataflows.Base;

namespace XCSJ.EditorTools.Windows.CodeCreaters.Base
{
    /// <summary>
    /// 枚举属性值代码生成器
    /// </summary>
    public class EnumPropertyValueCodeCreater : BasePropertyValue_T_CodeCreater
    {
        /// <summary>
        /// 枚举类型
        /// </summary>
        public virtual Type enumType => targetObjectType;

        /// <summary>
        /// 枚举类型名称
        /// </summary>
        public string enumTypeName => enumType?.Name ?? "";

        /// <summary>
        /// 基础类型定义字符串
        /// </summary>
        protected override string baseTypeDefineString => string.Format("EnumPropertyValue<{0}>", enumTypeName);

        /// <summary>
        /// 名称
        /// </summary>
        public override string defaultName => enumTypeName + "PropertyValue";

        /// <summary>
        /// 有效目标对象类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool ValidTargetObjectType(Type type)
        {
            return base.ValidTargetObjectType(type) && type.IsEnum;
        }

        /// <summary>
        /// 构造
        /// </summary>
        public EnumPropertyValueCodeCreater() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="enumType"></param>
        public EnumPropertyValueCodeCreater(Type enumType)
        {
            targetObjectType = enumType;
        }

        /// <summary>
        /// 当开始生成代码
        /// </summary>
        /// <param name="codeWirter"></param>
        protected override void OnBeginCreateCode(ICodeWirter codeWirter)
        {
            base.OnBeginCreateCode(codeWirter);
            AddUsedType(enumType, typeof(EnumPropertyValue<>));
        }
    }

    /// <summary>
    /// 枚举属性值代码生成器泛型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EnumPropertyValueCodeCreater<T> : EnumPropertyValueCodeCreater where T : struct
    {
        /// <summary>
        /// 枚举类型
        /// </summary>
        public override Type enumType => typeof(T);

        /// <summary>
        /// 目标对象类型
        /// </summary>
        public override Type targetObjectType { get => enumType; set { } }
    }
}
