using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.UGUI
{
    /// <summary>
    /// Toggle切换:Toggle切换组件是Toggle开关状态符合设定状态的触发器。当条件满足时，组件切换为完成态。
    /// </summary>
    [ComponentMenu("UGUI/"+ Title, typeof(SMSManager))]
    [Name(Title, "ToggleSwitch")]
    [Tip("Toggle切换组件是Toggle开关状态符合设定状态的触发器。当条件满足时，组件切换为完成态。")]
    [XCSJ.Attributes.Icon(EIcon.Toggle, index = 33609)]
    public class ToggleSwitch : ToggleTrigger<ToggleSwitch>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Toggle切换";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("UGUI", typeof(SMSManager))]
        [StateLib(SMSHelperExtension.CommonUseCategoryName, typeof(SMSManager))]
        [StateComponentMenu(SMSHelperExtension.CommonUseCategoryName + "/" + Title, typeof(SMSManager))]
        [StateComponentMenu("UGUI/"+ Title, typeof(SMSManager))]
        //[StateComponentMenu(SMSHelperExtension.CommonUseCategoryName + Title, typeof(SMSManager))]
        [Name(Title)]
        [Tip("Toggle切换组件是Toggle开关状态符合设定状态的触发器。当条件满足时，组件切换为完成态。")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreateToggleSwitch(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("Toggle控件")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup(typeof(Toggle))]
        [Readonly(EEditorMode.Runtime)]
        public Toggle toggle;

        protected override bool toggleState 
        { 
            get => toggle ? toggle.isOn : false; 
            set 
            {
                if (toggle)
                {
                    toggle.isOn = value;
                }
            } 
        }

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            if (toggle)
            {
                toggle.onValueChanged.AddListener(OnValueChanged);
            }
        }

        public override void OnExit(StateData data)
        {
            if (toggle)
            {
                toggle.onValueChanged.RemoveListener(OnValueChanged);
            }
            base.OnExit(data);
        }

        private void OnValueChanged(bool value)
        {
            switch (triggerType)
            {
                case EToggleTriggerType.Switch:
                    {
                        finished = true;
                        break;
                    }
                case EToggleTriggerType.SwitchOn:
                    {
                        if (toggle.isOn) finished = true;
                        break;
                    }
                case EToggleTriggerType.SwitchOff:
                    {
                        if (!toggle.isOn) finished = true;
                        break;
                    }
            }
        }

        public override bool DataValidity()
        {
            return toggle;
        }

        public override string ToFriendlyString()
        {
            return (toggle ? toggle.name : "") + " " + CommonFun.Name(triggerType);
        }
    }
}
