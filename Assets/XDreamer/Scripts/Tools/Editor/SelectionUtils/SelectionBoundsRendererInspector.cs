using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.SelectionUtils;

namespace XCSJ.EditorTools.SelectionUtils
{
    /// <summary>
    /// 选择集边界渲染器检查器
    /// </summary>
    [CustomEditor(typeof(SelectionBoundsRenderer))]
    public class SelectionBoundsRendererInspector : BaseInspector<SelectionBoundsRenderer>
    {
        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            var renderer = targetObject;
            if(renderer && !renderer._material)
            {
                renderer._material = CommonGL.commonMaterial;
            }
        }
    }
}
