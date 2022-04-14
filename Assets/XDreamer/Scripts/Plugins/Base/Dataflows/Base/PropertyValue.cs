using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Helpers;
using XCSJ.Helper;
using XCSJ.Interfaces;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.Extension.Base.Dataflows.Base
{
    /// <summary>
    /// 属性值类型
    /// </summary>
    [Name("属性值类型")]
    public enum EPropertyValueType
    {
        /// <summary>
        /// 值：属性对应类型的值
        /// </summary>
        [Name("值")]
        [Tip("属性对应类型的值")]
        Value = 0,

        /// <summary>
        /// 变量：中文脚本中的全局变量
        /// </summary>
        [Name("变量")]
        [Tip("中文脚本中的全局变量")]
        Variable,
    }

    /// <summary>
    /// 基础属性值
    /// </summary>
    [PropertyValueFieldName]
    public abstract class BasePropertyValue : IToFriendlyString
    {
        /// <summary>
        /// 属性值类型
        /// </summary>
        [Name("属性值类型")]
        [EnumPopup]
        public EPropertyValueType _propertyValueType = EPropertyValueType.Value;

        /// <summary>
        /// 值
        /// </summary>
        public abstract object value { get; }

        /// <summary>
        /// 变量名
        /// </summary>
        [Name("变量名")]
        [GlobalVariable]
        [HideInSuperInspector(nameof(_propertyValueType), EValidityCheckType.NotEqual, EPropertyValueType.Variable)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public string _variableName = "";

        /// <summary>
        /// 变量名
        /// </summary>
        public string variableName => _variableName;

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public virtual string ToFriendlyString()
        {
            switch (_propertyValueType)
            {
                case EPropertyValueType.Value:
                    {
                        return ValueToFriendlyString(value);
                    }
                case EPropertyValueType.Variable:
                    {
                        return ScriptOption.VarFlag + variableName;
                    }
            }
            return "";
        }

        /// <summary>
        /// 值转友好字符串：用于<see cref="ToFriendlyString"/>；
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual string ValueToFriendlyString(object value) => Converter.instance.TryConvertTo(value, out string output) ? output : "";

        /// <summary>
        /// 数据有效性；对当前对象的数据进行有效性判断；仅判断，不做其它处理；
        /// </summary>
        public virtual bool DataValidity()
        {
            switch (_propertyValueType)
            {
                case EPropertyValueType.Value:
                    {
                        return !ObjectHelper.ObjectIsNull(value);
                    }
                case EPropertyValueType.Variable:
                    {
                        return !string.IsNullOrEmpty(variableName);
                    }
            }
            return true;
        }
    }

    /// <summary>
    /// 基础属性值
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public abstract class BasePropertyValue<TValue> : BasePropertyValue
    {
        /// <summary>
        /// 构造
        /// </summary>
        public BasePropertyValue() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="value"></param>
        public BasePropertyValue(TValue value) { }

        /// <summary>
        /// 值
        /// </summary>
        public override object value => propertyValue;

        /// <summary>
        /// 属性值
        /// </summary>
        public abstract TValue propertyValue { get; }

        /// <summary>
        /// 尝试获取值：会先判断属性值类型<see cref="BasePropertyValue._propertyValueType"/>，之后尝试获取值；
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool TryGetValue(out TValue value)
        {
            switch (_propertyValueType)
            {
                case EPropertyValueType.Value:
                    {
                        value = this.propertyValue;
                        return true;
                    }
                case EPropertyValueType.Variable:
                    {
                        if (ScriptManager.TryGetGlobalVariableValue(_variableName, out string variableValue))
                        {
                            return TryConvert(variableValue, out value);
                        }
                        break;
                    }
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 获取值：会先判断属性值类型<see cref="BasePropertyValue._propertyValueType"/>，之后获取值；
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual TValue GetValue(TValue defaultValue = default) => TryGetValue(out TValue output) ? output : defaultValue;

        /// <summary>
        /// 尝试转化：将字符串尝试转化为期望类型的值；
        /// </summary>
        /// <param name="input"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool TryConvert(string input, out TValue value) => Converter.instance.TryConvertTo(input, out value);

        /// <inheritdoc/>
        protected override string ValueToFriendlyString(object value) => ValueToFriendlyString((TValue)value);

        /// <summary>
        /// 值转友好字符串：用于<see cref="ToFriendlyString"/>；
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual string ValueToFriendlyString(TValue value) => base.ValueToFriendlyString(value);
    }

    /// <summary>
    /// 属性值：用于存储单一类型参数与中文脚本变量的容器类；如期望存储多种不同类型参数（包括中文脚本变量），请参考实参<see cref="Argument"/>
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    [PropertyValueFieldName(nameof(_value))]
    public class PropertyValue<TValue> : BasePropertyValue<TValue>
    {
        /// <summary>
        /// 值
        /// </summary>
        [Name("值")]
        [HideInSuperInspector(nameof(_propertyValueType), EValidityCheckType.NotEqual, EPropertyValueType.Value)]
        public TValue _value = default;

        /// <summary>
        /// 属性值
        /// </summary>
        public override TValue propertyValue => _value;

        /// <summary>
        /// 构造
        /// </summary>
        public PropertyValue() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="value"></param>
        public PropertyValue(TValue value) : base(value) { this._value = value; }
    }

    /// <summary>
    /// 属性类型特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class PropertyTypeAttribute : Attribute
    {
        private string typeFullName = "";

        private Type _type = null;

        /// <summary>
        /// 可用于子级类型
        /// </summary>
        public bool forChildrenClass { get; set; } = false;

        /// <summary>
        /// 类型
        /// </summary>
        public Type type => _type ?? (_type = TypeCache.Get(typeFullName));

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="type"></param>
        public PropertyTypeAttribute(Type type)
        {
            this._type = type;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="typeFullName"></param>
        public PropertyTypeAttribute(string typeFullName)
        {
            this.typeFullName = typeFullName;
        }
    }

    /// <summary>
    /// 属性值类型缓存
    /// </summary>
    public class PropertyValueTypeCache : TIVCache<PropertyValueTypeCache, Type, PropertyValueTypeCacheValue> { }

    /// <summary>
    /// 属性值类型缓存值
    /// </summary>
    public class PropertyValueTypeCacheValue : TIVCacheValue<PropertyValueTypeCacheValue, Type>
    {
        /// <summary>
        /// 属性值类型名称列表
        /// </summary>
        public string[] propertyValueTypeNames { get; private set; } = Empty<string>.Array;

        /// <summary>
        /// 类型数据
        /// </summary>
        public class TypeData
        {
            /// <summary>
            /// 类型
            /// </summary>
            public Type type;

            /// <summary>
            /// 属性值类型可处理的最优类型
            /// </summary>
            public Type bestType;

            /// <summary>
            /// 属性值类型
            /// </summary>
            public Type propertyValueType;

            /// <summary>
            /// 是否是否最优的
            /// </summary>
            public bool isBest => IsBest(type);

            /// <summary>
            /// 是否需要类型转化
            /// </summary>
            public bool needTypeConvert => type != bestType && bestType.IsAssignableFrom(type);

            /// <summary>
            /// 是否是否最优的
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            public bool IsBest(Type type) => type == bestType;
        }

        /// <summary>
        /// 类型数据列表
        /// </summary>
        public List<TypeData> typeDatas = new List<TypeData>();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public override bool Init()
        {
            var propertyValueTypes = TypeHelper.GetTypesHasAttribute<BasePropertyValue, PropertyTypeAttribute>((t, atts) =>
            {
                bool valid = false;
                foreach(var a in atts)
                {
                    if(a.type == key1 || (a.forChildrenClass && a.type.IsAssignableFrom(key1)))
                    {
                        valid = true;
                        var data = new TypeData()
                        {
                            type = key1,
                            bestType = a.type,
                            propertyValueType = t,
                        };
                        typeDatas.Add(data);
                    }
                }
                return valid;
            });

            propertyValueTypeNames = propertyValueTypes.Cast(t => t.Name).Distinct().ToArray();
            return true;
        }

        /// <summary>
        /// 获取类型数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TypeData GetTypeData(string name) => typeDatas.FirstOrDefault(t => t.propertyValueType.Name == name);

        /// <summary>
        /// 获取属性值类型
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Type GetPropertyValueType(string name) => typeDatas.FirstOrDefault(t => t.propertyValueType.Name == name)?.propertyValueType;

        /// <summary>
        /// 获取最优的类型数据或第一个
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public TypeData GetBestTypeDataOrFirst(Type type) => typeDatas.FirstOrDefault(data => data.IsBest(type)) ?? typeDatas.FirstOrDefault();

        /// <summary>
        /// 获取最优的属性值类型或第一个
        /// </summary>
        /// <returns></returns>
        public Type GetBestPropertyValueTypeOrFirst(Type type) => GetBestTypeDataOrFirst(type)?.propertyValueType;
    }

    /// <summary>
    /// 属性值字段名称特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PropertyValueFieldNameAttribute : Attribute
    {
        /// <summary>
        /// 默认字段名
        /// </summary>
        public const string DefaultFieldName = "_" + nameof(BasePropertyValue.value);

        /// <summary>
        /// 字段名
        /// </summary>
        public string fieldName { get; private set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="fieldName"></param>
        public PropertyValueFieldNameAttribute(string fieldName = DefaultFieldName)
        {
            this.fieldName = fieldName;
        }

        /// <summary>
        /// 获取字段名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetFieldName(Type type) => type != null ? Cache.GetCacheValue(type) : DefaultFieldName;

        class Cache : TICache<Cache, Type, string>
        {
            protected override KeyValuePair<bool, string> CreateValue(Type key1)
            {
                var name = AttributeCache<PropertyValueFieldNameAttribute>.Get(key1,true)?.fieldName ?? DefaultFieldName;
                return new KeyValuePair<bool, string>(true, name);
            }
        }
    }

    #region 基础类型属性值

    /// <summary>
    /// 布尔类型属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(bool))]
    public class BoolPropertyValue : PropertyValue<bool>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public BoolPropertyValue() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="value"></param>
        public BoolPropertyValue(bool value) : base(value) { }
    }

    /// <summary>
    /// 字符串属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(string))]
    public class StringPropertyValue : BasePropertyValue<string>
    {
        /// <summary>
        /// 值
        /// </summary>
        [Name("值")]
        [HideInSuperInspector(nameof(_propertyValueType), EValidityCheckType.NotEqual, EPropertyValueType.Value)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public string _value = "";

        /// <summary>
        /// 属性值
        /// </summary>
        public override string propertyValue => _value;

        /// <summary>
        /// 构造
        /// </summary>
        public StringPropertyValue() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="value"></param>
        public StringPropertyValue(string value) : base(value) { this._value = value; }
    }

    /// <summary>
    /// 字符串属性值,字符串值被<see cref="TextAreaAttribute"/>修饰
    /// </summary>
    [Serializable]
    [PropertyType(typeof(string))]
    public class StringPropertyValue_TextArea : BasePropertyValue<string>
    {
        /// <summary>
        /// 值
        /// </summary>
        [Name("值")]
        [TextArea]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        [HideInSuperInspector(nameof(_propertyValueType), EValidityCheckType.NotEqual, EPropertyValueType.Value)]
        public string _value = "";

        /// <summary>
        /// 属性值
        /// </summary>
        public override string propertyValue => _value;

        /// <summary>
        /// 构造
        /// </summary>
        public StringPropertyValue_TextArea() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="value"></param>
        public StringPropertyValue_TextArea(string value) { this._value = value; }
    }

    /// <summary>
    /// 整形属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(int))]
    public class IntPropertyValue : PropertyValue<int>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public IntPropertyValue() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="value"></param>
        public IntPropertyValue(int value) : base(value) { }
    }

    /// <summary>
    /// 长整形属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(long))]
    public class LongPropertyValue : PropertyValue<long>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public LongPropertyValue() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="value"></param>
        public LongPropertyValue(int value) : base(value) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="value"></param>
        public LongPropertyValue(long value) : base(value) { }
    }

    /// <summary>
    /// 浮点数属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(float))]
    public class FloatPropertyValue : PropertyValue<float>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public FloatPropertyValue() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="value"></param>
        public FloatPropertyValue(float value) : base(value) { }
    }

    /// <summary>
    /// 正浮点数属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(float))]
    public class PositiveFloatPropertyValue : BasePropertyValue<float>
    {
        /// <summary>
        /// 正浮点数
        /// </summary>
        [Name("正浮点数")]
        [HideInSuperInspector(nameof(_propertyValueType), EValidityCheckType.NotEqual, EPropertyValueType.Value)]
        [Min(0)]
        public float _value;

        /// <summary>
        /// 属性值
        /// </summary>
        public override float propertyValue => _value;

        /// <summary>
        /// 构造
        /// </summary>
        public PositiveFloatPropertyValue() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="value"></param>
        public PositiveFloatPropertyValue(float value) : base(value) { this._value = value; }
    }

    /// <summary>
    /// 双精度浮点数属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(double))]
    public class DoublePropertyValue : PropertyValue<double>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public DoublePropertyValue() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="value"></param>
        public DoublePropertyValue(float value) : base(value) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="value"></param>
        public DoublePropertyValue(double value) : base(value) { }
    }

    /// <summary>
    /// 二维向量属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(Vector2))]
    public class Vector2PropertyValue : PropertyValue<Vector2>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public Vector2PropertyValue() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="value"></param>
        public Vector2PropertyValue(Vector2 value) : base(value) { }
    }

    /// <summary>
    /// 三维向量属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(Vector3))]
    public class Vector3PropertyValue : PropertyValue<Vector3>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public Vector3PropertyValue() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="value"></param>
        public Vector3PropertyValue(Vector3 value) : base(value) { }
    }

    /// <summary>
    /// 颜色属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(Color))]
    public class ColorPropertyValue : PropertyValue<Color>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public ColorPropertyValue() : base(Color.white) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="value"></param>
        public ColorPropertyValue(Color value) : base(value) { }
    }

    /// <summary>
    /// 图层遮罩属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(LayerMask))]
    public class LayerMaskPropertyValue : PropertyValue<LayerMask>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public LayerMaskPropertyValue()   { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="value"></param>
        public LayerMaskPropertyValue(LayerMask value) : base(value) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="value"></param>
        public LayerMaskPropertyValue(int value) : base(value) { }
    }

    /// <summary>
    /// 获取脚本集合接口
    /// </summary>
    public interface IGetScriptSet
    {
        /// <summary>
        /// 获取脚本集合
        /// </summary>
        /// <param name="propertyPath">属性路径</param>
        /// <returns></returns>
        ScriptSet GetScriptSet(string propertyPath);
    }

    /// <summary>
    /// 脚本集合属性值：对应序列化存储的Unity类需要继承<see cref="IGetScriptSet"/>接口
    /// </summary>
    [Serializable]
    [PropertyType(typeof(ScriptSet))]
    public class ScriptSetPropertyValue : PropertyValue<ScriptSet>
    {
        public IEnumerable<string> GetScriptStrings()
        {
            switch (_propertyValueType)
            {
                case EPropertyValueType.Variable:
                    {
                        var scriptManager = ScriptManager.instance;
                        if (!scriptManager || !scriptManager.TryGetVarValue(_variableName, out var value))
                        {
                            return Empty<string>.Array;
                        }
                        return value.Split('\r', '\n');
                    }
                case EPropertyValueType.Value:
                default:
                    {
                        return _value.ScriptStringList.Cast(ss => ss.scriptString);
                    }
            }
        }
    }

    #endregion 

    #region 枚举属性值

    /// <summary>
    /// 枚举属性值
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    [PropertyValueFieldName(nameof(_enumValue))]
    public class EnumPropertyValue<TEnum> : BasePropertyValue<TEnum>
        where TEnum : struct
    {
        /// <summary>
        /// 枚举值
        /// </summary>
        [Name("枚举值")]
        [HideInSuperInspector(nameof(_propertyValueType), EValidityCheckType.NotEqual, EPropertyValueType.Value)]
        [EnumPopup]
        public TEnum _enumValue = default;

        /// <summary>
        /// 属性值
        /// </summary>
        public override TEnum propertyValue => _enumValue;

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            switch (_propertyValueType)
            {
                case EPropertyValueType.Value:
                    {
                        return CommonFun.Name(typeof(TEnum), propertyValue.ToString());
                    }
                case EPropertyValueType.Variable:
                    {
                        return ScriptOption.VarFlag + variableName;
                    }
            }
            return "";
        }
    }

    /// <summary>
    /// 脚本参数布尔类型属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(EBool))]
    [PropertyType(typeof(bool))]
    public class EBoolPropertyValue : EnumPropertyValue<EBool>
    {
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public bool GetValue(bool value, EBool defaultValue = default) => CommonFun.BoolChange(value, GetValue(defaultValue));
    }

    /// <summary>
    /// 脚本参数布尔2类型属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(EBool2))]
    [PropertyType(typeof(bool))]
    public class EBool2PropertyValue : EnumPropertyValue<EBool2> { }

    /// <summary>
    /// 查询触发器交互
    /// </summary>
    [Serializable]
    [PropertyType(typeof(QueryTriggerInteraction))]
    public class QueryTriggerInteractionPropertyValue : EnumPropertyValue<QueryTriggerInteraction> { }

    /// <summary>
    /// 隐藏标识属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(HideFlags))]
    public class HideFlagsPropertyValue : EnumPropertyValue<HideFlags>
    {
    }

    /// <summary>
    /// 发送消息 Options属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(SendMessageOptions))]
    public class SendMessageOptionsPropertyValue : EnumPropertyValue<SendMessageOptions>
    {
    }

    /// <summary>
    /// Rigidbody Interpolation属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(RigidbodyInterpolation))]
    public class RigidbodyInterpolationPropertyValue : EnumPropertyValue<RigidbodyInterpolation>
    {
    }

    /// <summary>
    /// Force Mode属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(ForceMode))]
    public class ForceModePropertyValue : EnumPropertyValue<ForceMode>
    {
    }

    /// <summary>
    /// Collision Detection Mode属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(CollisionDetectionMode))]
    public class CollisionDetectionModePropertyValue : EnumPropertyValue<CollisionDetectionMode>
    {
    }

    /// <summary>
    /// Rigidbody Constraints属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(RigidbodyConstraints))]
    public class RigidbodyConstraintsPropertyValue : EnumPropertyValue<RigidbodyConstraints>
    {
    }

    /// <summary>
    /// 包围盒锚点属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(EBoundsAnchor))]
    public class EBoundsAnchorPropertyValue : EnumPropertyValue<EBoundsAnchor>
    {
    }


    /// <summary>
    /// 矩形锚点属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(ERectAnchor))]
    public class ERectAnchorPropertyValue : EnumPropertyValue<ERectAnchor>
    {
    }

    /// <summary>
    /// 空间类型属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(ESpaceType))]
    public class ESpaceTypePropertyValue : EnumPropertyValue<ESpaceType>
    {
    }

    /// <summary>
    /// 字符串类型属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(EStringType))]
    public class EStringTypePropertyValue : EnumPropertyValue<EStringType> { }

    #endregion

    #region Unity对象类型属性值

    /// <summary>
    /// 基础Unity对象属性值
    /// </summary>
    /// <typeparam name="TUnityObject"></typeparam>
    public abstract class BaseUnityObjectPropertyValue<TUnityObject> : BasePropertyValue<TUnityObject>
        where TUnityObject : UnityEngine.Object
    {
        /// <summary>
        /// 值转字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override string ValueToFriendlyString(TUnityObject value) => value ? value.name : "";
    }

    /// <summary>
    /// Unity对象属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(UnityEngine.Object), forChildrenClass = true)]
    [PropertyValueFieldName(nameof(_object))]
    public class UnityObjectPropertyValue : BaseUnityObjectPropertyValue<UnityEngine.Object>
    {
        /// <summary>
        /// Unity对象
        /// </summary>
        [Name("Unity对象")]
        [HideInSuperInspector(nameof(_propertyValueType), EValidityCheckType.NotEqual, EPropertyValueType.Value)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ObjectPopup]
        public UnityEngine.Object _object;

        /// <summary>
        /// 属性值
        /// </summary>
        public override UnityEngine.Object propertyValue => _object;
    }

    /// <summary>
    /// 游戏对象属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(GameObject))]
    [PropertyValueFieldName(nameof(_gameObject))]
    public class GameObjectPropertyValue : BaseUnityObjectPropertyValue<GameObject>
    {
        /// <summary>
        /// 游戏对象
        /// </summary>
        [Name("游戏对象")]
        [HideInSuperInspector(nameof(_propertyValueType), EValidityCheckType.NotEqual, EPropertyValueType.Value)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public GameObject _gameObject;

        /// <summary>
        /// 属性值
        /// </summary>
        public override GameObject propertyValue => _gameObject;
    }

    /// <summary>
    /// 转换属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(Transform), forChildrenClass = true)]
    [PropertyValueFieldName(nameof(_transfrom))]
    public class TransformPropertyValue : BaseUnityObjectPropertyValue<Transform>
    {
        /// <summary>
        /// 转换对象
        /// </summary>
        [Name("转换")]
        [HideInSuperInspector(nameof(_propertyValueType), EValidityCheckType.NotEqual, EPropertyValueType.Value)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform _transfrom;

        /// <summary>
        /// 属性值
        /// </summary>
        public override Transform propertyValue => _transfrom;
    }

    /// <summary>
    /// 相机组件属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(Camera))]
    [PropertyValueFieldName(nameof(_camera))]
    public class CameraPropertyValue : BaseUnityObjectPropertyValue<Camera>
    {
        /// <summary>
        /// 相机对象
        /// </summary>
        [Name("相机")]
        [HideInSuperInspector(nameof(_propertyValueType), EValidityCheckType.NotEqual, EPropertyValueType.Value)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Camera _camera;

        /// <summary>
        /// 属性值
        /// </summary>
        public override Camera propertyValue => _camera;
    }

    /// <summary>
    /// 基础组件属性值
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    [PropertyValueFieldName(nameof(_component))]
    public abstract class BaseComponentPropertyValue<TComponent> : BaseUnityObjectPropertyValue<TComponent>
        where TComponent : Component
    {
        /// <summary>
        /// 组件
        /// </summary>
        [Name("组件")]
        [HideInSuperInspector(nameof(_propertyValueType), EValidityCheckType.NotEqual, EPropertyValueType.Value)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public TComponent _component;

        /// <summary>
        /// 属性值
        /// </summary>
        public override TComponent propertyValue => _component;
    }

    /// <summary>
    /// 刚体属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(Rigidbody))]
    [PropertyValueFieldName(nameof(_rigidbody))]
    public class RigidbodyPropertyValue : BaseUnityObjectPropertyValue<Rigidbody>
    {
        /// <summary>
        /// 刚体
        /// </summary>
        [Name("刚体")]
        [HideInSuperInspector(nameof(_propertyValueType), EValidityCheckType.NotEqual, EPropertyValueType.Value)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Rigidbody _rigidbody;

        /// <summary>
        /// 属性值
        /// </summary>
        public override Rigidbody propertyValue => _rigidbody;
    }

    /// <summary>
    /// 材质属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(Material))]
    [PropertyValueFieldName(nameof(_material))]
    public class MaterialPropertyValue : BaseUnityObjectPropertyValue<Material>
    {
        /// <summary>
        /// 材质
        /// </summary>
        [Name("材质")]
        [HideInSuperInspector(nameof(_propertyValueType), EValidityCheckType.NotEqual, EPropertyValueType.Value)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Material _material;

        /// <summary>
        /// 属性值
        /// </summary>
        public override Material propertyValue => _material;
    }

    /// <summary>
    /// 精灵属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(Sprite))]
    [PropertyValueFieldName(nameof(_sprite))]
    public class SpritePropertyValue : BaseUnityObjectPropertyValue<Sprite>
    {
        /// <summary>
        /// 精灵
        /// </summary>
        [Name("精灵")]
        [HideInSuperInspector(nameof(_propertyValueType), EValidityCheckType.NotEqual, EPropertyValueType.Value)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Sprite _sprite;

        /// <summary>
        /// 属性值
        /// </summary>
        public override Sprite propertyValue => _sprite;
    }

    /// <summary>
    /// 贴图属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(Texture), forChildrenClass = true)]
    [PropertyValueFieldName(nameof(_texture))]
    public class TexturePropertyValue : BaseUnityObjectPropertyValue<Texture>
    {
        /// <summary>
        /// 贴图
        /// </summary>
        [Name("贴图")]
        [HideInSuperInspector(nameof(_propertyValueType), EValidityCheckType.NotEqual, EPropertyValueType.Value)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Texture _texture;

        /// <summary>
        /// 属性值
        /// </summary>
        public override Texture propertyValue => _texture;
    }

    /// <summary>
    /// 音频剪辑属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(AudioClip))]
    [PropertyValueFieldName(nameof(_audioClip))]
    public class AudioClipPropertyValue : BaseUnityObjectPropertyValue<AudioClip>
    {
        /// <summary>
        /// 音频剪辑
        /// </summary>
        [Name("音频剪辑")]
        [HideInSuperInspector(nameof(_propertyValueType), EValidityCheckType.NotEqual, EPropertyValueType.Value)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public AudioClip _audioClip;

        /// <summary>
        /// 属性值
        /// </summary>
        public override AudioClip propertyValue => _audioClip;
    }

    /// <summary>
    /// 视频剪辑属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(VideoClip))]
    [PropertyValueFieldName(nameof(_videoClip))]
    public class VideoClipPropertyValue : BaseUnityObjectPropertyValue<VideoClip>
    {
        /// <summary>
        /// 视频剪辑
        /// </summary>
        [Name("视频剪辑")]
        [HideInSuperInspector(nameof(_propertyValueType), EValidityCheckType.NotEqual, EPropertyValueType.Value)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public VideoClip _videoClip;

        /// <summary>
        /// 属性值
        /// </summary>
        public override VideoClip propertyValue => _videoClip;
    }

    #endregion
}
