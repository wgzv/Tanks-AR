using System;
using System.Linq;
using System.Reflection;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorTools.Windows.CodeCreaters.States
{
    /// <summary>
    /// 参数信息数据
    /// </summary>
    public class ParameterInfoData : BasePropertyValueData
    {
        /// <summary>
        /// 参数信息
        /// </summary>
        public ParameterInfo parameterInfo { get; private set; }

        /// <summary>
        /// 成员类型
        /// </summary>
        public override Type memberType => parameterInfo.ParameterType;

        /// <summary>
        /// 名称
        /// </summary>
        public override string name => parameterInfo.Name ?? "";      

        /// <summary>
        /// 名称特性
        /// </summary>
        public override string nameAttribute
        {
            get
            {
                if (string.IsNullOrEmpty(_nameAttribute))
                {
                    nameDictionary.TryGetValue(parameterInfo.Name, out _nameAttribute);

                    if (string.IsNullOrEmpty(_nameAttribute) && Attribute.GetCustomAttributes(parameterInfo, typeof(NameAttribute)).FirstOrDefault() is NameAttribute na)
                    {
                        _nameAttribute = na.language[Languages.ELanguageType.CN];
                    }
                    if (string.IsNullOrEmpty(_nameAttribute) && AttributeCache<NameAttribute>.Exist(parameterInfo.ParameterType))
                    {
                        _nameAttribute = CommonFun.Name(parameterInfo.ParameterType);
                    }
                    if (string.IsNullOrEmpty(_nameAttribute))
                    {
                        _nameAttribute = name ?? "";
                    }
                }
                return _nameAttribute;
            }
            set => _nameAttribute = value;
        }

        /// <summary>
        /// 提示特性
        /// </summary>
        public override string tipAttribute
        {
            get
            {
                if (string.IsNullOrEmpty(_tipAttribute))
                {
                    if (AttributeCache<TipAttribute>.Exist(parameterInfo.ParameterType))
                    {
                        _tipAttribute = CommonFun.Tip(parameterInfo.ParameterType);
                    }
                    if (string.IsNullOrEmpty(_tipAttribute))
                    {
                        _tipAttribute = nameAttribute;
                    }
                }
                return _tipAttribute;
            }
            set => _tipAttribute = value;
        }

        /// <summary>
        /// 属性属性名
        /// </summary>
        public override string propertyPropertyName => propertyEnumName + "__" + name;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="parameterInfo"></param>
        public ParameterInfoData(ParameterInfo parameterInfo)
        {
            this.parameterInfo = parameterInfo;
        }

        /// <summary>
        /// 通过创建类型初始化本类
        /// </summary>
        /// <param name="createType"></param>
        /// <param name="propertyNameEnumType"></param>
        public override void InitByCreateType(Type createType, Type propertyNameEnumType)
        {
            //base.InitByCreateType(createType, propertyNameEnumType);
            if (FieldInfoCache.Get(createType, propertyFieldName) is FieldInfo info)
            {
                nameAttribute = CommonFun.Name(info);
                tipAttribute = CommonFun.Tip(info);
            }
        }
    }
}
