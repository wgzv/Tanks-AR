using UnityEditor;
using UnityEngine;
using XCSJ.EditorXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.PluginART;
using XCSJ.PluginART.Tools;

namespace XCSJ.EditorART.Tools
{
    /// <summary>
    /// 姿态IO通过ART检查器
    /// </summary>
    [CustomEditor(typeof(PoseIOByART))]
    public class PoseIOByARTInspector : BaseAnalogControllerIOInspector<PoseIOByART>
    {
        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            EditorARTHelper.DrawSelectARTManager();
        }
    }
}
