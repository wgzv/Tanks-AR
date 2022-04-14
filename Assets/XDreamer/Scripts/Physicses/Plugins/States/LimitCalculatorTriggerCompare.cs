using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginPhysicses.Base.Limits;
using XCSJ.PluginPhysicses.Tools.Interactors;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginPhysicses.States
{
    /// <summary>
    /// 限定计算触发器比较：用于监听限定计算触发器触发的状态，并与对应值进行比较
    /// </summary>
    [ComponentMenu(PhysicsManager.Title + "/" + Title, typeof(PhysicsManager))]
    [Name(Title, nameof(LimitCalculatorTriggerCompare))]
    [Tip("用于监听限定计算触发器触发的状态，并与对应值进行比较")]
    [XCSJ.Attributes.Icon(EIcon.JoyStick)]
    [DisallowMultipleComponent]
    public class LimitCalculatorTriggerCompare : Trigger<LimitCalculatorTriggerCompare>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "限定计算触发器比较";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(PhysicsManager.Title, typeof(PhysicsManager))]
        [StateComponentMenu(PhysicsManager.Title + "/" + Title, typeof(PhysicsManager))]
        [Name(Title, nameof(LimitCalculatorTriggerCompare))]
        [Tip("用于监听限定计算触发器触发的状态，并与对应值进行比较")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 旋转限定触发器
        /// </summary>
        [Name("旋转限定触发器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public LimitCalculatorTrigger _limitCalculatorTrigger;

        /// <summary>
        /// 触发状态
        /// </summary>
        [Name("触发状态")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_limitCalculatorTrigger), EValidityCheckType.Null)]
        public ESwitchState _switchState = ESwitchState.On;

        private ESwitchState switchState = ESwitchState.None;

        /// <summary>
        /// 触发状态
        /// </summary>
        [Name("触发状态")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_limitCalculatorTrigger), EValidityCheckType.Null)]
        public EMinMiddleMaxState _minMiddleMaxState = EMinMiddleMaxState.Max;

        private EMinMiddleMaxState minMiddleMaxstate = EMinMiddleMaxState.None;

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);

            switchState = ESwitchState.None;
            minMiddleMaxstate = EMinMiddleMaxState.None;

            LimitCalculatorTrigger.onSwitchStateChanged += OnSwitchStateChanged;
            LimitCalculatorTrigger.onMinMiddleMaxStateChanged += OnMinMiddleMaxStateChanged;
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);

            LimitCalculatorTrigger.onSwitchStateChanged -= OnSwitchStateChanged;
            LimitCalculatorTrigger.onMinMiddleMaxStateChanged -= OnMinMiddleMaxStateChanged;
        }

        /// <summary>
        /// 数据有效
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return _limitCalculatorTrigger;
        }

        /// <summary>
        /// 提示字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            var str = "";
            if (_limitCalculatorTrigger)
            {
                str += _limitCalculatorTrigger.name;
                switch (_limitCalculatorTrigger._limitCalculatorType)
                {
                    case ELimitCalculatorType.Switch:
                        {
                            str += ":" + CommonFun.Name(_switchState);
                            break;
                        }
                    case ELimitCalculatorType.MinMiddleMax:
                        {
                            str += ":" + CommonFun.Name(_minMiddleMaxState);
                            break;
                        }
                }
            }
            return str;
        }

        /// <summary>
        /// 状态组件完成态判定
        /// </summary>
        /// <returns></returns>
        public override bool Finished() 
        {
            if (!DataValidity()) return false;

            switch (_limitCalculatorTrigger._limitCalculatorType)
            {
                case ELimitCalculatorType.Switch: return switchState == _switchState;
                case ELimitCalculatorType.MinMiddleMax: return minMiddleMaxstate == _minMiddleMaxState;
                case ELimitCalculatorType.None:
                default: return false;
            }
        }

        private void OnSwitchStateChanged(LimitCalculatorTrigger trigger, ESwitchState state)
        {
            if (_limitCalculatorTrigger == trigger)
            {
                this.switchState = state;
            }
        }

        private void OnMinMiddleMaxStateChanged(LimitCalculatorTrigger trigger, EMinMiddleMaxState state)
        {
            if (_limitCalculatorTrigger == trigger)
            {
                this.minMiddleMaxstate = state;
            }
        }
    }
}