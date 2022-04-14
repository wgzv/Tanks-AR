using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.CNScripts
{
    /// <summary>
    /// 变量赋值：变量赋值组件是用于执行变量赋值的执行体
    /// </summary>
    [ComponentMenu(SMSHelperExtension.CNScriptCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(VariableAssignment))]
    [XCSJ.Attributes.Icon(EIcon.Variable)]
    [Tip("变量赋值组件是用于执行变量赋值的执行体")]
    public class VariableAssignment : StateComponent<VariableAssignment>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "变量赋值";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.CNScriptCategoryName, typeof(SMSManager))]
        [StateComponentMenu(SMSHelperExtension.CNScriptCategoryName + "/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(VariableAssignment))]
        [Tip("变量赋值组件是用于执行变量赋值的执行体")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("变量")]
        [GlobalVariable(true)]
        public string variable;

        [Name("值类型")]
        [Tip("新值的类型")]
        [FormerlySerializedAs("newValueIsVariable")]
        [EnumPopup]
        public EValueType valueType = EValueType.Value;

        [Name("新值")]
        [GlobalVariable(nameof(valueType), EValidityCheckType.Equal, EValueType.Variable, enumMemberName = nameof(variable))]
        [HideInSuperInspector(nameof(valueType), EValidityCheckType.NotEqual | EValidityCheckType.And, EValueType.Value, nameof(valueType), EValidityCheckType.NotEqual, EValueType.Variable)]
        public string newValue;

        [Name("游戏对象")]
        [Tip("新值游戏对象")]
        [HideInSuperInspector(nameof(valueType), EValidityCheckType.Less | EValidityCheckType.Or, EValueType.GameOjbectName, nameof(valueType), EValidityCheckType.Greater, EValueType.GameOjbectAlias)]
        public GameObject go = null;

        [Name("赋值时机")]
        [EnumPopup]
        public ELifecycleEvent assignmentRule = ELifecycleEvent.OnEntry;

        /// <summary>
        /// 变量字符串
        /// </summary>
        public string variableString => ScriptOption.VarFlag + variable;

        /// <summary>
        /// 新值字符串
        /// </summary>
        public string newValueString => valueType.GetValueString(newValue, go);

        private void SetValue(ELifecycleEvent assignmentRule)
        {
            if (this.assignmentRule == assignmentRule)
            {
                ScriptManager.TrySetGlobalVariableValue(variable, valueType.GetValue(newValue, go));
            }
        }

        public override void OnBeforeEntry(StateData stateData)
        {
            base.OnBeforeEntry(stateData);
            SetValue(ELifecycleEvent.OnBeforeEntry);
        }

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);
            SetValue(ELifecycleEvent.OnEntry);
        }

        public override void OnAfterEntry(StateData stateData)
        {
            base.OnAfterEntry(stateData);
            SetValue(ELifecycleEvent.OnAfterEntry);
        }

        public override void OnUpdate(StateData stateData)
        {
            base.OnUpdate(stateData);
            SetValue(ELifecycleEvent.OnUpdate);
        }

        public override void OnBeforeExit(StateData stateData)
        {
            base.OnBeforeExit(stateData);
            SetValue(ELifecycleEvent.OnBeforeExit);
        }

        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
            SetValue(ELifecycleEvent.OnExit);
        }

        public override void OnAfterExit(StateData stateData)
        {
            base.OnAfterExit(stateData);
            SetValue(ELifecycleEvent.OnAfterExit);
        }

        public override bool Finished() => true;

        public override string ToFriendlyString()
        {
            return variableString + VariableCompareHelper.ToAbbreviations(ECompareType.Equal) + newValueString;
        }
    }
}
