using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.Scripts;

namespace XCSJ.Extension.CNScripts.Base
{
    /// <summary>
    /// 静态变量集合
    /// </summary>
    [Name(Title)]
    [Tip("应用程序运行期内均有效的变量；即变量的作用域在整个应用程序的运行期均有效；不会随着场景切换，而导致变量失效；应用程序退出后，变量信息会丢失；本变量存储在应用程序运行期的内存中；")]
    [Tool(CNScriptHelper.Title, nameof(ScriptManager))]
    [RequireComponent(typeof(ScriptManager))]
    [RequireManager(typeof(ScriptManager))]
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class StaticVarCollection : BaseVarCollection
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "静态变量集合";

        /// <summary>
        /// 静态层级变量字典
        /// </summary>
        protected static VarDictionary _varDictionary = new VarDictionary();

        /// <summary>
        /// 层级变量字典
        /// </summary>
        public override VarDictionary varDictionary => _varDictionary;

        /// <summary>
        /// 静态变量列表
        /// </summary>
        [Name("静态变量列表")]
        [Tip("应用程序运行期内均有效的变量；即变量的作用域在整个应用程序的运行期均有效；不会随着场景切换，而导致变量失效；应用程序退出后，变量信息会丢失；本变量存储在应用程序运行期的内存中；")]
        public List<Variable> _variableList = new List<Variable>();

        /// <summary>
        /// 变量作用域
        /// </summary>
        public override EVarScope varScope => EVarScope.Static;

        /// <summary>
        /// 变量列表
        /// </summary>
        public override List<Variable> variableList => _variableList;

        /// <summary>
        /// 当反序列化之后回调
        /// </summary>
        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            varDictionary.MustSerializeTo();
        }
    }
}
