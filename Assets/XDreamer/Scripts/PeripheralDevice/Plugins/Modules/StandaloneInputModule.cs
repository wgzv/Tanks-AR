using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UInput = UnityEngine.Input;

namespace XCSJ.PluginPeripheralDevice.Modules
{
    /// <summary>
    /// 平台输入模块类
    /// </summary>
    public class StandaloneInputModule : PointerInputModule
    {
        /// <summary>
        /// 上一动作时间记录
        /// </summary>
        private float _prevActionTime;
        /// <summary>
        /// 上一次移动向量
        /// </summary>
        Vector2 _lastMoveVector;

        /// <summary>
        /// 连续不断的移动数量
        /// </summary>
        int _consecutiveMoveCount = 0;

        /// <summary>
        /// 上一帧鼠标位置
        /// </summary>
        private Vector2 _lastMousePosition;
        /// <summary>
        /// 鼠标位置
        /// </summary>
        private Vector2 _mousePosition;

        /// <summary>
        /// 构造函数
        /// </summary>
        public StandaloneInputModule()
        {
            this._raycastResultCache = new List<RaycastResult>();
            this._eventSystem = EventSystem.current;
        }

        /// <summary>
        /// 水平轴名称
        /// </summary>
        private string _horizontalAxis = "Horizontal";

        /// <summary>
        /// 垂直轴名称
        /// </summary>
        private string _verticalAxis = "Vertical";

        /// <summary>
        /// 提交按钮
        /// </summary>
        private string _submitButton = "Submit";

        /// <summary>
        /// 取消按钮
        /// </summary>
        private string _cancelButton = "Cancel";

        /// <summary>
        /// 每秒几个输入动作
        /// </summary>
        private float _inputActionsPerSecond = 10;

        /// <summary>
        /// 重复延迟
        /// </summary>
        private float _repeatDelay = 0.5f;

        /// <summary>
        /// 强制模块激活
        /// </summary>
        private bool _forceModuleActive;

        public bool forceModuleActive
        {
            get { return _forceModuleActive; }
            set { _forceModuleActive = value; }
        }

        public float inputActionsPerSecond
        {
            get { return _inputActionsPerSecond; }
            set { _inputActionsPerSecond = value; }
        }

        public float repeatDelay
        {
            get { return _repeatDelay; }
            set { _repeatDelay = value; }
        }

        /// <summary>
        /// Name of the horizontal axis for movement (if axis events are used).
        /// </summary>
        public string horizontalAxis
        {
            get { return _horizontalAxis; }
            set { _horizontalAxis = value; }
        }

        /// <summary>
        /// Name of the vertical axis for movement (if axis events are used).
        /// </summary>
        public string verticalAxis
        {
            get { return _verticalAxis; }
            set { _verticalAxis = value; }
        }

        public string submitButton
        {
            get { return _submitButton; }
            set { _submitButton = value; }
        }

        public string cancelButton
        {
            get { return _cancelButton; }
            set { _cancelButton = value; }
        }

        /// <summary>
        /// 更新模块
        /// </summary>
        public override void UpdateModule()
        {
            _lastMousePosition = _mousePosition;
            _mousePosition = UInput.mousePosition;
        }

        /// <summary>
        /// 激活模块
        /// </summary>
        public override void ActivateModule()
        {
            base.ActivateModule();
            _mousePosition = UInput.mousePosition;
            _lastMousePosition = UInput.mousePosition;
            var toSelect = eventSystem.currentSelectedGameObject;
            if (toSelect == null)
                toSelect = eventSystem.firstSelectedGameObject;

            SetSelectedGameObject(toSelect, GetBaseEventData());
        }

        /// <summary>
        /// 非激活模块
        /// </summary>
        public override void DeactivateModule()
        {
            base.DeactivateModule();
            ClearSelection();
        }

        /// <summary>
        /// 处理事件
        /// </summary>
        public override void Process()
        {
            _pointerEvents.Clear();
            bool usedEvent = SendUpdateEventToSelectedObject();

            if (eventSystem.sendNavigationEvents)
            {
                if (!usedEvent)
                    usedEvent |= SendMoveEventToSelectedObject();

                if (!usedEvent)
                    SendSubmitEventToSelectedObject();
            }

            ProcessMouseEvent();
        }

        /// <summary>
        /// 处理提交事件
        /// </summary>
        protected bool SendSubmitEventToSelectedObject()
        {
            if (eventSystem.currentSelectedGameObject == null)
            {
                return false;
            }

            var data = GetBaseEventData();
            if (UInput.GetButtonDown(_submitButton))
            {
                //ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.submitHandler);
                AddPointerEvent(new SubmitEvent(eventSystem.currentSelectedGameObject, data));
            }


            if (UInput.GetButtonDown(_cancelButton))
            {
                //ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.cancelHandler);
                AddPointerEvent(new CancelEvent(eventSystem.currentSelectedGameObject, data));
            }
            return data.used;
        }

        /// <summary>
        /// 获取移动数据
        /// </summary>
        /// <returns>2D移动数据</returns>
        private Vector2 GetRawMoveVector()
        {
            Vector2 move = Vector2.zero;
            move.x = UInput.GetAxisRaw(_horizontalAxis);
            move.y = UInput.GetAxisRaw(_verticalAxis);

            if (UInput.GetButtonDown(_horizontalAxis))
            {
                if (move.x < 0)
                    move.x = -1f;
                if (move.x > 0)
                    move.x = 1f;
            }
            if (UInput.GetButtonDown(_verticalAxis))
            {
                if (move.y < 0)
                    move.y = -1f;
                if (move.y > 0)
                    move.y = 1f;
            }
            return move;
        }

        /// <summary>
        /// 设置移动事件到射线检测到的游戏对象
        /// </summary>
        protected bool SendMoveEventToSelectedObject()
        {
            float time = Time.unscaledTime;

            Vector2 movement = GetRawMoveVector();
            if (Mathf.Approximately(movement.x, 0f) && Mathf.Approximately(movement.y, 0f))
            {
                _consecutiveMoveCount = 0;
                return false;
            }

            // If user pressed key again, always allow event
            bool allow = UInput.GetButtonDown(_horizontalAxis) || UInput.GetButtonDown(_verticalAxis);
            bool similarDir = (Vector2.Dot(movement, _lastMoveVector) > 0);
            if (!allow)
            {
                // Otherwise, user held down key or axis.
                // If direction didn't change at least 90 degrees, wait for delay before allowing consequtive event.
                if (similarDir && _consecutiveMoveCount == 1)
                    allow = (time > _prevActionTime + _repeatDelay);
                // If direction changed at least 90 degree, or we already had the delay, repeat at repeat rate.
                else
                    allow = (time > _prevActionTime + 1f / _inputActionsPerSecond);
            }
            if (!allow)
                return false;

            // Debug.Log(m_ProcessingEvent.rawType + " axis:" + m_AllowAxisEvents + " value:" + "(" + x + "," + y + ")");
            var axisEventData = GetAxisEventData(movement.x, movement.y, 0.6f);
            //ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
            AddPointerEvent(new MoveEvent(eventSystem.currentSelectedGameObject, axisEventData));
            if (!similarDir)
                _consecutiveMoveCount = 0;
            _consecutiveMoveCount++;
            _prevActionTime = time;
            _lastMoveVector = movement;
            return axisEventData.used;
        }

        /// <summary>
        /// 处理鼠标事件
        /// </summary>
        protected void ProcessMouseEvent()
        {
            ProcessMouseEvent(0);
        }

        /// <summary>
        /// 处理某个鼠标按键事件
        /// </summary>
        /// <param name="id">按键ID</param>
        protected void ProcessMouseEvent(int id)
        {
            var mouseData = GetMousePointerEventData(id);
            var leftButtonData = mouseData.GetButtonState(PointerEventData.InputButton.Left).eventData;

            // Process the first mouse button fully
            ProcessMousePress(leftButtonData);
            ProcessMove(leftButtonData.buttonData);
            ProcessDrag(leftButtonData.buttonData);

            // Now process right / middle clicks
            ProcessMousePress(mouseData.GetButtonState(PointerEventData.InputButton.Right).eventData);
            ProcessDrag(mouseData.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonData);
            ProcessMousePress(mouseData.GetButtonState(PointerEventData.InputButton.Middle).eventData);
            ProcessDrag(mouseData.GetButtonState(PointerEventData.InputButton.Middle).eventData.buttonData);

            if (!Mathf.Approximately(leftButtonData.buttonData.scrollDelta.sqrMagnitude, 0.0f))
            {
                var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(leftButtonData.buttonData.pointerCurrentRaycast.gameObject);
                //ExecuteEvents.ExecuteHierarchy(scrollHandler, leftButtonData.buttonData, ExecuteEvents.scrollHandler);
                AddHierarchy<IScrollHandler, ScrollEvent>(scrollHandler, leftButtonData.buttonData);
            }
        }

        /// <summary>
        /// 设置UpdateEvent到射线检测到的游戏对象
        /// </summary>
        /// <returns></returns>
        protected bool SendUpdateEventToSelectedObject()
        {
            var data = GetBaseEventData();
            if (eventSystem.currentSelectedGameObject == null)
            {
                SetSelectedGameObject(null, data);
                return false;
            }

            if (eventSystem.currentSelectedGameObject != _currentSelected)
            {
                SetSelectedGameObject(eventSystem.currentSelectedGameObject, data);
            }
            else
            {
                //ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
                AddPointerEvent(new UpdateSelectedEvent(eventSystem.currentSelectedGameObject, data));
            }
            return data.used;
        }

        /// <summary>
        /// 处理鼠标按下事件
        /// </summary>
        protected void ProcessMousePress(MouseButtonEventData data)
        {
            var pointerEvent = data.buttonData;
            var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

            // PointerDown notification
            if (data.PressedThisFrame())
            {
                pointerEvent.eligibleForClick = true;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.dragging = false;
                pointerEvent.useDragThreshold = true;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

                DeselectIfSelectionChanged(currentOverGo, pointerEvent);

                // search for the control that will receive the press
                // if we can't find a press handler set the press
                // handler to be what would receive a click.
                var newPressed = AddHierarchy<IPointerDownHandler, PointerDownEvent>(currentOverGo, pointerEvent);
                //ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);


                // didnt find a press handler... search for a click handler
                if (newPressed == null)
                    newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                // Debug.Log("Pressed: " + newPressed);

                float time = Time.unscaledTime;

                if (newPressed == pointerEvent.lastPress)
                {
                    var diffTime = time - pointerEvent.clickTime;
                    if (diffTime < 0.3f)
                        ++pointerEvent.clickCount;
                    else
                        pointerEvent.clickCount = 1;

                    pointerEvent.clickTime = time;
                }
                else
                {
                    pointerEvent.clickCount = 1;
                }

                pointerEvent.pointerPress = newPressed;
                pointerEvent.rawPointerPress = currentOverGo;

                pointerEvent.clickTime = time;

                // Save the drag handler as well
                pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

                if (pointerEvent.pointerDrag != null)
                {
                    //ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
                    AddPointerEvent(new InitializePotentialDragEvent(pointerEvent.pointerDrag, pointerEvent));
                }
            }

            // PointerUp notification
            if (data.ReleasedThisFrame())
            {
                // Debug.Log("Executing pressup on: " + pointer.pointerPress);
                //ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);
                AddPointerEvent(new PointerUpEvent(pointerEvent.pointerPress, pointerEvent));
                // Debug.Log("KeyCode: " + pointer.eventData.keyCode);

                // see if we mouse up on the same element that we clicked on...
                var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                // PointerClick and Drop events
                if (pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick)
                {
                    //ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
                    AddPointerEvent(new PointerClickEvent(pointerEvent.pointerPress, pointerEvent));
                }
                else if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
                {
                    //ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.dropHandler);
                    AddHierarchy<IDropHandler, DropEvent>(currentOverGo, pointerEvent);
                }

                pointerEvent.eligibleForClick = false;
                pointerEvent.pointerPress = null;
                pointerEvent.rawPointerPress = null;

                if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
                {
                    //ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);
                    AddPointerEvent(new EndDragEvent(pointerEvent.pointerDrag, pointerEvent));
                }

                pointerEvent.dragging = false;
                pointerEvent.pointerDrag = null;

                // redo pointer enter / exit to refresh state
                // so that if we moused over somethign that ignored it before
                // due to having pressed on something else
                // it now gets it.
                if (currentOverGo != pointerEvent.pointerEnter)
                {
                    HandlePointerExitAndEnter(pointerEvent, null);
                    HandlePointerExitAndEnter(pointerEvent, currentOverGo);
                }
            }
        }
    }
}
