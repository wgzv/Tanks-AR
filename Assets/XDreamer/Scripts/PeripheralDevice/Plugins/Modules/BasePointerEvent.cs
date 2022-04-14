using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.PluginPeripheralDevice;
using UInput = UnityEngine.Input;

namespace XCSJ.PluginPeripheralDevice.Modules
{
    public abstract class BasePointerEvent
    {
        /// <summary>
        /// 事件响应的游戏对象
        /// </summary>
        protected GameObject _eventGameObject;
        public GameObject eventGameObject => _eventGameObject;

        /// <summary>
        /// 基础事件数据
        /// </summary>
        protected BaseEventData _eventData;

        /// <summary>
        /// BasePointerEvent构造函数
        /// </summary>
        public BasePointerEvent()
        { }

        /// <summary>
        /// BasePointerEvent构造函数
        /// </summary>
        /// <param name="go">事件响应的游戏对象</param>
        /// <param name="baseEventData">基础事件数据</param>
        public BasePointerEvent(GameObject go, BaseEventData baseEventData)
        {
            this._eventGameObject = go;
            this._eventData = baseEventData;
        }

        /// <summary>
        /// 设置对象属性
        /// </summary>
        /// <param name="go">事件响应的游戏对象</param>
        /// <param name="baseEventData">基础事件数据</param>
        public void SetProperty(GameObject go, BaseEventData baseEventData)
        {
            this._eventGameObject = go;
            this._eventData = baseEventData;
        }

        /// <summary>
        /// 处理射线检测事件
        /// </summary>
        public virtual void ProcessPointerEvent()
        {

        }

        /// <summary>
        /// 转化为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetType() + "    游戏对象：" + _eventGameObject?.name + " 参数：" + _eventData.ToString() ;
        }
    }

    public class PointerEnterEvent : BasePointerEvent
    {
        /// <summary>
        /// PointerEnterEvent构造函数
        /// </summary>
        public PointerEnterEvent()
        { }

        /// <summary>
        /// PointerEnterEvent构造函数
        /// </summary>
        /// <param name="go">事件响应的游戏对象</param>
        /// <param name="baseEventData">基础事件数据</param>
        public PointerEnterEvent(GameObject go, BaseEventData baseEventData) : base(go, baseEventData)
        { }

        /// <summary>
        /// 处理射线检测事件
        /// </summary>
        public override void ProcessPointerEvent()
        {
            ExecuteEvents.Execute(_eventGameObject, _eventData, ExecuteEvents.pointerEnterHandler);
        }
    }

    public class PointerExitEvent : BasePointerEvent
    {
        /// <summary>
        /// PointerExitEvent构造函数
        /// </summary>
        public PointerExitEvent()
        { }

        /// <summary>
        /// PointerExitEvent构造函数
        /// </summary>
        /// <param name="go">事件响应的游戏对象</param>
        /// <param name="baseEventData">基础事件数据</param>
        public PointerExitEvent(GameObject go, BaseEventData baseEventData) : base(go, baseEventData)
        { }

        /// <summary>
        /// 处理射线检测事件
        /// </summary>
        public override void ProcessPointerEvent()
        {
            ExecuteEvents.Execute(_eventGameObject, _eventData, ExecuteEvents.pointerExitHandler);
        }
    }

    public class PointerUpEvent : BasePointerEvent
    {
        /// <summary>
        /// PointerUpEvent构造函数
        /// </summary>
        public PointerUpEvent()
        { }

        /// <summary>
        /// PointerUpEvent构造函数
        /// </summary>
        /// <param name="go">事件响应的游戏对象</param>
        /// <param name="baseEventData">基础事件数据</param>
        public PointerUpEvent(GameObject go, BaseEventData baseEventData) : base(go, baseEventData)
        { }

        /// <summary>
        /// 处理射线检测事件
        /// </summary>
        public override void ProcessPointerEvent()
        {
            ExecuteEvents.Execute(_eventGameObject, _eventData, ExecuteEvents.pointerUpHandler);
        }
    }

    public class PointerDownEvent : BasePointerEvent
    {
        /// <summary>
        /// PointerDownEvent构造函数
        /// </summary>
        public PointerDownEvent()
        { }

        /// <summary>
        /// PointerDownEvent构造函数
        /// </summary>
        /// <param name="go">事件响应的游戏对象</param>
        /// <param name="baseEventData">基础事件数据</param>
        public PointerDownEvent(GameObject go, BaseEventData baseEventData) : base(go, baseEventData)
        { }

        /// <summary>
        /// 处理射线检测事件
        /// </summary>
        public override void ProcessPointerEvent()
        {
            ExecuteEvents.Execute(_eventGameObject, _eventData, ExecuteEvents.pointerDownHandler);
        }
    }

    public class PointerClickEvent : BasePointerEvent
    {
        /// <summary>
        /// PointerClickEvent构造函数
        /// </summary>
        public PointerClickEvent()
        { }

        /// <summary>
        /// PointerClickEvent构造函数
        /// </summary>
        /// <param name="go">事件响应的游戏对象</param>
        /// <param name="baseEventData">基础事件数据</param>
        public PointerClickEvent(GameObject go, BaseEventData baseEventData) : base(go, baseEventData)
        { }

        /// <summary>
        /// 处理射线检测事件
        /// </summary>
        public override void ProcessPointerEvent()
        {
            ExecuteEvents.Execute(_eventGameObject, _eventData, ExecuteEvents.pointerClickHandler);
        }
    }

    public class BeginDragEvent : BasePointerEvent
    {
        /// <summary>
        /// BeginDragEvent构造函数
        /// </summary>
        public BeginDragEvent()
        { }

        /// <summary>
        /// BeginDragEvent构造函数
        /// </summary>
        /// <param name="go">事件响应的游戏对象</param>
        /// <param name="baseEventData">基础事件数据</param>
        public BeginDragEvent(GameObject go, BaseEventData baseEventData) : base(go, baseEventData)
        { }

        /// <summary>
        /// 处理射线检测事件
        /// </summary>
        public override void ProcessPointerEvent()
        {
            ExecuteEvents.Execute(_eventGameObject, _eventData, ExecuteEvents.beginDragHandler);
        }
    }

    public class DragEvent : BasePointerEvent
    {
        /// <summary>
        /// DragEvent构造函数
        /// </summary>
        public DragEvent()
        { }

        /// <summary>
        /// DragEvent构造函数
        /// </summary>
        /// <param name="go">事件响应的游戏对象</param>
        /// <param name="baseEventData">基础事件数据</param>
        public DragEvent(GameObject go, BaseEventData baseEventData) : base(go, baseEventData)
        { }

        /// <summary>
        /// 处理射线检测事件
        /// </summary>
        public override void ProcessPointerEvent()
        {
            ExecuteEvents.Execute(_eventGameObject, _eventData, ExecuteEvents.dragHandler);
        }
    }

    public class EndDragEvent : BasePointerEvent
    {
        /// <summary>
        /// EndDragEvent构造函数
        /// </summary>
        public EndDragEvent()
        { }

        /// <summary>
        /// EndDragEvent构造函数
        /// </summary>
        /// <param name="go">事件响应的游戏对象</param>
        /// <param name="baseEventData">基础事件数据</param>
        public EndDragEvent(GameObject go, BaseEventData baseEventData) : base(go, baseEventData)
        { }

        /// <summary>
        /// 处理射线检测事件
        /// </summary>
        public override void ProcessPointerEvent()
        {
            ExecuteEvents.Execute(_eventGameObject, _eventData, ExecuteEvents.endDragHandler);
        }
    }

    public class DropEvent : BasePointerEvent
    {
        /// <summary>
        /// DropEvent构造函数
        /// </summary>
        public DropEvent()
        { }

        /// <summary>
        /// DropEvent构造函数
        /// </summary>
        /// <param name="go">事件响应的游戏对象</param>
        /// <param name="baseEventData">基础事件数据</param>
        public DropEvent(GameObject go, BaseEventData baseEventData) : base(go, baseEventData)
        { }

        /// <summary>
        /// 处理射线检测事件
        /// </summary>
        public override void ProcessPointerEvent()
        {
            ExecuteEvents.Execute(_eventGameObject, _eventData, ExecuteEvents.dropHandler);
        }
    }

    public class InitializePotentialDragEvent : BasePointerEvent
    {
        /// <summary>
        /// InitializePotentialDragEvent构造函数
        /// </summary>
        public InitializePotentialDragEvent()
        { }

        /// <summary>
        /// InitializePotentialDragEvent构造函数
        /// </summary>
        /// <param name="go">事件响应的游戏对象</param>
        /// <param name="baseEventData">基础事件数据</param>
        public InitializePotentialDragEvent(GameObject go, BaseEventData baseEventData) : base(go, baseEventData)
        { }

        /// <summary>
        /// 处理射线检测事件
        /// </summary>
        public override void ProcessPointerEvent()
        {
            ExecuteEvents.Execute(_eventGameObject, _eventData, ExecuteEvents.initializePotentialDrag);
        }
    }

    public class SelectEvent : BasePointerEvent
    {
        /// <summary>
        /// SelectEvent构造函数
        /// </summary>
        public SelectEvent()
        { }

        /// <summary>
        /// SelectEvent构造函数
        /// </summary>
        /// <param name="go">事件响应的游戏对象</param>
        /// <param name="baseEventData">基础事件数据</param>
        public SelectEvent(GameObject go, BaseEventData baseEventData) : base(go, baseEventData)
        { }

        /// <summary>
        /// 处理射线检测事件
        /// </summary>
        public override void ProcessPointerEvent()
        {
            ExecuteEvents.Execute(_eventGameObject, _eventData, ExecuteEvents.selectHandler);
        }
    }

    public class UpdateSelectedEvent : BasePointerEvent
    {
        /// <summary>
        /// UpdateSelectedEvent构造函数
        /// </summary>
        public UpdateSelectedEvent()
        { }

        /// <summary>
        /// UpdateSelectedEvent构造函数
        /// </summary>
        /// <param name="go">事件响应的游戏对象</param>
        /// <param name="baseEventData">基础事件数据</param>
        public UpdateSelectedEvent(GameObject go, BaseEventData baseEventData) : base(go, baseEventData)
        { }

        /// <summary>
        /// 处理射线检测事件
        /// </summary>
        public override void ProcessPointerEvent()
        {
            ExecuteEvents.Execute(_eventGameObject, _eventData, ExecuteEvents.updateSelectedHandler);
        }
    }

    public class DeSelectEvent : BasePointerEvent
    {
        /// <summary>
        /// DeSelectEvent构造函数
        /// </summary>
        public DeSelectEvent()
        { }

        /// <summary>
        /// DeSelectEvent构造函数
        /// </summary>
        /// <param name="go">事件响应的游戏对象</param>
        /// <param name="baseEventData">基础事件数据</param>
        public DeSelectEvent(GameObject go, BaseEventData baseEventData) : base(go, baseEventData)
        { }

        /// <summary>
        /// 处理射线检测事件
        /// </summary>
        public override void ProcessPointerEvent()
        {
            ExecuteEvents.Execute(_eventGameObject, _eventData, ExecuteEvents.deselectHandler);
        }
    }

    public class MoveEvent : BasePointerEvent
    {
        /// <summary>
        /// MoveEvent构造函数
        /// </summary>
        public MoveEvent()
        { }

        /// <summary>
        /// MoveEvent构造函数
        /// </summary>
        /// <param name="go">事件响应的游戏对象</param>
        /// <param name="baseEventData">基础事件数据</param>
        public MoveEvent(GameObject go, BaseEventData baseEventData) : base(go, baseEventData)
        { }

        /// <summary>
        /// 处理射线检测事件
        /// </summary>
        public override void ProcessPointerEvent()
        {
            ExecuteEvents.Execute(_eventGameObject, _eventData, ExecuteEvents.moveHandler);
        }
    }

    public class ScrollEvent : BasePointerEvent
    {
        /// <summary>
        /// ScrollEvent构造函数
        /// </summary>
        public ScrollEvent()
        { }

        /// <summary>
        /// ScrollEvent构造函数
        /// </summary>
        /// <param name="go">事件响应的游戏对象</param>
        /// <param name="baseEventData">基础事件数据</param>
        public ScrollEvent(GameObject go, BaseEventData baseEventData) : base(go, baseEventData)
        { }

        /// <summary>
        /// 处理射线检测事件
        /// </summary>
        public override void ProcessPointerEvent()
        {
            ExecuteEvents.Execute(_eventGameObject, _eventData, ExecuteEvents.scrollHandler);
        }
    }

    public class SubmitEvent : BasePointerEvent
    {
        /// <summary>
        /// SubmitEvent构造函数
        /// </summary>
        public SubmitEvent()
        { }

        /// <summary>
        /// SubmitEvent构造函数
        /// </summary>
        /// <param name="go">事件响应的游戏对象</param>
        /// <param name="baseEventData">基础事件数据</param>
        public SubmitEvent(GameObject go, BaseEventData baseEventData) : base(go, baseEventData)
        { }

        /// <summary>
        /// 处理射线检测事件
        /// </summary>
        public override void ProcessPointerEvent()
        {
            ExecuteEvents.Execute(_eventGameObject, _eventData, ExecuteEvents.submitHandler);
        }
    }

    public class CancelEvent : BasePointerEvent
    {
        /// <summary>
        /// CancelEvent构造函数
        /// </summary>
        public CancelEvent()
        { }

        /// <summary>
        /// CancelEvent构造函数
        /// </summary>
        /// <param name="go">事件响应的游戏对象</param>
        /// <param name="baseEventData">基础事件数据</param>
        public CancelEvent(GameObject go, BaseEventData baseEventData) : base(go, baseEventData)
        { }

        /// <summary>
        /// 处理射线检测事件
        /// </summary>
        public override void ProcessPointerEvent()
        {
            ExecuteEvents.Execute(_eventGameObject, _eventData, ExecuteEvents.cancelHandler);
        }
    }
}
