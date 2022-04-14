using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.Transitions.Base;

namespace XCSJ.PluginSMS.Transitions.UGUI
{
    [ComponentMenu("跳过/UI事件触发器简版跳过", typeof(SMSManager))]
    [Name("UI事件触发器简版跳过")]
    public class UGUIEventTriggerLiteSkip : UGUIEventTriggerLite
    {
        [Name("需触发条件成立")]
        [Tip("勾选时,需按钮点击才能跳转;不勾选时，输入状态完成后无条件跳转")]
        public bool needTrigger = true;

        private bool triggerFlag = false;

        public override void OnUpdate(StateData data)
        {
            base.OnUpdate(data);

            if (triggerFlag)
            {
                triggerFlag = false;

                OnSkip();

                SkipHelper.Skip(data, parent);
            }
        }

        protected override void OnEventTrigger(BaseEventData eventData) => triggerFlag = true;

        public override bool Finished() => needTrigger ? false : true;

        protected virtual void OnSkip() { }        
    }
}
