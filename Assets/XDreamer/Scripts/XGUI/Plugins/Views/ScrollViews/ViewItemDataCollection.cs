using System.Collections.Generic;

namespace XCSJ.PluginXGUI.Views.ScrollViews
{
    /// <summary>
    /// 视图项数据集
    /// </summary>
    public interface IViewItemDataCollection
    {
        /// <summary>
        /// 数据集合
        /// </summary>
        IEnumerable<IViewItemData> datas { get; }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="datas"></param>
        void AddData(IViewItemData data);

        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="datas"></param>
        void RemoveData(IViewItemData data);

        /// <summary>
        /// 清除
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// 视图项数据集合
    /// </summary>
    public abstract class ViewItemDataCollection  
    {
       

    }
}
