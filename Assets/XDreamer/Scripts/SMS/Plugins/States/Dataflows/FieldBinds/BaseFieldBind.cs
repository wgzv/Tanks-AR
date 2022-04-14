using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginSMS.States.Dataflows.FieldBinds
{
    /// <summary>
    /// 基础字段绑定
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseFieldBind<T> : StateComponent<T> where T : BaseFieldBind<T>
    {
        /// <summary>
        /// 实时更新:为True时,将实时刷新被绑定的对象信息,主要用于绑定的对象为绑定对象类型时使用；为False时,仅在进入时刷新一次被绑定的对象信息；
        /// </summary>
        [Name("实时更新")]
        [Tip("为True时,将实时刷新被绑定的对象信息,主要用于绑定的对象为绑定对象类型时使用；为False时,仅在进入时刷新一次被绑定的对象信息；")]
        public bool realtimeUpdate = false;

        /// <summary>
        /// 进入时规则:在进入时，根据所选规则进行一次数据同步；'双向'在本规则下失效
        /// </summary>
        [Name("进入时规则")]
        [Tip("在进入时，根据所选规则进行一次数据同步；'双向'在本规则下失效")]
        [EnumPopup]
        public EBindRule entryRule = EBindRule.ObjectToVariable;

        /// <summary>
        /// 绑定规则
        /// </summary>
        [Name("绑定规则")]
        [EnumPopup]
        public EBindRule bindRule = EBindRule.Both;

        /// <summary>
        /// 精简模式:本参数仅在关联对象为数组或链表时生效;如勾选,则数组或链表中数据以精简分隔符进行分割并进行解析与生成；不勾选，则使用Json模式，此时可处理数组或链表中对象为复杂类型的情况;
        /// </summary>
        [Name("精简模式")]
        [Tip("本参数仅在关联对象为数组或链表时生效;如勾选,则数组或链表中数据以精简分隔符进行分割并进行解析与生成；不勾选，则使用Json模式，此时可处理数组或链表中对象为复杂类型的情况;")]
        public bool liteMode = true;

        /// <summary>
        /// 精简分隔符:推荐使用英文逗号(',')做精简分隔符
        /// </summary>
        [Name("精简分隔符")]
        [Tip("推荐使用英文逗号(',')做精简分隔符")]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        [HideInSuperInspector(nameof(liteMode), EValidityCheckType.False)]
        public string liteSeparator = ",";

        /// <summary>
        /// 总是更新变量:为True时,会根据对象一直强制将数据同步更新到对应的变量上;为False时，则仅在对象发生修改时,才将数据同步更新到对应的变量上；
        /// </summary>
        [Name("总是更新变量")]
        [Tip("为True时,会根据对象一直强制将数据同步更新到对应的变量上;为False时，则仅在对象发生修改时,才将数据同步更新到对应的变量上；")]
        [HideInSuperInspector(nameof(bindRule), EValidityCheckType.NotEqual | EValidityCheckType.And, EBindRule.ObjectToVariable, nameof(bindRule), EValidityCheckType.NotEqual, EBindRule.Both)]
        public bool alwaysUpdateVariable = false;
    }
}
