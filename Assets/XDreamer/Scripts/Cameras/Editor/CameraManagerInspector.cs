using System;
using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Controls;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.PluginCamera;

namespace XCSJ.EditorCameras
{
    /// <summary>
    /// 相机管理器检查器
    /// </summary>
    [CustomEditor(typeof(CameraManager))]
    public class CameraManagerInspector : BaseManagerInspector<CameraManager>
    {
        private static CategoryList categoryList = null;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            if (categoryList == null) categoryList = EditorToolsHelper.GetWithPurposes(nameof(CameraManager));
            var targetObject = this.targetObject;
            if (targetObject)
            {
                if (!targetObject.legacyCameraManagerProvider) { }
                if (!targetObject.cameraManagerProvider) { }
            }
        }

        /// <summary>
        /// 当脚本对象纵向绘制之后回调
        /// </summary>
        public override void OnAfterScript()
        {
            categoryList.DrawVertical();

            EditorGUILayout.Separator();

            base.OnAfterScript();
        }

#pragma warning disable CS0618 // 类型或成员已过时

        /// <summary>
        /// 检查器绘制GUI扩展
        /// </summary>
        public static event Action<BaseInspector, BaseCamera> onInspectorGUIOfExtensionFunc;

        internal static void CallOnInspectorGUIOfExtensionFunc(BaseInspector inspector, BaseCamera camera)
        {
            onInspectorGUIOfExtensionFunc?.Invoke(inspector, camera);
        }

#pragma warning restore CS0618 // 类型或成员已过时
    }
}
