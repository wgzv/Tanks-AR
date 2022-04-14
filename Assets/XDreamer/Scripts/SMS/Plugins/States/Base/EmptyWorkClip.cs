using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Attributes;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Base
{
    [Name("空工作剪辑")]
    [Tip("空工作剪辑不具有任何控制表现逻辑;可用于做补间填充;")]
    public class EmptyWorkClip : WorkClip<EmptyWorkClip>
    {
        protected override void OnSetPercent(Percent percent, StateData stateData) { }
    }
}
