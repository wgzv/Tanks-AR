using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;

namespace XCSJ.Extension.CNScripts.Inputs
{
    /// <summary>
    /// 触摸脚本事件类型 
    /// </summary>
    public enum ETouchScriptEventType
    {
        /// <summary>
        /// 左滑时执行
        /// </summary>
        [Name("左滑时执行")]
        SwipeLeft = 0,

        [Name("右滑时执行")]
        SwipeRight,

        [Name("上滑时执行")]
        SwipeUp,

        [Name("下滑时执行")]
        SwipeDown,

        [Name("水平拖动时执行")]
        DragHorizon,

        [Name("垂直拖动时执行")]
        DragVertical,

        [Name("两指延伸时执行")]
        Spread,

        [Name("两指收缩时执行")]
        Pinch,

        [Name("启动时执行")]
        Start,

        [Name("双指水平拖动时执行")]
        DoubleDragHorizon,

        [Name("双指垂直拖动时执行")]
        DoubleDragVertical,
    }

    /// <summary>
    /// 触摸脚本事件集合类
    /// </summary>
    [Serializable]
    public class TouchScriptEventSet : BaseScriptEventSet<ETouchScriptEventType>
    {
        //
    }

    /// <summary>
    /// 触摸脚本事件
    /// </summary>
    [Serializable]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.InputMenu + Title)]
    public class TouchScriptEvent : BaseScriptEvent<ETouchScriptEventType, TouchScriptEventSet>
    {
        public const string Title = "触摸脚本事件";

        [Name("单指移动触发阈值")]
        [Range(0.01f, 960f)]
        public float oneFingerMoveTriggerThreshold = 10;

        [Name("双指移动触发阈值")]
        [Range(0.01f, 960f)]
        public float twoFingerMoveTriggerThreshold = 2;

        float x_dis = 0;

        float y_dis = 0;

        float m_currentTouchDistance = 0;

        float m_lastTouchDistance = 0;

        public override void Start()
        {
            RunScriptEvent(ETouchScriptEventType.Start);
        }

        public override void Update()
        {
            switch (Input.touchCount)
            {
                case 1://判断触摸数量为单点触摸  
                    {
                        if (Input.GetTouch(0).phase == TouchPhase.Moved)
                        {
                            x_dis += Input.GetAxis("Mouse X");
                            y_dis += Input.GetAxis("Mouse Y");
                            float xDelta = Input.GetAxis("Mouse X");
                            float yDelta = Input.GetAxis("Mouse Y");
                            OnDragHorizon(xDelta);
                            OnDragVertical(yDelta);
                        }
                        else if (Input.GetTouch(0).phase == TouchPhase.Ended)
                        {
                            if (Mathf.Abs(x_dis) > Mathf.Abs(y_dis))
                            {
                                if (x_dis > oneFingerMoveTriggerThreshold)
                                    OnSwipeRight(x_dis);
                                else if (x_dis < -oneFingerMoveTriggerThreshold)
                                    OnSwipeLeft(x_dis);
                            }
                            else
                            {
                                if (y_dis > oneFingerMoveTriggerThreshold)
                                    OnSwipeUp(y_dis);
                                else if (y_dis < -oneFingerMoveTriggerThreshold)
                                    OnSwipeDown(y_dis);
                            }
                            x_dis = 0;
                            y_dis = 0;
                        }
                        break;
                    }
                case 2:
                    {
                        Touch touch1 = Input.GetTouch(0);
                        Touch touch2 = Input.GetTouch(1);
                        //前两只手指触摸类型都为移动触摸   
                        if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                        {
                            m_currentTouchDistance = Vector2.Distance(touch1.position, touch2.position);
                            m_lastTouchDistance = Vector2.Distance((touch1.position - touch1.deltaPosition), (touch2.position - touch2.deltaPosition));
                            float touchDeltaDistance = m_currentTouchDistance - m_lastTouchDistance;
                            if (touchDeltaDistance > twoFingerMoveTriggerThreshold)
                            {
                                OnSpread(touchDeltaDistance);
                            }
                            else if (touchDeltaDistance < -twoFingerMoveTriggerThreshold)
                            {
                                OnPinch(touchDeltaDistance);
                            }
                            else
                            {
                                float xDelta = Input.GetAxis("Mouse X");
                                float yDelta = Input.GetAxis("Mouse Y");
                                OnDoubleDragHorizon(xDelta);
                                OnDoubleDragVertical(yDelta);
                            }
                        }
                        break;
                    }
            }//end switch 触摸点数目
        }

        public void OnSwipeRight(float distance)
        {
            RunScriptEvent(ETouchScriptEventType.SwipeRight, distance.ToString());
        }

        public void OnSwipeLeft(float distance)
        {
            RunScriptEvent(ETouchScriptEventType.SwipeLeft, distance.ToString());
        }

        public void OnSwipeUp(float distance)
        {
            RunScriptEvent(ETouchScriptEventType.SwipeUp, distance.ToString());
        }

        public void OnSwipeDown(float distance)
        {
            RunScriptEvent(ETouchScriptEventType.SwipeDown, distance.ToString());
        }

        public void OnDragHorizon(float distance)
        {
            RunScriptEvent(ETouchScriptEventType.DragHorizon, distance.ToString());
        }

        public void OnDragVertical(float distance)
        {
            RunScriptEvent(ETouchScriptEventType.DragVertical, distance.ToString());
        }

        public void OnSpread(float distance)
        {
            RunScriptEvent(ETouchScriptEventType.Spread, distance.ToString());
        }

        public void OnPinch(float distance)
        {
            RunScriptEvent(ETouchScriptEventType.Pinch, distance.ToString());
        }

        public void OnDoubleDragHorizon(float distance)
        {
            RunScriptEvent(ETouchScriptEventType.DoubleDragHorizon, distance.ToString());
        }

        public void OnDoubleDragVertical(float distance)
        {
            RunScriptEvent(ETouchScriptEventType.DoubleDragVertical, distance.ToString());
        }
    }
}
