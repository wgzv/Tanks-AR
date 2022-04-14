using System;
using System.Reflection;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Helper;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Dataflows.Binders
{
    /// <summary>
    /// 类型绑定器
    /// </summary>
    [Serializable]
    public class TypeBinder : ITypeBinder, IToFriendlyString
    {
        #region 别名

        /// <summary>
        /// 别名
        /// </summary>
        [Name("别名")]
        [Tip("用于表示当前绑定器的全局唯一描述名称；")]
        public string _alias = "";

        /// <summary>
        /// 别名
        /// </summary>
        public string alias
        {
            get => _alias;
            set
            {
                if (_alias != value)
                {
                    _alias = value;
                    AliasCache.instance.Clear();
                }
            }
        }

        #endregion

        #region 绑定器规则

        /// <summary>
        /// 绑定器规则
        /// </summary>
        [Name("绑定器规则")]
        [EnumPopup]
        public EBinderRule _typeBindRule = EBinderRule.Instance;

        /// <summary>
        /// 绑定器规则
        /// </summary>
        public EBinderRule typeBindRule { get => _typeBindRule; set => _typeBindRule = value; }

        /// <summary>
        /// 绑定器规则
        /// </summary>
        [Name("绑定器规则")]
        public enum EBinderRule
        {
            /// <summary>
            /// 静态 : 可绑定类类型中静态成员（包含被static修饰的成员）
            /// </summary>
            [Name("静态")]
            [Tip("可绑定类类型中静态成员（包含被static修饰的成员）")]
            Static,

            /// <summary>
            /// 实例 : 可绑定类类型中实例成员
            /// </summary>
            [Name("实例")]
            [Tip("可绑定类类型中实例成员")]
            Instance,

            /// <summary>
            /// 实例 : 可绑定类类型中实例成员
            /// </summary>
            [Name("实例类型")]
            [Tip("可绑定类类型中非静态实例类型")]
            InstanceType,

            /// <summary>
            /// 别名 : 用于指向绑定器的别名
            /// </summary>
            [Name("目标别名")]
            [Tip("用于指向绑定器的别名")]
            Alias,
        }

        #endregion

        #region 包括基础类型

        /// <summary>
        /// 包括基础类型：为True时，‘成员名称’可以是‘目标类型’的基础类型中的私有成员，此时处理逻辑复杂耗时长；为False时，无法处理‘成员名称’是‘目标类型’的基础类型中的私有成员的情况；
        [Name("包括基础类型")]
        [Tip("为True时，‘成员名称’可以是‘目标类型’的基础类型中的私有成员，此时处理逻辑复杂耗时长；为False时，无法处理‘成员名称’是‘目标类型’的基础类型中的私有成员的情况；")]
        [HideInSuperInspector(nameof(_typeBindRule), EValidityCheckType.Equal, EBinderRule.Alias)]
        public bool _includeBaseType = true;

        /// <summary>
        /// 包括基础类型
        /// </summary>
        public bool includeBaseType { get => _includeBaseType; set => _includeBaseType = value; }

        #endregion

        #region 静态成员类型存储字符串

        /// <summary>
        /// 目标类型：期望绑定的目标类型的全名称，用于绑定静态成员类型时使用；命名空间的层级使用分隔符/间隔；
        /// </summary>
        [Name("目标类型")]
        [Tip("期望绑定的目标类型的全名称，用于绑定静态成员类型时使用；命名空间的层级使用分隔符/间隔；")]
        [TypeFullNamePopup]
        [HideInSuperInspector(nameof(_typeBindRule), EValidityCheckType.NotEqual, EBinderRule.Static)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public string _targetType = "";

        /// <summary>
        /// 目标类型全名称:期望绑定的目标类型的全名称，用于绑定静态成员类型时使用；命名空间的层级使用分隔符/间隔；
        /// </summary>
        public string targetTypeFullName { get => _targetType; set => _targetType = value; }

        #endregion

        #region 实例目标存储对象

        /// <summary>
        /// 目标对象
        /// </summary>
        [Name("目标对象")]
        [Tip("期望绑定的目标对象")]
        [ObjectPopup]
        [HideInSuperInspector(nameof(_typeBindRule), EValidityCheckType.NotEqual, EBinderRule.Instance)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public UnityEngine.Object _target;

        /// <summary>
        /// 目标对象
        /// </summary>
        public UnityEngine.Object target
        {
            get
            {
                switch (typeBindRule)
                {
                    case EBinderRule.Instance: return _target;
                    case EBinderRule.InstanceType: return _instanceTypeObject as UnityEngine.Object;
                    case EBinderRule.Static: return null;
                    case EBinderRule.Alias:
                        {
                            var binder = AliasCache.instance.Get(targetAlias);
                            if (binder!=null && binder!=this)
                            {
                                return binder.entityObject as UnityEngine.Object;
                            }
                            return null;
                         }
                    default: return null;
                }
            }
            set
            {
                switch (typeBindRule)
                {
                    case EBinderRule.Instance: _target = value; break;
                    case EBinderRule.InstanceType: _instanceTypeObject = value; break;
                    case EBinderRule.Static: break;
                    case EBinderRule.Alias:
                        {
                            var binder = AliasCache.instance.Get(_targetAlias);
                            if (binder != null && binder != this)
                            {
                                binder.entityObject = value;
                            }
                            break;
                        }
                    default: break;
                }
            }
        }

        /// <summary>
        /// 目标类型
        /// </summary>
        public Type targetType
        {
            get
            {
                switch (typeBindRule)
                {
                    case EBinderRule.Static: return TypeCache.Get(targetTypeFullName);
                    case EBinderRule.Instance: return target ? target.GetType() : null;
                    case EBinderRule.InstanceType: return TypeCache.Get(instanceTypeFullName);
                    case EBinderRule.Alias:
                        {
                            var binder = AliasCache.instance.Get(targetAlias);
                            if (binder != null && binder != this)
                            {
                                return binder.entityType;
                            }
                            return null;
                        }
                    default: return null;
                }
            }
            set
            {
                switch (typeBindRule)
                {
                    case EBinderRule.Static: targetTypeFullName = TypeToString(value); break;
                    case EBinderRule.Instance: break;
                    case EBinderRule.InstanceType: break;
                    case EBinderRule.Alias: break;
                    default: break;
                }
            }
        }

        #endregion

        #region 实例类型存储字符串

        /// <summary>
        /// 实例类型
        /// </summary>
        [Name("实例类型")]
        [Tip("期望绑定的实例类型的全名称，用于绑定非静态成员类型时使用；")]
        [TypeFullNamePopup]
        [HideInSuperInspector(nameof(_typeBindRule), EValidityCheckType.NotEqual, EBinderRule.InstanceType)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public string _instanceTypeFullName = "";

        /// <summary>
        /// 目标类型全名称
        /// </summary>
        public string instanceTypeFullName { get => _instanceTypeFullName; set => _instanceTypeFullName = value; }

        /// <summary>
        /// 实例类型对象
        /// </summary>
        protected object _instanceTypeObject { get; set; }

        #endregion

        #region 目标别名

        /// <summary>
        /// 目标别名
        /// </summary>
        [Name("目标别名")]
        [Tip("用于指向绑定器的别名；")]
        [HideInSuperInspector(nameof(_typeBindRule), EValidityCheckType.NotEqual, EBinderRule.Alias)]
        [AliasPopup(hasField = false)]
        public string _targetAlias = "";

        /// <summary>
        /// 目标别名
        /// </summary>
        public string targetAlias { get => _targetAlias; set => _targetAlias = value; }

        #endregion

        #region ITypeBinder

        /// <summary>
        /// 主体类型
        /// </summary>
        public Type mainType => targetType;

        /// <summary>
        /// 主体对象
        /// </summary>
        public object mainObject
        {
            get
            {
                switch (typeBindRule)
                {
                    case EBinderRule.Static: return null;
                    case EBinderRule.Instance: return target;
                    case EBinderRule.InstanceType: return _instanceTypeObject;
                    case EBinderRule.Alias:
                        {
                            var binder = AliasCache.instance.Get(targetAlias);
                            if (binder != null && binder != this)
                            {
                                return binder.entityObject;// 别名主体指向别名所在实体对象
                            }
                            return null;
                        }
                    default: return null;
                }
            }
            set
            {
                switch (typeBindRule)
                {
                    case EBinderRule.Static: break;
                    case EBinderRule.Instance:
                        {
                            target = value as UnityEngine.Object;
                            break;
                        }
                    case EBinderRule.InstanceType:
                        {
                            if (value == null || targetType.IsAssignableFrom(value.GetType()))
                            {
                                _instanceTypeObject = value;
                            }
                            break;
                        }
                    case EBinderRule.Alias:
                        {
                            var binder = AliasCache.instance.Get(targetAlias);
                            if (binder != null && binder != this)
                            {
                                binder.entityObject = value;
                            }
                            break;
                        }
                    default: break;
                }
            }
        }

        /// <summary>
        /// 实体类型
        /// </summary>
        public virtual Type entityType { get => targetType; }

        /// <summary>
        /// 实体对象
        /// </summary>
        public virtual object entityObject { get => mainObject; set => mainObject = value; }

        /// <summary>
        /// 是否有成员
        /// </summary>
        public virtual bool hasMember => false;

        /// <summary>
        /// 成员信息
        /// </summary>
        public virtual MemberInfo memberInfo => null;

        /// <summary>
        /// 成员名称
        /// </summary>
        public virtual string memberName { get => null; set { } }

        /// <summary>
        /// 成员类型
        /// </summary>
        public virtual Type memberType => null;

        /// <summary>
        /// 成员值
        /// </summary>
        public virtual object memberValue { get; set; }

        #endregion

        #region 绑定标志

        /// <summary>
        /// 绑定标志
        /// </summary>
        public virtual BindingFlags bindingFlags => GetBindingFlags();

        /// <summary>
        /// 获取绑定标志
        /// </summary>
        /// <param name="bindStatic"></param>
        /// <returns></returns>
        public BindingFlags GetBindingFlags()
        {
            switch (typeBindRule)
            {
                case EBinderRule.Static: return TypeHelper.DefaultStaticHierarchy;
                case EBinderRule.Instance: return TypeHelper.DefaultInstanceHierarchy;
                case EBinderRule.InstanceType: return TypeHelper.DefaultInstanceHierarchy;
                case EBinderRule.Alias:
                    {
                        var binder = AliasCache.instance.Get(targetAlias);
                        return (binder != null && binder != this) ? binder.bindingFlags : BindingFlags.Default;
                    }
                default: return TypeHelper.Default;
            }
        }

        #endregion

        /// <summary>
        /// 类型转为字符串；用于<see cref="targetType"/>类型与<see cref="_targetType"/>字符串的转化；
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual string TypeToString(Type type) => type.FullNameToHierarchyString() ?? "";

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public virtual string ToFriendlyString()
        {
            switch (typeBindRule)
            {
                case EBinderRule.Static: return CommonFun.Name(targetType);
                case EBinderRule.Instance: return (target ? target.name : "");
                case EBinderRule.Alias: return targetAlias;
                default: return "";
            }
        }
    }


}
