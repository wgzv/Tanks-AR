using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Binders;

namespace XCSJ.PluginDataflows.Models
{
    /// <summary>
    /// 字段属性方法绑定器MB
    /// </summary>
    [ExecuteInEditMode]
    [Name("数据注册器")]
    public class DataRegister : BaseDataflowMB, IBinderGetter, IDropdownPopupAttribute, IBinderRegister
    {
        /// <summary>
        /// 注册至缓存
        /// </summary>
        [Name("注册全局别名")]
        [Tip("将数据源注册至别名缓存中")]
        public bool _registerAliasCache = false;

        /// <summary>
        /// 注册至缓存
        /// </summary>
        public bool registerAliasCache
        {
            get => _registerAliasCache;
            set
            {
                if (_registerAliasCache != value)
                {
                    _registerAliasCache = value;
                    if (_registerAliasCache)
                    {
                        AliasCache.instance.Register(this);
                    }
                    else
                    {
                        AliasCache.instance.Unregister(this);
                    }

                    AliasCache.instance.Clear();
                }
            }
        }

        /// <summary>
        /// 绑定器列表:用于绑定对象的字段、属性或无形参方法的信息对象
        /// </summary>
        [Name("绑定器列表")]
        [Tip("用于绑定对象的字段或属性信息的对象")]
        public List<FieldPropertyMethodBinder> _binders = new List<FieldPropertyMethodBinder>();

        /// <summary>
        /// 绑定器列表属性
        /// </summary>
        public List<FieldPropertyMethodBinder> binders { get => _binders; set => _binders = value; }

        #region IRegisterBinder

        /// <summary>
        /// 获取待注册的绑定器
        /// </summary>
        public IEnumerable<ITypeMemberBinder> GetBinders() => binders.Cast(b => b);

        #endregion

        #region IBinderGetter

        /// <summary>
        /// 获取绑定器
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public ITypeMemberBinder Get(string alias) => binders.Find(b => b.alias == alias);

        /// <summary>
        /// 获取别名列表
        /// </summary>
        /// <returns></returns>
        public string[] GetAliases()
        {
            return binders.ConvertAll<string>(b => b.alias).ToArray();
        } 

        #endregion

        /// <summary>
        /// 有效
        /// </summary>
        public bool valid => this && enabled;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            if (_registerAliasCache) AliasCache.instance.Register(this);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            AliasCache.instance.Unregister(this);
        }

        /// <summary>
        /// 有效
        /// </summary>
        protected void OnValidate()
        {
            if (_registerAliasCache)
            {
                AliasCache.instance.Register(this);
            }
            else
            {
                AliasCache.instance.Unregister(this);
            }

            AliasCache.instance.Clear();
        }

        #region IDropdownPopupAttribute实现

        /// <summary>
        /// 尝试获取选项文本列表；
        /// </summary>
        /// <param name="purpose">目标用途</param>
        /// <param name="propertyPath">属性路径</param>
        /// <param name="options">选项文本列表；如果期望下拉式弹出菜单出现层级，需要数组元素中有'/'</param>
        /// <returns></returns>
        public virtual bool TryGetOptions(string purpose, string propertyPath, out string[] options)
        {
            switch (purpose)
            {
                case nameof(MemberNamePopupAttribute):
                    {
                        if (propertyPath.Contains(nameof(_binders)))
                        {
                            var match = Regex.Match(propertyPath, @"\d+");
                            if (match.Success && _binders.ElementAtOrDefault(Converter.instance.ConvertTo<int>(match.Value)) is FieldPropertyMethodBinder setBinder)
                            {
                                options = FieldPropertyMethodBinder.GetMemberNames(setBinder.targetType, setBinder.bindMemberInfoType, setBinder.bindingFlags, setBinder.includeBaseType);
                                return true;
                            }

                        }
                        break;
                    }
                case nameof(TypeFullNamePopupAttribute):
                    {
                        if (propertyPath.Contains(nameof(_binders)))
                        {
                            var match = Regex.Match(propertyPath, @"\d+");
                            if (match.Success && _binders.ElementAtOrDefault(Converter.instance.ConvertTo<int>(match.Value)) is FieldPropertyMethodBinder setBinder)
                            {
                                options = FieldPropertyMethodBinder.GetTypeFullNames(setBinder.bindMemberInfoType, setBinder.bindingFlags, setBinder.includeBaseType);
                                return true;
                            }
                        }
                        break;
                    }
                case nameof(AliasPopupAttribute):
                    {
                        options = AliasCache.instance.GetAliases();
                        return true;
                    }
            }
            options = default;
            return false;
        }

        /// <summary>
        /// 尝试获取文本选项
        /// </summary>
        /// <param name="purpose">目标用途</param>
        /// <param name="propertyPath">属性路径</param>
        /// <param name="propertyValue">属性值</param>
        /// <param name="option">选项文本</param>
        /// <returns></returns>
        public virtual bool TryGetOption(string purpose, string propertyPath, object propertyValue, out string option)
        {
            switch (purpose)
            {
                case nameof(MemberNamePopupAttribute):
                    {
                        option = (propertyValue as string) ?? "";
                        return true;
                    }
                case nameof(TypeFullNamePopupAttribute):
                    {
                        option = (propertyValue as string) ?? "";
                        return true;
                    }
                case nameof(AliasPopupAttribute):
                    {
                        option = (propertyValue as string) ?? "";
                        return true;
                    }
            }
            option = default;
            return false;
        }

        /// <summary>
        /// 尝试获取属性值
        /// </summary>
        /// <param name="purpose">目标用途</param>
        /// <param name="propertyPath">属性路径</param>
        /// <param name="option">选项文本</param>
        /// <param name="propertyValue">属性值</param>
        /// <returns></returns>
        public virtual bool TryGetPropertyValue(string purpose, string propertyPath, string option, out object propertyValue)
        {
            switch (purpose)
            {
                case nameof(MemberNamePopupAttribute):
                    {
                        propertyValue = option;
                        return true;
                    }
                case nameof(TypeFullNamePopupAttribute):
                    {
                        propertyValue = option;
                        return true;
                    }
                case nameof(AliasPopupAttribute):
                    {
                        propertyValue = option;
                        return true;
                    }
            }
            propertyValue = default;
            return false;
        }

        #endregion
    }
}
