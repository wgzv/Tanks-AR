using System;
using XCSJ.Attributes;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.Extension.Base.Dataflows.Base
{
    /// <summary>
    /// 实参：用于存储多种不同类型参数的容器类（包括中文脚本变量）；如期望仅存储单一类型参数与中文脚本变量，请参考属性值<see cref="PropertyValue{T}"/>
    /// </summary>
    [Serializable]
    [Name("实参")]
    public class Argument : IToFriendlyString
    {
        [Name("实参类型")]
        [EnumPopup]
        public EArgumentType _argumentType = EArgumentType.UnityObject;

        [Name("全局变量名")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.Variable)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        [GlobalVariable]
        public string _variableName;

        [Name("对象值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.UnityObject)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public UnityEngine.Object _objectValue;

        [Name("布尔值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.Bool)]
        public bool _boolValue;

        [Name("整形值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.Int)]
        public int _intValue;

        [Name("长整形值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.Long)]
        public long _longValue;

        [Name("浮点值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.Float)]
        public float _floatValue;

        [Name("双精度浮点值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.Double)]
        public double _doubleValue;

        [Name("字符串值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.String)]
        public string _stringValue;

        [Name("枚举长整型值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.EnumLong)]
        public long _enumLongValue;

        [Name("枚举字符串值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.EnumString)]
        public string _enumStringValue;

        [Name("Unity事件")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.UnityEvent)]
        public string _unityEventStringValue;

        [Name("链表接口类型")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.IList)]
        public string _IListStringValue;

        [Name("颜色类型")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.Color)]
        public string _color;

        /// <summary>
        /// 值
        /// </summary>
        public object value { get => GetValue(); set => SetValue(value); }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <returns></returns>
        public object GetValue()
        {
            switch (_argumentType)
            {
                case EArgumentType.Variable: return ScriptManager.GetGlobalVariableValue(_variableName);
                case EArgumentType.UnityObject: return _objectValue;
                case EArgumentType.Bool: return _boolValue;
                case EArgumentType.Int: return _intValue;
                case EArgumentType.Long: return _longValue;
                case EArgumentType.Float: return _floatValue;
                case EArgumentType.Double: return _doubleValue;
                case EArgumentType.String: return _stringValue;
                case EArgumentType.EnumLong: return _enumLongValue;
                case EArgumentType.EnumString: return _enumStringValue;
                case EArgumentType.UnityEvent: return _unityEventStringValue;
                case EArgumentType.IList: return _IListStringValue;
                case EArgumentType.Color: return _color;
                case EArgumentType.Void:
                default: return null;
            }
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(object value)
        {
            if (value == null) return;
            if(value is UnityEngine.Object obj)
            {
                _argumentType = EArgumentType.UnityObject;
                _objectValue = obj;
            }
            else if(value is string str)
            {
                if (VariableHelper.IsVariable(str))
                {
                    _argumentType = EArgumentType.Variable;
                    _variableName = str;
                }
                else
                {
                    _argumentType = EArgumentType.String;
                    _stringValue = str;
                }
            }
            else if (value is int i)
            {
                _argumentType = EArgumentType.Int;
                _intValue = i;
            }
            else if (value is float f)
            {
                _argumentType = EArgumentType.Float;
                _floatValue = f;
            }
            else if (value is bool b)
            {
                _argumentType = EArgumentType.Bool;
                _boolValue = b;
            }
            else if (value is double d)
            {
                _argumentType = EArgumentType.Double;
                _doubleValue = d;
            }
            else if (value is long l)
            {
                _argumentType = EArgumentType.Long;
                _longValue = l;
            }
        }

        /// <summary>
        /// 判断能否处理传入的类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool CanHandle(Type type) => ArgumentTypeHelper.CanHandle(type);

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public virtual string ToFriendlyString() => DefaultFriendlyString();

        /// <summary>
        /// 默认友好字符串
        /// </summary>
        /// <returns></returns>
        public string DefaultFriendlyString()
        {
            switch (_argumentType)
            {
                case EArgumentType.Variable: return ScriptOption.VarFlag + _variableName;
                default: return CommonFun.ObjectToString(value) ?? "";
            }
        }
    }
}
