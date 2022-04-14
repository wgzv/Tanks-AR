using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Tools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.Notes
{
    [Tool("标注")]
    [Tip("弹出式的提示界面")]
    [Name("提示(UGUI)")]
    [XCSJ.Attributes.Icon(EIcon.Tip)]
    [RequireComponent(typeof(Selectable))]
    public class UGUITip : Tip
    {
        private RectTransform rectTransform = null;

        private EventTrigger eventTrigger = null;

        private EventTrigger.Entry pointerEnter = null;
        private EventTrigger.Entry pointerExit = null;
        private EventTrigger.Entry pointerClick = null;

        protected override void Awake()
        {
            base.Awake();

            rectTransform = GetComponent<RectTransform>();

            eventTrigger = GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>();

            pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            pointerEnter.callback.AddListener(OnPointerEnter);
            eventTrigger.triggers.Add(pointerEnter);

            pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            pointerExit.callback.AddListener(OnPointerExit);
            eventTrigger.triggers.Add(pointerExit);

            pointerClick = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
            pointerClick.callback.AddListener(OnPointerClick);
            eventTrigger.triggers.Add(pointerClick);
        }

        protected void OnDestroy()
        {
            eventTrigger.triggers.Remove(pointerEnter);
            eventTrigger.triggers.Remove(pointerExit);
            eventTrigger.triggers.Remove(pointerClick);
        }

        protected void OnPointerEnter(BaseEventData baseEventData) => OnEnter();

        protected void OnPointerExit(BaseEventData baseEventData) => OnExit();

        protected void OnPointerClick(BaseEventData baseEventData) => OnClick();

        protected override Vector3 GetTipPosition() => rectTransform.transform.position;

        protected override void SetTipPosition(Vector3 position) => ugui.position = position;
    }
}
