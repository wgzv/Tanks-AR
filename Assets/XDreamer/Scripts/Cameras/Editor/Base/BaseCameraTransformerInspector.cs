using UnityEditor;
using XCSJ.PluginsCameras.Base;

namespace XCSJ.EditorCameras.Base
{
    /// <summary>
    /// 基础相机变换器检查器
    /// </summary>
    [CustomEditor(typeof(BaseCameraTransformer), true)]
    public class BaseCameraTransformerInspector : BaseCameraTransformerInspector<BaseCameraTransformer>
    {
    }

    /// <summary>
    /// 基础相机变换器检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseCameraTransformerInspector<T> : BaseCameraCoreControllerInspector<T>
       where T : BaseCameraTransformer
    {
    }
}

