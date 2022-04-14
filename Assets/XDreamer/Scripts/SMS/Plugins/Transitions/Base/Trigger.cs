using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.Transitions.Base
{
    /// <summary>
    /// 触发器:用于等待触发特定逻辑并执行对应处理规则的跳转组件
    /// </summary>
    [Name("触发器")]
    [Tip("用于等待触发特定逻辑并执行对应处理规则的跳转组件")]
    [XCSJ.Attributes.Icon(EIcon.Event)]
    public abstract class Trigger : TransitionComponent
    {
        /// <summary>
        /// 检查触发规则
        /// </summary>
        [Name("检查触发规则")]
        [EnumPopup]
        public ECheckTriggerRule _checkTriggerRule = ECheckTriggerRule.InStateFinished;

        /// <summary>
        /// 可以检查触发
        /// </summary>
        public bool canCheckTrigger
        {
            get
            {
                switch (_checkTriggerRule)
                {
                    case ECheckTriggerRule.Always: return true;
                    case ECheckTriggerRule.InStateFinished: return inStateFinished;
                    default: return false;
                }
            }
        }

        /// <summary>
        /// 触发后处理规则
        /// </summary>
        [Name("触发后处理规则")]
        [EnumPopup]
        public EHandleRuleOnTriggered _handleRuleOnTriggered = EHandleRuleOnTriggered.None;

        /// <summary>
        /// 触发器完成规则
        /// </summary>
        [Name("触发器完成规则")]
        [EnumPopup]
        public ETriggerFinishedRule _triggerFinishedRule = ETriggerFinishedRule.NeedTriggered;

        /// <summary>
        /// 标识入状态是否已完成；当前跳转处于激活工作时，本参数才有意义；
        /// </summary>
        public bool inStateFinished { get; private set; } = false;

        /// <summary>
        /// 可触发标记量:通过本标记量标识触发器事件是否已发生并允许执行触发后处理规则；
        /// </summary>
        public bool canTrigger { get; protected set; } = false;

        /// <summary>
        /// 已触发标记量
        /// </summary>
        public bool hasTriggered { get; private set; } = false;

        /// <summary>
        /// 当进入时
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            hasTriggered = false;
            canTrigger = false;
            inStateFinished = false;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnUpdate(StateData stateData)
        {
            base.OnUpdate(stateData);
            CheckTriggerInUpdate(stateData);
        }

        /// <summary>
        /// 是否完成：能执行本函数说明入状态已经处于完成状态
        /// </summary>
        /// <returns></returns>
        public override bool Finished()
        {
            inStateFinished = parent.inState.finished;
            switch (_triggerFinishedRule)
            {
                case ETriggerFinishedRule.Default: return base.Finished();
                case ETriggerFinishedRule.AlwayFinished: return true;
                case ETriggerFinishedRule.NeedTriggered: return hasTriggered;
            }
            return false;
        }

        private void CheckTriggerInUpdate(StateData stateData)
        {
            if (canCheckTrigger && canTrigger)
            {
                canTrigger = false;
                hasTriggered = true;
                OnTriggered(stateData);
            }
        }

        /// <summary>
        /// 设置触发
        /// </summary>
        public virtual void SetTrigger()
        {
            canTrigger = true;
        }

        /// <summary>
        /// 当已触发
        /// </summary>
        protected virtual void OnTriggered(StateData data)
        {
            switch (_handleRuleOnTriggered)
            {
                case EHandleRuleOnTriggered.SkipInStateAndActiveOutState:
                    {
                        OnSkip();
                        SkipHelper.Skip(data, parent);
                        break;
                    }
                case EHandleRuleOnTriggered.ActiveOutState:
                    {
                        parent.outState.active = true;
                        break;
                    }
            }
        }

        /// <summary>
        /// 当跳过
        /// </summary>
        protected virtual void OnSkip() { }
    }

    /// <summary>
    /// 检查触发器的规则
    /// </summary>
    public enum ECheckTriggerRule
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 入状态已完成：需等待入状态完成后（即检测跳转组件的完成状态时）才可触发对应的检测逻辑
        /// </summary>
        [Name("入状态已完成")]
        [Tip("需等待入状态完成后（即检测跳转组件的完成状态时）才可触发对应的检测逻辑")]
        InStateFinished,

        /// <summary>
        /// 总是
        /// </summary>
        [Name("总是")]
        [Tip("只要跳转组件处于激活态（即处于跳更新逻辑中）就可以触发对应的检测逻辑")]
        Always,
    }

    /// <summary>
    /// 已触发后处理规则
    /// </summary>
    public enum EHandleRuleOnTriggered
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 跳过入状态并激活出状态
        /// </summary>
        [Name("跳过入状态并激活出状态")]
        SkipInStateAndActiveOutState,

        /// <summary>
        /// 激活出状态
        /// </summary>
        [Name("激活出状态")]
        ActiveOutState,
    }

    /// <summary>
    /// 触发器完成规则
    /// </summary>
    public enum ETriggerFinishedRule
    {
        /// <summary>
        /// 无：总是处于未完成态
        /// </summary>
        [Name("无")]
        [Tip("总是处于未完成态")]
        None,

        /// <summary>
        /// 默认:使用默认的完成标记量
        /// </summary>
        [Name("默认")]
        [Tip("使用默认的完成标记量")]
        Default,

        /// <summary>
        /// 总是完成的:总是处于完成态
        /// </summary>
        [Name("总是完成的")]
        [Tip("总是处于完成态")]
        AlwayFinished,

        /// <summary>
        /// 需要已触发:已触发标记量为True时，标识触发器组件处于完成态
        /// </summary>
        [Name("需要已触发")]
        [Tip("已触发标记量为True时，标识触发器组件处于完成态")]
        NeedTriggered,
    }

    #region 将废弃的触发器跳转组件

    /// <summary>
    /// 将废弃的触发器跳转组件；可用于检测是否需要输入状态完成；
    /// </summary>
    public abstract class ObsoleteTrigger : TransitionComponent
    {
        [Name("需等待入状态完成")]
        [Tip("为True时，需等待入状态完成后（即检测跳转组件的完成状态时）才可触发对应的检测逻辑；为False时，在跳转组件的更新中就可以触发对应的检测逻辑")]
        public bool needWaitInStateFinished = true;

        /// <summary>
        /// 标识入状态是否已完成；当前跳转处于激活工作时，本参数才有意义；
        /// </summary>
        public bool inStateFinished { get; private set; } = false;

        /// <summary>
        /// 可触发成立的条件：不需要等待入状态完成 或是 入状态完成；
        /// </summary>
        protected virtual bool canTrigger => !needWaitInStateFinished || inStateFinished;

        /// <summary>
        /// 当进入时
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);
            inStateFinished = false;
        }

        /// <summary>
        /// 是否完成：能执行本函数说明入状态已经处于完成状态
        /// </summary>
        /// <returns></returns>
        public override bool Finished()
        {
            inStateFinished = true;
            return base.Finished();
        }
    }

    #endregion
}
