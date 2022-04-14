using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.LocomotionProviders;

namespace XCSJ.EditorXXR.Interaction.Toolkit.Tools.LocomotionProviders
{
    /// <summary>
    /// 模拟运动提供者检查器
    /// </summary>
    [CustomEditor(typeof(AnalogLocomotionProvider))]
    class AnalogLocomotionProviderInspector
#if XDREAMER_XR_INTERACTION_TOOLKIT
        : Editor
#else
        : MBInspector<AnalogLocomotionProvider>        
#endif
    {

#if XDREAMER_XR_INTERACTION_TOOLKIT

        private static CategoryList categoryList = null;

        /// <summary>
        /// 启用
        /// </summary>
        public void OnEnable()
        {
            if (categoryList == null) categoryList = EditorToolsHelper.GetWithPurposes(nameof(AnalogLocomotionProvider));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            categoryList.DrawVertical();
        }

#endif
    }
}
