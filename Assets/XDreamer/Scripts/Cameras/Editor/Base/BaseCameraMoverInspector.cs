using UnityEditor;
using XCSJ.PluginsCameras.Base;

namespace XCSJ.EditorCameras.Base
{
    /// <summary>
    /// 基础相机移动器检查器
    /// </summary>
    [CustomEditor(typeof(BaseCameraMover), true)]
    public class BaseCameraMoverInspector : BaseCameraMoverInspector<BaseCameraMover>
    {
    }

    /// <summary>
    /// 基础相机移动器检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseCameraMoverInspector<T> : BaseCameraCoreControllerInspector<T>
       where T : BaseCameraMover
    {
    }
}
