using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.Scripts;

namespace XCSJ.Extension.CNScripts.Base
{
    /// <summary>
    /// 基础变量集合
    /// </summary>
    [RequireManager(typeof(ScriptManager))]
    [XCSJ.Attributes.Icon(EIcon.Variable)]
    public abstract class BaseVarCollection : MB, IVariableHandle, ISerializationCallbackReceiver
    {
        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            var manager = GetComponent<ScriptManager>();
            if (manager)
            {
                UpdateVariable();
                manager.RegisterVarCollection(this);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();

            var manager = GetComponent<ScriptManager>();
            if (manager)
            {
                manager.UnegisterVarCollection(this);
            }
        }

        #region IVarCollection接口实现

        /// <summary>
        /// 变量作用域
        /// </summary>
        public abstract EVarScope varScope { get; }

        /// <summary>
        /// 层级变量字典
        /// </summary>
        public abstract VarDictionary varDictionary { get; }

        /// <summary>
        /// 尝试添加变量(增)
        /// </summary>
        /// <param name="varName">变量名称</param>
        /// <param name="varValue">变量值</param>
        /// <param name="varType">变量类型</param>
        /// <param name="hierarchyVar">层级变量：根层级变量</param>
        /// <returns></returns>
        public bool TryAddVar(string varName, string varValue, EVarType varType, out IHierarchyVar hierarchyVar) => varDictionary.TryAddVar(varName, varValue, varType, out hierarchyVar);

        /// <summary>
        /// 尝试移除(删)
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        public bool TryRemove(string varName) => varDictionary.TryRemove(varName);

        /// <summary>
        /// 尝试设置变量值(改)
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="varValue"></param>
        /// <param name="hierarchyVar"></param>
        /// <returns></returns>
        public bool TrySetVarValue(string varName, string varValue, out IHierarchyVar hierarchyVar) => varDictionary.TrySetVarValue(varName, varValue, out hierarchyVar);

        /// <summary>
        /// 尝试获取变量(查)
        /// </summary>
        /// <param name="varName">变量名称</param>
        /// <param name="hierarchyVar">层级变量：根层级变量</param>
        /// <returns></returns>
        public bool TryGetVar(string varName, out IHierarchyVar hierarchyVar) => varDictionary.TryGetVar(varName, out hierarchyVar);

        /// <summary>
        /// 尝试获取变量值(查)
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="varValue"></param>
        /// <returns></returns>
        public bool TryGetVarValue(string varName, out string varValue) => varDictionary.TryGetVarValue(varName, out varValue);

        /// <summary>
        /// 尝试设置或添加设置变量值：有则设置，无则添加并设置
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="varValue"></param>
        /// <param name="varType"></param>
        /// <param name="hierarchyVar"></param>
        /// <returns></returns>
        public bool TrySetOrAddSetVarValue(string varName, string varValue, EVarType varType, out IHierarchyVar hierarchyVar) => varDictionary.TrySetOrAddSetVarValue(varName, varValue, varType, out hierarchyVar);

        /// <summary>
        /// 尝试设置或添加设置变量值：有则设置，无则添加并设置
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="varValue"></param>
        /// <param name="varType"></param>
        /// <returns></returns>
        public bool TrySetOrAddSetVarValue(string varName, string varValue, EVarType varType = EVarType.String) => varDictionary.TrySetOrAddSetVarValue(varName, varValue, varType, out _);

        /// <summary>
        /// 尝试获取或添加变量：有则获取，无则添加
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="varType"></param>
        /// <param name="hierarchyVar"></param>
        /// <returns></returns>
        public bool TryGetOrAddVar(string varName, EVarType varType, out IHierarchyVar hierarchyVar) => varDictionary.TryGetOrAddVar(varName, varType, out hierarchyVar);

        #endregion

        #region ISerializationCallbackReceiver接口实现

        /// <summary>
        /// 当序列化之前回调
        /// </summary>
        public virtual void OnBeforeSerialize() => varDictionary.SerializeTo();

        /// <summary>
        /// 当反序列化之后回调
        /// </summary>
        public virtual void OnAfterDeserialize() => UpdateVariable();

        #endregion

        #region IVariableHandle接口实现

        /// <summary>
        /// 变量列表
        /// </summary>
        public abstract List<Variable> variableList { get; }

        /// <summary>
        /// 将当前变量名按照数组形式返回；将 字典(运行期有效)中Keys 数组化后返回；
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetVariableNames() => varDictionary.Keys.ToArray();

        /// <summary>
        /// 更新变量
        /// </summary>
        public virtual void UpdateVariable() => varDictionary.SerializeFromByGetOrAdd(this);

        /// <summary>
        /// 获取脚本上下文信息
        /// </summary>
        /// <returns></returns>
        public string GetFunctionContextInfo() => string.Format("组件型[{0}]({1})对象[{2}]", CommonFun.Name(GetType()), GetType().FullName, CommonFun.GameObjectToStringWithoutAlias(gameObject));

        #endregion
    }
}
