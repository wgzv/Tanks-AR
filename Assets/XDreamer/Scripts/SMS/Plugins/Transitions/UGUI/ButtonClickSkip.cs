using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.Transitions.Base;

namespace XCSJ.PluginSMS.Transitions.UGUI
{
    [ComponentMenu("跳过/按钮点击跳过", typeof(SMSManager))]
    [Name("按钮点击跳过")]
    public class ButtonClickSkip : ButtonClick
    {
        [Name("需触发条件成立")]
        [Tip("勾选时,需按钮点击才能跳转;不勾选时，输入状态完成后无条件跳转")]
        public bool needTrigger = true;

        private bool click = false;

        public override void OnUpdate(StateData data)
        {
            base.OnUpdate(data);

            if (click)
            {
                click = false;

                OnSkip();

                SkipHelper.Skip(data, parent);
            }
        }

        protected override bool canTrigger => true;

        protected override void OnClicked() => click = true;

        public override bool Finished() => needTrigger ? false : true;

        protected virtual void OnSkip() { }
    }
}
