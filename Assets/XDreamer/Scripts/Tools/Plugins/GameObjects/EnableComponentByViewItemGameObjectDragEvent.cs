using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.Scripts;

namespace XCSJ.PluginTools.GameObjects
{
    /// <summary>
    /// 启用禁用组件通过视图项游戏对象拖拽事件
    /// </summary>
    [Name("启用禁用组件通过视图项游戏对象拖拽事件")]
    [RequireManager(typeof(ToolsManager))]
    [Tool("游戏对象", rootType = typeof(ToolsManager))]
    [XCSJ.Attributes.Icon(EIcon.Put)]
    public class EnableComponentByViewItemGameObjectDragEvent : MB
    {
        /// <summary>
        /// 视图项游戏对象摆放组件
        /// </summary>
        [Name("视图项游戏对象摆放组件")]
        public ViewItemGameObjectPut _viewItemGameObjectPut;

        /// <summary>
        /// 视图项游戏对象摆放组件
        /// </summary>
        public ViewItemGameObjectPut viewItemGameObjectPut => this.XGetComponentInGlobal(ref _viewItemGameObjectPut);

        /// <summary>
        /// 启用组件信息列表
        /// </summary>
        [Serializable]
        public class EnableComponentInfoList : EnableComponentInfoList<EViewItemGameObjectPutEvent> { }

        /// <summary>
        /// 启用组件事件集合
        /// </summary>
        public class EnableComponentEventSet : EnableComponentEventSet<EViewItemGameObjectPutEvent, EnableComponentInfoList> { }

        /// <summary>
        /// 启用组件事件集合
        /// </summary>
        [Name("启用组件事件集合")]
        public EnableComponentEventSet _enableComponentEventSet = new EnableComponentEventSet();

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            if (viewItemGameObjectPut) { }
        }

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            if (viewItemGameObjectPut) { }
            ViewItemGameObjectPut.onDragEvent += OnDragEvent;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();

            ViewItemGameObjectPut.onDragEvent -= OnDragEvent;
        }

        /// <summary>
        /// 拖拽事件回调函数
        /// </summary>
        /// <param name="viewItemGameObjectPut"></param>
        /// <param name="dragEvent"></param>
        private void OnDragEvent(ViewItemGameObjectPut viewItemGameObjectPut, DragGameObjectViewItemEventArgs dragGameObjectViewItemData)
        {
            if (_viewItemGameObjectPut != viewItemGameObjectPut) return;

            _enableComponentEventSet.Enable(dragGameObjectViewItemData.viewItemGameObjectPutEvent);
        }
    }
}
