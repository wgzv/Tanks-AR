using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Views.ScrollViews
{
    /// <summary>
    /// 滚动视图控制器
    /// </summary>
    [Name("滚动视图控制器")]
    public class ScrollViewController : View, IViewItemCollection
    {
        /// <summary>
        /// 视图项模板
        /// </summary>
        [Name("视图项模板")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Readonly(EEditorMode.Runtime)]
        public ViewItem _template;

        /// <summary>
        /// 视图项选中色
        /// </summary>
        [Name("视图项选中色")]
        public Color _selectedColor = new Color(1, 0.4f, 0, 1);// 当前值与Unity编辑器选中对象颜色保持一致

        /// <summary>
        /// 视图项缓存
        /// </summary>
        protected WorkObjectPool<ViewItem> viewItems
        {
            get 
            {
                if (_viewItem==null && DataValid())
                {
                    _viewItem = new WorkObjectPool<ViewItem>();
                    _template.gameObject.SetActive(false);

                    _viewItem.Init(() =>
                    {
                        if (_template)
                        {
                            var newGO = _template.gameObject.XCloneAndSetParent(_template.transform.parent);
                            newGO.transform.localScale = _template.transform.localScale;
                            return newGO.GetComponent<ViewItem>();
                        }
                        return null;
                    },
                        item =>
                        {
                            if (item && item.gameObject)
                            {
                                item.selectedColor = _selectedColor;
                                item.gameObject.SetActive(true);
                            }
                        },
                        item =>
                        {
                            if (item && item.gameObject)
                            {
                                item.gameObject.SetActive(false);
                            }
                        },
                        item => item);
                }
                return _viewItem;
            }
        }
        protected WorkObjectPool<ViewItem> _viewItem = null;

        protected void Reset()
        {
            if (!_template) _template = GetComponentInChildren<ViewItem>();
        }

        /// <summary>
        /// 数据有效
        /// </summary>
        /// <returns></returns>
        protected bool DataValid() => _template;

        /// <summary>
        /// 固定更新
        /// </summary>
        protected void FixedUpdate()
        {
            // 释放无效对象
            viewItems?.Free(item => !item.valid);
        }

        #region IViewItemCollection

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="datas"></param>
        public IViewItem AddData(IViewItemData data)
        {
            if (!DataValid()) return null;

            var view = viewItems.FindOrAlloc(v => v.data == data);
            if (view)
            {
                view.data = data;
                view.transform.SetAsLastSibling();
            }
            return view;
        }

        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="data"></param>
        public void RemoveData(IViewItemData data)
        {
            viewItems.Free(v => v ? v.data == data : false);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        public void UpdateData()
        {
            viewItems.workObjects.ForEach(item => item.UpdateData());
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            viewItems.Clear();
        }

        #endregion
    }
}
