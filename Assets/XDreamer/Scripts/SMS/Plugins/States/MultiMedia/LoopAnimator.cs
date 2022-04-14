using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.MultiMedia
{
    /// <summary>
    /// 循环动画:循环动画组件是播放Unity的Animator动画的对象。可设置播放动画的循环次数，播放完之后，组件切换为完成态。
    /// </summary>
    [ComponentMenu(SMSHelperExtension.MultiMediaCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(LoopAnimator))]
    [Tip("循环动画组件是播放Unity的Animator动画的对象。可设置播放动画的循环次数，播放完之后，组件切换为完成态。")]
    [XCSJ.Attributes.Icon(index = 33639)]
    public class LoopAnimator : UnityAnimator<LoopAnimator>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "循环动画";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.MultiMediaCategoryName, typeof(SMSManager))]
        [StateComponentMenu(SMSHelperExtension.MultiMediaCategoryName + "/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(LoopAnimator))]
        [Tip("循环动画组件是播放Unity的Animator动画的对象。可设置播放动画的循环次数，播放完之后，组件切换为完成态。")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        public override void Reset()
        {
            base.Reset();

            loopType =  ELoopType.Loop;
        }
    }
}
