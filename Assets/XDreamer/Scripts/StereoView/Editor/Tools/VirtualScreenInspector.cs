using UnityEditor;
using XCSJ.PluginStereoView.Tools;

namespace XCSJ.EditorStereoView.Tools
{
    /// <summary>
    /// 虚拟屏幕检查器
    /// </summary>
    [CustomEditor(typeof(VirtualScreen))]
    public class VirtualScreenInspector : BaseScreenInspector<VirtualScreen>
    {
    }
}
