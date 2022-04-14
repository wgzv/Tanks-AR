using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Legacies.Tools;

namespace XCSJ.EditorCameras.Legacies
{
    /// <summary>
    /// 相机检查器扩展
    /// </summary>
    public class CameraInspectorExtension
    {
        /// <summary>
        /// 初始化
        /// </summary>
        [InitializeOnLoadMethod]
        public static void Init()
        {
            CameraManagerInspector.onInspectorGUIOfExtensionFunc += OnInspectorGUIOfExtensionFunc;
        }

#pragma warning disable CS0618 // 类型或成员已过时

        /// <summary>
        /// 扩展检查器GUI绘制
        /// </summary>
        /// <param name="inspector"></param>
        /// <param name="camera"></param>
        private static void OnInspectorGUIOfExtensionFunc(BaseInspector inspector, BaseCamera camera)
        {
            var component = camera.gameObject.GetComponent<CameraTargetModifyBySelection>();
            bool componentFlag = EditorGUILayout.Toggle(CommonFun.NameTooltip(typeof(CameraTargetModifyBySelection)), component);
            if (componentFlag && !component) camera.gameObject.AddComponent<CameraTargetModifyBySelection>();
            else if (!componentFlag && component) inspector.RemoveComponent(component);
        }

#pragma warning restore CS0618 // 类型或成员已过时
    }
}
