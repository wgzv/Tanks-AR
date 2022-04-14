using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.Transitions.Base;
using XCSJ.PluginXGUI;

namespace XCSJ.PluginSMS.Transitions.UGUI
{
    [ComponentMenu("基础/UI事件触发器简版", typeof(SMSManager))]
    [Name("UI事件触发器简版")]
    public class UGUIEventTriggerLite : ObsoleteTrigger
    {
        [Name("UGUI对象")]
        [ComponentPopup()]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public RectTransform rectTransform;

        [Name("触发事件类型(简版)")]
        [EnumPopup]
        public EEventTriggerTypeLite eventTriggerTypeLite;

        protected EventTrigger eventTrigger;

        public override bool Init(StateData data)
        {
            if (rectTransform)
            {
                eventTrigger = rectTransform.gameObject.AddComponent<EventTrigger>();

                XGUIHelper.AddEventTrigger(eventTrigger, eventTriggerTypeLite.ToEventTriggerType(), OnEventTrigger);
            }

            return base.Init(data);
        }

        public override bool Delete(bool deleteObject)
        {
            if (eventTrigger) GameObject.Destroy(eventTrigger);

            return base.Delete(deleteObject);
        }

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);
            if (eventTrigger) eventTrigger.enabled = true;
        }

        public override void OnExit(StateData data)
        {
            if (eventTrigger) eventTrigger.enabled = false;
            base.OnExit(data);
        }

        public override bool DataValidity()
        {
            return rectTransform;
        }

        protected virtual void OnEventTrigger(BaseEventData eventData)
        {
            if (canTrigger)
            {
                finished = true;
            }
        }

        public override string ToFriendlyString()
        {
            return rectTransform ? rectTransform.name : "";
        }
    }
}
