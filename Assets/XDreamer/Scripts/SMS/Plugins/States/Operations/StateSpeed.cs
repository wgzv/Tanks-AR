﻿using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Operations
{
    /// <summary>
    /// 状态速度:状态速度组件是设置状态速度的执行体。组件执行完毕后切换为完成态
    /// </summary>
    [ComponentMenu("状态操作/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(StateSpeed))]
    [Tip("状态速度组件是设置状态速度的执行体。组件执行完毕后切换为完成态")]
    [XCSJ.Attributes.Icon(index = 33664)]
    public class StateSpeed : LifecycleExecutor<StateSpeed>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "状态速度";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib("状态操作", typeof(SMSManager))]
        [StateComponentMenu("状态操作/"+ Title, typeof(SMSManager))]
#endif
        [Name(Title, nameof(StateSpeed))]
        [Tip("状态速度组件是设置状态速度的执行体。组件执行完毕后切换为完成态")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("状态")]
        [StatePopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public State state;

        [Name("使用变量值")]
        public bool useVariable = false;

        [Name("速度")]
        [Range(0, 32)]
        [HideInSuperInspector(nameof(useVariable), EValidityCheckType.Equal, true)]
        public float speed = 1;

        [Name("变量")]
        [GlobalVariable(true)]
        [HideInSuperInspector(nameof(useVariable), EValidityCheckType.NotEqual, true)]
        public string variable = "";

        public override void Execute(StateData data, EExecuteMode executeMode)
        {
            if (state)
            {
                if (useVariable)
                {
                    if (ScriptManager.TryGetGlobalVariableValue(variable, out string varStringValue) &&
                        float.TryParse(varStringValue, out float varFloatValue))
                    {
                        state.speed = varFloatValue;
                    }
                }
                else
                {
                    state.speed = speed;
                }
            }
        }

        public override string ToFriendlyString()
        {
            return state ? state.name : "";
        }
    }
}
