using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Dataflows.FieldBinds
{
    /// <summary>
    /// MonoBehaviour参数绑定:MonoBehaviour参数绑定组件是用于将继承于MonoBehaviour组件的属性与一个全局变量绑定在一起，并让属性与变量值保持同步的执行体。状态没退出时，会一直同步属性与变量值。组件在激活之后随即切换为完成态。
    /// </summary>
    [ComponentMenu(DataflowHelper.FieldBinds + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(MonoBehaviourParamBind))]
    [Tip("MonoBehaviour参数绑定组件是用于将继承于MonoBehaviour组件的属性与一个全局变量绑定在一起，并让属性与变量值保持同步的执行体。状态没退出时，会一直同步属性与变量值。组件在激活之后随即切换为完成态。")]
    [XCSJ.Attributes.Icon(index = 33651)]
    public class MonoBehaviourParamBind : FieldBind<MonoBehaviourParamBind>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "MonoBehaviour参数绑定";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib(DataflowHelper.FieldBinds, typeof(SMSManager))]
        [StateComponentMenu(DataflowHelper.FieldBinds + "/" + Title, typeof(SMSManager))]
#endif
        [Name(Title, nameof(MonoBehaviourParamBind))]
        [Tip("MonoBehaviour参数绑定组件是用于将继承于MonoBehaviour组件的属性与一个全局变量绑定在一起，并让属性与变量值保持同步的执行体。状态没退出时，会一直同步属性与变量值。组件在激活之后随即切换为完成态。")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// Mono组件
        /// </summary>
        [Name("Mono组件")]
        [ComponentPopup(typeof(MonoBehaviour))]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public MonoBehaviour monoBehaviour;

        /// <summary>
        /// 参数名
        /// </summary>
        [Name("参数名")]
        public string paramName;

        /// <summary>
        /// 对象
        /// </summary>
        public override object obj => monoBehaviour;

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
