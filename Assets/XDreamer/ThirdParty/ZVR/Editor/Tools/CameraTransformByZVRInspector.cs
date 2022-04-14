using UnityEditor;
using UnityEngine;
using XCSJ.EditorCameras.Base;
using XCSJ.PluginZVR;
using XCSJ.PluginZVR.Tools;

namespace XCSJ.EditorZVR.Tools
{
    /// <summary>
    /// 相机变换通过ZVR检查器
    /// </summary>
    [CustomEditor(typeof(CameraTransformByZVR))]
    public class CameraTransformByZVRInspector : BaseCameraToolControllerInspector<CameraTransformByZVR>
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
