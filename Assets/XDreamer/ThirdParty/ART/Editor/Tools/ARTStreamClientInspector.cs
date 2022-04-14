using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginART.Tools;

namespace XCSJ.EditorART.Tools
{
    /// <summary>
    /// ART流客户端检查器
    /// </summary>
    [CustomEditor(typeof(ARTStreamClient), true)]
    public class ARTStreamClientInspector : BaseInspector<ARTStreamClient>
    {
        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            EditorGUILayout.LabelField("连接状态", targetObject.IsConnected() ? "已连接" : "未连接");
        }
    }
}
