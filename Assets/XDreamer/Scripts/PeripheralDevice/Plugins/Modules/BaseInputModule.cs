using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace XCSJ.PluginPeripheralDevice.Modules
{
    /// <summary>
    /// 基础输入模块类
    /// </summary>
    public abstract class BaseInputModule
    {
        /// <summary>
        /// 射线检测结果集缓存
        /// </summary>
        protected List<RaycastResult> _raycastResultCache = new List<RaycastResult>();

        /// <summary>
        /// Axis事件数据
        /// </summary>
        private AxisEventData _axisEventData;
        /// <summary>
        /// 基础事件数据
        /// </summary>
        private BaseEventData _baseEventData;

        /// <summary>
        /// 事件系统
        /// </summary>
        protected EventSystem _eventSystem;
        public EventSystem eventSystem
        {
            get { return _eventSystem; }
        }

        /// <summary>
        /// 事件列表
        /// </summary>
        protected List<BasePointerEvent> _pointerEvents = new List<BasePointerEvent>();
        public List<BasePointerEvent> pointerEvents => _pointerEvents;

        /// <summary>
        /// 当前选择的游戏对象
        /// </summary>
        protected GameObject _currentSelectedGameObject;

        public abstract void Process();

        /// <summary>
        /// 找到第一个射线检测结果
        /// </summary>
        /// <param name="candidates">射线检测结果集</param>
        /// <returns>射线检测结果</returns>
        protected static RaycastResult FindFirstRaycast(List<RaycastResult> candidates)
        {
            for (var i = 0; i < candidates.Count; ++i)
            {
                if (candidates[i].gameObject == null)
                    continue;

                return candidates[i];
            }
            return new RaycastResult();
        }

        /// <summary>
        /// 定义移动方向
        /// </summary>
        /// <param name="x">水平值</param>
        /// <param name="y">垂直值</param>
        /// <returns>移动方向</returns>
        protected static MoveDirection DetermineMoveDirection(float x, float y)
        {
            return DetermineMoveDirection(x, y, 0.6f);
        }

        /// <summary>
        /// 定义移动方向
        /// </summary>
        /// <param name="x">水平值</param>
        /// <param name="y">垂直值</param>
        /// <param name="deadZone">范围值</param>
        /// <returns>移动方向</returns>
        protected static MoveDirection DetermineMoveDirection(float x, float y, float deadZone)
        {
            // if vector is too small... just return
            if (new Vector2(x, y).sqrMagnitude < deadZone * deadZone)
                return MoveDirection.None;

            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                if (x > 0)
                    return MoveDirection.Right;
                return MoveDirection.Left;
            }
            else
            {
                if (y > 0)
                    return MoveDirection.Up;
                return MoveDirection.Down;
            }
        }

        /// <summary>
        /// 寻找共同的根对象
        /// </summary>
        /// <param name="g1">游戏对象1</param>
        /// <param name="g2">游戏对象2</param>
        /// <returns>共同的根对象</returns>
        protected static GameObject FindCommonRoot(GameObject g1, GameObject g2)
        {
            if (g1 == null || g2 == null)
                return null;

            var t1 = g1.transform;
            while (t1 != null)
            {
                var t2 = g2.transform;
                while (t2 != null)
                {
                    if (t1 == t2)
                        return t1.gameObject;
                    t2 = t2.parent;
                }
                t1 = t1.parent;
            }
            return null;
        }

        // walk up the tree till a common root between the last entered and the current entered is foung
        // send exit events up to (but not inluding) the common root. Then send enter events up to
        // (but not including the common root).
        /// <summary>
        /// 处理移入移出事件
        /// </summary>
        /// <param name="currentPointerData">当前事件数据</param>
        /// <param name="newEnterTarget">当前检测到的游戏对象</param>
        protected void HandlePointerExitAndEnter(PointerEventData currentPointerData, GameObject newEnterTarget)
        {
            // if we have no target / pointerEnter has been deleted
            // just send exit events to anything we are tracking
            // then exit
            if (newEnterTarget == null || currentPointerData.pointerEnter == null)
            {
                for (var i = 0; i < currentPointerData.hovered.Count; ++i)
                {
                    //ExecuteEvents.Execute(currentPointerData.hovered[i], currentPointerData, ExecuteEvents.pointerExitHandler);
                    AddPointerEvent(new PointerExitEvent(currentPointerData.hovered[i], currentPointerData));
                }

                currentPointerData.hovered.Clear();

                if (newEnterTarget == null)
                {
                    currentPointerData.pointerEnter = newEnterTarget;
                    return;
                }
            }

            // if we have not changed hover target
            if (currentPointerData.pointerEnter == newEnterTarget && newEnterTarget)
                return;

            GameObject commonRoot = FindCommonRoot(currentPointerData.pointerEnter, newEnterTarget);

            // and we already an entered object from last time
            if (currentPointerData.pointerEnter != null)
            {
                // send exit handler call to all elements in the chain
                // until we reach the new target, or null!
                Transform t = currentPointerData.pointerEnter.transform;

                while (t != null)
                {
                    // if we reach the common root break out!
                    if (commonRoot != null && commonRoot.transform == t)
                        break;

                    //ExecuteEvents.Execute(t.gameObject, currentPointerData, ExecuteEvents.pointerExitHandler);
                    AddPointerEvent(new PointerExitEvent(t.gameObject, currentPointerData));
                    currentPointerData.hovered.Remove(t.gameObject);
                    t = t.parent;
                }
            }

            // now issue the enter call up to but not including the common root
            currentPointerData.pointerEnter = newEnterTarget;
            if (newEnterTarget != null)
            {
                Transform t = newEnterTarget.transform;

                while (t != null && t.gameObject != commonRoot)
                {
                    //ExecuteEvents.Execute(t.gameObject, currentPointerData, ExecuteEvents.pointerEnterHandler);
                    AddPointerEvent(new PointerEnterEvent(t.gameObject, currentPointerData));
                    currentPointerData.hovered.Add(t.gameObject);
                    t = t.parent;
                }
            }
        }

        /// <summary>
        /// 获取AxisEventData数据
        /// </summary>
        /// <param name="x">x值</param>
        /// <param name="y">y值</param>
        /// <param name="moveDeadZone">阈值</param>
        /// <returns></returns>
        protected virtual AxisEventData GetAxisEventData(float x, float y, float moveDeadZone)
        {
            if (_axisEventData == null)
                _axisEventData = new AxisEventData(eventSystem);

            _axisEventData.Reset();
            _axisEventData.moveVector = new Vector2(x, y);
            _axisEventData.moveDir = DetermineMoveDirection(x, y, moveDeadZone);
            return _axisEventData;
        }

        /// <summary>
        /// 获取基础事件数据值
        /// </summary>
        /// <returns>基础事件数据</returns>
        protected virtual BaseEventData GetBaseEventData()
        {
            if (_baseEventData == null)
                _baseEventData = new BaseEventData(eventSystem);

            _baseEventData.Reset();
            return _baseEventData;
        }

        /// <summary>
        /// 是否在游戏对象上
        /// </summary>
        /// <param name="pointerId">按键id</param>
        /// <returns>布尔值</returns>
        public virtual bool IsPointerOverGameObject(int pointerId)
        {
            return false;
        }

        private bool _selectionGuard;
        protected GameObject _currentSelected;

        public GameObject currentSelected => _currentSelected;

        /// <summary>
        /// 设置当前检测到的游戏对象
        /// </summary>
        /// <param name="selected">当前选择</param>
        /// <param name="pointer">基础事件数据</param>
        public void SetSelectedGameObject(GameObject selected, BaseEventData pointer)
        {
            if (_selectionGuard)
            {
                Debug.LogError("Attempting to select " + selected + "while already selecting an object.");
                return;
            }

            _selectionGuard = true;
            if (selected == _currentSelected)
            {
                _selectionGuard = false;
                return;
            }

            //ExecuteEvents.Execute(_currentSelected, pointer, ExecuteEvents.deselectHandler);
            AddPointerEvent(new DeSelectEvent(_currentSelected, pointer));
            _currentSelected = selected;
            //ExecuteEvents.Execute(_currentSelected, pointer, ExecuteEvents.selectHandler);
            AddPointerEvent(new SelectEvent(_currentSelected, pointer));
            _selectionGuard = false;
        }

        /// <summary>
        /// 获取对象所有子对象集合
        /// </summary>
        /// <param name="root">根对象</param>
        /// <param name="eventChain">对象链表</param>
        protected void GetEventChain(GameObject root, IList<Transform> eventChain)
        {
            eventChain.Clear();
            if (root == null)
                return;

            var t = root.transform;
            while (t != null)
            {
                eventChain.Add(t);
                t = t.parent;
            }
        }

        private List<Transform> _internalTransformList = new List<Transform>();
        /// <summary>
        /// 添加可以游戏对象所有子对象中可以响应事件的游戏对象
        /// </summary>
        /// <typeparam name="T1">事件类型</typeparam>
        /// <typeparam name="T2">事件数据类型</typeparam>
        /// <param name="root">根对象</param>
        /// <param name="eventData">数据</param>
        /// <returns></returns>
        public GameObject AddHierarchy<T1, T2>(GameObject root, BaseEventData eventData) where T1 : IEventSystemHandler where T2 : BasePointerEvent, new()
        {
            GetEventChain(root, _internalTransformList);

            for (var i = 0; i < _internalTransformList.Count; i++)
            {
                if(_internalTransformList[i].GetComponent<T1>() != null)
                {
                    T2 poniterEvent = new T2();
                    poniterEvent.SetProperty(_internalTransformList[i].gameObject, eventData);
                    AddPointerEvent(poniterEvent);
                    return _internalTransformList[i].gameObject;
                }
            }
            return null;
        }

        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="basePointerEvent"></param>
        public void AddPointerEvent(BasePointerEvent basePointerEvent)
        {
            if (_pointerEvents.Exists(eve=> eve.GetType() == basePointerEvent.GetType() && eve.eventGameObject == basePointerEvent.eventGameObject))
                return;
            _pointerEvents.Add(basePointerEvent);
        }


        /// <summary>
        /// 非激活模块
        /// </summary>
        public virtual void DeactivateModule()
        { }

        /// <summary>
        /// 激活模块
        /// </summary>
        public virtual void ActivateModule()
        { }

        /// <summary>
        /// 更新模块
        /// </summary>
        public virtual void UpdateModule()
        { }

        /// <summary>
        /// 检测所有对象
        /// </summary>
        /// <param name="eventData">事件数据</param>
        /// <param name="raycastResults">检测结果集</param>
        public virtual void RaycastAll(PointerEventData eventData, List<RaycastResult> raycastResults)
        {
            eventSystem.RaycastAll(eventData, raycastResults);
        }
    }
}
