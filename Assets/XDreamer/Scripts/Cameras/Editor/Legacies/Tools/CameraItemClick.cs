using System;
using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCamera;
using XCSJ.PluginsCameras.Legacies;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;

namespace XCSJ.EditorCameras.Legacies.Tools
{
    /// <summary>
    /// 相机项点击
    /// </summary>

    [ToolItemClick(typeof(BaseCamera), true)]
    [Obsolete("产品功能升级，不推荐用户再使用本功能组件，请使用新的功能组件替代！", false)]
    public class CameraItemClick : IItemClick
    {
        /// <summary>
        /// 能否点击
        /// </summary>
        /// <param name="toolItem"></param>
        /// <returns></returns>
        public bool CanClick(IItem toolItem) => true;

        /// <summary>
        /// 点击时
        /// </summary>
        /// <param name="toolItem"></param>
        public void OnClick(IItem toolItem)
        {
            if (!CameraManager.instance) return;
            var cam = CameraManager.instance.CreateCamera(toolItem.memberInfo as Type);
            if (cam)
            {
                EditorGUIUtility.PingObject(cam.gameObject);
            }
        }
    }
}
