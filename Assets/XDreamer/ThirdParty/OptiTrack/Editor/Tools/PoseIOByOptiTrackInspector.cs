using UnityEditor;
using UnityEngine;
using XCSJ.EditorXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.PluginOptiTrack;
using XCSJ.PluginOptiTrack.Tools;

namespace XCSJ.EditorOptiTrack.Tools
{
    /// <summary>
    /// 姿态IO通过OptiTrack检查器
    /// </summary>
    [CustomEditor(typeof(PoseIOByOptiTrack))]
    public class PoseIOByOptiTrackInspector : BaseAnalogControllerIOInspector<PoseIOByOptiTrack>
    {
        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            EditorOptiTrackHelper.DrawSelectOptiTrackManager();
        }
    }
}
