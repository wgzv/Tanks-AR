using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginStereoView.Tools;

namespace XCSJ.EditorStereoView.Tools
{
    /// <summary>
    /// 基础屏幕检查器
    /// </summary>
    [CustomEditor(typeof(BaseScreen),true)]
    public class BaseScreenInspector : MBInspector<BaseScreen>
    {
    }

    /// <summary>
    /// 基础屏幕检查器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseScreenInspector<T> : MBInspector<BaseScreen>
        where T : BaseScreen
    {
        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();
            if (GUILayout.Button("屏幕变更"))
            {
                targetObject.CallScreenChanged();
            }
        }
    }
}
