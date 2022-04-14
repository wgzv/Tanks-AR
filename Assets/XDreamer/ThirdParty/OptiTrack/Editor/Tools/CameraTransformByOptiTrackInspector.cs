using UnityEditor;
using UnityEngine;
using XCSJ.EditorCameras.Base;
using XCSJ.PluginOptiTrack;
using XCSJ.PluginOptiTrack.Tools;

namespace XCSJ.EditorOptiTrack.Tools
{
    /// <summary>
    /// 相机变换通过OptiTrack检查器
    /// </summary>
    [CustomEditor(typeof(CameraTransformByOptiTrack))]
    public class CameraTransformByOptiTrackInspector : BaseCameraToolControllerInspector<CameraTransformByOptiTrack>
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
