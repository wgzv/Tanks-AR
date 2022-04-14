using UnityEditor;
using UnityEngine;
using XCSJ.EditorCameras.Base;
using XCSJ.PluginART;
using XCSJ.PluginART.Tools;

namespace XCSJ.EditorART.Tools
{
    /// <summary>
    /// 相机变换通过ART检查器
    /// </summary>
    [CustomEditor(typeof(CameraTransformByART))]
    public class CameraTransformByARTInspector : BaseCameraToolControllerInspector<CameraTransformByART>
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
