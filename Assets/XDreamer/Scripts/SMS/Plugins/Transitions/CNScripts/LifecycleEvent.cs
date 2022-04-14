using System;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.CNScripts;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.Transitions.CNScripts
{
    [Serializable]
    [ComponentMenu("中文脚本/生命周期事件", typeof(SMSManager))]
    [Name("生命周期事件")]
    public class LifecycleEvent : TransitionScriptComponent<ELifecycleEvent, LifecycleEventSet>
    {
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
            RunScriptEventWithNamePath(ELifecycleEvent.OnUpdate);
        }

        public override void OnBeforeExit(StateData data)
        {
            if (ScriptManager.instance) RunScriptEventWithNamePath(ELifecycleEvent.OnBeforeExit);
        }

        public override void OnExit(StateData data)
        {
            if (ScriptManager.instance) RunScriptEventWithNamePath(ELifecycleEvent.OnExit);
        }

        public override void OnAfterExit(StateData data)
        {
            if (ScriptManager.instance) RunScriptEventWithNamePath(ELifecycleEvent.OnAfterExit);
        }
    }
}
