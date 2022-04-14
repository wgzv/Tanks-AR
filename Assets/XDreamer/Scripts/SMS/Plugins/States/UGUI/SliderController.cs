using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginSMS.States.UGUI
{
    /// <summary>
    /// 滑块控制：滑块控制组件是滑块当前值与设定值符合设定条件的触发器。当条件满足时，组件切换为完成态。
    /// </summary>
    [ComponentMenu("UGUI/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(SliderController))]
    [XCSJ.Attributes.Icon(EIcon.Slider, index = 33608)]
    [Tip("滑块控制组件是滑块当前值与设定值符合设定条件的触发器。当条件满足时，组件切换为完成态。")]
    public class SliderController : Trigger<SliderController>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "滑块控制";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("UGUI", typeof(SMSManager))]
        [StateComponentMenu("UGUI/"+ Title, typeof(SMSManager))]
        [Name(Title, nameof(SliderController))]
        [Tip("滑块控制组件是滑块当前值与设定值符合设定条件的触发器。当条件满足时，组件切换为完成态。")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreateSliderController(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("滑块")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup(typeof(Slider))]
        [Readonly(EEditorMode.Runtime)]
        public Slider slider;

        [Name("数值比较触发器")]
        public FloatValueTrigger numberValueTrigger = new FloatValueTrigger();

        [Name("完成一次")]
        [Tip("勾选时：条件成立即为完成状态；不勾选时：完成状态随着滑动条值不断变化")]
        public bool finishOnce = true;

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);
            if (slider)
            {
                slider.onValueChanged.AddListener(OnSliderValueChanged);
            }
        }

        public override void OnExit(StateData data)
        {
            if (slider)
            {
                slider.onValueChanged.RemoveListener(OnSliderValueChanged);
            }
            base.OnExit(data);
        }

        private void OnSliderValueChanged(float value)
        {
            if (finishOnce && finished)
            {
                return;
            }
            finished = numberValueTrigger.IsTrigger(value);
        }

        public override bool DataValidity() => slider;

        public override string ToFriendlyString() => slider ? slider.name : "";
    }
}