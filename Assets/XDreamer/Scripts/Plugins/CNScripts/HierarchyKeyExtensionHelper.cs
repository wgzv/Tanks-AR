using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.Extension.CNScripts
{
    /// <summary>
    /// 层级键扩展组手：层级变量中的层级键扩展机制
    /// </summary>
    public static class HierarchyKeyExtensionHelper
    {
        /// <summary>
        /// 扩展数据
        /// </summary>
        public static Dictionary<EHierarchyKeyMode, List<HierarchyKeyExtensionData>> extensionDatas { get; } = new Dictionary<EHierarchyKeyMode, List<HierarchyKeyExtensionData>>();

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            extensionDatas.Clear();
            foreach (var info in MethodHelper.GetStaticMethodsAndAttribute<HierarchyKeyAttribute>())
            {
                var attribute = info.attribute;
                switch (attribute.hierarchyKeyMode)
                {
                    case EHierarchyKeyMode.Get:
                        {
                            var func = ToGetFunc(info.methodInfo);
                            if (func == null) break;
                            AddGetFunc(info, func);
                            HierarchyVarHelper.RegisterGetHierarchyVarValueFunc(attribute.hierarchyKey, func);
                            foreach (var key in attribute.hierarchyKeys)
                            {
                                HierarchyVarHelper.RegisterGetHierarchyVarValueFunc(key, func);
                            }
                            break;
                        }
                    case EHierarchyKeyMode.Set:
                        {
                            var func = ToSetFunc(info.methodInfo);
                            if (func == null) break;
                            AddSetFunc(info, func);
                            HierarchyVarHelper.RegisterSetHierarchyVarValueFunc(attribute.hierarchyKey, func);
                            foreach (var key in attribute.hierarchyKeys)
                            {
                                HierarchyVarHelper.RegisterSetHierarchyVarValueFunc(key, func);
                            }
                            break;
                        }
                }                
            }
        }

        #region 获取扩展机制

        private static void AddGetFunc(MethodHelper.Info<HierarchyKeyAttribute> info, Func<IVarContext, IHierarchyVar, string, string> getFunc)
        {
            var mode = info.attribute.hierarchyKeyMode;
            if (!extensionDatas.TryGetValue(mode, out var list))
            {
                extensionDatas[mode] = list = new List<HierarchyKeyExtensionData>();
            }
            list.Add(new HierarchyKeyExtensionData(info, getFunc));
        }

        private static Func<IVarContext, IHierarchyVar, string, string> ToGetFunc(MethodInfo methodInfo)
        {
            try
            {
                return Delegate.CreateDelegate(typeof(Func<IVarContext, IHierarchyVar, string, string>), methodInfo) as Func<IVarContext, IHierarchyVar, string, string>;
            }
            catch { return default; }
        }

        #endregion

        #region 设置扩展机制

        private static void AddSetFunc(MethodHelper.Info<HierarchyKeyAttribute> info, Func<IVarContext, IHierarchyVar, string, string, bool> setFunc)
        {
            var mode = info.attribute.hierarchyKeyMode;
            if (!extensionDatas.TryGetValue(mode, out var list))
            {
                extensionDatas[mode] = list = new List<HierarchyKeyExtensionData>();
            }
            list.Add(new HierarchyKeyExtensionData(info, setFunc));
        }

        private static Func<IVarContext, IHierarchyVar, string, string, bool> ToSetFunc(MethodInfo methodInfo)
        {
            try
            {
                return Delegate.CreateDelegate(typeof(Func<IVarContext, IHierarchyVar, string, string, bool>), methodInfo) as Func<IVarContext, IHierarchyVar, string, string, bool>;
            }
            catch { return default; }
        }

        #endregion
    }

    /// <summary>
    /// 层级键扩展数据
    /// </summary>
    public class HierarchyKeyExtensionData
    {
        /// <summary>
        /// 信息
        /// </summary>
        public MethodHelper.Info<HierarchyKeyAttribute> info{ get; private set; }

        /// <summary>
        /// 层级键特性
        /// </summary>
        public HierarchyKeyAttribute hierarchyKeyAttribute => info.attribute;

        /// <summary>
        /// 获取函数
        /// </summary>
        public Func<IVarContext, IHierarchyVar, string, string> getFunc { get; private set; }

        /// <summary>
        /// 设置函数
        /// </summary>
        public Func<IVarContext, IHierarchyVar, string, string, bool> setFunc { get; private set; }

        private string _hierarchyKeysString= null;

        /// <summary>
        /// 层级键数组字符串
        /// </summary>
        public string hierarchyKeysString => _hierarchyKeysString ?? (_hierarchyKeysString = hierarchyKeyAttribute.hierarchyKeys.ToStringDirect(","));

        private string _tip = null;

        /// <summary>
        /// 提示
        /// </summary>
        public string tip => _tip ?? (_tip = CommonFun.Tip(info.methodInfo));

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="info"></param>
        /// <param name="getFunc"></param>
        public HierarchyKeyExtensionData(MethodHelper.Info<HierarchyKeyAttribute> info, Func<IVarContext, IHierarchyVar, string, string> getFunc)
        {
            this.info = info;
            this.getFunc = getFunc;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="info"></param>
        /// <param name="setFunc"></param>
        public HierarchyKeyExtensionData(MethodHelper.Info<HierarchyKeyAttribute> info, Func<IVarContext, IHierarchyVar, string, string, bool> setFunc)
        {
            this.info = info;
            this.setFunc = setFunc;
        }
    }

    /// <summary>
    /// 层级键特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class HierarchyKeyAttribute : Attribute
    {
        /// <summary>
        /// 层级键模式
        /// </summary>
        public EHierarchyKeyMode hierarchyKeyMode { get; private set; }

        /// <summary>
        /// 层级键
        /// </summary>
        public string hierarchyKey { get; private set; }

        /// <summary>
        /// 层级键数组
        /// </summary>
        public string[] hierarchyKeys { get; private set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="hierarchyKey"></param>
        /// <param name="hierarchyKeys"></param>
        public HierarchyKeyAttribute(EHierarchyKeyMode hierarchyKeyMode, string hierarchyKey, params string[] hierarchyKeys)
        {
            this.hierarchyKeyMode = hierarchyKeyMode;
            this.hierarchyKey = !string.IsNullOrEmpty(hierarchyKey) ? hierarchyKey : throw new ArgumentException(nameof(hierarchyKey));
            this.hierarchyKeys = hierarchyKeys ?? Empty<string>.Array;
        }
    }

    /// <summary>
    /// 层级键模式：用于扩展层级键机制时的具体模式
    /// </summary>
    public enum EHierarchyKeyMode
    {
        /// <summary>
        /// 获取
        /// </summary>
        [Name("主页")]
        [XCSJ.Attributes.Icon(EIcon.Home)]
        Main,

        /// <summary>
        /// 获取
        /// </summary>
        [Name("获取")]
        [XCSJ.Attributes.Icon(EIcon.Property)]
        Get,

        /// <summary>
        /// 设置
        /// </summary>
        [Name("设置")]
        [XCSJ.Attributes.Icon(EIcon.Property)]
        Set,
    }
}
