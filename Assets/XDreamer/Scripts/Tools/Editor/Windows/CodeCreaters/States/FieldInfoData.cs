using System;
using System.Reflection;
using XCSJ.Caches;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorTools.Windows.CodeCreaters.States
{
    /// <summary>
    /// 字段信息数据
    /// </summary>
    public class FieldInfoData : MemberInfoData
    {
        /// <summary>
        /// 属性枚举字段名目录
        /// </summary>
        public override string propertyEnumFieldNameCategory => "字段";

        /// <summary>
        /// 字段信息
        /// </summary>
        public FieldInfo fieldInfo { get; private set; }

        /// <summary>
        /// 成员信息
        /// </summary>
        public override MemberInfo memberInfo => fieldInfo;

        /// <summary>
        /// 成员类型
        /// </summary>
        public override Type memberType => fieldInfo.FieldType;

        /// <summary>
        /// 是否是静态成员信息
        /// </summary>
        public override bool isStaticMemberInfo => fieldInfo.IsStatic;

        /// <summary>
        /// 属性枚举名
        /// </summary>
        public override string propertyEnumName => "Field_" + base.propertyEnumName;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="parameterInfo"></param>
        public FieldInfoData(FieldInfo fieldInfo)
        {
            this.fieldInfo = fieldInfo;
        }
    }
}
