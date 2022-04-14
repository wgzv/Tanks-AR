using UnityEditor;
using XCSJ.Algorithms;
using XCSJ.EditorCameras.Base;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.PluginsCameras.Controllers;

namespace XCSJ.EditorCameras.Controllers
{
    /// <summary>
    /// 相机控制器检查器
    /// </summary>
    [CustomEditor(typeof(CameraController), true)]
    public class CameraControllerInspector : BaseCameraMainControllerInspector<CameraController>
    {
        /// <summary>
        /// 目录列表
        /// </summary>
        public static XObject<CategoryList> categoryList { get; } = new XObject<CategoryList>(cl => cl != null, x => EditorToolsHelper.GetWithPurposes(nameof(CameraController)));

        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            EditorCameraHelperExtension.DrawSelectCameraManager();
            EditorCameraHelperExtension.AlignWithView(targetObject.transform);
            CategoryListExtension.DrawVertical(categoryList);
        }
    }
}
