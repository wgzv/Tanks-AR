using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.Extension.CNScripts.Base
{
    /// <summary>
    /// 用户定义脚本事件类型
    /// </summary>
    public enum EUserDefineScriptEventType
    {
        /// <summary>
        /// 用户定义事件1
        /// </summary>
        [Name("用户定义事件1")]
        UserDefineEvent1,

        /// <summary>
        /// 用户定义事件2
        /// </summary>
        [Name("用户定义事件2")]
        UserDefineEvent2,

        /// <summary>
        /// 用户定义事件3
        /// </summary>
        [Name("用户定义事件3")]
        UserDefineEvent3,

        /// <summary>
        /// 用户定义事件4
        /// </summary>
        [Name("用户定义事件4")]
        UserDefineEvent4,

        /// <summary>
        /// 用户定义事件5
        /// </summary>
        [Name("用户定义事件5")]
        UserDefineEvent5,

        /// <summary>
        /// 用户定义事件6
        /// </summary>
        [Name("用户定义事件6")]
        UserDefineEvent6,

        /// <summary>
        /// 用户定义事件7
        /// </summary>
        [Name("用户定义事件7")]
        UserDefineEvent7,

        /// <summary>
        /// 用户定义事件8
        /// </summary>
        [Name("用户定义事件8")]
        UserDefineEvent8,
    }

    /// <summary>
    /// 用户定义脚本事件集
    /// </summary>
    [Serializable]
    public class UserDefineScriptEventSet : BaseScriptEventSet<EUserDefineScriptEventType> { }

    /// <summary>
    /// 用户自定义脚本事件
    /// </summary>
    [Serializable]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.CNScriptMenu + Title)]
    public class UserDefineScriptEvent : BaseScriptEvent<EUserDefineScriptEventType, UserDefineScriptEventSet>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "用户自定义脚本事件";

        /// <summary>
        /// 获取函数显示名
        /// </summary>
        /// <param name="languageType"></param>
        /// <returns></returns>
        public override string[] GetFunctionDisplayNames(ELanguageType languageType = ELanguageType.CN) => GetFunctionNames();

        /// <summary>
        /// 获取函数名
        /// </summary>
        /// <returns></returns>
        public override string[] GetFunctionNames()
        {
            List<string> names = new List<string>();
            foreach (var l in functionList)
            {
                if (l.Enable) names.Add(l.name);
            }
            return names.ToArray();
        }

        public override T GetFunction<T>(int index) //where T : ScriptList
        {
            //return base.GetScriptList<T>(index);
            if (index < 0 || index >= functionList.Count) return null;
            for (int i = 0, f = 0; i < functionList.Count; ++i)
            {
                if (functionList[i].Enable)
                {
                    if (f == index) return functionList[i] as T;
                    ++f;
                }
            }
            return null;
        }

        public override bool AddFunction(string name)
        {
            if (string.IsNullOrEmpty(name) || GetFunction(name) != null) return false;
            foreach (var l in functionList)
            {
                if (!l.Enable)
                {
                    l.Enable = true;
                    l.name = name;
                    return true;
                }
            }
            return false;
        }

        public int IndexOfCanAddScriptEvent(string name)
        {
            if (string.IsNullOrEmpty(name) || GetFunction(name) != null) return -1;
            for (int i = 0; i < functionList.Count; ++i)
            {
                var l = functionList[i];
                if (!l.Enable)
                {
                    return i;
                }
            }
            return -1;
        }

        public override GUIContent GetGUIContent(UserDefineScriptEventSet set)
        {
            return new GUIContent(set.name, CommonFun.Tip(GetType()));
        }
    }
}
