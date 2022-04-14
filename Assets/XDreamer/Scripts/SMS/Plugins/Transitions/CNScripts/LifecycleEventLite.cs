using System;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.CNScripts;

namespace XCSJ.PluginSMS.Transitions.CNScripts
{
    [ComponentMenu("中文脚本/生命周期事件简版", typeof(SMSManager))]
    [Name("生命周期事件简版")]
    [Serializable]
    public class LifecycleEventLite : TransitionScriptComponent<ELifecycleEventLite, LifecycleEventLiteSet>
    {
        public override void Reset()
        {
            base.Reset();

            GetScriptListFromList(ELifecycleEventLite.OnEntry).Enable = true;
            GetScriptListFromList(ELifecycleEventLite.OnExit).Enable = true;
        }

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            RunScriptEventWithNamePath(ELifecycleEventLite.OnEntry);
        }

        public override void OnExit(StateData data)
        {
            if (ScriptManager.instance) RunScriptEventWithNamePath(ELifecycleEventLite.OnExit);
        }

        public override void OnUpdate(StateData data)
        {
            RunScriptEventWithNamePath(ELifecycleEventLite.OnUpdate);
        }
    }
}
