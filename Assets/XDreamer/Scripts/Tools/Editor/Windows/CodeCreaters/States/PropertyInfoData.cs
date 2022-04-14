using System;
using System.Reflection;
using XCSJ.Caches;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorTools.Windows.CodeCreaters.States
{
    /// <summary>
    /// 属性信息数据
    /// </summary>
    public class PropertyInfoData : MemberInfoData
    {
        /// <summary>
        /// 属性枚举字段名目录
        /// </summary>
        public override string propertyEnumFieldNameCategory => "属性";

        /// <summary>
        /// 属性信息
        /// </summary>
        public PropertyInfo propertyInfo { get; private set; }

        /// <summary>
        /// 成员信息
        /// </summary>
        public override MemberInfo memberInfo => propertyInfo;

        /// <summary>
        /// 成员类型
        /// </summary>
        public override Type memberType => propertyInfo.PropertyType;

        /// <summary>
        /// 是否是静态成员信息
        /// </summary>
        protected bool? _isStaticMemberInfo;

        /// <summary>
        /// 是否是静态成员信息
        /// </summary>
        public override bool isStaticMemberInfo
        {
            get
            {
                if (!_isStaticMemberInfo.HasValue)
                {
                    if (propertyInfo.GetMethod is MethodInfo getMethodInfo && getMethodInfo.IsStatic)
                    {
                        _isStaticMemberInfo = true;
                        return true;
                    }
                    if (propertyInfo.SetMethod is MethodInfo setMethodInfo && setMethodInfo.IsStatic)
                    {
                        _isStaticMemberInfo = true;
                        return true;
                    }
                    _isStaticMemberInfo = false;
                    return false;
                }
                return _isStaticMemberInfo.Value;
            }
        }

        /// <summary>
        /// 属性枚举名
        /// </summary>
        public override string propertyEnumName => "Property_" + base.propertyEnumName;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="parameterInfo"></param>
        public PropertyInfoData(PropertyInfo propertyInfo)
        {
            this.propertyInfo = propertyInfo;
        }
    }
}
