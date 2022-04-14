using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginSamsungWMR;

namespace XCSJ.EditorSamsungWMR
{
    /// <summary>
    /// 三星玄龙管理器属性器
    /// </summary>
    [CustomEditor(typeof(SamsungWMRManager))]
    public class SamsungWMRManagerInspector : BaseManagerInspector<SamsungWMRManager>
    {
        /// <summary>
        /// 当纵向绘制之前回调
        /// </summary>
        public override void OnBeforeVertical()
        {
            // 安装XR交互工具包
            XCSJ.EditorXXR.Interaction.Toolkit.XXRInteractionToolkitManagerInspector.InstallXRInteractionToolkitPackage();

            base.OnBeforeVertical();
        }
    }
}
