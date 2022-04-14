using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCamera;
using XCSJ.PluginsCameras;
using XCSJ.PluginsCameras.Controllers;

namespace XCSJ.EditorCameras.Controllers
{
    /// <summary>
    /// 相机拥有者检查器
    /// </summary>
    [CustomEditor(typeof(CameraOwner), true)]
    public class CameraOwnerInspector : BaseInspector<CameraOwner>
    {
        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            if (GUILayout.Button("选中[" + CameraHelperExtension.CategoryName + "]管理器") && CameraManager.instance)
            {
                Selection.activeObject = CameraManager.instance;
            }
        }
    }
}
