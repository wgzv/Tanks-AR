using System;
using System.Collections.Generic;
using System.Reflection;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.EditorTools.Windows.CodeCreaters.Base;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorTools.Windows.CodeCreaters.States
{
    /// <summary>
    /// 基础属性值数据
    /// </summary>
    public abstract class BasePropertyValueData
    {
        /// <summary>
        /// 成员类型
        /// </summary>
        public abstract Type memberType { get; }

        /// <summary>
        /// 名称
        /// </summary>
        public abstract string name { get; }

        /// <summary>
        /// 有效属性类型：即<see cref="memberType"/>可能存在有效的属性值类型与之对应
        /// </summary>
        public bool validPropertyType { get; protected set; } = true;

        /// <summary>
        /// 属性值类型名称
        /// </summary>
        public string[] propertyValueTypeNames { get; protected set; } = Empty<string>.Array;

        /// <summary>
        /// 属性值类型缓存值
        /// </summary>
        protected PropertyValueTypeCacheValue _cacheValue;

        /// <summary>
        /// 属性值类型缓存值类型数据
        /// </summary>
        protected PropertyValueTypeCacheValue.TypeData _cacheValue_TypeData;

        /// <summary>
        /// 属性值类型
        /// </summary>
        public Type propertyValueType
        {
            get
            {
                if (_cacheValue_TypeData == null)
                {
                    var memberType = this.memberType;
                    if (_cacheValue == null)
                    {
                        _cacheValue = PropertyValueTypeCache.GetCacheValue(memberType);
                    }
                    if (_cacheValue != null)
                    {
                        _cacheValue_TypeData = _cacheValue.GetBestTypeDataOrFirst(memberType);
                        propertyValueTypeNames = _cacheValue.propertyValueTypeNames;
                    }
                }
                return _cacheValue_TypeData?.propertyValueType;
            }
        }

        /// <summary>
        /// 是否需要类型转化
        /// </summary>
        public bool needTypeConvert => _cacheValue_TypeData != null && _cacheValue_TypeData.needTypeConvert;

        /// <summary>
        /// 有效属性值类型
        /// </summary>
        public bool validPropertyValueType => propertyValueType != null;

        /// <summary>
        /// 属性值类型名称
        /// </summary>
        public string propertyValueTypeName
        {
            get => propertyValueType?.Name ?? "";
            set
            {
                if (propertyValueTypeName != value)
                {
                    _cacheValue_TypeData = _cacheValue?.GetTypeData(value);
                }
            }
        }

        /// <summary>
        /// 能否生成属性值类型
        /// </summary>
        public bool canCreatePropertyValueType => validPropertyType && !validPropertyValueType;

        /// <summary>
        /// 是否生成属性值类型
        /// </summary>
        public bool createPropertyValueType { get; set; } = true;

        /// <summary>
        /// 是否需要生成属性值类型,即检测能否可生成属性值类型<see cref="canCreatePropertyValueType"/>并且要生成属性值类型<see cref="createPropertyValueType"/>
        /// </summary>
        public bool needCreatePropertyValueType => createPropertyValueType && canCreatePropertyValueType;

        /// <summary>
        /// 名称特性
        /// </summary>
        protected string _nameAttribute = "";

        /// <summary>
        /// 名称特性
        /// </summary>
        public virtual string nameAttribute
        {
            get
            {
                if (string.IsNullOrEmpty(_nameAttribute))
                {
                    _nameAttribute = GetNameAttributeText(memberType);
                }
                return _nameAttribute;
            }
            set => _nameAttribute = value;
        }

        /// <summary>
        /// 名称字典
        /// </summary>
        public static Dictionary<string, string> nameDictionary = new Dictionary<string, string>()
        {
            { "BroadcastMessage", "广播消息" },
            { "BroadcastMessage_String", "广播消息(方法名)" },
            { "BroadcastMessage_String_SendMessageOptions", "广播消息(方法名+发送消息选项)" },
            { "camera", "相机" },
            { "CancelInvoke", "取消调用" },
            { "CancelInvoke_String", "取消调用(方法名)" },
            { "CompareTag", "比较标签" },
            { "CompareTag_String", "比较标签(标签)" },
            { "enabled", "启用" },
            { "Equals", "相等" },
            { "GetComponent", "获取组件" },
            { "GetComponent_String", "获取组件(类型)" },
            { "GetHashCode", "获取哈希码" },
            { "GetInstanceID", "获取实例ID" },
            { "GetType", "获取类型" },
            { "hideFlags", "隐藏标识" },
            { "index", "索引" },
            { "Invoke", "调用" },
            { "Invoke_String_Single", "是否调用(方法名+时间)" },
            { "InvokeRepeating", "重复调用" },
            { "isActiveAndEnabled", "是激活并启用" },
            { "IsInvoking", "是否调用" },
            { "IsInvoking_String", "是否调用(方法名)" },
            { "methodName", "方法名" },
            { "name", "名称" },
            { "options", "选项" },
            { "parent", "父级" },
            { "repeatRate", "重复率" },
            { "Reset", "重置" },
            { "SendMessage", "发送消息" },
            { "SendMessage_String", "发送消息(方法名)" },
            { "SendMessage_String_SendMessageOptions", "发送消息(方法名+发送消息选项)" },
            { "SendMessageUpwards", "向上发送消息" },
            { "SendMessageUpwards_String", "向上发送消息(方法名)" },
            { "SendMessageUpwards_String_SendMessageOptions", "向上发送消息(方法名+发送消息选项)" },
            { "StartCoroutine", "启动协程" },
            { "StopCoroutine", "停止协程" },
            { "StopAllCoroutines", "停止全部协程" },
            { "tag", "标签" },
            { "time", "时间" },
            { "type", "类型" },
            { "ToString", "转字符串" },
            { "useGUILayout", "使用GUI布局" },
            { "value", "值" },
        };

        /// <summary>
        /// 获取名称特性文本
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public virtual string GetNameAttributeText(MemberInfo memberInfo)
        {
            if (memberInfo == null) return "";
            if (AttributeCache<NameAttribute>.Exist(memberInfo)) return CommonFun.Name(memberInfo);
            if (nameDictionary.TryGetValue(name, out string value1)) return value1;
            if (nameDictionary.TryGetValue(memberInfo.Name, out string value2)) return value2;
            return CommonFun.Name(memberInfo);//用于拆分字符串
        }

        /// <summary>
        /// 名称特性类型字符串
        /// </summary>
        public string nameAttributeTypeString
        {
            get
            {
                var s = propertyEnumFieldNameCategory;
                return string.IsNullOrEmpty(s) ? "" : ("(" + s + ")");
            }
        }

        /// <summary>
        /// 带类型的名称特性
        /// </summary>
        public string nameAttributeWithType
        {
            get => nameAttribute + nameAttributeTypeString;
            set
            {
                var s = nameAttributeTypeString;
                nameAttribute = string.IsNullOrEmpty(s) ? value : value.Replace(s, "");
            }
        }

        /// <summary>
        /// 提示特性
        /// </summary>
        protected string _tipAttribute = "";

        /// <summary>
        /// 提示特性
        /// </summary>
        public virtual string tipAttribute
        {
            get
            {
                if (string.IsNullOrEmpty(_tipAttribute))
                {
                    var text = AttributeCache<TipAttribute>.Exist(memberType) ? CommonFun.Tip(memberType) : null;
                    _tipAttribute = string.IsNullOrEmpty(text) ? nameAttribute : text;
                }
                return _tipAttribute;
            }
            set => _tipAttribute = value;
        }

        /// <summary>
        /// 属性枚举名
        /// </summary>
        public virtual string propertyEnumName { get; set; } = "";

        /// <summary>
        /// 属性属性名称
        /// </summary>
        public virtual string propertyPropertyName => propertyEnumName;

        /// <summary>
        /// 属性字段名
        /// </summary>
        public string propertyFieldName => "_" + propertyPropertyName;

        /// <summary>
        /// 显示属性枚举名，即不使用<see cref="HideInSuperInspectorAttribute"/>特性修饰对应的枚举字段，
        /// </summary>
        public bool displayPropertyEnumName { get; set; } = true;

        /// <summary>
        /// 是否有效
        /// </summary>
        public virtual bool valid => validPropertyType && validPropertyValueType;

        /// <summary>
        /// 能否有效；如果已经有效<see cref="valid"/>,则认为能有效；否则判断能否生成属性值类型<see cref="canCreatePropertyValueType"/>；
        /// </summary>
        public virtual bool canValid => valid || canCreatePropertyValueType;

        /// <summary>
        /// 是否期望有效；如果已经有效<see cref="valid"/>,则认为是期望有效；否则判断是否需要生成属性值类型<see cref="needCreatePropertyValueType"/>；
        /// </summary>
        public virtual bool wantValid => valid || needCreatePropertyValueType;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="codeCreater"></param>
        public virtual void Init(BaseCodeCreater codeCreater)
        {
            validPropertyType = memberType.ValidPropertyType();
        }

        /// <summary>
        /// 属性枚举字段名目录
        /// </summary>
        public virtual string propertyEnumFieldNameCategory { get; } = "";

        /// <summary>
        /// 有字段区域
        /// </summary>
        public bool hasFieldRegion { get; set; } = false;

        /// <summary>
        /// 有字段结束区域
        /// </summary>
        public bool hasFieldEndRegion { get; set; } = false;

        /// <summary>
        /// 写入字段区域
        /// </summary>
        /// <param name="codeWirter"></param>
        protected void WriteFieldRegion(ICodeWirter codeWirter)
        {
            if (hasFieldRegion)
            {
                codeWirter.WriteRegion(propertyEnumFieldNameCategory);
                codeWirter.Write();
            }
        }

        /// <summary>
        /// 写入字段结束区域
        /// </summary>
        /// <param name="codeWirter"></param>
        protected void WriteFieldEndRegion(ICodeWirter codeWirter)
        {
            if (hasFieldEndRegion)
            {
                codeWirter.WriteEndRegion();
                codeWirter.Write();
            }
        }

        /// <summary>
        /// 当生成代码字段属性值
        /// </summary>
        /// <param name="codeCreater"></param>
        /// <param name="codeWirter"></param>
        public virtual void OnCreateCode_Field_PropertyValue(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter)
        {
            WriteFieldRegion(codeWirter);

            var propertyValueTypeName = this.propertyValueTypeName;

            if (codeCreater.FindOrNewCodeCreater(this, out var isNew) is BaseCodeCreater propertyValueTypeCodeCreater)
            {
                if (isNew)
                {
                    propertyValueTypeCodeCreater.CreateCode(codeWirter);
                    codeWirter.Write();
                }
                propertyValueTypeName = propertyValueTypeCodeCreater.name;
            }

            codeWirter.WriteSummaryFormat("{0}:{1}", nameAttribute, tipAttribute);
            codeWirter.WriteFormat("[Name(\"{0}\")]", nameAttribute);
            codeWirter.WriteFormat("[Tip(\"{0}\")]", tipAttribute);
            codeWirter.WriteFormat("[HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.{0})]", propertyEnumName);
            codeWirter.Write("[OnlyMemberElements]");
            codeWirter.WriteFormat("public {0} {1} = new {0}();", propertyValueTypeName, propertyFieldName);
            codeWirter.Write();

            WriteFieldEndRegion(codeWirter);
        }

        /// <summary>
        /// 当开始生成代码
        /// </summary>
        /// <param name="codeWirter"></param>
        public virtual void OnBeginCreateCode(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter)
        {
            codeCreater.AddUsedType(memberType, propertyValueType);
        }

        /// <summary>
        /// 当结束生成代码
        /// </summary>
        /// <param name="codeWirter"></param>
        public virtual void OnEndCreateCode(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter) { }

        /// <summary>
        /// 当生成代码调用GetValue
        /// </summary>
        /// <param name="codeCreater"></param>
        /// <param name="codeWirter"></param>
        /// <returns></returns>
        public virtual string OnCreateCode_Call_GetValue(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter)
        {
            if (needTypeConvert)
            {
                if(memberType.IsClass)
                {
                    return string.Format("{0}.GetValue() as {1}",
                        propertyFieldName,
                        memberType.Name);
                }
                else
                {
                    return string.Format("{1}{0}.GetValue()",
                        propertyFieldName,
                        memberType.Name);
                }
            }
            return string.Format("{0}.GetValue()", propertyFieldName);
        }

        /// <summary>
        /// 通过创建类型初始化本类
        /// </summary>
        /// <param name="createType"></param>
        /// <param name="propertyNameEnumType"></param>
        public virtual void InitByCreateType(Type createType, Type propertyNameEnumType)
        {
            if (FieldInfoCache.Get(propertyNameEnumType, propertyEnumName) is FieldInfo info)
            {
                nameAttributeWithType = CommonFun.Name(info);
                tipAttribute = CommonFun.Tip(info);
            }
        }
    }
}
