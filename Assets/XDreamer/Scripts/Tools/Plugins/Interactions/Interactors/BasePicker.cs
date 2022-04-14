using XCSJ.Extension.Base.Interactions;
using XCSJ.PluginTools.Interactions.Interactables;

namespace XCSJ.PluginTools.Interactions.Interactors
{
    /// <summary>
    /// 基础拾取器
    /// </summary>
    public abstract class BasePicker : BaseInteractor, IPicker
    {
        /// <summary>
        /// 捡起
        /// </summary>
        /// <param name="interactInput"></param>
        /// <returns></returns>
        public abstract IGrabbable Pickup(IInteractorInput interactInput);

        /// <summary>
        /// 放下
        /// </summary>
        /// <param name="interactInput"></param>
        /// <returns></returns>
        public abstract IGrabbable Drop(IInteractorInput interactInput);

        /// <summary>
        /// 拾取状态
        /// </summary>
        public virtual EPickState pickState { get; protected set; } = EPickState.None;
    }
}
