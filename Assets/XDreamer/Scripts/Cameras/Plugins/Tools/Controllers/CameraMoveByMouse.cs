﻿using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
using XCSJ.Tools;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginsCameras.Tools.Controllers
{
    /// <summary>
    /// 相机移动通过鼠标:默认通过鼠标在屏幕窗口中位置控制相机的移动
    /// </summary>
    [Name("相机移动通过鼠标")]
    [Tip("默认通过鼠标在屏幕窗口中位置控制相机的移动")]
    [Tool(CameraHelperExtension.ControllersCategoryName, /*nameof(CameraController),*/ nameof(CameraMover))]
    [XCSJ.Attributes.Icon(EIcon.Move)]
    public class CameraMoveByMouse : BaseCameraMoveController
    {
        /// <summary>
        /// 移动触发规则
        /// </summary>
        public enum EMoveTriggerRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 靠近屏幕边框：当鼠标靠近屏幕边框时，触发移动逻辑
            /// </summary>
            [Name("靠近屏幕边框")]
            [Tip("当鼠标靠近屏幕边框时，触发移动逻辑")]
            CloseToScreenBorder,
        }

        /// <summary>
        /// 移动触发规则
        /// </summary>
        [Name("移动触发规则")]
        [EnumPopup]
        public EMoveTriggerRule _moveTriggerRule = EMoveTriggerRule.CloseToScreenBorder;

        /// <summary>
        /// 移动方向类型
        /// </summary>
        [Name("移动方向类型")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_moveTriggerRule), EValidityCheckType.NotEqual, EMoveTriggerRule.CloseToScreenBorder)]
        public EMoveDirType _moveDirType = EMoveDirType.Dir8;

        /// <summary>
        /// 移动方向隐射规则
        /// </summary>
        public enum EMoveDirProjectRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 垂直隐射Z_水平隐射X：将相机前方向投影到世界X0Z平面得到隐射Z（前向量），使用隐射Z与世界上向量（Y）基于左手坐标系规则得到隐射X（右向量）；本隐射方式类似于Dota、红警等类型游戏的相机移动控制方式
            /// </summary>
            [Name("垂直隐射Z_水平隐射X")]
            [Tip("将相机前方向投影到世界X0Z平面得到隐射Z（前向量），使用隐射Z与世界上向量（Y）基于左手坐标系规则得到隐射X（右向量）；基于本隐射方式类似于Dota、红警等类型游戏的相机移动控制方式")]
            VerticalToProjectZ_HorizontalToProjectX,

            /// <summary>
            /// 垂直Z_水平X
            /// </summary>
            [Name("垂直Z_水平X")]
            VerticalToZ_HorizontalToX,
        }

        /// <summary>
        /// 移动方向隐射规则
        /// </summary>
        [Name("移动方向隐射规则")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_moveTriggerRule), EValidityCheckType.NotEqual, EMoveTriggerRule.CloseToScreenBorder)]
        public EMoveDirProjectRule _moveDirProjectRule = EMoveDirProjectRule.VerticalToProjectZ_HorizontalToProjectX;

        /// <summary>
        /// 内边框宽度
        /// </summary>
        [Name("内边框宽度")]
        [HideInSuperInspector(nameof(_moveTriggerRule), EValidityCheckType.NotEqual, EMoveTriggerRule.CloseToScreenBorder)]
        [Range(5, 100)]
        public float _innerBorderWidth = 20;

        /// <summary>
        /// 外边框宽度
        /// </summary>
        [Name("外边框宽度")]
        [HideInSuperInspector(nameof(_moveTriggerRule), EValidityCheckType.NotEqual, EMoveTriggerRule.CloseToScreenBorder)]
        [Range(5, 1000)]
        public float _outerBorderWidth = 20;

        /// <summary>
        /// 隐射移动
        /// </summary>
        /// <param name="moveValue">移动值：其中X标识水平（Horizontal）的移动量，Y标识垂直（Vertical）的移动量，Z恒定为0</param>
        private void ProjectMove(Vector3 moveValue)
        {
            switch (_moveDirProjectRule)
            {
                case EMoveDirProjectRule.VerticalToProjectZ_HorizontalToProjectX:
                    {
                        var forward = Vector3.ProjectOnPlane(cameraTransformer.forward, Vector3.up);//z
                        var right = Vector3.Cross(Vector3.up, forward);//x
                        var dir = forward.normalized * moveValue.y + right.normalized * moveValue.x;
                        _offset = Vector3.Scale(speedRealtime, dir.normalized);
                        Move();
                        break;
                    }
                case EMoveDirProjectRule.VerticalToZ_HorizontalToX:
                    {
                        _offset = Vector3.Scale(speedRealtime, new Vector3(moveValue.x, 0, moveValue.y));
                        Move();
                        break;
                    }
            }

        }

        private void HandleCloseToScreenBorder()
        {
            var width = Screen.width;
            var height = Screen.height;
            var mousePosition = Input.mousePosition;

            if (mousePosition.x < -_outerBorderWidth || mousePosition.x > width + _outerBorderWidth || mousePosition.y < -_outerBorderWidth || mousePosition.y > height + _outerBorderWidth)
            {
                //在外边框之外,不处理
                return;
            }

            switch (_moveDirType)
            {
                case EMoveDirType.Dir4:
                    {
                        if (HasMove4(width, height, mousePosition, _innerBorderWidth, _innerBorderWidth, _innerBorderWidth, _innerBorderWidth, out Vector3 moveValue))
                        {
                            ProjectMove(moveValue);
                        }
                        break;
                    }
                case EMoveDirType.Dir8:
                    {
                        if (HasMove8(width, height, mousePosition, _innerBorderWidth, _innerBorderWidth, _innerBorderWidth, _innerBorderWidth, out Vector3 moveValue))
                        {
                            ProjectMove(moveValue);
                        }
                        break;
                    }
                case EMoveDirType.DirAny:
                    {
                        if (HasMoveAny(width, height, mousePosition, _innerBorderWidth, _innerBorderWidth, _innerBorderWidth, _innerBorderWidth, out Vector3 moveValue))
                        {
                            ProjectMove(moveValue);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public override void Update()
        {
            base.Update();

            switch (_moveTriggerRule)
            {
                case EMoveTriggerRule.CloseToScreenBorder:
                    {
                        HandleCloseToScreenBorder();
                        break;
                    }
            }
        }

        /// <summary>
        /// 绘制边框:仅在编辑器中生效
        /// </summary>
        [Name("绘制边框")]
        [Tip("仅在编辑器中生效")]
        public bool drawBorder = true;

#if UNITY_EDITOR

        private void OnGUI()
        {
            if (!drawBorder) return;
            switch (_moveTriggerRule)
            {
                case EMoveTriggerRule.CloseToScreenBorder:
                    {
                        var width = Screen.width;
                        var height = Screen.height;
                        GUI.Box(new Rect(0, 0, width, _innerBorderWidth), GUIContent.none);//上
                        GUI.Box(new Rect(0, 0, _innerBorderWidth, height), GUIContent.none);//左
                        GUI.Box(new Rect(0, height - _innerBorderWidth, width, _innerBorderWidth), GUIContent.none);//下
                        GUI.Box(new Rect(width - _innerBorderWidth, 0, _innerBorderWidth, height), GUIContent.none);//右
                        break;
                    }
            }
        }

#endif

        /// <summary>
        /// 移动方向类型
        /// </summary>
        public enum EMoveDirType
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 4方向
            /// </summary>
            [Name("4方向")]
            Dir4,

            /// <summary>
            /// 8方向
            /// </summary>
            [Name("8方向")]
            Dir8,

            /// <summary>
            /// 任意方向
            /// </summary>
            [Name("任意方向")]
            DirAny,
        }

        /// <summary>
        /// 是否有任意方向的移动
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="position"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="moveValue">移动值：其中X标识水平（Horizontal）的移动量（范围[-1,1]），Y标识垂直（Vertical）的移动量（范围[-1,1]），Z恒定为0</param>
        /// <returns></returns>
        private bool HasMoveAny(float width, float height, Vector2 position, float left, float right, float top, float bottom, out Vector3 moveValue)
        {
            var x = position.x;
            var y = position.y;
            moveValue = default;
            var dir = new Vector3(x, y) - new Vector3(width / 2, height / 2);
            if (x > left && x < width - right && y > bottom && y < height - top)
            {
                return false;
            }
            moveValue = dir.normalized;
            return true;
        }

        /// <summary>
        /// 是否有4方向的移动
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="position"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="moveValue">移动值：其中X标识水平（Horizontal）的移动量（范围[-1,1]），Y标识垂直（Vertical）的移动量（范围[-1,1]），Z恒定为0</param>
        /// <returns></returns>
        private bool HasMove4(float width, float height, Vector2 position, float left, float right, float top, float bottom, out Vector3 moveValue)
        {
            var x = position.x;
            var y = position.y;
            moveValue = default;

            if (x <= left)//147
            {
                if (y <= bottom)//7
                {
                    if (Mathf.Abs(x - left) >= Mathf.Abs(y - bottom))//7->4
                    {
                        moveValue = new Vector3(-1, 0);
                    }
                    else//7->8
                    {
                        moveValue = new Vector3(0, -1);
                    }
                }
                else if (y >= height - top)//1
                {
                    if (Mathf.Abs(x - left) >= Mathf.Abs(y - height + top))//1->4
                    {
                        moveValue = new Vector3(-1, 0);
                    }
                    else//1->2
                    {
                        moveValue = new Vector3(0, 1);
                    }
                }
                else//4
                {
                    moveValue = new Vector3(-1, 0, 0);
                }
                return true;
            }
            else if (x >= width - right)//369
            {
                if (y <= bottom)//9
                {
                    if (Mathf.Abs(x - width + left) >= Mathf.Abs(y - bottom))//9->6
                    {
                        moveValue = new Vector3(1, 0);
                    }
                    else//9->8
                    {
                        moveValue = new Vector3(0, -1);
                    }
                }
                else if (y >= height - top)//3
                {
                    if (Mathf.Abs(x - width + left) >= Mathf.Abs(y - height + top))//3->6
                    {
                        moveValue = new Vector3(1, 0);
                    }
                    else//3->2
                    {
                        moveValue = new Vector3(0, 1);
                    }
                }
                else//6
                {
                    moveValue = new Vector3(1, 0);
                }
                return true;
            }
            else
            {
                if (y <= bottom)//8
                {
                    moveValue = new Vector3(0, -1);
                    return true;
                }
                else if (y >= height - top)//2
                {
                    moveValue = new Vector3(0, 1);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否有8方向的移动
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="position"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="moveValue">移动值：其中X标识水平（Horizontal）的移动量（范围[-1,1]），Y标识垂直（Vertical）的移动量（范围[-1,1]），Z恒定为0</param>
        /// <returns></returns>
        private bool HasMove8(float width, float height, Vector2 position, float left, float right, float top, float bottom, out Vector3 moveValue)
        {
            var x = position.x;
            var y = position.y;
            moveValue = default;

            if (x <= left)//147
            {
                if (y <= bottom)//7
                {
                    moveValue = new Vector3(-1, -1);
                }
                else if (y >= height - top)//1
                {
                    moveValue = new Vector3(-1, 1);
                }
                else//4
                {
                    moveValue = new Vector3(-1, 0);
                }
                return true;
            }
            else if (x >= width - right)//369
            {
                if (y <= bottom)//9
                {
                    moveValue = new Vector3(1, -1);
                }
                else if (y >= height - top)//3
                {
                    moveValue = new Vector3(1, 1);
                }
                else//6
                {
                    moveValue = new Vector3(1, 0);
                }
                return true;
            }
            else
            {
                if (y <= bottom)//8
                {
                    moveValue = new Vector3(0, -1);
                    return true;
                }
                else if (y >= height - top)//2
                {
                    moveValue = new Vector3(0, 1);
                    return true;
                }
            }
            return false;
        }
    }
}
