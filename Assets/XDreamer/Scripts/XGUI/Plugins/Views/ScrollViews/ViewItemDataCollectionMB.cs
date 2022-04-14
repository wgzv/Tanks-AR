using System.Collections.Generic;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginXGUI.Views.ScrollViews
{
    /// <summary>
    /// 视图项数据集合组件
    /// </summary>
    public abstract class ViewItemDataCollectionMB : MB, IViewItemDataCollection
    {
        /// <summary>
        /// 视图集合
        /// </summary>
        protected IViewItemCollection viewItemCollection
        {
            get
            {
                if (_viewItemCollection == null)
                {
                    _viewItemCollection = GetComponent<IViewItemCollection>();
                }
                return _viewItemCollection;
            }
        }
        private IViewItemCollection _viewItemCollection = null;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            Load();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            Clear();
        }

        /// <summary>
        /// 视图数据集合
        /// </summary>
        public abstract IEnumerable<IViewItemData> datas { get; }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="datas"></param>
        public void AddData(IViewItemData data) => AddDatas(new IViewItemData[1] { data });

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="datas"></param>
        public virtual void AddDatas(IEnumerable<IViewItemData> datas)
        {
            if (viewItemCollection == null) return;

            foreach (var d in datas)
            {
                viewItemCollection.AddData(d);
            }
        }

        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="datas"></param>
        public void RemoveData(IViewItemData data) => RemoveDatas(new IViewItemData[1] { data });

        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="datas"></param>
        public virtual void RemoveDatas(IEnumerable<IViewItemData> datas)
        {
            if (viewItemCollection == null) return;

            foreach (var d in datas)
            {
                viewItemCollection.RemoveData(d);
            }
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        public virtual void Load() => AddDatas(datas);

        /// <summary>
        /// 清除数据
        /// </summary>
        public virtual void Clear() => RemoveDatas(datas);

        /// <summary>
        /// 更新数据
        /// </summary>
        public virtual void UpdateData()
        {
            viewItemCollection?.UpdateData();
        }
    }
}
