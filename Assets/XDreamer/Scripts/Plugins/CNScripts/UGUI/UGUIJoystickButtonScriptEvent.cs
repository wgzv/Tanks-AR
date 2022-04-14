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
    /// UGUI操纵杆按钮脚本事件类型
    /// </summary>
    public enum EUGUIJoystickButtonScriptEventType
    {
        [Name("点击时执行")]
        OnClick,

        [Name("拖拽时执行")]
        OnDrag,

        [Name("指针按下时执行")]
        OnPointerDown,

        [Name("指针抬起时执行")]
        OnPointerUp,

        [Name("启动时执行")]
        Start,
    }

    /// <summary>
    /// UGUI操纵杆按钮脚本事件集合
    /// </summary>
    [Serializable]
    public class UGUIJoystickButtonScriptEventSet : BaseScriptEventSet<EUGUIJoystickButtonScriptEventType>
    {
        //
    }

    /// <summary>
    /// UGUI操纵杆按钮脚本事件
    /// </summary>
    [Serializable]
    [RequireComponent(typeof(Button))]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.UGUIMenu + Title)]
    public class UGUIJoystickButtonScriptEvent : BaseScriptEvent<EUGUIJoystickButtonScriptEventType, UGUIJoystickButtonScriptEventSet>, IPointerUpHandler, IPointerDownHandler, IDragHandler
    {
        public const string Title = "UGUI操纵杆按钮脚本事件";

        public Button button { get; protected set; }

        /// <summary>
        /// 当前Button开始拖拽时相对父节点的坐标
        /// </summary>
        public Vector3 originLocalPosition { get; protected set; }

        /// <summary>
        /// 当前Button开始拖拽时屏幕坐标
        /// </summary>
        public Vector3 originPosition { get; protected set; }

        public Matrix4x4 originWorldMatrix { get; protected set; }

        /// <summary>
        /// 控件拖拽中相对开始拖拽时的位置偏移量
        /// </summary>
        protected Vector3 offset = new Vector3();

        /// <summary>
        /// 标识当前Button是否在拖拽中
        /// </summary>
        public bool isDrag { get; protected set; }

        /// <summary>
        /// 限定范围的类型
        /// </summary>
        public enum ELimitRangeType
        {
            [Name("半边长")]
            [Tip("正方形的限定范围类型；为限定正方形的边长的一半；")]
            HalfLength,

            [Name("半径")]
            [Tip("圆形的限定范围类型；为限定圆的半径；")]
            Radius,
        }

        public enum ELimitAxisType
        {
            [Name("全部")]
            [Tip("限定在横向轴与纵向轴均可移动；")]
            Both,

            [Name("仅横向")]
            [Tip("限定在横向轴移动；")]
            OnlyHorizontal,

            [Name("仅纵向")]
            [Tip("限定在纵向轴移动；")]
            OnlyVertical
        }

        [Name("运动范围")]
        [Tip("当前按钮被拖拽时以初始位置为中心可移动的范围；")]
        [Range(0.01f, 540f)]
        public float movementRange = 30;

        [Name("运动范围限定类型")]
        [Tip("当前按钮被拖拽时可移动的范围的类型；")]
        [EnumPopup]
        public ELimitRangeType limitRangeType = ELimitRangeType.Radius;

        [Name("运动轴向限定类型")]
        [Tip("当前按钮被拖拽时可移动的轴向的类型；")]
        [EnumPopup]
        public ELimitAxisType limitAxisType = ELimitAxisType.Both;

        public override void OnEnable()
        {
            base.OnEnable();
            button = gameObject.GetComponent<Button>();
            if (button)
            {
                button.onClick.AddListener(this.OnClick);
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (button)
            {
                button.onClick.RemoveListener(this.OnClick);
            }
        }

        public override void Start()
        {
            RunScriptEvent(EUGUIJoystickButtonScriptEventType.Start);
        }

        public virtual void OnClick()
        {
            RunScriptEvent(EUGUIJoystickButtonScriptEventType.OnClick);
        }

        public virtual void OnDrag(Vector3 v3)
        {
            RunScriptEvent(EUGUIJoystickButtonScriptEventType.OnDrag, CommonFun.Vector3ToString(v3));
        }

        private void FixedUpdate()
        {
            if (!isDrag || offset == Vector3.zero) return;
            OnDrag(offset);
        }

        /// <summary>
        /// 拖拽时
        /// </summary>
        /// <param name="data"></param>
        public virtual void OnDrag(PointerEventData data)
        {
            offset.x = data.position.x - originPosition.x;
            offset.y = data.position.y - originPosition.y;
            //Debug.Log("offset: " + offset + " , delta: " + data.delta + ", data.position: " + data.position );
            switch (limitRangeType)
            {
                case ELimitRangeType.HalfLength:
                    {
                        offset.x = Mathf.Clamp(offset.x, -movementRange, movementRange);
                        offset.y = Mathf.Clamp(offset.y, -movementRange, movementRange);
                        break;
                    }
                case ELimitRangeType.Radius:
                default:
                    {
                        offset = offset.normalized * Mathf.Min(offset.magnitude, movementRange);
                        break;
                    }
            }
            switch (limitAxisType)
            {
                case ELimitAxisType.OnlyHorizontal:
                    {
                        offset.y = 0;
                        break;
                    }
                case ELimitAxisType.OnlyVertical:
                    {
                        offset.x = 0;
                        break;
                    }
            }
            transform.localPosition = originLocalPosition + offset;
        }

        /// <summary>
        /// 触摸抬起
        /// </summary>
        /// <param name="data"></param>
        public virtual void OnPointerUp(PointerEventData data)
        {
            isDrag = false;
            transform.localPosition = originLocalPosition;
            RunScriptEvent(EUGUIJoystickButtonScriptEventType.OnPointerUp);
        }

        /// <summary>
        /// 触摸按下
        /// </summary>
        /// <param name="data"></param>
        public virtual void OnPointerDown(PointerEventData data)
        {
            isDrag = true;
            originLocalPosition = transform.localPosition;
            originPosition = transform.position;
            originWorldMatrix = transform.localToWorldMatrix;
            RunScriptEvent(EUGUIJoystickButtonScriptEventType.OnPointerDown);
        }

        public virtual void OnDrawGizmosSelected()
        {
            // 设置矩阵
            Matrix4x4 defaultMatrix = Gizmos.matrix;
            if (Application.isPlaying) Gizmos.matrix = originWorldMatrix;
            else Gizmos.matrix = this.transform.localToWorldMatrix;

            // 设置颜色
            Color defaultColor = Gizmos.color;
            Gizmos.color = Color.green;

            switch (limitRangeType)
            {
                case ELimitRangeType.HalfLength:
                    {
                        Gizmos.DrawLine(new Vector3(movementRange, movementRange, 0), new Vector3(movementRange, -movementRange, 0));
                        Gizmos.DrawLine(new Vector3(movementRange, -movementRange, 0), new Vector3(-movementRange, -movementRange, 0));
                        Gizmos.DrawLine(new Vector3(-movementRange, -movementRange, 0), new Vector3(-movementRange, movementRange, 0));
                        Gizmos.DrawLine(new Vector3(-movementRange, movementRange, 0), new Vector3(movementRange, movementRange, 0));
                        break;
                    }
                case ELimitRangeType.Radius:
                default:
                    {
                        // 绘制圆环
                        Vector3 beginPoint = Vector3.zero;
                        Vector3 firstPoint = Vector3.zero;
                        for (float theta = 0; theta < 2 * Mathf.PI; theta += 0.0001f)
                        {
                            float x = movementRange * Mathf.Cos(theta);
                            float y = movementRange * Mathf.Sin(theta);
                            Vector3 endPoint = new Vector3(x, y, 0);
                            if (theta == 0)
                            {
                                firstPoint = endPoint;
                            }
                            else
                            {
                                Gizmos.DrawLine(beginPoint, endPoint);
                            }
                            beginPoint = endPoint;
                        }

                        // 绘制最后一条线段
                        Gizmos.DrawLine(firstPoint, beginPoint);
                        break;
                    }
            }
            switch (limitAxisType)
            {
                case ELimitAxisType.OnlyHorizontal:
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawLine(new Vector3(-movementRange, 0, 0), new Vector3(movementRange, 0, 0));
                        break;
                    }
                case ELimitAxisType.OnlyVertical:
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawLine(new Vector3(0, movementRange, 0), new Vector3(0, -movementRange, 0));
                        break;
                    }
            }

            // 恢复默认颜色
            Gizmos.color = defaultColor;

            // 恢复默认矩阵
            Gizmos.matrix = defaultMatrix;
        }
    }
}
