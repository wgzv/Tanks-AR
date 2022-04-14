using System;
using System.Collections.Generic;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.CNScripts
{
    /// <summary>
    /// 变量赋值批量:变量赋值批量组件是用于执行多个变量赋值的执行体
    /// </summary>
    [ComponentMenu(SMSHelperExtension.CNScriptCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(VariableAssignmentBatch))]
    [XCSJ.Attributes.Icon(EIcon.Variable)]
    [Tip("变量赋值批量组件是用于执行多个变量赋值的执行体")]
    public class VariableAssignmentBatch : LifecycleExecutor<VariableAssignmentBatch>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "变量赋值批量";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.CNScriptCategoryName, typeof(SMSManager))]
        [StateComponentMenu(SMSHelperExtension.CNScriptCategoryName + "/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(VariableAssignmentBatch))]
        [Tip("变量赋值批量组件是用于执行多个变量赋值的执行体")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("变量信息")]
        [OnlyMemberElements]
        public List<Info> variables = new List<Info>();

        public override void Execute(StateData data, EExecuteMode executeMode)
        {
            if (ScriptManager.instance)
            {
                foreach (var info in variables)
                {
                    info.SetValue();
                }
            }
        }

        public override string ToFriendlyString()
        {
            return variables.Count.ToString();
        }

        [Serializable]
        public class Info : IDisplayAsArrayElement
        {
            [Name("变量")]
            [GlobalVariable(true)]
            public string variable;

            [Name("新值")]
            [GlobalVariable(nameof(newValueIsVariable), EValidityCheckType.True, enumMemberName = nameof(variable))]
            public string newValue;

            [Name("新值是变量")]
            public bool newValueIsVariable = false;

            public string variableString => ScriptOption.VarFlag + variable;

            public string newValueString => (newValueIsVariable ? ScriptOption.VarFlag : "") + newValue;

            public string GetGUIContentText(int index) => string.Format("变量{0}: {1}", (index + 1).ToString(), variable);

            public string GetGUIContentTooltip(int index) => string.Format("{0}={1}", variableString, newValueString);

            public void SetValue()
            {
                if (newValueIsVariable)
                {
                    string tmpNewValue;
                    if (ScriptManager.TryGetGlobalVariableValue(newValue, out tmpNewValue))
                    {
                        ScriptManager.TrySetGlobalVariableValue(variable, tmpNewValue);
                    }
                }
                else
                {
                    ScriptManager.TrySetGlobalVariableValue(variable, newValue);
                }
            }
        }
    }
}
