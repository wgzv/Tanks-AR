using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginXGUI.Views.ScrollViews
{
    /// <summary>
    /// 视图项数据MB模版
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ViewItemDataMB<T> : MB where T : IViewItemData
    {
        /// <summary>
        /// 视图数据
        /// </summary>
        public abstract T data { get; }

        /// <summary>
        /// 所属视图数据集合
        /// </summary>
        [Name("所属视图数据集合")]
        [Tip("当对象为空时，会在场景中查找第一个视图数据集合进行数据添加")]
        public ViewItemDataCollectionMB _viewItemDataCollection = null;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            InitViewItemDataCollection();

            if (_viewItemDataCollection)
            {
                _viewItemDataCollection.AddData(data);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            if (_viewItemDataCollection)
            {
                _viewItemDataCollection.RemoveData(data);
            }
        }

        /// <summary>
        /// 所属视图数据集合
        /// </summary>
        private void InitViewItemDataCollection()
        {
            if (!_viewItemDataCollection)
            {
                var collection = FindObjectsOfType<ViewItemDataCollectionMB>();
                _viewItemDataCollection = collection.Find(c => c.gameObject.activeInHierarchy);
                if (!_viewItemDataCollection)
                {
                    _viewItemDataCollection = collection.First();
                }
            }
        }
    }
}
