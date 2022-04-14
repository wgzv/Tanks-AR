using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Components;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 组件闪烁:组件闪烁组件是组件闪烁的动画。组件在设定的时间区间内按设定的频率闪烁，在播放完毕后，组件切换为完成态。
    /// </summary>
    [ComponentMenu("动作/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(ComponentFlash))]
    [Tip("组件闪烁组件是组件闪烁的动画。组件在设定的时间区间内按设定的频率闪烁，在播放完毕后，组件切换为完成态。")]
    [XCSJ.Attributes.Icon(index = 33619)]
    [RequireComponent(typeof(ComponentSet))]
    public class ComponentFlash : Flash<ComponentFlash, ComponentFlash.Recorder>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "组件闪烁";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("动作", typeof(SMSManager))]
        [StateComponentMenu("动作/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(ComponentFlash))]
        [Tip("组件闪烁组件是组件闪烁的动画。组件在设定的时间区间内按设定的频率闪烁，在播放完毕后，组件切换为完成态。")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        public ComponentSet componentSet => GetComponent<ComponentSet>(true);

        public class Recorder : ComponentRecorder, IPercentRecorder<ComponentFlash>
        {
            public ComponentFlash flash;

            public void Record(ComponentFlash flash)
            {
                this.flash = flash;
                if (!flash.componentSet) return;
                _records.Clear();
                Record(flash.componentSet.objects);
            }

            public void SetPercent(Percent percent)
            {
                foreach (var info in _records)
                {
                    info.component.XSetEnable(!flash.inChangeArea);
                }
            }
        }
    }
}
