using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginTools;
using XCSJ.PluginTools.Renderers;

namespace XCSJ.EditorTools.Inspectors
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GizmoRenderer))]
    public class GizmoRendererInspector : BaseInspector<GizmoRenderer>
    {
    }
}
