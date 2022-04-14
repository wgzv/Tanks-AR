using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Extension.Base.Dataflows.DataBinders;
using XCSJ.Extension.Base.Dataflows.Links;
using XCSJ.Extension.Base.Dataflows.Models;
using XCSJ.PluginDataflows.Models;

namespace XCSJ.PluginDataflows.Binders
{
    /// <summary>
    /// 视图工厂
    /// 用于创建、释放和更新视图
    /// </summary>
    public interface IViewFactory
    {
        /// <summary>
        /// 使用数据创建视图
        /// </summary>
        /// <param name="data)"></param>
        /// <returns></returns>
        GameObject CreateView(object data);

        /// <summary>
        /// 释放视图
        /// </summary>
        /// <param name="view"></param>
        void DestoryView(GameObject view);

        /// <summary>
        /// 更新视图
        /// </summary>
        void UpdateView();
    }

    /// <summary>
    /// 数据集合链类
    /// 建立源集合与集合视图的对应关系
    /// 当前类对象就是目标对象，数据为单向 从源 -> 目标，
    /// </summary>
    [DataBinder(typeof(CollectionDataAdapter), nameof(CollectionDataAdapter.collection))]
    public class CollectionDataBinder : TypeMemberDataBinder<CollectionDataAdapter>
    {
        /// <summary>
        /// 视图工厂
        /// </summary>
        protected IViewFactory viewFactory => target;

        /// <summary>
        /// 解绑
        /// </summary>
        public override void Unbind(Action<XValueEventArg> onReceiveEvent)
        {
            base.Unbind(onReceiveEvent);

            OnClear();
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="eventArg"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TryGet(XValueEventArg eventArg, out object value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// 数据源属性为集合的对象
        /// </summary>
        protected IEnumerable collection = null;

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="linkedBindData"></param>
        /// <param name="eventArg"></param>
        public override void Set(ITypeMemberDataBinder linkedBindData, XValueEventArg eventArg)
        {
            collection = linkedBindData.typeMemberBinder.memberValue as IEnumerable;

            if (eventArg.eventType == EDefaultEventType.Init && eventArg.hasValue)
            {
                var v = eventArg.value as IEnumerable;
                if (v != null)
                {
                    Init(v);
                }
            }

            if (eventArg is XCollectionChangeEventArgs collectionEventArg)
            {
                switch (collectionEventArg.collectionAction)
                {
                    case ECollectionAction.Init: Init(collectionEventArg.newItems); break;
                    case ECollectionAction.Add: OnAdd(collectionEventArg.newItems); break;
                    case ECollectionAction.Remove: OnRemove(collectionEventArg.oldItems); break;
                    case ECollectionAction.Replace: OnReplace(collectionEventArg.oldItems, collectionEventArg.newItems); break;
                    case ECollectionAction.Sort: OnSort(collectionEventArg.newItems); break;
                    case ECollectionAction.Clear: OnClear(); break;
                    case ECollectionAction.None:
                    default: break;
                }
            }
        }

        #region 集合视图操作

        /// <summary>
        /// 数据-视图缓存
        /// 参数1 = 集合元素对象，
        /// 参数2 =  2.1 Unity游戏对象（游戏对象组件，由工厂创建）
        ///         2.2 数据链列表（当前视图游戏对象上所有的数据绑定列表）
        /// </summary>
        protected Dictionary<object, ViewData> viewDataDic = new Dictionary<object, ViewData>();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="newItems"></param>
        protected void Init(IEnumerable newItems)
        {
            OnClear();
            OnAdd(newItems);
        }

        /// <summary>
        /// 新项
        /// </summary>
        /// <param name="newItem"></param>
        protected void OnAdd(IEnumerable newItems)
        {
            if (viewFactory == null) return;

            foreach (var data in newItems)
            {
                if (data == null || viewDataDic.ContainsKey(data)) return;

                var dataType = data.GetType();
                var view = viewFactory.CreateView(data);
                if (view && view.GetComponent<IDataLinkSet>() is IDataLinkSet dls)
                {
                    // 设置数据-视图缓存
                    viewDataDic.Add(data, new ViewData(view, dls, viewFactory));

                    // 将数据对象设置给主对象，并主动刷新数据
                    foreach (var datalink in dls.links)
                    {
                        datalink.SetDataMainObject(data);
                    }
                }
            }

            //依据链表顺序重新刷新
            if (collection != null)
            {
                OnSort(collection);
            }
        }

        /// <summary>
        /// 批量移除项
        /// </summary>
        /// <param name="removeItems"></param>
        protected void OnRemove(IEnumerable removeItems)
        {
            foreach (var item in removeItems)
            {
                if (viewDataDic.TryGetValue(item, out ViewData viewData))
                {
                    viewData.Destory();
                    viewDataDic.Remove(item);
                }
            }
        }

        /// <summary>
        /// 替换项
        /// </summary>
        /// <param name="oldItems"></param>
        /// <param name="newItems"></param>
        protected void OnReplace(IEnumerable oldItems, IEnumerable newItems)
        {
            OnRemove(oldItems);
            OnAdd(newItems);
        }

        /// <summary>
        /// 排序项
        /// </summary>
        /// <param name="newItems"></param>
        protected void OnSort(IEnumerable newItems)
        {
            int index = 0;
            foreach (var item in newItems)
            {
                if (!viewDataDic.TryGetValue(item, out ViewData viewData))
                {
                    Debug.LogErrorFormat("未知项 {0}", item);
                    continue;
                }

                viewData.SetIndex(index++);
            }
        }

        /// <summary>
        /// 清除项 ：移除视图游戏对象并解除所有对象绑定
        /// </summary>
        protected void OnClear()
        {
            // 移除视图游戏对象并解除所有对象绑定
            foreach (var viewData in viewDataDic.Values)
            {
                viewData.Destory();
            }

            viewDataDic.Clear();
        }

        /// <summary>
        /// 视图数据
        /// </summary>
        protected class ViewData
        {
            /// <summary>
            /// 游戏对象
            /// </summary>
            public GameObject go { get; }

            /// <summary>
            /// 关联的数据链列表
            /// </summary>
            public IDataLinkSet dataLinkList { get; }

            /// <summary>
            /// 视图工厂
            /// </summary>
            public IViewFactory viewFactory { get; }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="go"></param>
            /// <param name="dataLinkList"></param>
            /// <param name="viewFactory"></param>
            public ViewData(GameObject go, IDataLinkSet dataLinkList, IViewFactory viewFactory)
            {
                this.go = go;
                this.dataLinkList = dataLinkList;
                this.viewFactory = viewFactory;
            }

            /// <summary>
            /// 设置游戏对象层级
            /// </summary>
            /// <param name="index"></param>
            public void SetIndex(int index)
            {
                go.transform.SetSiblingIndex(index);
            }

            /// <summary>
            /// 销毁
            /// </summary>
            public void Destory()
            {
                foreach (var dl in dataLinkList.links)
                {
                    dl.Unbind();
                }

                viewFactory.DestoryView(go);
            }
        }

        #endregion
    }
}
