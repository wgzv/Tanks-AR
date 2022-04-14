using UnityEditor;
using XCSJ.PluginXGUI.Views.Dropdowns;

namespace XCSJ.EditorXGUI.Views.Dropdowns
{
    /// <summary>
    /// 下拉框事件触发器检查器
    /// </summary>
    [CustomEditor(typeof(DropdownEventTrigger), true)]
    public class DropdownEventTriggerInspector : DropdownMBInspecotr<DropdownEventTrigger>
    {
    }
}
