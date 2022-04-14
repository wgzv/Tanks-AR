using UnityEditor;
using XCSJ.EditorTools.Base;
using XCSJ.PluginXGUI.Windows.Weathers;

namespace XCSJ.EditorXGUI.Windows.Weathers
{
    /// <summary>
    /// 天气数据文本检查器
    /// </summary>
    [CustomEditor(typeof(WeatherDataText))]
    public class WeatherDataTextInspector : ToolMBInspector<WeatherDataText>
    {

    }
}
