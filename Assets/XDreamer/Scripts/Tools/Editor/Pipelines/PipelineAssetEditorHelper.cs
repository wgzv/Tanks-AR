#if UNITY_2019_3_OR_NEWER

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

#if XDREAMER_PROJECT_URP
using UnityEngine.Rendering.Universal;
#endif

#if XDREAMER_PROJECT_HDRP
using UnityEngine.Rendering.HighDefinition;
#endif

namespace XCSJ.EditorTools.Pipelines
{

    /// <summary>
    /// 渲染管线助手
    /// </summary>
#if (XDREAMER_PROJECT_URP || XDREAMER_PROJECT_HDRP)
    public static class PipelineAssetEditorHelper
    {
        /// <summary>
        /// 当前渲染管线配置
        /// </summary>
        public static RenderPipelineAsset CurrentRenderPipelineAsset
        {
            get
            {
                return QualitySettings.renderPipeline ? QualitySettings.renderPipeline : GraphicsSettings.renderPipelineAsset;
            }
        }

        public static List<RenderPipelineAsset> ActiveAssets
        {
            get
            {
                var assetList = new List<RenderPipelineAsset>();

                if (GraphicsSettings.renderPipelineAsset)
                {
                    assetList.Add(GraphicsSettings.renderPipelineAsset);
                }

                var qualitySettingNames = QualitySettings.names;
                for (var index = 0; index < qualitySettingNames.Length; index++)
                {
                    var assset = QualitySettings.GetRenderPipelineAssetAt(index);
                    if (assset && !assetList.Contains(assset))
                    {
                        assetList.Add(assset);
                    }
                }
                return assetList;
            }
        }

        public static bool IsURP(RenderPipelineAsset asset)
        {
            return asset && asset.GetType().Name.Equals("UniversalRenderPipelineAsset");
        }

        public static bool IsHDRP(RenderPipelineAsset asset)
        {
            return asset && asset.GetType().Name.Equals("HDRenderPipelineAsset");
        }

        public static int activeRenderersCount => ActiveAssets.Count;

#if XDREAMER_PROJECT_URP

        public static RenderPipelineAsset CreateAsset(ForwardRendererData data)
        {
            return UniversalRenderPipelineAsset.Create(data);
        }

        public static ForwardRendererData CreateRenderData()
        {
            return ScriptableObject.CreateInstance<ForwardRendererData>();
        }

        public static bool ContainFeature<T>() where T : ScriptableRendererFeature
        {
            var activeAssets = ActiveAssets;

            foreach (var asset in activeAssets)
            {
                if (IsAssetContainsSRPOutlineFeature<T>(asset)) return true;
            }

            return false;
        }

        public static bool AddFeatureIfNotContain<T>() where T : ScriptableRendererFeature
        {
            var activeAssets = ActiveAssets;

            if (activeAssets.Count==0)
            {
                return false;
            }
            foreach (var asset in activeAssets)
            {
                if (IsAssetContainsSRPOutlineFeature<T>(asset)) return false;
            }

            var firstAsset = activeAssets[0];
            if (firstAsset is UniversalRenderPipelineAsset urpAsset && urpAsset)
            {
                var data = GetRenderer(firstAsset);
                if (data)
                {
                    var so = ScriptableObject.CreateInstance<T>();
                    so.name = typeof(T).Name;
                    data.rendererFeatures.Add(so);
                    AssetDatabase.AddObjectToAsset(so, data);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static bool IsAssetContainsSRPOutlineFeature<T>(RenderPipelineAsset asset) where T : ScriptableRendererFeature
        {
            if (asset is UniversalRenderPipelineAsset urpAsset && urpAsset)
            {
                var data = GetRenderer(asset);
                return data ? data.rendererFeatures.Exists(x => x is T) : false;
            }

            return false;
        }

        private static ScriptableRendererData GetRenderer(RenderPipelineAsset asset)
        {
            using (var so = new SerializedObject(asset))
            {
                so.Update();

                var rendererDataList = so.FindProperty("m_RendererDataList");
                var assetIndex = so.FindProperty("m_DefaultRendererIndex");
                var item = rendererDataList.GetArrayElementAtIndex(assetIndex.intValue);

                return item.objectReferenceValue as ScriptableRendererData;
            }
        }
#endif

#if XDREAMER_PROJECT_HDRP
        public static RenderPipelineAsset CreateHDRPAsset()
        {
            return ScriptableObject.CreateInstance<HDRenderPipelineAsset>();
        }
#endif

    }
#endif
}

#endif