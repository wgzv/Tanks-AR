using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UInput = UnityEngine.Input;

namespace XCSJ.PluginPeripheralDevice.Modules
{
    public class TouchInputModule : PointerInputModule
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public TouchInputModule()
        {
            this._raycastResultCache = new List<RaycastResult>();
            this._eventSystem = EventSystem.current;
        }
        private Vector2 _lastMousePosition;
        private Vector2 _mousePosition;

        [SerializeField]
        private bool _forceModuleActive;

        public bool forceModuleActive
        {
            get { return _forceModuleActive; }
            set { _forceModuleActive = value; }
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
        /// 是否假触摸
        /// </summary>
        /// <returns>布尔值，是否假触摸</returns>
        private bool UseFakeInput()
        {
            return !UInput.touchSupported;
        }

        /// <summary>
        /// 处理触摸事件函数
        /// </summary>
        public override void Process()
        {
            _pointerEvents.Clear();
            if (UseFakeInput())
                FakeTouches();
            else
                ProcessTouchEvents();
        }

        /// <summary>
        /// 假触摸处理，处理鼠标事件
        /// </summary>
        private void FakeTouches()
        {
            var pointerData = GetMousePointerEventData(0);

            var leftPressData = pointerData.GetButtonState(PointerEventData.InputButton.Left).eventData;

            // fake touches... on press clear delta
            if (leftPressData.PressedThisFrame())
                leftPressData.buttonData.delta = Vector2.zero;

            ProcessTouchPress(leftPressData.buttonData, leftPressData.PressedThisFrame(), leftPressData.ReleasedThisFrame());

            // only process move if we are pressed...
            if (UInput.GetMouseButton(0))
            {
                ProcessMove(leftPressData.buttonData);
                ProcessDrag(leftPressData.buttonData);
            }
        }

        /// <summary>
        /// 处理所有的触摸事件
        /// </summary>
        private void ProcessTouchEvents()
        {
            for (int i = 0; i < UInput.touchCount; ++i)
            {
                Touch input = UInput.GetTouch(i);

                bool released;
                bool pressed;
                var pointer = GetTouchPointerEventData(input, out pressed, out released);

                ProcessTouchPress(pointer, pressed, released);

                if (!released)
                {
                    ProcessMove(pointer);
                    ProcessDrag(pointer);
                }
                else
                    RemovePointerData(pointer);
            }
        }

        /// <summary>
        /// 处理触摸按下事件
        /// </summary>
        /// <param name="pointerEvent">事件数据</param>
        /// <param name="pressed">按下</param>
        /// <param name="released">释放</param>
        private void ProcessTouchPress(PointerEventData pointerEvent, bool pressed, bool released)
        {
            var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

            // PointerDown notification
            if (pressed)
            {
                pointerEvent.eligibleForClick = true;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.dragging = false;
                pointerEvent.useDragThreshold = true;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

                DeselectIfSelectionChanged(currentOverGo, pointerEvent);

                if (pointerEvent.pointerEnter != currentOverGo)
                {
                    // send a pointer enter to the touched element if it isn't the one to select...
                    HandlePointerExitAndEnter(pointerEvent, currentOverGo);
                    pointerEvent.pointerEnter = currentOverGo;
                }

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
            if (released)
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

                if (pointerEvent.pointerDrag != null)
                {
                    //ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);
                    AddPointerEvent(new EndDragEvent(pointerEvent.pointerDrag, pointerEvent));
                }

                pointerEvent.pointerDrag = null;

                // send exit events as we need to simulate this on touch up on touch device
                //ExecuteEvents.ExecuteHierarchy(pointerEvent.pointerEnter, pointerEvent, ExecuteEvents.pointerExitHandler);
                AddHierarchy<IPointerExitHandler, PointerExitEvent>(pointerEvent.pointerEnter, pointerEvent);
                pointerEvent.pointerEnter = null;
            }
        }

        /// <summary>
        /// 非激活模块
        /// </summary>
        public override void DeactivateModule()
        {
            base.DeactivateModule();
            ClearSelection();
        }
    }
}
