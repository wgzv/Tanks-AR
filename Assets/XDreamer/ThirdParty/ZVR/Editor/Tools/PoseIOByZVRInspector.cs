using UnityEditor;
using UnityEngine;
using XCSJ.EditorXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.PluginZVR;
using XCSJ.PluginZVR.Tools;

namespace XCSJ.EditorZVR.Tools
{
    /// <summary>
    /// 姿态IO通过ZVR检查器
    /// </summary>
    [CustomEditor(typeof(PoseIOByZVR))]
    public class PoseIOByZVRInspector : BaseAnalogControllerIOInspector<PoseIOByZVR>
    {
        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            EditorZVRHelper.DrawSelectZVRManager();
        }
    }
}
