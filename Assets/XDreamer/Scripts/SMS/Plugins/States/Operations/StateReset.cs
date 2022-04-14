﻿using System.Collections.Generic;
using System.Linq;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Operations
{
    /// <summary>
    /// 状态重置：状态重置组件是对某个状态或者状态机执行重置操作的执行体，组件执行完毕后切换为完成态。
    /// </summary>
    [ComponentMenu("状态操作/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(StateReset))]
    [XCSJ.Attributes.Icon(EIcon.Reset)]
    [Tip("状态重置组件是对某个状态或者状态机执行重置操作的执行体，组件执行完毕后切换为完成态。")]
    public class StateReset : LifecycleExecutor<StateReset>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "状态重置";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib("状态操作", typeof(SMSManager))]
        [StateComponentMenu("状态操作/" + Title, typeof(SMSManager))]
#endif
        [Name(Title, nameof(StateReset))]
        [Tip("状态重置组件是对某个状态或者状态机执行重置操作的执行体，组件执行完毕后切换为完成态。")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("状态")]
        [StatePopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public State state;

        [Name("遍历规则")]
        [EnumPopup]
        public ETraversalRule traversalRule = ETraversalRule.Entry_Any_Free;

        [Name("序列规则")]
        [Tip("对状态集中状态根据遍历规则生成的序列化结果再处理的规则")]
        [EnumPopup]
        public ESequenceRule sequenceRule = ResetData.DefaultSequenceRule;

        [Name("数据规则")]
        [Tip("重置时，状态上各状态组件数据重置的规则")]
        [EnumPopup]
        public EDataRule dataRule = EDataRule.Init;

        [Name("用户定义数据规则")]
        [HideInSuperInspector(nameof(dataRule), EValidityCheckType.NotEqual, EDataRule.UserDefine)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public string userDefineDataRule = "";

        public override void Execute(StateData data, EExecuteMode executeMode)
        {
            if (state && !IsChildOf(state))
            {
                state.Reset(new Data(data, dataRule, sequenceRule, traversalRule, userDefineDataRule));
            }
        }

        public override bool DataValidity()
        {
            return state;
        }

        public override string ToFriendlyString()
        {
            return state ? state.name : "";
        }

        public enum ETraversalRule
        {
            [Name("存储")]
            Storage = 0,

            [Name("进入_任意_游离")]
            Entry_Any_Free,

            [Name("任意_进入_游离")]
            Any_Entry_Free,
        }

        public class Data : ResetData
        {
            public ETraversalRule traversalRule { get; private set; }

            public Data(StateData stateData, EDataRule dataRule, ESequenceRule sequenceRule, ETraversalRule traversalRule, string userDefineDataRule) : base(stateData, dataRule, sequenceRule)
            {
                this.traversalRule = traversalRule;
                this.userDefineDataRule = userDefineDataRule;
            }

            protected override void OnReset(StateCollection stateCollection)
            {
                switch (traversalRule)
                {
                    case ETraversalRule.Storage:
                        {
                            Reset(stateCollection.states.ToArray(), this, sequenceRule);
                            break;
                        }
                    case ETraversalRule.Entry_Any_Free:
                        {
                            List<State> entryStates;
                            List<State> anyStates;
                            List<State> freeStates;
                            if (SMSHelper.TryGetStatesWithBreadthFirst(stateCollection, out entryStates, out anyStates, out freeStates))
                            {
                                var states = new List<State>();
                                states.AddRangeWithDistinct(entryStates);
                                states.AddRangeWithDistinct(anyStates);
                                states.AddRangeWithDistinct(freeStates);

                                Reset(states.ToArray(), this, sequenceRule);
                            }
                            break;
                        }
                    case ETraversalRule.Any_Entry_Free:
                        {
                            List<State> entryStates;
                            List<State> anyStates;
                            List<State> freeStates;
                            if (SMSHelper.TryGetStatesWithBreadthFirst(stateCollection, out entryStates, out anyStates, out freeStates))
                            {
                                var states = new List<State>();
                                states.AddRangeWithDistinct(anyStates);
                                states.AddRangeWithDistinct(entryStates);
                                states.AddRangeWithDistinct(freeStates);

                                Reset(states.ToArray(), this, sequenceRule);
                            }
                            break;
                        }
                    default:
                        {
                            base.OnReset(stateCollection);
                            break;
                        }
                }
            }
        }
    }
}
