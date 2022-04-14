using System;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;

namespace XCSJ.PluginSMS.CNScripts
{
    /// <summary>
    /// 生命周期事件简版
    /// </summary>
    public enum ELifecycleEventLite
    {
        [Name("进入")]
        [Tip("当事件进入时调用")]
        OnEntry,

        [Name("更新")]
        [Tip("当事件更新时调用")]
        OnUpdate,

        [Name("退出")]
        [Tip("当事件退出时调用")]
        OnExit,
    }

    /// <summary>
    /// 生命周期事件简版集合
    /// </summary>
    [Serializable]
    public class LifecycleEventLiteSet : BaseScriptEventSet<ELifecycleEventLite>
    {
    }

    /// <summary>
    /// 生命周期事件
    /// </summary>
    public enum ELifecycleEvent
    {
        [Name("预进入")]
        [Tip("当事件进入时调用")]
        OnBeforeEntry,

        [Name("进入")]
        [Tip("当事件进入时调用")]
        OnEntry,

        [Name("已进入")]
        [Tip("当事件进入时调用")]
        OnAfterEntry,

        [Name("更新")]
        [Tip("当事件更新时调用")]
        OnUpdate,

        [Name("预退出")]
        [Tip("当事件退出时调用")]
        OnBeforeExit,

        [Name("退出")]
        [Tip("当事件退出时调用")]
        OnExit,

        [Name("已退出")]
        [Tip("当事件退出时调用")]
        OnAfterExit,
    }

    /// <summary>
    /// 生命周期事件集合
    /// </summary>
    [Serializable]
    public class LifecycleEventSet : BaseScriptEventSet<ELifecycleEvent>
    {
    }
}
