
namespace XCSJ.Extension.Base.Interactions
{
    /// <summary>
    /// 交互器接口：用于描述交互时的使用的交互工具；
    /// </summary>
    public interface IInteractor
    {
        /// <summary>
        /// 能否交互：用于判断在特定环境<see cref="IInteractableContext"/>中，针对可交互对象<see cref="IInteractable"/>，使用当前交互工具<see cref="IInteractor"/>通过期望的交互方式<see cref="IInteractorInput"/>能否交互；
        /// </summary>
        /// <param name="context">语境：特定环境</param>
        /// <param name="interactable">可交互对象</param>
        /// <param name="interactInput">交互器输入：交互方式</param>
        /// <param name="canInteractableResult">能否交互结果</param>
        /// <returns></returns>
        bool CanInteractable(IInteractableContext context, IInteractable interactable, IInteractorInput interactInput, out ICanInteractableResult canInteractableResult);

        /// <summary>
        /// 尝试处理交互：用于在特定环境<see cref="IInteractableContext"/>中，针对可交互对象<see cref="IInteractable"/>，使用当前交互工具<see cref="IInteractor"/>通过期望的交互方式<see cref="IInteractorInput"/>尝试处理交互；
        /// </summary>
        /// <param name="context">语境：特定环境</param>
        /// <param name="interactable">可交互对象</param>
        /// <param name="interactInput">交互器输入：交互方式</param>
        /// <param name="handleInteractableResult">处理交互结果</param>
        /// <returns></returns>
        bool TryHandleInteractable(IInteractableContext context, IInteractable interactable, IInteractorInput interactInput, out IHandleInteractableResult handleInteractableResult);
    }
}
