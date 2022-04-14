using XCSJ.Extension.Base.Interactions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.Interactions.Interactables;
using XCSJ.PluginTools.Interactions.Interactors;

namespace XCSJ.PluginTools.Interactions.Hands
{
    /// <summary>
    /// 手交互器
    /// </summary>
    [RequireManager(typeof(ToolsManager), typeof(ToolsExtensionManager))]
    public class Hand : BasePicker
    {
        public override IGrabbable Drop(IInteractorInput interactInput)
        {
            throw new System.NotImplementedException();
        }

        public override IGrabbable Pickup(IInteractorInput interactInput)
        {
            throw new System.NotImplementedException();
        }
    }
}
