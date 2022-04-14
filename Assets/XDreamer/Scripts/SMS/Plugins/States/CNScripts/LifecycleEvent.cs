using System;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.CNScripts;

namespace XCSJ.PluginSMS.States.CNScripts
{
    /// <summary>
    /// 生命周期事件:生命周期事件组件是响应状态生命周期发生的事件并运行中文脚本的执行体。事件包括预进入、进入、已进入、更新、预已退、退出和已退出，组件激活后即刻切换为完成态。
    /// </summary>
    [Serializable]
    [ComponentMenu(SMSHelperExtension.CNScriptCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(LifecycleEvent))]
    [Tip("生命周期事件组件是响应状态生命周期发生的事件并运行中文脚本的执行体。事件包括预进入、进入、已进入、更新、预已退、退出和已退出，组件激活后即刻切换为完成态。")]
    [XCSJ.Attributes.Icon(EIcon.CNScript)]
    public class LifecycleEvent : StateScriptComponent<LifecycleEvent, ELifecycleEvent, LifecycleEventSet>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "生命周期事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.CNScriptCategoryName, typeof(SMSManager))]
        [StateComponentMenu(SMSHelperExtension.CNScriptCategoryName + "/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(LifecycleEvent))]
        [Tip("生命周期事件组件是响应状态生命周期发生的事件并运行中文脚本的执行体。事件包括预进入、进入、已进入、更新、预已退、退出和已退出，组件激活后即刻切换为完成态。")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        public override void Reset()
        {
            base.Reset();

            GetScriptListFromList(ELifecycleEvent.OnEntry).Enable = true;
            GetScriptListFromList(ELifecycleEvent.OnExit).Enable = true;
        }

        public override void OnBeforeEntry(StateData data)
        {
            base.OnBeforeEntry(data);

            RunScriptEventWithNamePath(ELifecycleEvent.OnBeforeEntry);
        }

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            RunScriptEventWithNamePath(ELifecycleEvent.OnEntry);
        }

        public override void OnAfterEntry(StateData data)
        {
            base.OnEntry(data);

            RunScriptEventWithNamePath(ELifecycleEvent.OnAfterEntry);
        }

        public override void OnUpdate(StateData data)
        {
            base.OnUpdate(data);

            RunScriptEventWithNamePath(ELifecycleEvent.OnUpdate);
        }

        public override void OnBeforeExit(StateData data)
        {
            base.OnBeforeExit(data);

            if (ScriptManager.instance) RunScriptEventWithNamePath(ELifecycleEvent.OnBeforeExit);
        }

        public override void OnExit(StateData data)
        {
            base.OnExit(data);

            if (ScriptManager.instance) RunScriptEventWithNamePath(ELifecycleEvent.OnExit);
        }

        public override void OnAfterExit(StateData data)
        {
            base.OnAfterExit(data);

            if (ScriptManager.instance) RunScriptEventWithNamePath(ELifecycleEvent.OnAfterExit);
        }
    }
}
