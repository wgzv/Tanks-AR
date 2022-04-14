using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.Transitions.Base
{
    [ComponentMenu("基础/变量比较", typeof(SMSManager))]
    [Name("变量比较")]
    public class VariableCompare : ObsoleteTrigger
    {
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
            return base.Finished() || CheckVariable();
        }

        //public override void OnUpdate(StateData data)
        //{
        //    base.OnUpdate(data);

        //    if (canTrigger)
        //    {
        //        finished = CheckVariable();
        //    }
        //}

        protected bool CheckVariable()
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
