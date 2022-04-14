using XCSJ.Extension.Base.Interactions;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Interactions.Interactables
{
    /// <summary>
    /// 可交互对象
    /// </summary>
    [RequireManager(typeof(ToolsManager))]
    public abstract class BaseInteractable : MB, IInteractable
    {
        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactor"></param>
        /// <param name="interactInput"></param>
        /// <param name="canInteractableResult"></param>
        /// <returns></returns>
        public abstract bool CanInteractable(IInteractableContext context, IInteractor interactor, IInteractorInput interactInput, out ICanInteractableResult canInteractableResult);

        /// <summary>
        /// 尝试处理交互
        /// </summary>
        /// <param name="interactor"></param>
        /// <param name="interactInput"></param>
        /// <param name="handleInteractableResult"></param>
        /// <returns></returns>
        public abstract bool TryHandleInteractable(IInteractableContext context, IInteractor interactor, IInteractorInput interactInput, out IHandleInteractableResult handleInteractableResult);

        public abstract void Reset();
    }
}
