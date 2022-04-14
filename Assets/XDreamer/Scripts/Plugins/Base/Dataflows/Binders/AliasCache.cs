using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;

namespace XCSJ.Extension.Base.Dataflows.Binders
{
    /// <summary>
    /// 类型成员绑定器获取接口
    /// </summary>
    public interface IBinderGetter
    {
        /// <summary>
        /// 通过类型获取绑定器成员
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        ITypeMemberBinder Get(string alias);

        /// <summary>
        /// 获取别名
        /// </summary>
        /// <returns></returns>
        string[] GetAliases();
    }

    /// <summary>
    /// 绑定器注册器接口
    /// </summary>
    public interface IBinderRegister
    {
        /// <summary>
        /// 有效性
        /// </summary>
        bool valid { get; }

        IEnumerable<ITypeMemberBinder> GetBinders();
    }

    /// <summary>
    /// 空绑定器
    /// </summary>
    public class EmptyTypeMemberBinder : ITypeMemberBinder
    {
        /// <summary>
        /// 无成员
        /// </summary>
        public bool hasMember => false;

        /// <summary>
        /// 默认别名
        /// </summary>
        public string alias => "无";

        /// <summary>
        /// 实体类型
        /// </summary>
        public Type entityType => mainType;

        /// <summary>
        /// 实体对象
        /// </summary>
        public object entityObject { get => mainObject; set => mainObject = value; }

        /// <summary>
        /// 绑定标识
        /// </summary>
        public BindingFlags bindingFlags => BindingFlags.Default;

        /// <summary>
        /// 主类型
        /// </summary>
        public Type mainType => null;

        /// <summary>
        /// 主体对象
        /// </summary>
        public object mainObject { get => null; set { } }

        /// <summary>
        /// 成员类型
        /// </summary>
        public MemberInfo memberInfo => null;

        /// <summary>
        /// 成员对象
        /// </summary>
        public Type memberType => null;

        /// <summary>
        /// 成员值
        /// </summary>
        public object memberValue { get => null; set { } }

        /// <summary>
        /// 成员名
        /// </summary>
        public string memberName { get => null; set { } }

        /// <summary>
        /// 包含基础类型
        /// </summary>
        public bool includeBaseType => false;
    }

    /// <summary>
    /// 空绑定器注册器
    /// </summary>
    public class EmptyBinderRegister : IBinderRegister
    {
        /// <summary>
        /// 有效
        /// </summary>
        public bool valid => true;

        private ITypeMemberBinder[] _registerBinders = new ITypeMemberBinder[1] { new EmptyTypeMemberBinder() };

        /// <summary>
        /// 获取绑定器
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ITypeMemberBinder> GetBinders() => _registerBinders;
    }

    /// <summary>
    /// 类型成员绑定器别名缓存
    /// </summary>
    public sealed class AliasCache : InstanceClass<AliasCache>, IBinderGetter
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public AliasCache() 
        {
            //注入空对象
            Register(emptyTypeMemberBinder);
        }

        private EmptyBinderRegister emptyTypeMemberBinder = new EmptyBinderRegister();

        /// <summary>
        /// 别名缓存
        /// </summary>
        private Dictionary<string, ITypeMemberBinder> aliasDic = new Dictionary<string, ITypeMemberBinder>();

        /// <summary>
        /// 别名已添加事件 ：当别名已添加到缓存之后的回调事件
        /// </summary>
        public event Action<string, ITypeMemberBinder> onAddedAlias = null;

        /// <summary>
        /// 别名已移除事件 ：当别名从缓存已移除之后的回调事件
        /// </summary>
        public event Action<string, ITypeMemberBinder> onRemovedAlias = null;

        /// <summary>
        /// 别名清除事件 ：当别名缓存全部清理之后的回调事件
        /// </summary>
        public event Action onClearedAlias = null;

        /// <summary>
        /// 注册对象
        /// </summary>
        private HashSet<IBinderRegister> registerBinders = new HashSet<IBinderRegister>();

        private bool _needUpdateCache = false;

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="register"></param>
        public void Register(IBinderRegister register)
        {
            if (register == null) return;
            if (!registerBinders.Add(register)) return;// 未发生改动

            _needUpdateCache = true;

            Udpate();
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <param name="register"></param>
        public void Unregister(IBinderRegister register)
        {
            if (register == null) return;
            if (!registerBinders.Remove(register)) return;// 未发生改动

            Clear();

            foreach (var b in register.GetBinders())
            {
                onRemovedAlias?.Invoke(b.alias, b);
            }
        }

        /// <summary>
        /// 清除所有注册绑定器
        /// </summary>
        public void RemoveAllRegister()
        {
            registerBinders.Clear();
        }

        /// <summary>
        /// 通过别名获取绑定器
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public ITypeMemberBinder Get(string alias)
        {
            Udpate();
            aliasDic.TryGetValue(alias, out ITypeMemberBinder typeMemberBinder);
            return typeMemberBinder;
        }

        /// <summary>
        /// 获取缓存别名
        /// </summary>
        /// <returns></returns>
        public string[] GetAliases()
        {
            Udpate();
            var list = aliasDic.Keys.ToList();
            list.Sort();
            return list.ToArray();
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        public void Clear()
        {
            aliasDic.Clear();

            onClearedAlias?.Invoke();

            // 注入空对象
            Register(emptyTypeMemberBinder);
        }

        /// <summary>
        /// 更新缓存
        /// </summary>
        private void Udpate()
        {
            if (_needUpdateCache)
            {
                _needUpdateCache = false;

                // 因为初始化别名缓存 => 触发绑定 => 触发初始化 => 集合类触发克隆  => 克隆触发OnEnable  => 触发注册 
                // 导致在遍历过程中会修改当前遍历的集合对象，所以需要重新创建一个容器去遍历
                foreach (var item in new HashSet<IBinderRegister>(registerBinders))
                {
                    if (item != null && item.valid)
                    {
                        foreach (var b in item.GetBinders())
                        {
                            AddBinder(b.alias, b);
                        }
                    }
                }
            }
        }

        private bool AddBinder(string alias, ITypeMemberBinder binder)
        {
            if (string.IsNullOrEmpty(alias)) return false;

            if (binder == null) return false;

            if (aliasDic.ContainsKey(alias))
            {
                //Debug.Log(string.Format("{0}别名注册重名！", alias));
                return false;
            }

            aliasDic.Add(alias, binder);

            onAddedAlias?.Invoke(alias, binder);

            return true;
        }

        /// <summary>
        /// 获取绑定器
        /// </summary>
        /// <param name="aliasDataSource"></param>
        /// <param name="go"></param>
        /// <returns></returns>
        public IBinderGetter[] GetBinderGetter(EAliasDataSource aliasDataSource, GameObject go)
        {
            switch (aliasDataSource)
            {
                case EAliasDataSource.Global: return new IBinderGetter[1] { this };
                case EAliasDataSource.Local: return GetBinderGetter(go);
                case EAliasDataSource.LocalOrGlobal: return GetBinderGetter(go).Union(new IBinderGetter[1] { this }).ToArray();
                default: return new IBinderGetter[0];
            }
        }

        /// <summary>
        /// 获取别名列表
        /// </summary>
        /// <param name="aliasDataSource"></param>
        /// <param name="go"></param>
        /// <returns></returns>
        public string[] GetAliases(EAliasDataSource aliasDataSource, GameObject go)
        {
            switch (aliasDataSource)
            {
                case EAliasDataSource.Global:  return GetAliases();
                case EAliasDataSource.Local: return GetAliases(go);
                case EAliasDataSource.LocalOrGlobal: return GetAliases(go).Union(GetAliases()).ToArray();
                default: return new string[0];
            }
        }

        /// <summary>
        /// 从游戏对象上获取绑定器
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static IBinderGetter[] GetBinderGetter(GameObject go)
        {
            if (!go) return new IBinderGetter[0];
            return go.GetComponentsInChildren<IBinderGetter>(true);
        }

        /// <summary>
        /// 从游戏对象上获取别名
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static string[] GetAliases(GameObject go)
        {
            var list = new List<string>();

            var getters = GetBinderGetter(go);
            foreach (var item in getters)
            {
                foreach (var alias in item.GetAliases())
                {
                    list.Add(alias);
                }
            }
            
            return list.ToArray();
        }
    }

    /// <summary>
    /// 别名数据源
    /// </summary>
    [Name("别名数据源")]
    public enum EAliasDataSource
    {
        /// <summary>
        /// 全局
        /// </summary>
        [Name("全局")]
        [Tip("仅查找全局注册数据源")]
        Global,

        /// <summary>
        /// 本地
        /// </summary>
        [Name("本地")]
        [Tip("仅从当前游戏对象及子对象上查找别名数据源")]
        Local,

        /// <summary>
        /// 本地或全局
        /// </summary>
        [Name("本地或全局")]
        [Tip("优先从当前游戏对象及子对象上查找别名数据源, 本地无数据时，则从已全局缓存中查找")]
        LocalOrGlobal,
    }
}
