using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.Extension.CNScripts.UGUI
{
    /// <summary>
    /// UGUI触发脚本事件类型 
    /// </summary>
    public enum EUGUITriggerScriptEventType
    {
        [Name("指针按下时执行")]
        OnPointerDown,

        [Name("指针进入时执行")]
        OnPointerEnter,

        [Name("指针离开时执行")]
        OnPointerExit,

        [Name("指针抬起时执行")]
        OnPointerUp,

        [Name("指针点击时执行")]
        OnPointerClick,

        [Name("拖拽时执行")]
        OnDrag,

        [Name("指针点击时执行")]
        OnDrop,

        [Name("滚动时执行")]
        OnScroll,

        [Name("更新选择时执行")]
        OnUpdateSelected,

        [Name("选择时执行")]
        OnSelect,

        [Name("取消选择时执行")]
        OnDeselect,

        [Name("移动时执行")]
        OnMove,

        [Name("初始化潜在的拖拽时执行")]
        OnInitializePotentialDrag,

        [Name("开始拖拽时执行")]
        OnBeginDrag,

        [Name("结束拖拽时执行")]
        OnEndDrag,

        [Name("提交时执行")]
        OnSubmit,

        [Name("取消时执行")]
        OnCancel,

        [Name("启动时执行")]
        Start,
    }

    /// <summary>
    /// UGUI触发脚本事件集合类
    /// </summary>
    [Serializable]
    public class UGUITriggerScriptEventSet : BaseScriptEventSet<EUGUITriggerScriptEventType>
    {
        //
    }

    /// <summary>
    /// UGUI触发脚本事件
    /// </summary>
    [Serializable]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.UGUIMenu + Title)]
    [ExecuteInEditMode]
    public class UGUITriggerScriptEvent : BaseScriptEvent<EUGUITriggerScriptEventType, UGUITriggerScriptEventSet>, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IBeginDragHandler, IInitializePotentialDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IScrollHandler, IUpdateSelectedHandler, ISelectHandler, IDeselectHandler, IMoveHandler, ISubmitHandler, ICancelHandler
    {
        public const string Title = "UGUI触发脚本事件";

        public override void Start()
        {
            RunScriptEvent(EUGUITriggerScriptEventType.Start);
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            //Log.Debug("OnBeginDrag");
            RunScriptEvent(EUGUITriggerScriptEventType.OnBeginDrag, eventData.ToString());
        }

        public virtual void OnCancel(BaseEventData eventData)
        {
            //Log.Debug("OnCancel");
            RunScriptEvent(EUGUITriggerScriptEventType.OnCancel, eventData.ToString());
        }

        public virtual void OnDeselect(BaseEventData eventData)
        {
            //Log.Debug("OnDeselect");
            RunScriptEvent(EUGUITriggerScriptEventType.OnDeselect, eventData.ToString());
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            //Log.Debug("OnDrag");
            RunScriptEvent(EUGUITriggerScriptEventType.OnDrag, eventData.ToString());
        }

        public virtual void OnDrop(PointerEventData eventData)
        {
            //Log.Debug("OnDrop");
            RunScriptEvent(EUGUITriggerScriptEventType.OnDrop, eventData.ToString());
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            //Log.Debug("OnEndDrag");
            RunScriptEvent(EUGUITriggerScriptEventType.OnEndDrag, eventData.ToString());
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            //Log.Debug("OnInitializePotentialDrag");
            RunScriptEvent(EUGUITriggerScriptEventType.OnInitializePotentialDrag, eventData.ToString());
        }

        public virtual void OnMove(AxisEventData eventData)
        {
            //Log.Debug("OnMove");
            RunScriptEvent(EUGUITriggerScriptEventType.OnMove, eventData.ToString());
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            //Log.Debug("OnPointerClick");
            RunScriptEvent(EUGUITriggerScriptEventType.OnPointerClick, eventData.ToString());
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            //Log.Debug("OnPointerDown");
            RunScriptEvent(EUGUITriggerScriptEventType.OnPointerDown, eventData.ToString());
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            //Log.Debug("OnPointerEnter");
            RunScriptEvent(EUGUITriggerScriptEventType.OnPointerEnter, eventData.ToString());
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            //Log.Debug("OnPointerExit");
            RunScriptEvent(EUGUITriggerScriptEventType.OnPointerExit, eventData.ToString());
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            //Log.Debug("OnPointerUp");
            RunScriptEvent(EUGUITriggerScriptEventType.OnPointerUp, eventData.ToString());
        }

        public virtual void OnScroll(PointerEventData eventData)
        {
            //Log.Debug("OnScroll");
            RunScriptEvent(EUGUITriggerScriptEventType.OnScroll, eventData.ToString());
        }

        public virtual void OnSelect(BaseEventData eventData)
        {
            //Log.Debug("OnSelect");
            RunScriptEvent(EUGUITriggerScriptEventType.OnSelect, eventData.ToString());
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            //Log.Debug("OnSubmit");
            RunScriptEvent(EUGUITriggerScriptEventType.OnSubmit, eventData.ToString());
        }

        public virtual void OnUpdateSelected(BaseEventData eventData)
        {
            //Log.Debug("OnUpdateSelected");
            RunScriptEvent(EUGUITriggerScriptEventType.OnUpdateSelected, eventData.ToString());
        }
    }
}
