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
    /// 相机旋转器检查器
    /// </summary>
    [CustomEditor(typeof(CameraRotator), true)]
    public class CameraRotatorInspector : BaseCameraRotatorInspector<CameraRotator>
    {
    }
}

