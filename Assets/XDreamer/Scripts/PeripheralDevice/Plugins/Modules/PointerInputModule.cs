using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UInput = UnityEngine.Input;

namespace XCSJ.PluginPeripheralDevice.Modules
{
    public abstract class PointerInputModule : BaseInputModule
    {
        /// <summary>
        /// 定义鼠标左键ID
        /// </summary>
        public const int kMouseLeftId = -1;
        /// <summary>
        /// 定义鼠标右键ID
        /// </summary>
        public const int kMouseRightId = -2;
        /// <summary>
        /// 定义鼠标中键ID
        /// </summary>
        public const int kMouseMiddleId = -3;

        /// <summary>
        /// 定义触摸点ID
        /// </summary>
        public const int kFakeTouchesId = -4;

        /// <summary>
        /// 检测点数据
        /// </summary>
        protected Dictionary<int, PointerEventData> _pointerData = new Dictionary<int, PointerEventData>();

        /// <summary>
        /// 获取监测点数据
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="data">数据</param>
        /// <param name="create">是否新建</param>
        /// <returns></returns>
        protected bool GetPointerData(int id, out PointerEventData data, bool create)
        {
            if (!_pointerData.TryGetValue(id, out data) && create)
            {
                data = new PointerEventData(eventSystem)
                {
                    pointerId = id,
                };
                _pointerData.Add(id, data);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 移除指定检测点数据
        /// </summary>
        /// <param name="data">数据</param>
        protected void RemovePointerData(PointerEventData data)
        {
            _pointerData.Remove(data.pointerId);
        }

        /// <summary>
        /// 获取触摸点数据
        /// </summary>
        /// <param name="input">触摸点</param>
        /// <param name="pressed">按下</param>
        /// <param name="released">释放</param>
        /// <returns></returns>
        protected PointerEventData GetTouchPointerEventData(Touch input, out bool pressed, out bool released)
        {
            PointerEventData pointerData;
            var created = GetPointerData(input.fingerId, out pointerData, true);

            pointerData.Reset();

            pressed = created || (input.phase == TouchPhase.Began);
            released = (input.phase == TouchPhase.Canceled) || (input.phase == TouchPhase.Ended);

            if (created)
                pointerData.position = input.position;

            if (pressed)
                pointerData.delta = Vector2.zero;
            else
                pointerData.delta = input.position - pointerData.position;

            pointerData.position = input.position;

            pointerData.button = PointerEventData.InputButton.Left;

            RaycastAll(pointerData, _raycastResultCache);

            var raycast = FindFirstRaycast(_raycastResultCache);
            pointerData.pointerCurrentRaycast = raycast;
            _raycastResultCache.Clear();
            return pointerData;
        }

        /// <summary>
        /// 拷贝数据
        /// </summary>
        /// <param name="from">数据源</param>
        /// <param name="to">目标源</param>
        protected void CopyFromTo(PointerEventData @from, PointerEventData @to)
        {
            @to.position = @from.position;
            @to.delta = @from.delta;
            @to.scrollDelta = @from.scrollDelta;
            @to.pointerCurrentRaycast = @from.pointerCurrentRaycast;
            @to.pointerEnter = @from.pointerEnter;
        }

        /// <summary>
        /// 获取鼠标按键状态
        /// </summary>
        /// <param name="buttonId">按键ID</param>
        /// <returns>按键状态</returns>
        protected static PointerEventData.FramePressState StateForMouseButton(int buttonId)
        {
            var pressed = Input.GetMouseButtonDown(buttonId);
            var released = Input.GetMouseButtonUp(buttonId);
            if (pressed && released)
                return PointerEventData.FramePressState.PressedAndReleased;
            if (pressed)
                return PointerEventData.FramePressState.Pressed;
            if (released)
                return PointerEventData.FramePressState.Released;
            return PointerEventData.FramePressState.NotChanged;
        }

        protected class ButtonState
        {
            private PointerEventData.InputButton m_Button = PointerEventData.InputButton.Left;

            public MouseButtonEventData eventData
            {
                get { return m_EventData; }
                set { m_EventData = value; }
            }

            public PointerEventData.InputButton button
            {
                get { return m_Button; }
                set { m_Button = value; }
            }

            private MouseButtonEventData m_EventData;
        }

        protected class MouseState
        {
            private List<ButtonState> m_TrackedButtons = new List<ButtonState>();

            public bool AnyPressesThisFrame()
            {
                for (int i = 0; i < m_TrackedButtons.Count; i++)
                {
                    if (m_TrackedButtons[i].eventData.PressedThisFrame())
                        return true;
                }
                return false;
            }

            public bool AnyReleasesThisFrame()
            {
                for (int i = 0; i < m_TrackedButtons.Count; i++)
                {
                    if (m_TrackedButtons[i].eventData.ReleasedThisFrame())
                        return true;
                }
                return false;
            }

            public ButtonState GetButtonState(PointerEventData.InputButton button)
            {
                ButtonState tracked = null;
                for (int i = 0; i < m_TrackedButtons.Count; i++)
                {
                    if (m_TrackedButtons[i].button == button)
                    {
                        tracked = m_TrackedButtons[i];
                        break;
                    }
                }

                if (tracked == null)
                {
                    tracked = new ButtonState { button = button, eventData = new MouseButtonEventData() };
                    m_TrackedButtons.Add(tracked);
                }
                return tracked;
            }

            public void SetButtonState(PointerEventData.InputButton button, PointerEventData.FramePressState stateForMouseButton, PointerEventData data)
            {
                var toModify = GetButtonState(button);
                toModify.eventData.buttonState = stateForMouseButton;
                toModify.eventData.buttonData = data;
            }
        }

        public class MouseButtonEventData
        {
            public PointerEventData.FramePressState buttonState;
            public PointerEventData buttonData;

            public bool PressedThisFrame()
            {
                return buttonState == PointerEventData.FramePressState.Pressed || buttonState == PointerEventData.FramePressState.PressedAndReleased;
            }

            public bool ReleasedThisFrame()
            {
                return buttonState == PointerEventData.FramePressState.Released || buttonState == PointerEventData.FramePressState.PressedAndReleased;
            }
        }

        private readonly MouseState m_MouseState = new MouseState();

        /// <summary>
        /// 获取鼠标左键事件数据
        /// </summary>
        /// <returns>MouseState</returns>
        protected virtual MouseState GetMousePointerEventData()
        {
            return GetMousePointerEventData(0);
        }

        /// <summary>
        /// 获取鼠标按键事件数据
        /// </summary>
        /// <returns>MouseState</returns>
        protected virtual MouseState GetMousePointerEventData(int id)
        {
            // Populate the left button...
            PointerEventData leftData;
            var created = GetPointerData(kMouseLeftId, out leftData, true);

            leftData.Reset();

            if (created)
                leftData.position = UInput.mousePosition;

            Vector2 pos = UInput.mousePosition;
            leftData.delta = pos - leftData.position;
            leftData.position = pos;
            leftData.scrollDelta = UInput.mouseScrollDelta;
            leftData.button = PointerEventData.InputButton.Left;
            RaycastAll(leftData, _raycastResultCache);
            var raycast = FindFirstRaycast(_raycastResultCache);
            leftData.pointerCurrentRaycast = raycast;
            _raycastResultCache.Clear();

            // copy the apropriate data into right and middle slots
            PointerEventData rightData;
            GetPointerData(kMouseRightId, out rightData, true);
            CopyFromTo(leftData, rightData);
            rightData.button = PointerEventData.InputButton.Right;

            PointerEventData middleData;
            GetPointerData(kMouseMiddleId, out middleData, true);
            CopyFromTo(leftData, middleData);
            middleData.button = PointerEventData.InputButton.Middle;

            m_MouseState.SetButtonState(PointerEventData.InputButton.Left, StateForMouseButton(0), leftData);
            m_MouseState.SetButtonState(PointerEventData.InputButton.Right, StateForMouseButton(1), rightData);
            m_MouseState.SetButtonState(PointerEventData.InputButton.Middle, StateForMouseButton(2), middleData);

            return m_MouseState;
        }

        /// <summary>
        /// 获得上一个事件数据
        /// </summary>
        /// <param name="id">按键ID</param>
        /// <returns>PointerEventData</returns>
        protected PointerEventData GetLastPointerEventData(int id)
        {
            PointerEventData data;
            GetPointerData(id, out data, false);
            return data;
        }

        /// <summary>
        /// 是否开始拖拽
        /// </summary>
        /// <param name="pressPos">按下位置</param>
        /// <param name="currentPos">当前位置</param>
        /// <param name="threshold">阈值</param>
        /// <param name="useDragThreshold">是否使用阈值</param>
        /// <returns></returns>
        private static bool ShouldStartDrag(Vector2 pressPos, Vector2 currentPos, float threshold, bool useDragThreshold)
        {
            if (!useDragThreshold)
                return true;

            return (pressPos - currentPos).sqrMagnitude >= threshold * threshold;
        }

        /// <summary>
        /// 处理移动事件
        /// </summary>
        /// <param name="pointerEvent">事件数据</param>
        protected virtual void ProcessMove(PointerEventData pointerEvent)
        {
            var targetGO = pointerEvent.pointerCurrentRaycast.gameObject;
            HandlePointerExitAndEnter(pointerEvent, targetGO);
        }

        /// <summary>
        /// 处理拖拽事件
        /// </summary>
        /// <param name="pointerEvent">事件数据</param>
        protected virtual void ProcessDrag(PointerEventData pointerEvent)
        {
            bool moving = pointerEvent.IsPointerMoving();

            if (moving && pointerEvent.pointerDrag != null
                && !pointerEvent.dragging
                && ShouldStartDrag(pointerEvent.pressPosition, pointerEvent.position, eventSystem.pixelDragThreshold, pointerEvent.useDragThreshold))
            {
                //ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.beginDragHandler);
                AddPointerEvent(new BeginDragEvent(pointerEvent.pointerDrag, pointerEvent));
                pointerEvent.dragging = true;
            }

            // Drag notification
            if (pointerEvent.dragging && moving && pointerEvent.pointerDrag != null)
            {
                // Before doing drag we should cancel any pointer down state
                // And clear selection!
                if (pointerEvent.pointerPress != pointerEvent.pointerDrag)
                {
                    //ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);
                    AddPointerEvent(new PointerUpEvent(pointerEvent.pointerPress, pointerEvent));
                    pointerEvent.eligibleForClick = false;
                    pointerEvent.pointerPress = null;
                    pointerEvent.rawPointerPress = null;
                }
                //ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.dragHandler);
                AddPointerEvent(new DragEvent(pointerEvent.pointerDrag, pointerEvent));
            }
        }

        /// <summary>
        /// 当前是否检测到游戏对象
        /// </summary>
        /// <param name="pointerId">按键ID</param>
        /// <returns>布尔值</returns>
        public override bool IsPointerOverGameObject(int pointerId)
        {
            var lastPointer = GetLastPointerEventData(pointerId);
            if (lastPointer != null)
                return lastPointer.pointerEnter != null;
            return false;
        }

        /// <summary>
        /// 清空选择
        /// </summary>
        protected void ClearSelection()
        {
            var baseEventData = GetBaseEventData();

            foreach (var pointer in _pointerData.Values)
            {
                // clear all selection
                HandlePointerExitAndEnter(pointer, null);
            }

            _pointerData.Clear();
            //eventSystem.SetSelectedGameObject(null, baseEventData);
            SetSelectedGameObject(null, baseEventData);
        }

        /// <summary>
        /// 改变游戏对象，处理Deselect事件
        /// </summary>
        /// <param name="currentOverGo">当前选择对象</param>
        /// <param name="pointerEvent">事件数据</param>
        protected void DeselectIfSelectionChanged(GameObject currentOverGo, BaseEventData pointerEvent)
        {
            // Selection tracking
            var selectHandlerGO = ExecuteEvents.GetEventHandler<ISelectHandler>(currentOverGo);
            // if we have clicked something new, deselect the old thing
            // leave 'selection handling' up to the press event though.
            if (selectHandlerGO != _currentSelected)
                SetSelectedGameObject(null, pointerEvent);
        }
    }
}
