using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginTools.Renderers;

namespace XCSJ.EditorTools.Inspectors
{
    /// <summary>
    /// 让UV平移动画可多选编辑
    /// </summary>
    [CustomEditor(typeof(UVMoveMotion))]
    [CanEditMultipleObjects]
    public class UVMoveMotionInspector : BaseInspectorWithLimit<UVMoveMotion>
    {

    }
}
