using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.UGUI;
using XCSJ.PluginSMS.Transitions.Base;

namespace XCSJ.PluginSMS.Transitions.UGUI
{
    [ComponentMenu("基础/按钮点击", typeof(SMSManager))]
    [Name("按钮点击")]
    public class ButtonClick : ObsoleteTrigger
    {
        /// <summary>
        /// 批量
        /// </summary>
        [Name("批量")]
        public bool batch = false;

        [Name("按钮")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(batch), EValidityCheckType.True)]
        [ComponentPopup()]
        public Button button;

        /// <summary>
        /// 批量规则
        /// </summary>
        [Name("批量规则")]
        [HideInSuperInspector(nameof(batch), EValidityCheckType.False)]
        [EnumPopup]
        public EBatchRule batchRule = EBatchRule.Any;

        /// <summary>
        /// 按钮列表
        /// </summary>
        [Name("按钮列表")]
        [HideInSuperInspector(nameof(batch), EValidityCheckType.False)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public List<Button> buttons = new List<Button>();

        /// <summary>
        /// 按钮信息
        /// </summary>
        protected ButtonInfoList buttonInfoList = new ButtonInfoList();

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            buttonInfoList.RemoveAllListener();
            AddAllListener();
        }

        public override void OnExit(StateData data)
        {
            buttonInfoList.RemoveAllListener();

            base.OnExit(data);
        }

        /// <summary>
        /// 添加监听
        /// </summary>
        private void AddAllListener()
        {
            if (batch)
            {
                buttonInfoList.AddListener(OnClicked, buttons.ToArray());
            }
            else
            {
                buttonInfoList.AddListener(OnClicked, button);
            }
        }

        private void OnClicked(ButtonInfo buttonInfo)
        {
            if (!canTrigger) return;

            switch (batchRule)
            {
                case EBatchRule.Any:
                    {
                        OnClicked();
                        break;
                    }
                case EBatchRule.All:
                    {
                        var count = buttonInfoList.buttonInfos.Count(info => info.count > 0);
                        if (count == buttonInfoList.buttonInfos.Count)
                        {
                            OnClicked();
                        }
                        break;
                    }
            }
        }

        protected virtual void OnClicked()
        {
            finished = true;
        }

        /// <summary>
        /// 有效
        /// </summary>
        protected void OnValidate()
        {
            if (parent.isActive)
            {
                buttonInfoList.RemoveAllListener();
                AddAllListener();
            }
        }

        public override bool DataValidity()
        {
            return batch? (buttons.Count > 0) : button;
        }

        public override string ToFriendlyString()
        {
            return button ? button.name : "";
        }
    }
}
