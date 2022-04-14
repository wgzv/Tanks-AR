using UnityEditor;
using XCSJ.EditorTools.Base;
using XCSJ.PluginTools.AI.Autos;

namespace XCSJ.EditorTools.AI.Autos
{
    [CustomEditor(typeof(AutoWait), true)]
    public class AutoWaitInspector : AutoWaitInspector<AutoWait>
    {
    }

    public class AutoWaitInspector<T> : ToolMBInspector<T> where T : AutoWait
    {
    }
}
