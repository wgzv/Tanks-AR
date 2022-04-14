using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Tools;

namespace XCSJ.PluginTools.SelectionUtils
{
    /// <summary>
    /// 选择集材质渲染器
    /// </summary>
    [DisallowMultipleComponent]
    [Name("选择集材质渲染器")]
    public class SelectionMaterialRenderer : SelectionRecorderRenderer<SelectionMaterialRenderer>
    {
        /// <summary>
        /// 材质组
        /// </summary>
        [Name("材质组")]
        public Material[] materials;

        /// <summary>
        /// 更新渲染器
        /// </summary>
        protected override void UpdateRenderer()
        {
            if (materials == null || materials.Length == 0) return;

            foreach (var rendererRecorderInfo in rendererRecorder._records)
            {
                rendererRecorderInfo.SetMaterial(materials);
            }
        }
    }
}
