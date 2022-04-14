using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.CNScripts
{
    /// <summary>
    /// 变量比较:变量比较组件是比较变量与某个值相等的触发器；当比较条件成立后，状态组件切换为完成态；
    /// </summary>
    [ComponentMenu(SMSHelperExtension.CNScriptCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(VariableCompare))]
    [Tip("变量比较组件是比较变量与某个值相等的触发器；当比较条件成立后，状态组件切换为完成态；")]
    [XCSJ.Attributes.Icon(EIcon.Variable)]
    public class VariableCompare : BasePropertyCompare<VariableCompare>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "变量比较";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.CNScriptCategoryName, typeof(SMSManager))]
        [StateComponentMenu(SMSHelperExtension.CNScriptCategoryName + "/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(VariableCompare))]
        [Tip("变量比较组件是比较变量与某个值相等的触发器；当比较条件成立后，状态组件切换为完成态；")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("变量")]
        [GlobalVariable(true)]
        public string variable;

        [Name("比较类型")]
        [EnumPopup]
        public ECompareType compareType = ECompareType.Equal;

        [Name("待比较值")]
        [GlobalVariable(nameof(compareValueIsVariable), EValidityCheckType.True, enumMemberName = nameof(variable))]
        public string compareValue;

        [Name("待比较值是变量")]
        public bool compareValueIsVariable = false;

        [Name("比较规则")]
        [EnumPopup]
        public ECompareRule compareRule = ECompareRule.String;

        public string variableString => ScriptOption.VarFlag + variable;

        public string compareValueString => (compareValueIsVariable ? ScriptOption.VarFlag : "") + compareValue;

        public override bool Finished()
        {
            string variableValue;
            if (ScriptManager.TryGetGlobalVariableValue(variable, out variableValue))
            {
                if (compareValueIsVariable)
                {
                    string tmpCompareValue;
                    return ScriptManager.TryGetGlobalVariableValue(compareValue, out tmpCompareValue) && VariableCompareHelper.ValueCompareValue(variableValue, compareType, tmpCompareValue, compareRule);
                }
                else
                {
                    return VariableCompareHelper.ValueCompareValue(variableValue, compareType, compareValue, compareRule);
                }
            }
            return false;
        }

        public override string ToFriendlyString()
        {
            return variableString + VariableCompareHelper.ToAbbreviations(compareType) + compareValueString;
        }
    }
}
