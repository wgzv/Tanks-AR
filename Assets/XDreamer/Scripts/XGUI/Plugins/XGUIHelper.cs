﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginXGUI
{
    /// <summary>
    /// XGUI助手
    /// </summary>
    public static class XGUIHelper
    {
        #region 设置锚点与轴心

        /// <summary>
        /// 设置轴心点
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="nineDirection"></param>
        public static void SetPivot(this RectTransform rectTransform, ENineDirection nineDirection)
        {
            switch (nineDirection)
            {
                case ENineDirection.LeftTop:
                    {
                        rectTransform.pivot = new Vector2(0, 1);
                        break;
                    }
                case ENineDirection.LeftMiddle:
                    {
                        rectTransform.pivot = new Vector2(0, 0.5f);
                        break;
                    }
                case ENineDirection.LeftBottom:
                    {
                        rectTransform.pivot = new Vector2(0, 0);
                        break;
                    }
                case ENineDirection.MiddleTop:
                    {
                        rectTransform.pivot = new Vector2(0.5f, 1);
                        break;
                    }
                case ENineDirection.Center:
                    {
                        rectTransform.pivot = new Vector2(0.5f, 0.5f);
                        break;
                    }
                case ENineDirection.MiddleBottom:
                    {
                        rectTransform.pivot = new Vector2(0.5f, 0);
                        break;
                    }
                case ENineDirection.RightTop:
                    {
                        rectTransform.pivot = new Vector2(1, 1);
                        break;
                    }
                case ENineDirection.RightMiddle:
                    {
                        rectTransform.pivot = new Vector2(1, 0.5f);
                        break;
                    }
                case ENineDirection.RightBottom:
                    {
                        rectTransform.pivot = new Vector2(1, 0);
                        break;
                    }
            }
        }

        /// <summary>
        /// 使用4个方位类型，设置最小和最大锚点方位
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="fourDirection"></param>
        public static void SetMinMaxAnchored(this RectTransform rectTransform, EFourDirection fourDirection)
        {
            switch (fourDirection)
            {
                case EFourDirection.Top:
                    {
                        rectTransform.anchorMin = new Vector2(0, 1);
                        rectTransform.anchorMax = Vector2.one;
                        break;
                    }
                case EFourDirection.Bottom:
                    {
                        rectTransform.anchorMin = Vector2.zero;
                        rectTransform.anchorMax = new Vector2(1, 0);
                        break;
                    }
                case EFourDirection.Left:
                    {
                        rectTransform.anchorMin = Vector2.zero;
                        rectTransform.anchorMax = new Vector2(0, 1);
                        break;
                    }
                case EFourDirection.Right:
                    {
                        rectTransform.anchorMin = new Vector2(1, 0);
                        rectTransform.anchorMax = Vector2.one;
                        break;
                    }
            }
        }

        /// <summary>
        /// 使用4个方位类型，设置最小和最大锚点方位，并通过传入尺寸对齐rectTransform的位置
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="fourDirection"></param>
        public static void SetMinMaxAnchoredAndFitPosition(this RectTransform rectTransform, EFourDirection fourDirection, Vector2 size)
        {
            rectTransform.sizeDelta = size;
            switch (fourDirection)
            {
                case EFourDirection.Top:
                    {
                        rectTransform.anchorMin = new Vector2(0, 1);
                        rectTransform.anchorMax = Vector2.one;
                        rectTransform.anchoredPosition = new UnityEngine.Vector2(0, -size.y / 2);
                        break;
                    }
                case EFourDirection.Bottom:
                    {
                        rectTransform.anchorMin = Vector2.zero;
                        rectTransform.anchorMax = new Vector2(1, 0);
                        rectTransform.anchoredPosition = new UnityEngine.Vector2(0, size.y / 2);
                        break;
                    }
                case EFourDirection.Left:
                    {
                        rectTransform.anchorMin = Vector2.zero;
                        rectTransform.anchorMax = new Vector2(0, 1);
                        rectTransform.anchoredPosition = new UnityEngine.Vector2(size.x / 2, 0);
                        break;
                    }
                case EFourDirection.Right:
                    {
                        rectTransform.anchorMin = new Vector2(1, 0);
                        rectTransform.anchorMax = Vector2.one;
                        rectTransform.anchoredPosition = new UnityEngine.Vector2(-size.x / 2, 0);
                        break;
                    }
            }
        }

        /// <summary>
        /// 使用9个方位类型，设置最小和最大锚点方位
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="nineDirection"></param>
        public static void SetMinMaxAnchored(this RectTransform rectTransform, ENineDirection nineDirection)
        {
            switch (nineDirection)
            {
                case ENineDirection.LeftTop:
                    {
                        rectTransform.anchorMin = new Vector2(0, 1);
                        rectTransform.anchorMax = new Vector2(0, 1);
                        break;
                    }
                case ENineDirection.LeftMiddle:
                    {
                        rectTransform.anchorMin = new Vector2(0, 0.5f);
                        rectTransform.anchorMax = new Vector2(0, 0.5f);
                        break;
                    }
                case ENineDirection.LeftBottom:
                    {
                        rectTransform.anchorMin = new Vector2(0, 0);
                        rectTransform.anchorMax = new Vector2(0, 0);
                        break;
                    }
                case ENineDirection.MiddleTop:
                    {
                        rectTransform.anchorMin = new Vector2(0.5f, 1);
                        rectTransform.anchorMax = new Vector2(0.5f, 1);
                        break;
                    }
                case ENineDirection.Center:
                    {
                        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                        rectTransform.anchoredPosition = new Vector2(0, 0);
                        break;
                    }
                case ENineDirection.MiddleBottom:
                    {
                        rectTransform.anchorMin = new Vector2(0.5f, 0);
                        rectTransform.anchorMax = new Vector2(0.5f, 0);
                        break;
                    }
                case ENineDirection.RightTop:
                    {
                        rectTransform.anchorMin = new Vector2(1, 1);
                        rectTransform.anchorMax = new Vector2(1, 1);
                        break;
                    }
                case ENineDirection.RightMiddle:
                    {
                        rectTransform.anchorMin = new Vector2(1, 0.5f);
                        rectTransform.anchorMax = new Vector2(1, 0.5f);
                        break;
                    }
                case ENineDirection.RightBottom:
                    {
                        rectTransform.anchorMin = new Vector2(1, 0);
                        rectTransform.anchorMax = new Vector2(1, 0);
                        break;
                    }
            }
        }

        public static void SetMinMaxAnchoredAndFitPosition(this RectTransform rectTransform, EFourDirection fourDirection, Vector2 size, EHorizontalPosition horizontalPosition)
        {
            switch (fourDirection)
            {
                case EFourDirection.Top:
                    {
                        switch (horizontalPosition)
                        {
                            case EHorizontalPosition.Left:
                                {
                                    rectTransform.SetMinMaxAnchoredAndFitPosition(ENineDirection.LeftTop, size);
                                    break;
                                }
                            case EHorizontalPosition.Middle:
                                {
                                    rectTransform.SetMinMaxAnchoredAndFitPosition(ENineDirection.MiddleTop, size);
                                    break;
                                }
                            case EHorizontalPosition.Right:
                                {
                                    rectTransform.SetMinMaxAnchoredAndFitPosition(ENineDirection.RightTop, size);
                                    break;
                                }
                        }
                        break;
                    }
                case EFourDirection.Bottom:
                    {
                        switch (horizontalPosition)
                        {
                            case EHorizontalPosition.Left:
                                {
                                    rectTransform.SetMinMaxAnchoredAndFitPosition(ENineDirection.LeftBottom, size);
                                    break;
                                }
                            case EHorizontalPosition.Middle:
                                {
                                    rectTransform.SetMinMaxAnchoredAndFitPosition(ENineDirection.MiddleBottom, size);
                                    break;
                                }
                            case EHorizontalPosition.Right:
                                {
                                    rectTransform.SetMinMaxAnchoredAndFitPosition(ENineDirection.RightBottom, size);
                                    break;
                                }
                        }
                        break;
                    }
            }
        }

        public static void SetMinMaxAnchoredAndFitPosition(this RectTransform rectTransform, EFourDirection fourDirection, Vector2 size, EVerticalPosition verticalPosition)
        {
            switch (fourDirection)
            {
                case EFourDirection.Left:
                    {
                        switch (verticalPosition)
                        {
                            case EVerticalPosition.Top:
                                {
                                    rectTransform.SetMinMaxAnchoredAndFitPosition(ENineDirection.LeftTop, size);
                                    break;
                                }
                            case EVerticalPosition.Middle:
                                {
                                    rectTransform.SetMinMaxAnchoredAndFitPosition(ENineDirection.LeftMiddle, size);
                                    break;
                                }
                            case EVerticalPosition.Bottom:
                                {
                                    rectTransform.SetMinMaxAnchoredAndFitPosition(ENineDirection.LeftBottom, size);
                                    break;
                                }
                        }
                        break;
                    }
                case EFourDirection.Right:
                    {
                        switch (verticalPosition)
                        {
                            case EVerticalPosition.Top:
                                {
                                    rectTransform.SetMinMaxAnchoredAndFitPosition(ENineDirection.RightTop, size);
                                    break;
                                }
                            case EVerticalPosition.Middle:
                                {
                                    rectTransform.SetMinMaxAnchoredAndFitPosition(ENineDirection.RightMiddle, size);
                                    break;
                                }
                            case EVerticalPosition.Bottom:
                                {
                                    rectTransform.SetMinMaxAnchoredAndFitPosition(ENineDirection.RightBottom, size);
                                    break;
                                }
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 使用9个方位类型，设置最小和最大锚点，并设定尺寸对齐rectTransform的位置
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="nineDirection"></param>
        public static void SetMinMaxAnchoredAndFitPosition(this RectTransform rectTransform, ENineDirection nineDirection, Vector2 size)
        {
            rectTransform.sizeDelta = size;
            switch (nineDirection)
            {
                case ENineDirection.LeftTop:
                    {
                        rectTransform.anchorMin = new Vector2(0, 1);
                        rectTransform.anchorMax = new Vector2(0, 1);
                        rectTransform.anchoredPosition = new Vector2(size.x / 2, -size.y / 2);
                        break;
                    }
                case ENineDirection.LeftMiddle:
                    {
                        rectTransform.anchorMin = new Vector2(0, 0.5f);
                        rectTransform.anchorMax = new Vector2(0, 0.5f);
                        rectTransform.anchoredPosition = new Vector2(size.x / 2, 0);
                        break;
                    }
                case ENineDirection.LeftBottom:
                    {
                        rectTransform.anchorMin = new Vector2(0, 0);
                        rectTransform.anchorMax = new Vector2(0, 0);
                        rectTransform.anchoredPosition = new Vector2(size.x / 2, size.y / 2);
                        break;
                    }
                case ENineDirection.MiddleTop:
                    {
                        rectTransform.anchorMin = new Vector2(0.5f, 1);
                        rectTransform.anchorMax = new Vector2(0.5f, 1);
                        rectTransform.anchoredPosition = new Vector2(0, -size.y / 2);
                        break;
                    }
                case ENineDirection.Center:
                    {
                        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                        rectTransform.anchoredPosition = new Vector2(0, 0);
                        break;
                    }
                case ENineDirection.MiddleBottom:
                    {
                        rectTransform.anchorMin = new Vector2(0.5f, 0);
                        rectTransform.anchorMax = new Vector2(0.5f, 0);
                        rectTransform.anchoredPosition = new Vector2(0, size.y / 2);
                        break;
                    }
                case ENineDirection.RightTop:
                    {
                        rectTransform.anchorMin = new Vector2(1, 1);
                        rectTransform.anchorMax = new Vector2(1, 1);
                        rectTransform.anchoredPosition = new Vector2(-size.x / 2, -size.y / 2);
                        break;
                    }
                case ENineDirection.RightMiddle:
                    {
                        rectTransform.anchorMin = new Vector2(1, 0.5f);
                        rectTransform.anchorMax = new Vector2(1, 0.5f);
                        rectTransform.anchoredPosition = new Vector2(-size.x / 2, 0);
                        break;
                    }
                case ENineDirection.RightBottom:
                    {
                        rectTransform.anchorMin = new Vector2(1, 0);
                        rectTransform.anchorMax = new Vector2(1, 0);
                        rectTransform.anchoredPosition = new Vector2(-size.x / 2, size.y / 2);
                        break;
                    }
            }
        }

        /// <summary>
        /// 设置全屏
        /// </summary>
        /// <param name="rectTransform"></param>
        public static void SetFullScreen(this RectTransform rectTransform)
        {
            rectTransform.XSetAnchorMin(Vector2.zero);
            rectTransform.XSetAnchorMax(Vector2.one);
            rectTransform.XSetOffsetMin(Vector2.zero);
            rectTransform.XSetOffsetMax(Vector2.zero);
        }

        #endregion

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="selectable"></param>
        /// <param name="color"></param>
        public static void SetColor(this Selectable selectable, Color color)
        {
            if (selectable)
            {
                ColorBlock selectCB = selectable.colors;
                selectCB.normalColor = color;
                selectCB.highlightedColor = color;
                selectable.colors = selectCB;
            }
        }

        /// <summary>
        /// 贴图2D 转 精灵
        /// </summary>
        /// <param name="texture2D"></param>
        /// <returns></returns>
        public static Sprite Texture2DToSprite(Texture2D texture2D)
        {
            if (!texture2D) return null;
            return Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
        }

        /// <summary>
        /// 渲染贴图 转 贴图2D
        /// </summary>
        /// <param name="renderTexture">渲染图</param>
        /// <returns>2D贴图</returns>
        public static Texture2D RenderTextureToTexture2D(this RenderTexture renderTexture)
        {
            if (!renderTexture) return null;

            var orgActive = RenderTexture.active;
            try
            {
                var texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
                RenderTexture.active = renderTexture;
                texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                texture2D.Apply();
                return texture2D;
            }
            finally
            {
                RenderTexture.active = orgActive;
            }
        }

        /// <summary>
        /// 渲染贴图 转 精灵
        /// </summary>
        /// <param name="renderTexture"></param>
        /// <returns></returns>
        public static Sprite RenderTextureToSprite(this RenderTexture renderTexture)
        {
            return Texture2DToSprite(RenderTextureToTexture2D(renderTexture));
        }

        /// <summary>
        /// 查找画布下的父节点
        /// </summary>
        /// <param name="needCanvasIsRoot">画布为根画布：其父节点之上再无画布</param>
        /// <param name="includeSelf">包含自身</param>
        /// <returns></returns>
        public static List<Transform> GetParentUnderCanvas(this Transform transform, bool needCanvasIsRoot, bool includeSelf = false)
        {
            var parents = new List<Transform>();
            var current = transform.parent;
            while (current)
            {
                parents.Add(current);
                current = current.parent;
            }

            // 上诉查找结果是从子级到父级的顺序
            var rs = new List<Transform>();
            if (includeSelf) rs.Add(transform);
            var index = needCanvasIsRoot ? parents.FindLastIndex(t => t.GetComponent<Canvas>()) : parents.FindIndex(t => t.GetComponent<Canvas>());
            if (index >= 1)
            {
                rs.AddRange(parents.GetRange(0, index));
            }
            return rs;
        }

        public static void SetSiblingIndex(this Transform transform, ESiblingIndexRule siblingIndexRule)
        {
            switch (siblingIndexRule)
            {
                case ESiblingIndexRule.Last:
                    {
                        transform.SetAsLastSibling();
                        break;
                    }
                case ESiblingIndexRule.Last_CurrentInclude_ParentCanvasNotInclude:
                    {
                        transform.GetParentUnderCanvas(false, true).ForEach(t => t.SetAsLastSibling());
                        break;
                    }
                case ESiblingIndexRule.Last_CurrentInclude_RootCanvasNotInclude:
                    {
                        transform.GetParentUnderCanvas(true, true).ForEach(t => t.SetAsLastSibling());
                        break;
                    }
                case ESiblingIndexRule.Last_CurrentInclude_RootGameObjectInclude:
                    {
                        CommonFun.GetParentsGameObject(transform, true).ForEach(go => go.transform.SetAsLastSibling());
                        break;
                    }
                case ESiblingIndexRule.First:
                    {
                        transform.SetAsFirstSibling();
                        break;
                    }
                case ESiblingIndexRule.First_CurrentInclude_ParentCanvasNotInclude:
                    {
                        transform.GetParentUnderCanvas(false, true).ForEach(t => t.SetAsFirstSibling());
                        break;
                    }
                case ESiblingIndexRule.First_CurrentInclude_RootCanvasNotInclude:
                    {
                        transform.GetParentUnderCanvas(true, true).ForEach(t => t.SetAsFirstSibling());
                        break;
                    }
                case ESiblingIndexRule.First_CurrentInclude_RootGameObjectInclude:
                    {
                        CommonFun.GetParentsGameObject(transform, true).ForEach(go => go.transform.SetAsFirstSibling());
                        break;
                    }
            }
        }

        /// <summary>
        /// 获取rect在屏幕坐标系下的矩形
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <returns></returns>
        public static Rect GetScreenRect(this RectTransform rectTransform)
        {
            var world = new Vector3[4];
            rectTransform.GetWorldCorners(world);
            var min = Vector3.Min(Vector3.Min(Vector3.Min(world[0], world[1]), world[2]), world[3]);
            var max = Vector3.Max(Vector3.Max(Vector3.Max(world[0], world[1]), world[2]), world[3]);
            return new Rect(min.x, max.y, max.x - min.x, max.y - min.y);
        }

        public static void GetScrollBarRange(int itemIndex, int totalCount, float viewHeight, float cellHeight, float cellSpaceHeight,
            out float topValue, out float bottomValue)
        {
            topValue = GetScrollbarValueOnTop(itemIndex, totalCount, viewHeight, cellHeight, cellSpaceHeight);
            bottomValue = GetScrollbarValueOnBottom(itemIndex, totalCount, viewHeight, cellHeight, cellSpaceHeight);
        }

        /// <summary>
        /// 获取项在视图顶部时候的滚动条值
        /// </summary>
        /// <param name="itemIndex">项索引</param>
        /// <param name="totalCount">总数</param>
        /// <param name="viewHeight">视图高度</param>
        /// <param name="cellHeight">单元格高度</param>
        /// <param name="cellSpaceHeight">单元格间隔高度</param>
        /// <returns>滚动条值</returns>
        public static float GetScrollbarValueOnTop(int itemIndex, int totalCount, float viewHeight, float cellHeight, float cellSpaceHeight = 0)
        {
            float totalHeight = GetItemHeight(totalCount, cellHeight, cellSpaceHeight);
            // 滑动条可滚动的总量
            float scrollHeight = totalHeight - viewHeight;
            if (MathX.ApproximatelyZero(scrollHeight))
            {
                return 1;
            }

            // 项和空白数量一样
            float value = (cellSpaceHeight + GetItemHeight(itemIndex, cellHeight, cellSpaceHeight)) / scrollHeight;
            return Mathf.Clamp(value, 0, 1);
        }

        /// <summary>
        /// 获取项在视图底部时候的滚动条值
        /// </summary>
        /// <param name="itemIndex">项索引</param>
        /// <param name="totalCount">总数</param>
        /// <param name="viewHeight">视图高度</param>
        /// <param name="cellHeight">单元格高度</param>
        /// <param name="cellSpaceHeight">单元格间隔高度</param>
        /// <returns>滚动条值</returns>
        public static float GetScrollbarValueOnBottom(int itemIndex, int totalCount, float viewHeight, float cellHeight, float cellSpaceHeight = 0)
        {
            float totalHeight = GetItemHeight(totalCount, cellHeight, cellSpaceHeight);
            float scrollHeight = totalHeight - viewHeight;
            if (MathX.ApproximatelyZero(scrollHeight))
            {
                return 1;
            }

            // 项和空白数量一样
            float value = (cellSpaceHeight + GetItemHeight(itemIndex, cellHeight, cellSpaceHeight) - (viewHeight - cellHeight)) / scrollHeight;
            return Mathf.Clamp(value, 0, 1);
        }

        private static float GetItemHeight(int index, float cellHeight, float spaceHeight)
        {
            float itemHeight = index * cellHeight;
            if (index > 1)
            {
                itemHeight += spaceHeight * (index - 1);
            }
            return itemHeight;
        }

        public static void AddEventTrigger(EventTrigger trigger, EventTriggerType eventType, UnityAction<BaseEventData> callback)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = eventType
            };
            entry.callback.AddListener(callback);
            trigger.triggers.Add(entry);
        }

        public static EventTriggerType ToEventTriggerType(this EEventTriggerTypeLite eventTriggerTypeLite) => (EventTriggerType)eventTriggerTypeLite;
    }

    /// <summary>
    /// Event Trigger事件类型简版
    /// </summary>
    public enum EEventTriggerTypeLite
    {
        [Name("指针进入时 执行")]
        PointerEnter,

        [Name("指针离开时 执行")]
        PointerExit,

        [Name("指针按下时 执行")]
        PointerDown,

        [Name("指针抬起时 执行")]
        PointerUp,

        [Name("指针点击时 执行")]
        PointerClick,

        //[Name("拖拽时 执行")]
        //Drag,

        //[Name("指针点击时 执行")]
        //Drop,

        //[Name("滚动时 执行")]
        //Scroll,

        //[Name("更新选择时 执行")]
        //UpdateSelected,

        [Name("选择时 执行")]
        Select = EventTriggerType.Select,

        [Name("取消选择时 执行")]
        Deselect = EventTriggerType.Deselect,

        //[Name("移动时 执行")]
        //Move,

        //[Name("初始化潜在的拖拽时 执行")]
        //InitializePotentialDrag,

        [Name("开始拖拽时 执行")]
        BeginDrag = EventTriggerType.BeginDrag,

        [Name("结束拖拽时 执行")]
        EndDrag = EventTriggerType.EndDrag,

        //[Name("提交时 执行")]
        //Submit,

        //[Name("取消时 执行")]
        //Cancel,

        //[Name("启动时 执行")]
        //Start,
    }

    public enum ESiblingIndexRule
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 最后同级索引:将当前游戏对的同级索引设置为最后
        /// </summary>
        [Name("最后同级索引")]
        [Tip("将当前游戏对的同级索引设置为最后")]
        Last,

        /// <summary>
        /// 最后同级索引[从当前(含)到父级画布(不含)]:将从当前游戏对象(含)到父级画布游戏对象(不含)的同级索引设置为最后
        /// </summary>
        [Name("最后同级索引[从当前(含)到父级画布(不含)]")]
        [Tip("将从当前游戏对象(含)到父级画布游戏对象(不含)的同级索引设置为最后")]
        Last_CurrentInclude_ParentCanvasNotInclude,

        /// <summary>
        /// 最后同级索引[从当前(含)到根画布(不含)]:将从当前游戏对象(含)到根画布游戏对象(不含)的同级索引设置为最后
        /// </summary>
        [Name("最后同级索引[从当前(含)到根画布(不含)]")]
        [Tip("将从当前游戏对象(含)到根画布游戏对象(不含)的同级索引设置为最后")]
        Last_CurrentInclude_RootCanvasNotInclude,

        /// <summary>
        /// 最后同级索引[从当前(含)到根游戏对象(含)]：将从当前游戏对象(含)到根游戏对象(不含)的同级索引设置为最后
        /// </summary>
        [Name("最后同级索引[从当前(含)到根游戏对象(不含)]")]
        [Tip("将从当前游戏对象(含)到根游戏对象(不含)的同级索引设置为最后")]
        Last_CurrentInclude_RootGameObjectInclude,

        /// <summary>
        /// 最前同级索引：将当前游戏对的同级索引设置为最前
        /// </summary>
        [Name("最前同级索引")]
        [Tip("将当前游戏对的同级索引设置为最前")]
        First,

        /// <summary>
        /// 最前同级索引[从当前(含)到父级画布(不含)]:将从当前游戏对象(含)到父级画布游戏对象(不含)的同级索引设置为最前
        /// </summary>
        [Name("最前同级索引[从当前(含)到父级画布(不含)]")]
        [Tip("将从当前游戏对象(含)到父级画布游戏对象(不含)的同级索引设置为最前")]
        First_CurrentInclude_ParentCanvasNotInclude,

        /// <summary>
        /// 最前同级索引[从当前(含)到根画布(不含)]:将从当前游戏对象(含)到根画布游戏对象(不含)的同级索引设置为最前
        /// </summary>
        [Name("最前同级索引[从当前(含)到根画布(不含)]")]
        [Tip("将从当前游戏对象(含)到根画布游戏对象(不含)的同级索引设置为最前")]
        First_CurrentInclude_RootCanvasNotInclude,

        /// <summary>
        /// 最前同级索引[从当前(含)到根游戏对象(含)]：将从当前游戏对象(含)到根游戏对象(不含)的同级索引设置为最前
        /// </summary>
        [Name("最前同级索引[从当前(含)到根游戏对象(不含)]")]
        [Tip("将从当前游戏对象(含)到根游戏对象(不含)的同级索引设置为最前")]
        First_CurrentInclude_RootGameObjectInclude,
    }

    /// <summary>
    /// 标题水平对齐方位
    /// </summary>
    public enum EFourDirection
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 顶部
        /// </summary>
        [Name("顶部")]
        Top,

        /// <summary>
        /// 底部
        /// </summary>
        [Name("底部")]
        Bottom,

        /// <summary>
        /// 左侧
        /// </summary>
        [Name("左侧")]
        Left,

        /// <summary>
        /// 右侧
        /// </summary>
        [Name("右侧")]
        Right
    }

    /// <summary>
    /// 水平方位
    /// </summary>
    public enum EHorizontalPosition
    {
        /// <summary>
        /// 左
        /// </summary>
        [Name("左")]
        Left,

        /// <summary>
        /// 中
        /// </summary>
        [Name("中")]
        Middle,

        /// <summary>
        /// 右
        /// </summary>
        [Name("右")]
        Right,
    }

    /// <summary>
    /// 垂直方位
    /// </summary>
    public enum EVerticalPosition
    {
        /// <summary>
        /// 上
        /// </summary>
        [Name("上")]
        Top,

        /// <summary>
        /// 中
        /// </summary>
        [Name("中")]
        Middle,

        /// <summary>
        /// 下
        /// </summary>
        [Name("下")]
        Bottom,
    }

    /// <summary>
    /// 轴点类型
    /// </summary>
    public enum ENineDirection
    {
        /// <summary>
        /// 左上
        /// </summary>
        [Name("左上")]
        LeftTop,

        /// <summary>
        /// 左中
        /// </summary>
        [Name("左中")]
        LeftMiddle,

        /// <summary>
        /// 左下
        /// </summary>
        [Name("左下")]
        LeftBottom,

        /// <summary>
        /// 中上
        /// </summary>
        [Name("中上")]
        MiddleTop,

        /// <summary>
        /// 中心
        /// </summary>
        [Name("中心")]
        Center,

        /// <summary>
        /// 中下
        /// </summary>
        [Name("中下")]
        MiddleBottom,

        /// <summary>
        /// 右上
        /// </summary>
        [Name("右上")]
        RightTop,

        /// <summary>
        /// 右中
        /// </summary>
        [Name("右中")]
        RightMiddle,

        /// <summary>
        /// 右下
        /// </summary>
        [Name("右下")]
        RightBottom,
    }
}
