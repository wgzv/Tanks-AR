using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Views.ScrollViews
{
    /// <summary>
    /// 视图项数据集合
    /// </summary>
    public interface IViewItemCollection
    {
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="data"></param>
        IViewItem AddData(IViewItemData data);

        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="data"></param>
        void RemoveData(IViewItemData data);

        /// <summary>
        /// 更新数据:使用数据刷新视图界面
        /// </summary>
        void UpdateData();

        /// <summary>
        /// 移除
        /// </summary>
        void Clear();
    }


    public class ViewItemCollection
    {
    }
}
