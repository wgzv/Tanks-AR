using XCSJ.Attributes;

namespace XCSJ.Extension.Base.Dataflows.Links
{
    /// <summary>
    /// 绑定模式
    /// </summary>

    [Name("数据连接模式")]
    public enum EDataLinkMode
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("X")]
        [Tip("无")]
        None = 0,

        /// <summary>
        /// 数据从源到目标
        /// </summary>
        [Name("-->")]
        [Tip("源到目标")]
        ToTarget,

        /// <summary>
        /// 数据从目标到源
        /// </summary>
        [Name("<--")]
        [Tip("目标到源")]
        ToSource,

        /// <summary>
        /// 数据双向流动
        /// </summary>
        [Name("<-->")]
        [Tip("数据双向流动")]
        Both,
    }
}
