using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.Base
{
    /// <summary>
    /// 游戏对象激活：游戏对象激活组件是控制游戏对象激活或非激活的执行体。随着状态生命周期发生的事件（进入和退出），激活和非激活设置的游戏对象，组件激活后即刻切换为完成态。
    /// </summary>
    [ComponentMenu(SMSHelperExtension.CommonUseCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(GameObjectActive))]
    [Tip("游戏对象激活组件是控制游戏对象激活或非激活的执行体。随着状态生命周期发生的事件（进入和退出），激活和非激活设置的游戏对象，组件激活后即刻切换为完成态。")]
    [XCSJ.Attributes.Icon(EIcon.GameObjectActive)]
    [RequireComponent(typeof(GameObjectSet))]
    public class GameObjectActive : StateComponent<GameObjectActive>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "游戏对象激活";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.CommonUseCategoryName, typeof(SMSManager))]
        [StateComponentMenu(SMSHelperExtension.CommonUseCategoryName + "/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(GameObjectActive))]
        [Tip("游戏对象激活组件是控制游戏对象激活或非激活的执行体。随着状态生命周期发生的事件（进入和退出），激活和非激活设置的游戏对象，组件激活后即刻切换为完成态。")]
        [XCSJ.Attributes.Icon(EIcon.GameObjectActive)]
        public static State CreateGameObjectSet(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("初始化激活")]
        [EnumPopup]
        public EBool initActive = EBool.None;

        [Name("进入激活")]
        [EnumPopup]
        public EBool entryActive = EBool.Yes;

        [Name("退出激活")]
        [EnumPopup]
        public EBool exitActive = EBool.None;

        public GameObjectSet gameObjectSet => GetComponent<GameObjectSet>();

        private GameObjectSet _gameObjectSet;

        public override bool Init(StateData data)
        {
            _gameObjectSet = gameObjectSet;

            SetGameObjectsActive(initActive);
            return base.Init(data);
        }

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            SetGameObjectsActive(entryActive);
        }

        public override void OnExit(StateData data)
        {
            base.OnExit(data);

            SetGameObjectsActive(exitActive);
        }

        public override bool Finished()
        {
            return true;
        }

        private void SetGameObjectsActive(EBool active)
        {
            try
            {
                if (!_gameObjectSet || active == EBool.None) return;

                _gameObjectSet.objects.ForEach(go =>
                {
                    go.XSetActive(active);

                });
            }
            catch { }
        }
    }
}
