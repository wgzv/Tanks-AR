using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 游戏对象闪烁:游戏对象闪烁组件是游戏对象闪烁的动画。游戏对象在设定的时间区间内按设定的频率闪烁，在播放完毕后，组件切换为完成态。
    /// </summary>
    [ComponentMenu("动作/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(GameObjectFlash))]
    [Tip("游戏对象闪烁组件是游戏对象闪烁的动画。游戏对象在设定的时间区间内按设定的频率闪烁，在播放完毕后，组件切换为完成态。")]
    [XCSJ.Attributes.Icon(index = 33621)]
    [RequireComponent(typeof(GameObjectSet))]
    public class GameObjectFlash : Flash<GameObjectFlash, GameObjectFlash.Recorder>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "游戏对象闪烁";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("动作", typeof(SMSManager))]
        [StateComponentMenu("动作/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(GameObjectFlash))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("游戏对象闪烁组件是游戏对象闪烁的动画。游戏对象在设定的时间区间内按设定的频率闪烁，在播放完毕后，组件切换为完成态。")]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        public GameObjectSet gameObjectSet => GetComponent<GameObjectSet>(true);

        public class Recorder : GameObjectRecorder, IPercentRecorder<GameObjectFlash>
        {
            public GameObjectFlash flash;

            public void Record(GameObjectFlash flash)
            {
                this.flash = flash;
                if (!flash.gameObjectSet) return;
                _records.Clear();
                Record(flash.gameObjectSet.objects);
            }

            public void SetPercent(Percent percent)
            {
                foreach (var info in _records)
                {
                    info.gameObject.SetActive(!flash.inChangeArea);
                }
            }
        }
    }
}
