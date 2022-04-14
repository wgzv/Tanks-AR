using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;

namespace XCSJ.PluginTools.SelectionUtils
{
    /// <summary>
    /// 选择集
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Select)]
    [RequireManager(typeof(ToolsManager))]
    public abstract class SelectionListenerMB : ToolMB
    {
        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            //计算对象
            Selection.selectionChanged += OnSelectionChanged;

            OnSelectionChanged(new GameObject[0], false);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();

            Selection.selectionChanged -= OnSelectionChanged;
        }

        /// <summary>
        /// 当选择集变更
        /// </summary>
        /// <param name="oldSelections"></param>
        /// <param name="flag"></param>
        protected abstract void OnSelectionChanged(GameObject[] oldSelections, bool flag);
    }

    /// <summary>
    /// 单例化选择集监听组件
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Select)]
    [RequireManager(typeof(ToolsManager))]
    public abstract class SelectionListenerMB<T> : SingleInstanceMB<T> where T : SelectionListenerMB<T>
    {
        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            //计算对象
            Selection.selectionChanged += OnSelectionChanged;

            OnSelectionChanged(new GameObject[0], false);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();

            Selection.selectionChanged -= OnSelectionChanged;
        }

        /// <summary>
        /// 当选择集变更
        /// </summary>
        /// <param name="oldSelections"></param>
        /// <param name="flag"></param>
        protected abstract void OnSelectionChanged(GameObject[] oldSelections, bool flag);
    }

    /// <summary>
    /// 选择集渲染器
    /// </summary>
    public abstract class SelectionRender<T> : SelectionListenerMB<T>, ISelectionRender where T : SelectionRender<T> { }

    /// <summary>
    /// 选择集渲染器接口
    /// </summary>
    public interface ISelectionRender { }
}
