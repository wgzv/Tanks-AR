using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorTools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginStereoView;
using XCSJ.PluginStereoView.Tools;
using XCSJ.PluginXRSpaceSolution;
using XCSJ.PluginXXR.Interaction.Toolkit;

namespace XCSJ.EditorXRSpaceSolution
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    public static class ToolsMenu
    {
        const string XRSpaceTitle = "单机多通道XR空间网络";

        /// <summary>
        /// 创建XR空间网络，创建单机多通道动立体虚拟屏幕等工具组件构建的XR交互空间网络；
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(XRITHelper.SpaceSolution)]
        [Name(XRSpaceTitle)]
        [Tip("创建单机多通道动立体虚拟屏幕等工具组件构建的XR交互空间网络；")]
        [XCSJ.Attributes.Icon(EIcon.State)]
        [RequireManager(typeof(XRSpaceSolutionManager), typeof(StereoViewManager))]
        public static void CreateXRSpace_MultiScreen_ActiveStereo(ToolContext toolContext)
        {
            XRSpaceSolutionHelper.CreateXRSpaceNet(XRSpaceTitle);
        }
    }
}
