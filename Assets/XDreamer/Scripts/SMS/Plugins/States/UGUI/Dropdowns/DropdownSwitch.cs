﻿using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginXGUI.Views.Dropdowns;

namespace XCSJ.PluginSMS.States.UGUI.Dropdowns
{
    /// <summary>
    /// 下拉框切换:下拉框切换组件是下拉框当前值符合设定值的触发器。当值相等时，组件切换为完成态。
    /// </summary>
    [ComponentMenu("UGUI/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(DropdownSwitch))]
    [XCSJ.Attributes.Icon(EIcon.Dropdown, index = 33605)]
    [Tip("下拉框切换组件是下拉框当前值符合设定值的触发器。当值相等时，组件切换为完成态。")]
    public class DropdownSwitch : BaseDropdownSwitch<DropdownSwitch>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "下拉框切换";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("UGUI", typeof(SMSManager))]
        [StateComponentMenu("UGUI/"+ Title, typeof(SMSManager))]
        [Name(Title, nameof(DropdownSwitch))]
        [Tip("下拉框切换组件是下拉框当前值符合设定值的触发器。当值相等时，组件切换为完成态。")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreateDropdownSwitch(IGetStateCollection obj) => CreateNormalState(obj);        

        /// <summary>
        /// 触发值类型
        /// </summary>
        [Name("触发值类型")]
        [EnumPopup]
        public EDropdownValueType triggerValueType = EDropdownValueType.Value;

        [Name("触发值")]
        [HideInSuperInspector(nameof(triggerValueType), EValidityCheckType.NotEqual, EDropdownValueType.Value)]
        public int triggerValue = 0;

        [Name("触发文本")]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        [HideInSuperInspector(nameof(triggerValueType), EValidityCheckType.NotEqual, EDropdownValueType.Text)]
        public string triggerText = "";

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);
            if (dropdown)
            {
                dropdown.onValueChanged.AddListener(OnDropdownSwitch);
            }
        }

        public override void OnExit(StateData data)
        {
            if (dropdown)
            {
                dropdown.onValueChanged.RemoveListener(OnDropdownSwitch);
            }
            base.OnExit(data);
        }

        private void OnDropdownSwitch(int val)
        {
            switch (triggerValueType)
            {
                case EDropdownValueType.Value:
                    {
                        if (val == triggerValue)
                        {
                            finished = true;
                        }
                        break;
                    }
                case EDropdownValueType.Text:
                    {
                        if (dropdown.TryGetTextValue(out string text) && text == triggerText)
                        {
                            finished = true;
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            if (!dropdown) return "";
            switch (triggerValueType)
            {
                case EDropdownValueType.Value: return dropdown.name + ".值=" + triggerValue;
                case EDropdownValueType.Text: return dropdown.name + ".文本=" + triggerText;
                default: return dropdown.name + "=";
            }
        }
    }
}
