using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions;
using XCSJ.PluginTools.Interactions.Interactables;

namespace XCSJ.PluginTools.Interactions.Interactors
{
    /// <summary>
    /// 拾取接口
    /// </summary>
    public interface IPicker
    {
        /// <summary>
        /// 捡起
        /// </summary>
        /// <param name="interactInput"></param>
        /// <returns></returns>
        IGrabbable Pickup(IInteractorInput interactInput);

        /// <summary>
        /// 放下
        /// </summary>
        /// <param name="interactInput"></param>
        /// <returns></returns>
        IGrabbable Drop(IInteractorInput interactInput);

        /// <summary>
        /// 拾取状态
        /// </summary>
        EPickState pickState { get; }
    }

    /// <summary>
    /// 拾取状态
    /// </summary>
    [Name("拾取状态")]
    public enum EPickState
    {
        [Name("无")]
        None,

        [Name("拿着")]
        Hold,
    }
}
