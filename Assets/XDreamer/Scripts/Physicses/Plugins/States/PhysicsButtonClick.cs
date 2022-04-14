using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginPhysicses.Base;
using XCSJ.PluginPhysicses.Tools.Interactors;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginPhysicses.States
{
    /// <summary>
    /// 物理按钮点击：监听物理按钮按下或弹起状态
    /// </summary>
    [ComponentMenu(PhysicsManager.Title + "/" + Title, typeof(PhysicsManager))]
    [Name(Title, nameof(PhysicsButtonClick))]
    [Tip("监听物理按钮按下或弹起状态")]
    [XCSJ.Attributes.Icon(EIcon.Button)]
    [DisallowMultipleComponent]
    public class PhysicsButtonClick : Trigger<PhysicsButtonClick>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "物理按钮点击";

        /// <summary>
        /// 创建物理按钮点击
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(PhysicsManager.Title, typeof(PhysicsManager))]
        [StateComponentMenu(PhysicsManager.Title + "/" + Title, typeof(PhysicsManager))]
        [Name(Title, nameof(PhysicsButtonClick))]
        [Tip("监听物理按钮按下或弹起状态")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 物理按钮
        /// </summary>
        [Name("物理按钮")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public PhysicsButton _physicsButton;

        /// <summary>
        /// 触发类型
        /// </summary>
        [Name("触发类型")]
        [EnumPopup]
        public EPhysicsButtonState _triggerState = EPhysicsButtonState.Press;

        private EPhysicsButtonState state = EPhysicsButtonState.None;

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);

            state = EPhysicsButtonState.None;

            PhysicsButton.onPressed += OnPhysicsButtonPressed;
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);

            PhysicsButton.onPressed -= OnPhysicsButtonPressed;
        }

        /// <summary>
        /// 数据有效
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return _physicsButton;
        }

        /// <summary>
        /// 提示字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return (_physicsButton ? _physicsButton.name : "") + CommonFun.Name(_triggerState);
        }

        /// <summary>
        /// 状态组件完成态判定
        /// </summary>
        /// <returns></returns>
        public override bool Finished() => state == _triggerState;

        private void OnPhysicsButtonPressed(PhysicsButton physicsButton, bool isOn)
        {
            if (_physicsButton == physicsButton)
            {
                state = isOn ? EPhysicsButtonState.Press : EPhysicsButtonState.Release;
            }
        }
    }

    /// <summary>
    /// 物理按钮状态
    /// </summary>
    [Name("物理按钮状态")]
    public enum EPhysicsButtonState
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 按下
        /// </summary>
        [Name("按下")]
        Press,

        /// <summary>
        /// 弹起
        /// </summary>
        [Name("弹起")]
        Release,
    }
}
