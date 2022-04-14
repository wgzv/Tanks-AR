using UnityEditor;
using XCSJ.PluginsCameras.Base;

namespace XCSJ.EditorCameras.Base
{
    /// <summary>
    /// 基础相机旋转器检查器
    /// </summary>
    [CustomEditor(typeof(BaseCameraRotator), true)]
    public class BaseCameraRotatorInspector : BaseCameraRotatorInspector<BaseCameraRotator>
    {
    }

    /// <summary>
    /// 基础相机旋转器检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseCameraRotatorInspector<T> : BaseCameraCoreControllerInspector<T>
       where T : BaseCameraRotator
    {
    }
}

