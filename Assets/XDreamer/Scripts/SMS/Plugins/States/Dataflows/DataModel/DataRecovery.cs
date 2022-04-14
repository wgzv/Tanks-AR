using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.CNScripts;

namespace XCSJ.PluginSMS.States.Dataflows.DataModel
{
    /// <summary>
    /// 数据恢复；用于恢复数据记录器中记录的数据信息；
    /// </summary>
    [ComponentMenu(DataflowHelper.DataModel + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(DataRecovery))]
    [Tip("用于恢复数据记录器中记录的数据信息")]
    [XCSJ.Attributes.Icon(EIcon.State)]
    public class DataRecovery : StateComponent<DataRecovery>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "数据恢复";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib(DataflowHelper.DataModel, typeof(SMSManager))]
        [StateComponentMenu(DataflowHelper.DataModel + "/" + Title, typeof(SMSManager))]
#endif
        [Name(Title, nameof(DataRecovery))]
        [Tip("用于恢复数据记录器中记录的数据信息")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 数据记录器
        /// </summary>
        [Name("数据记录器")]
        [StateComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public DataRecorder dataRecorder;

        /// <summary>
        /// 数据恢复模式:数据恢复到数据记录器中记录的哪个时刻的数据；仅可选择单一值，不支持多值操作；
        /// </summary>
        [Name("数据恢复模式")]
        [Tip("数据恢复到数据记录器中记录的哪个时刻的数据；仅可选择单一值，不支持多值操作；")]
        [EnumPopup]
        public EDataRecordMode dataRecoveryMode = EDataRecordMode.Init;

        /// <summary>
        /// 数据恢复规则
        /// </summary>
        [Name("数据恢复规则")]
        [EnumPopup]
        public EDataRecoveryRule dataRecoveryRule = EDataRecoveryRule.All;

        /// <summary>
        /// 数据恢复规则值类型
        /// </summary>
        [Name("数据恢复规则值类型")]
        [EnumPopup]
        public EDataRecoveryRuleValueType dataRecoveryRuleValueType = EDataRecoveryRuleValueType.None;

        /// <summary>
        /// 数据恢复规则值类型
        /// </summary>
        [Name("数据恢复规则值类型")]
        public enum EDataRecoveryRuleValueType
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 值
            /// </summary>
            [Name("值")]
            Value,

            /// <summary>
            /// 变量
            /// </summary>
            [Name("变量")]
            Variable,
        }

        /// <summary>
        /// 数据恢复规则值
        /// </summary>
        [Name("数据恢复规则值")]
        [HideInSuperInspector(nameof(dataRecoveryRuleValueType), EValidityCheckType.NotEqual, EDataRecoveryRuleValueType.Value)]
        public string dataRecoveryRuleValue = "";

        /// <summary>
        /// 数据恢复规则变量名
        /// </summary>
        [Name("数据恢复规则变量名")]
        [GlobalVariable]
        [HideInSuperInspector(nameof(dataRecoveryRuleValueType), EValidityCheckType.NotEqual, EDataRecoveryRuleValueType.Variable)]
        public string dataRecoveryRuleVariable = "";

        /// <summary>
        /// 恢复时机:用于标识在当前状态组件生命周期的哪个时刻执行数据记录器的数据恢复操作
        /// </summary>
        [Name("恢复时机")]
        [Tip("用于标识在当前状态组件生命周期的哪个时刻执行数据记录器的数据恢复操作")]
        [EnumPopup]
        public ELifecycleEvent recoveryTime = ELifecycleEvent.OnEntry;

        private string GetDataRecoveryRuleValue()
        {
            switch (dataRecoveryRuleValueType)
            {
                case EDataRecoveryRuleValueType.Value: return dataRecoveryRuleValue;
                case EDataRecoveryRuleValueType.Variable:
                    {
                        if (ScriptManager.TryGetGlobalVariableValue(dataRecoveryRuleVariable, out string value))
                        {
                            return value;
                        }
                        return "";
                    }
                case EDataRecoveryRuleValueType.None:
                default: return "";
            }
        }

        private void Recover(ELifecycleEvent recoveryTime)
        {
            if (this.recoveryTime == recoveryTime)
            {
                dataRecorder.Recover(dataRecoveryMode, dataRecoveryRule, GetDataRecoveryRuleValue());
            }
        }

        /// <summary>
        /// 进入激活态前回调
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnBeforeEntry(StateData stateData)
        {
            base.OnBeforeEntry(stateData);
            Recover(ELifecycleEvent.OnBeforeEntry);
        }

        /// <summary>
        /// 进入激活时回调
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);
            Recover(ELifecycleEvent.OnEntry);
        }

        /// <summary>
        /// 进入激活态之后回调
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnAfterEntry(StateData stateData)
        {
            base.OnAfterEntry(stateData);
            Recover(ELifecycleEvent.OnAfterEntry);
        }

        /// <summary>
        /// 更新时回调
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnUpdate(StateData stateData)
        {
            base.OnUpdate(stateData);
            Recover(ELifecycleEvent.OnUpdate);
        }

        /// <summary>
        /// 退出激活态前回调
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnBeforeExit(StateData stateData)
        {
            base.OnBeforeExit(stateData);
            Recover(ELifecycleEvent.OnBeforeExit);
        }

        /// <summary>
        /// 退出激活态时回调
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
            Recover(ELifecycleEvent.OnExit);
        }

        /// <summary>
        /// 退出激活态之后回调
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnAfterExit(StateData stateData)
        {
            base.OnAfterExit(stateData);
            Recover(ELifecycleEvent.OnAfterExit);
        }

        /// <summary>
        /// 检测当前对象是否处于完成态
        /// </summary>
        /// <returns></returns>
        public override bool Finished() => true;

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return (dataRecorder ? dataRecorder.parent.name : "") + "->" + CommonFun.Name(dataRecoveryMode);
        }

        /// <summary>
        /// 数据有效性；对当前对象的数据进行有效性判断；仅判断，不做其它处理；
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return base.DataValidity() && dataRecorder && (dataRecorder.dataRecordMode & dataRecoveryMode) != 0;
        }
    }

    /// <summary>
    /// 数据恢复规则
    /// </summary>
    [Name("数据恢复规则")]
    public enum EDataRecoveryRule
    {
        /// <summary>
        /// 所有:将所有数据记录信息的全部恢复
        /// </summary>
        [Name("所有")]
        [Tip("将所有数据记录信息的全部恢复")]
        All = -1,

        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None = 0,

        /// <summary>
        /// 名称相等
        /// </summary>
        [Name("名称相等")]
        NameEquals,

        /// <summary>
        /// 名称不相等
        /// </summary>
        [Name("名称不相等")]
        NameNotEquals,

        /// <summary>
        /// 名称包含
        /// </summary>
        [Name("名称包含")]
        NameContains,

        /// <summary>
        /// 名称不包含
        /// </summary>
        [Name("名称不包含")]
        NameNotContains,

        /// <summary>
        /// 名称正则匹配
        /// </summary>
        [Name("名称正则匹配")]
        NameRegexMatch,

        /// <summary>
        /// 名称正则不匹配
        /// </summary>
        [Name("名称正则不匹配")]
        NameRegexNotMatch,

        /// <summary>
        /// 是游戏对象的子级(通过名称路径)
        /// </summary>
        [Name("是游戏对象的子级(通过名称路径)")]
        IsChildOfGameObjectByNamePath,

        /// <summary>
        /// 不是游戏对象的子级(通过名称路径)
        /// </summary>
        [Name("不是游戏对象的子级(通过名称路径)")]
        NotIsChildOfGameObjectByNamePath,
    }
}
