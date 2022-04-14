using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.Interactions.Interactables;

namespace XCSJ.PluginTools.Interactions.Hands
{
    /// <summary>
    /// 可被手抓对象
    /// </summary>
    [RequireManager(typeof(ToolsManager), typeof(ToolsExtensionManager))]
    public abstract class HandGrabbable : Grabbable
    {
        /// <summary>
        /// 抓手势
        /// </summary>
        public HandPose _handPose;
    }
}
