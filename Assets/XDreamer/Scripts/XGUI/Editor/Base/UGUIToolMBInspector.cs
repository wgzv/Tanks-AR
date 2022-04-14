using UnityEditor;
using XCSJ.EditorTools.Base;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.EditorXGUI.Base
{
    /// <summary>
    /// UGUI工具组件检查器
    /// </summary>
    [CustomEditor(typeof(View), true)]
    public class XViewInspector : ToolMBInspector<View> { }

    /// <summary>
    /// UGUI工具组件检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class XViewInspector<T> : ToolMBInspector<T> where T : View
    {
    }
}
