using System.Collections;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginDataflows.Binders;

namespace XCSJ.PluginDataflows.Models
{
    /// <summary>
    /// 集合数据适配器
    /// </summary>
    [Name("集合数据适配器")]
    public class CollectionDataAdapter : BaseDataflowMB, IViewFactory
    {
        /// <summary>
        /// 专用与关联集合数据的属性器
        /// 如果选定当前数据绑定器未指定当前属性，则无法关联真实集合绑定器
        /// </summary>
        public IList collection { get; set; }

        /// <summary>
        /// 模板
        /// </summary>
        [Name("模板")]
        [Tip("用于生成集合列表的模版游戏对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public GameObject _templateView = null;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected void Awake()
        {
            if (_templateView)
            {
                _templateView.XSetActive(false);
            }
        }

        #region IViewFactory

        /// <summary>
        /// 创建视图
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public GameObject CreateView(object data)
        {
            if (!_templateView) return null;

            // 克隆模板
            var view = GameObject.Instantiate<GameObject>(_templateView.gameObject);
            if (view)
            {
                view.name = CommonFun.GetGameObjectUniqueName(_templateView.transform.parent.gameObject, _templateView.name);
                view.transform.XSetTransformParent(_templateView.transform.parent);
                view.transform.SetAsLastSibling();

                if (!view.activeSelf)
                {
                    view.SetActive(true);
                }
            }

            return view;
        }

        /// <summary>
        /// 解除创建视图
        /// </summary>
        /// <param name="view"></param>
        public void DestoryView(GameObject view)
        {
            if (view)
            {
                GameObject.Destroy(view);
            }
        }

        /// <summary>
        /// 更新视图
        /// </summary>
        public void UpdateView()
        {
        } 

        #endregion
    }
}
