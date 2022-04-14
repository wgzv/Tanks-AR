using UnityEditor;
using XCSJ.Algorithms;
using XCSJ.EditorCameras.Base;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Controls;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.EditorTools;

namespace XCSJ.EditorCameras.Controllers
{
    /// <summary>
    /// 相机实体控制器检查器
    /// </summary>
    [CustomEditor(typeof(CameraEntityController), true)]
    public class CameraEntityControllerInspector : BaseCameraEntityControllerInspector<CameraEntityController>
    {
    }
}
