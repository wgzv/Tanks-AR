using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginTools;

namespace XCSJ.EditorTools.Base
{
    /// <summary>
    /// 抽象工具组件检查器
    /// </summary>
    [CustomEditor(typeof(ToolMB), true)]
    public class ToolMBInspector : ToolMBInspector<ToolMB>
    {
    }

    /// <summary>
    /// 抽象工具组件检查器泛型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ToolMBInspector<T> : MBInspector<T> where T: ToolMB
    {
    }
}
