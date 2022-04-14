using UnityEngine;
using XCSJ.Extension.Base.Interactions;
using XCSJ.PluginTools.Interactions.Interactors;

namespace XCSJ.PluginTools.Interactions.Interactables
{
    /// <summary>
    /// 可抓取对象
    /// </summary>
    public class Grabbable : BaseInteractable, IGrabbable
    {
        /// <summary>
        /// 抓取点
        /// </summary>
        public Transform _grapPoint;

        public Transform grapPoint => _grapPoint;

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            
        }

        public virtual void OnPickUp(IInteractor interactor, GrabData grabData) { }

        public virtual void OnDrop(IInteractor interactor, GrabData grabData) { }

        public override bool CanInteractable(IInteractableContext context, IInteractor interactor, IInteractorInput interactInput, out ICanInteractableResult canInteractableResult)
        {
            canInteractableResult = default;
            return interactor is IPicker;
        }

        public override bool TryHandleInteractable(IInteractableContext context, IInteractor interactor, IInteractorInput interactInput, out IHandleInteractableResult handleInteractableResult)
        {
            handleInteractableResult = default;
            return true;
        }
    }
}
