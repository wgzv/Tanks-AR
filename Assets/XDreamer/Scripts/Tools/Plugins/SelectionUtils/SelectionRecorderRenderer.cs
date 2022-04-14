using UnityEngine;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginSMS.Base;

namespace XCSJ.PluginTools.SelectionUtils
{
    /// <summary>
    /// 选择集记录器渲染器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SelectionRecorderRenderer<T> : SelectionRender<T> where T : SelectionRecorderRenderer<T>
    {
        /// <summary>
        /// 渲染器记录器
        /// </summary>
        protected RendererRecorder rendererRecorder = new RendererRecorder();

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();

            rendererRecorder.Recover();
            rendererRecorder._records.Clear();
        }

        /// <summary>
        /// 当选择集变换
        /// </summary>
        /// <param name="oldSelections"></param>
        /// <param name="flag"></param>
        protected override void OnSelectionChanged(GameObject[] oldSelections, bool flag)
        {
            rendererRecorder.Recover();
            rendererRecorder._records.Clear();

            foreach (var item in Selection.selections)
            {
                if (item)
                {
                    rendererRecorder.Record(item.GetComponentsInChildren<Transform>());
                }
            }
            
            UpdateRenderer();
        }

        protected abstract void UpdateRenderer();
    }
}
