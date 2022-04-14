using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using XCSJ.CommonUtils.PluginHighlightingSystem;
using XCSJ.CommonUtils.PluginOutline;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;

#if XDREAMER_PROJECT_URP
using XCSJ.EditorTools.Pipelines;
#endif

namespace XCSJ.CommonUtils.EditorHighlightingSystem
{
    /// <summary>
    /// 轮廓线数据加载器监视器
    /// </summary>
    [CustomEditor(typeof(OutlineDataLoader))]
    public class OutlineDataLoaderInspector : BaseInspectorWithLimit<OutlineDataLoader>
    {
        /// <summary>
        /// 启用回调函数
        /// </summary>
        [InitializeOnLoadMethod]
        private static void AddEnableEventListener()
        {
            OutlineDataLoader.enableChanged -= OnSelectionHighlightingRendererEnableChanged;
            OutlineDataLoader.enableChanged += OnSelectionHighlightingRendererEnableChanged;
        }

        /// <summary>
        /// 后绘制
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

#if XDREAMER_PROJECT_URP
            if (GUILayout.Button("添加【轮廓线特性】至【渲染管线资产配置文件】"))
            {
                AddOutlineFeatureIfPipelineNotContain();
            }
#endif
        }

        private static void OnSelectionHighlightingRendererEnableChanged(OutlineDataLoader selectionHighlightingRenderer, bool enable)
        {
            if (enable)
            {
                AddOutlineFeatureIfPipelineNotContain();
            }
        }

        /// <summary>
        /// 检查轮廓线特性是否添加至渲染管线配置文件中
        /// </summary>
        private static void AddOutlineFeatureIfPipelineNotContain()
        {
#if XDREAMER_PROJECT_URP

            // 创建通用渲染管线资产
            if (PipelineAssetEditorHelper.activeRenderersCount == 0)
            {
                var rendererAsset = PipelineAssetEditorHelper.CreateRenderData();
                var asset = PipelineAssetEditorHelper.CreateAsset(rendererAsset);
                GraphicsSettings.renderPipelineAsset = asset;
                AssetDatabase.CreateAsset(rendererAsset, Product.Name + "_UniversalRenderPipelineAsset.asset");
                AssetDatabase.CreateAsset(asset, "");

                Debug.Log("创建URP【渲染管线资产配置文件】！");
            }

            // 添加特性
            if (PipelineAssetEditorHelper.AddFeatureIfNotContain<OutlineFeature>())
            {
                Debug.Log("添加【轮廓线特色】至【渲染管线资产配置文件】!");
            }
#endif
        }


    }
}
