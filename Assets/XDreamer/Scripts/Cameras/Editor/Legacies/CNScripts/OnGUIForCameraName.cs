using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.CNScripts;
using XCSJ.PluginCamera;
using XCSJ.PluginsCameras.Legacies;
using XCSJ.PluginsCameras.Legacies.CNScripts;

namespace XCSJ.EditorCameras.Legacies.CNScripts
{
    /// <summary>
    /// 相机名称专用的GUI绘制
    /// </summary>
    [CommonEditor(typeof(ForCameraName))]
    public class OnGUIForCameraName : StringScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            EditorGUI.indentLevel = 2;
            paramObject = UICommonFun.Popup(paramObject as string, CameraManager.instance ? CameraManager.instance.GetCameras().ToArray() : null);
        }
    }
}
