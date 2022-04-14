using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.Transitions.Base
{
    [ComponentMenu("跳过/变量比较跳过", typeof(SMSManager))]
    [Name("变量比较跳过")]
    public class VariableCompareSkip : VariableCompare
    {
        [Name("需触发条件成立")]
        [Tip("勾选时,需变量比较条件成立时才能跳转;不勾选时，输入状态完成后无条件跳转")]
        public bool needTrigger = true;

        public override void OnUpdate(StateData data)
        {
            base.OnUpdate(data);

            if (CheckVariable())
            {
                OnSkip();

                SkipHelper.Skip(data, parent);
            }
        }

        public override bool Finished() => needTrigger ? false : true;

        protected virtual void OnSkip() { }
    }
}
