using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Dataflows.Binders
{
    /// <summary>
    /// 字段或属性绑定器
    /// </summary>
    [Serializable]
    public class FieldPropertyMethodBinder : TypeMemberBinder
    {
        /// <summary>
        /// 绑定类型:标识类型中可绑定成员信息的类型
        /// </summary>
        [Name("绑定类型")]
        [Tip("标识类型中可绑定成员信息的类型;")]
        [EnumPopup]
        public EBindType _bindType = EBindType.PropertyDeclaredOnly;

        /// <summary>
        /// 绑定标志
        /// </summary>
        public override BindingFlags bindingFlags
        {
            get
            {
                switch (_bindType)
                {
                    case EBindType.FieldDeclaredOnly:
                    case EBindType.PropertyDeclaredOnly:
                    case EBindType.MethodDeclaredOnly:
                        {
                            return base.bindingFlags | BindingFlags.DeclaredOnly;
                        }
                }
                return base.bindingFlags;
            }
        }

        /// <summary>
        /// 绑定成员信息类型
        /// </summary>
        public EBindMemberInfoType bindMemberInfoType
        {
            get
            {
                switch (_bindType)
                {
                    case EBindType.Field: 
                    case EBindType.FieldDeclaredOnly: return EBindMemberInfoType.Field;
                    case EBindType.Property: 
                    case EBindType.PropertyDeclaredOnly: return EBindMemberInfoType.Property;
                    case EBindType.Method:
                    case EBindType.MethodDeclaredOnly: return EBindMemberInfoType.Method;
                    default: throw new ArgumentException();
                }
            }
        }

        /// <summary>
        /// 成员信息对象
        /// </summary>
        public override MemberInfo memberInfo 
        { get
            {
                switch (bindMemberInfoType)
                {
                    case EBindMemberInfoType.Field: return fieldInfo;
                    case EBindMemberInfoType.Property: return propertyInfo;
                    case EBindMemberInfoType.Method: return methodInfo;
                    default: throw new ArgumentException();
                }
            } 
        }

        /// <summary>
        /// 成员值，当成员信息类型为字段或属性时本参数有意义；
        /// </summary>
        public override object memberValue 
        {
            get
            {
                switch (bindMemberInfoType)
                {
                    case EBindMemberInfoType.Field: 
                    case EBindMemberInfoType.Property: return GetMemberValue();
                    case EBindMemberInfoType.Method:
                    default: return null;
                }
            }
            set
            {
                switch (bindMemberInfoType)
                {
                    case EBindMemberInfoType.Field:
                    case EBindMemberInfoType.Property: SetMemberValue(value); break;
                    case EBindMemberInfoType.Method: methodInfo?.Invoke(mainObject, null); break;
                    default: break;
                }
            }
        }

        /// <summary>
        /// 字段信息对象
        /// </summary>
        public FieldInfo fieldInfo => bindMemberInfoType == EBindMemberInfoType.Field ? FieldInfoCache.Get(mainType, memberName, bindingFlags, includeBaseType) : null;

        /// <summary>
        /// 属性信息对象
        /// </summary>
        public PropertyInfo propertyInfo => bindMemberInfoType == EBindMemberInfoType.Property ? PropertyInfoCache.Get(mainType, memberName, bindingFlags, includeBaseType) : null;

        /// <summary>
        /// 属性信息对象
        /// </summary>
        public MethodInfo methodInfo => bindMemberInfoType == EBindMemberInfoType.Method ? MethodInfoCache.Get(mainType, memberName, bindingFlags, includeBaseType) : null;

        /// <summary>
        /// 绑定类型:标识类型中可绑定成员信息的类型
        /// </summary>
        [Name("绑定类型")]
        [Tip("标识类型中可绑定成员信息的类型")]
        public enum EBindType
        {
            /// <summary>
            /// 字段:标识可绑定类型中字段信息
            /// </summary>
            [Name("字段")]
            [Tip("标识可绑定类型中字段信息")]
            Field,

            /// <summary>
            /// 属性:标识可绑定类型中属性信息
            /// </summary>
            [Name("属性")]
            [Tip("标识可绑定类型中属性信息")]
            Property,

            /// <summary>
            /// 方法:标识可绑定类型中方法信息（仅使用空参数方法）
            /// </summary>
            [Name("方法")]
            [Tip("标识可绑定类型中方法信息（仅使用空参数方法）")]
            Method,

            /// <summary>
            /// 仅声明字段:标识可绑定类型中仅声明字段信息（不包含基类）
            /// </summary>
            [Name("仅声明字段")]
            [Tip("标识可绑定类型中仅声明字段信息（不包含基类）")]
            FieldDeclaredOnly,

            /// <summary>
            /// 仅声明属性:标识可绑定类型中仅声明属性信息（不包含基类）
            /// </summary>
            [Name("仅声明属性")]
            [Tip("标识可绑定类型中仅声明属性信息（不包含基类）")]
            PropertyDeclaredOnly,

            /// <summary>
            /// 仅声明方法:标识可绑定类型中仅声明方法信息（不包含基类，仅使用空参数方法）
            /// </summary>
            [Name("仅声明方法")]
            [Tip("标识可绑定类型中仅声明方法信息（不包含基类，仅使用空参数方法）")]
            MethodDeclaredOnly,
        }

        /// <summary>
        /// 绑定成员信息类型
        /// </summary>
        public enum EBindMemberInfoType
        {
            Field,

            Property,

            Method,
        }

        #region 针对字段与属性的缓存机制

        /// <summary>
        /// 判断类型是否时可被处理的
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool CanHandled(Type type) => Argument.CanHandle(type);

        /// <summary>
        /// 获取所有具有字段或属性的类型全名称字符串；类型全名称的命名空间层级以'/'间隔；
        /// </summary>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public static string[] GetTypeFullNames(EBindMemberInfoType bindMemberInfoType, BindingFlags bindingFlags, bool includeBaseType) => TypeFullNameCache.GetTypeFullNames(bindMemberInfoType, bindingFlags, includeBaseType);

        /// <summary>
        /// 获取字段或属性名称列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string[] GetMemberNames(Type type, EBindMemberInfoType bindMemberInfoType, BindingFlags bindingFlags, bool includeBaseType) => MemberNameCache.GetMemberNames(type, bindMemberInfoType, bindingFlags, includeBaseType);

        internal class TypeFullNameCache : TIVCache<TypeFullNameCache, EBindMemberInfoType, BindingFlags, bool, TypeFullNameCacheValue>
        {
            public static string[] GetTypeFullNames(EBindMemberInfoType bindMemberInfoType, BindingFlags bindingFlags, bool includeBaseType)
            {
                return GetCacheValue(bindMemberInfoType, bindingFlags, includeBaseType).typeFullNames;
            }
        }

        internal class TypeFullNameCacheValue : TIVCacheValue<TypeFullNameCacheValue, EBindMemberInfoType, BindingFlags, bool>
        {
            public string[] typeFullNames { get; private set; } = Empty<string>.Array;

            public override bool Init()
            {
                var names = new List<string>();
                TypeHelper.GetTypes(type =>
                {
                    if (!type.IsClass || !type.IsPublic) return false;
                    if (type.IsGenericType || type.IsAbstract) return false;
                    if (!MemberNameCache.HasMembers(type, key1, key2, key3)) return false;

                    names.Add(type.FullNameToHierarchyString());
                    return true;
                });
#if UNITY_EDITOR
                //排序
                names.Sort();
#endif
                //转数组
                typeFullNames = names.ToArray();
                return true;
            }
        }

        internal class MemberNameCache : TIVCache<MemberNameCache, Type, EBindMemberInfoType, BindingFlags, bool, MemberNameCacheValue>
        {
            public static string[] GetMemberNames(Type type, EBindMemberInfoType bindMemberInfoType, BindingFlags bindingFlags, bool includeBaseType)
            {
                if (type == null) return Empty<string>.Array;
                return GetCacheValue(type, bindMemberInfoType, bindingFlags, includeBaseType)?.memberNames ?? Empty<string>.Array;
            }

            public static bool HasMembers(Type type, EBindMemberInfoType bindMemberInfoType, BindingFlags bindingFlags, bool includeBaseType)
            {
                if (type == null) return false;
                return GetCacheValue(type, bindMemberInfoType, bindingFlags, includeBaseType).memberNames.Length > 0;
            }
        }

        /// <summary>
        /// 成员名称缓存；键值依次为：类型、属性或字段或事件或方法、绑定标志、是否包含基类
        /// </summary>
        internal class MemberNameCacheValue : TIVCacheValue<MemberNameCacheValue, Type, EBindMemberInfoType, BindingFlags, bool>
        {
            public string[] memberNames { get; private set; } = Empty<string>.Array;

            public override bool Init()
            {
                IEnumerable<string> names = null;
                switch (key2)
                {
                    case EBindMemberInfoType.Field:
                        names = FieldInfosCache.Get(key1, key3, key4).Where(member => CanHandled(member.FieldType)).Cast(member => member.Name);
                        break;
                    case EBindMemberInfoType.Property:
                        names = PropertyInfosCache.Get(key1, key3, key4).Where(member => CanHandled(member.PropertyType)).Cast(member => member.Name);
                        break;
                    case EBindMemberInfoType.Method:
                        names = MethodInfosCache.Get(key1, key3, key4).Where(member =>
                        {
                            return (member.ReturnType == typeof(void) && member.GetParameters().Length == 0) ? true : false;
                        }).Cast(member => member.Name);
                        break;
                    default:
                        break;
                }

                //去重
                names = names.Distinct();

#if UNITY_EDITOR
                //排序
                names = names.OrderBy(n => n);
#endif

                //转数组
                memberNames = names.ToArray();

                return true;
            }
        }

        #endregion
    }
}
