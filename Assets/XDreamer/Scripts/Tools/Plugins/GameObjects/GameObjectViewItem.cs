using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Views.ScrollViews;

namespace XCSJ.PluginTools.GameObjects
{
    /// <summary>
    /// 游戏对象视图项
    /// </summary>
    [Name("游戏对象视图项")]
    [RequireManager(typeof(ToolsManager))]
    public class GameObjectViewItem : ViewItem
    {
        /// <summary>
        /// 游戏对象数量
        /// </summary>
        [Name("数量文本")]
        public Text _count = null;

        /// <summary>
        /// 游戏对象数据
        /// </summary>
        protected IGameObjectViewItemData gameObjectData => _data as IGameObjectViewItemData;

        /// <summary>
        /// 有效性
        /// </summary>
        public override bool valid => gameObjectData != null ? gameObjectData.count > 0 : base.valid;

        /// <summary>
        /// 开始拖拽
        /// </summary>
        public static Action<PointerEventData, IGameObjectViewItemData> onBeginDrag = null;

        /// <summary>
        /// 拖拽进行中
        /// </summary>
        public static Action<PointerEventData, IGameObjectViewItemData> onDraging = null;

        /// <summary>
        /// 结束拖拽
        /// </summary>
        public static Action<PointerEventData, IGameObjectViewItemData> onEndDrag = null;

        /// <summary>
        /// 拖拽状态
        /// </summary>
        private EDragState dragState = EDragState.None;

        private enum EDragState
        {
            None,

            BeginDrag,

            Draging,

            EndDrag,
        }

        /// <summary>
        /// 指针按下
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            if (gameObjectData == null || !gameObjectData.prototype || !gameObjectData.enable) return;

            // 保证成对执行
            if (dragState == EDragState.Draging)
            {
                onEndDrag?.Invoke(eventData, gameObjectData);

                CommonFun.EndOnUI(); 
            }
            dragState = EDragState.BeginDrag;

            CommonFun.BeginOnUI();

            onBeginDrag?.Invoke(eventData, gameObjectData);
        }

        /// <summary>
        /// 拖拽
        /// </summary>
        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);

            if (dragState == EDragState.BeginDrag)
            {
                dragState = EDragState.Draging;
            }

            if (dragState == EDragState.Draging)
            {
                onDraging?.Invoke(eventData, gameObjectData);
            }
        }

        /// <summary>
        /// 弹起
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            if (dragState == EDragState.BeginDrag || dragState == EDragState.Draging)
            {
                dragState = EDragState.EndDrag;

                onEndDrag?.Invoke(eventData, gameObjectData);

                CommonFun.EndOnUI();
            }
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            if (_count && gameObjectData != null)
            {
                _count.text = gameObjectData.count.ToString();
            }
        }
    }
}
