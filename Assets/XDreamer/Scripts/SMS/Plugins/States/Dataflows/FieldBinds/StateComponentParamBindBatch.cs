using System;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Dataflows.FieldBinds
{
    /// <summary>
    /// 状态组件参数绑定批量:状态组件参数绑定批量组件是用于将多个状态组件的属性与一个全局变量绑定在一起，并让属性与变量值保持同步的执行体。状态没退出时，会一直同步属性与变量值。组件在激活之后随即切换为完成态。
    /// </summary>
    [ComponentMenu(DataflowHelper.FieldBinds+"/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(StateComponentParamBindBatch))]
    [Tip("状态组件参数绑定批量组件是用于将多个状态组件的属性与一个全局变量绑定在一起，并让属性与变量值保持同步的执行体。状态没退出时，会一直同步属性与变量值。组件在激活之后随即切换为完成态。")]
    [XCSJ.Attributes.Icon(index = 33653)]
    public class StateComponentParamBindBatch : FieldBindBatchOfVirtual<StateComponentParamBindBatch, StateComponentBindInfo>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "状态组件参数绑定批量";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib(DataflowHelper.FieldBinds , typeof(SMSManager))]
        [StateComponentMenu(DataflowHelper.FieldBinds + "/" + Title, typeof(SMSManager))]
#endif
        [Name(Title, nameof(StateComponentParamBindBatch))]
        [Tip("状态组件参数绑定批量组件是用于将多个状态组件的属性与一个全局变量绑定在一起，并让属性与变量值保持同步的执行体。状态没退出时，会一直同步属性与变量值。组件在激活之后随即切换为完成态。")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);
    }

    /// <summary>
    /// 状态组件绑定信息
    /// </summary>
    [Serializable]
    public class StateComponentBindInfo : BindInfoOfVirtual
    {
        /// <summary>
        /// 状态组件
        /// </summary>
        [Name("状态组件")]
        [StateComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public StateComponent stateComponent;

        /// <summary>
        /// 参数名
        /// </summary>
        [Name("参数名")]
        public string paramName;

        /// <summary>
        /// 对象
        /// </summary>
        public override object obj => stateComponent;

        /// <summary>
        /// 字段名
        /// </summary>
        public override string fieldName => paramName;

        /// <summary>
        /// 字段名的字段名
        /// </summary>
        public override string fieldNameOfFieldName => nameof(paramName);
    }
}
