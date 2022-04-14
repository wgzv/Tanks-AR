using UnityEngine;
using XCSJ.Extension.Base.Interactions;

namespace XCSJ.PluginTools.Interactions.Interactables
{
    /// <summary>
    /// 可抓接口
    /// </summary>
    public interface IGrabbable : IInteractable
    {
        /// <summary>
        /// 把握点
        /// </summary>
        Transform grapPoint { get; }

        /// <summary>
        /// 拿起
        /// </summary>
        /// <returns></returns>
        void OnPickUp(IInteractor interactor, GrabData grabData);

        /// <summary>
        /// 放下
        /// </summary>
        /// <returns></returns>
        void OnDrop(IInteractor interactor, GrabData grabData);
    }

    public class GrabData { }

}
