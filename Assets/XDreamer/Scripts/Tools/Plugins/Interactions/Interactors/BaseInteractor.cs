using XCSJ.Extension.Base.Interactions;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Interactions.Interactors
{
    /// <summary>
    /// 交互器:发起对可交互对象交互动动作
    /// </summary>
    public abstract class BaseInteractor : MB, IInteractor
    {
        public bool CanInteractable(IInteractableContext context, IInteractable interactable, IInteractorInput interactInput, out ICanInteractableResult canInteractableResult)
        {
            throw new System.NotImplementedException();
        }

        public bool TryHandleInteractable(IInteractableContext context, IInteractable interactable, IInteractorInput interactInput, out IHandleInteractableResult handleInteractableResult)
        {
            throw new System.NotImplementedException();
        }
    }
}
