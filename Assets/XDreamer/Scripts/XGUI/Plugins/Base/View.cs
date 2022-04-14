using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI;

namespace XCSJ.PluginXGUI.Base
{
    /// <summary>
    /// 视图接口
    /// </summary>
    public interface IView
    {

    }

    /// <summary>
    /// 视图：XGUI基类
    /// </summary>
    [RequireManager(typeof(XGUIManager))]
    public abstract class View : ToolMB, IView
    {
        /// <summary>
        /// 窗口自身变换
        /// </summary>
        public RectTransform rectTransform
        {
            get
            {
                if (!_rectTransform)
                {
                    _rectTransform = GetComponent<RectTransform>();
                }
                return _rectTransform;
            }
        }
        private RectTransform _rectTransform;
    }

    /// <summary>
    /// 可拖拽视图接口
    /// </summary>
    public interface IDraggableView : IView, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        /// <summary>
        /// 可拖拽
        /// </summary>
        bool canDrag { get; set; }
    }

    /// <summary>
    /// 可拖拽视图
    /// </summary>
    public abstract class DraggableView : View, IDraggableView, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// 同级索引规则
        /// </summary>
        [Name("同级索引规则")]
        [EnumPopup]
        public ESiblingIndexRule _siblingIndexRule = ESiblingIndexRule.Last_CurrentInclude_RootCanvasNotInclude;

        /// <summary>
        /// 可拖拽
        /// </summary>
        public virtual bool canDrag { get; set; } = false;

        // 鼠标按下时的偏移值
        private UnityEngine.Vector2 _offsetOnPointerDown;

        /// <summary>
        /// 指针按下
        /// </summary>
        private bool isPointerDown = false;

        /// <summary>
        /// 按下
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            CommonFun.BeginOnUI();

            if (canDrag)
            {
                isPointerDown = true;

                _offsetOnPointerDown = eventData.position - (Vector2)transform.position;
            }
        }

        /// <summary>
        /// 拖拽
        /// </summary>
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (canDrag && isPointerDown)
            {
                transform.position = eventData.position - _offsetOnPointerDown;
            }
        }

        /// <summary>
        /// 弹起
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            CommonFun.EndOnUI();

            if (canDrag)
            {
                isPointerDown = false;
            }
        }

        /// <summary>
        /// 指针按下
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(PointerEventData eventData) => transform.SetSiblingIndex(_siblingIndexRule);

        /// <summary>
        /// 指针弹起
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerUp(PointerEventData eventData) { }
    }
}
