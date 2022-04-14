using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginPeripheralDevice;

namespace XCSJ.EditorPeripheralDevice
{
    /// <summary>
    /// 外部设备输入管理器检查器
    [CustomEditor(typeof(PeripheralDeviceInputManager))]
    /// </summary>
    public class PeripheralDeviceInputManagerInspector: BaseManagerInspector<PeripheralDeviceInputManager>
    {
        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            EditorGUILayout.Separator();
            EditorPeripheralDeviceInputHelper.DrawOpenInputDubugger();
        }
    }
}
