using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Show;
using XCSJ.PluginSMS.Transitions.UGUI;

namespace XCSJ.PluginRepairman.Task
{
    [ComponentMenu("跳过/按钮点击时修理步骤跳过", typeof(RepairmanManager))]
    [Name("按钮点击时修理步骤跳过")]
    [RequireManager(typeof(RepairmanManager))]
    public class RepairStepSkipWhenButtonClick : ButtonClickSkip
    {
        [Name("按钮点击时修理步骤跳过")]
        [Tip("如果步骤为空，它就会查找输入状态内的修理步骤或者步骤剪辑对应的修理步骤")]
        [StateComponentPopup(typeof(RepairStep), stateCollectionType = EStateCollectionType.Current)]
        public Step step = null;

        public override void OnCreated()
        {
            base.OnCreated();

            step = TaskHelper.FindStepInPreviousState(parent);
        }        

        public override bool Init(StateData data)
        {
            step = TaskHelper.FindStepInPreviousState(parent);
            return base.Init(data);
        }

        protected override void OnSkip()
        {
            base.OnSkip();

            if (step)
            {
                step.Skip();
            }
        }

        public override bool DataValidity()
        {
            return base.DataValidity() && step;
        }
    }
}
