using XCSJ.Algorithms;

namespace XCSJ.Extension.Base.Interactions
{
    /// <summary>
    /// 可交互接口：用于描述交互时的被作用的对象，即
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// 能否交互：用于判断在特定环境<see cref="IInteractableContext"/>中，使用指定交互工具<see cref="IInteractor"/>通过期望的交互方式<see cref="IInteractorInput"/>，针对当前可交互对象<see cref="IInteractable"/>能否交互；
        /// </summary>
        /// <param name="context">语境：特定环境</param>
        /// <param name="interactor">交互器：交互工具</param>
        /// <param name="interactInput">交互器输入：交互方式</param>
        /// <param name="canInteractableResult">能否交互结果</param>
        /// <returns></returns>
        bool CanInteractable(IInteractableContext context, IInteractor interactor, IInteractorInput interactInput, out ICanInteractableResult canInteractableResult);

        /// <summary>
        /// 尝试处理交互：用于在特定环境<see cref="IInteractableContext"/>中，使用指定交互工具<see cref="IInteractor"/>通过期望的交互方式<see cref="IInteractorInput"/>，针对当前可交互对象<see cref="IInteractable"/>尝试处理交互；
        /// </summary>
        /// <param name="context">语境：特定环境</param>
        /// <param name="interactor">交互器：交互工具</param>
        /// <param name="interactInput">交互器输入：交互方式</param>
        /// <param name="handleInteractableResult">处理交互结果</param>
        /// <returns></returns>
        bool TryHandleInteractable(IInteractableContext context, IInteractor interactor, IInteractorInput interactInput, out IHandleInteractableResult handleInteractableResult);
    }

    /// <summary>
    /// 能否交互结果接口
    /// </summary>
    public interface ICanInteractableResult { }

    /// <summary>
    /// 无效交互器：无法有效处理的交互器
    /// </summary>
    public class InvalidInteractor : ICanInteractableResult, IHandleInteractableResult
    {
        /// <summary>
        /// 默认实例
        /// </summary>
        public static InvalidInteractor Default => Default<InvalidInteractor>.Instance;
    }

    /// <summary>
    /// 处理交互结果
    /// </summary>
    public interface IHandleInteractableResult { }
}
