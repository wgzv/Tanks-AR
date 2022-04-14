using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginStereoView.Tools;

namespace XCSJ.EditorStereoView.Tools
{
    /// <summary>
    /// 屏幕锚点关联检查器
    /// </summary>
    [CustomEditor(typeof(ScreenAnchorLink))]
    public class ScreenAnchorLinkInspector : MBInspector<ScreenAnchorLink>
    {
        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();
            if (GUILayout.Button("更新屏幕TRS"))
            {
                targetObject.UpdateScreen();
            }
        }
    }
}
