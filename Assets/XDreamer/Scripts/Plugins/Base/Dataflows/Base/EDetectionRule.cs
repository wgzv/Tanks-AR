using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;

namespace XCSJ.Extension.Base.Dataflows.Base
{
    /// <summary>
    /// 检测规则:对左值与右值执行检测的检测规则
    /// </summary>
    [Name("检测规则")]
    [Tip("对左值与右值执行检测的检测规则")]
    public enum EDetectionRule
    {
        /// <summary>
        /// 无：不做任何检测，即总认为检测规则成立
        /// </summary>
        [Name("无")]
        [Tip("不做任何检测，即总认为检测规则成立")]
        [Abbreviation("")]
        None = 0,

        /// <summary>
        /// 真：左值为True、Unity对象有效时检测规则成立;此时不对右值做任何处理
        /// </summary>
        [Name("真")]
        [Tip("左值为True、Unity对象有效时检测规则成立;此时不对右值做任何处理;")]
        [Abbreviation("真")]
        True,

        /// <summary>
        /// 假：相对'真'的反义，即左值为False、Unity对象无效时检测规则成立;此时不对右值做任何处理
        /// </summary>
        [Name("假")]
        [Tip("相对'真'的反义，即左值为False、Unity对象无效时检测规则成立;此时不对右值做任何处理")]
        [Abbreviation("假")]
        False,

        /// <summary>
        /// 默认:左值为默认值时检测规则成立;此时不对右值做任何处理
        /// </summary>
        [Name("默认")]
        [Tip("左值为默认值时检测规则成立;此时不对右值做任何处理")]
        [Abbreviation("默认")]
        Default,

        /// <summary>
        /// 非默认:相对'默认'的反义，即左值为非默认值时检测规则成立;此时不对右值做任何处理
        /// </summary>
        [Name("非默认")]
        [Tip("相对'默认'的反义，即左值为非默认值时检测规则成立;此时不对右值做任何处理")]
        [Abbreviation("非默认")]
        NotDefault,

        /// <summary>
        /// 0:左值为0时检测规则成立;此时不对右值做任何处理
        /// </summary>
        [Name("0")]
        [Tip("左值为0时检测规则成立;此时不对右值做任何处理")]
        [Abbreviation("=0")]
        Zero,

        /// <summary>
        /// 非0:相对'0'的反义，即左值为非0值时检测规则成立;此时不对右值做任何处理
        /// </summary>
        [Name("非0")]
        [Tip("相对'0'的反义，即左值为非0值时检测规则成立;此时不对右值做任何处理")]
        [Abbreviation("非0")]
        NotZero,

        /// <summary>
        /// Null:左值为null值时检测规则成立;此时不对右值做任何处理
        /// </summary>
        [Name("Null")]
        [Tip("左值为null值时检测规则成立;此时不对右值做任何处理")]
        [Abbreviation("Null")]
        Null,

        /// <summary>
        /// 非null:相对'null'的反义，即左值为非null值时检测规则成立;此时不对右值做任何处理
        /// </summary>
        [Name("非Null")]
        [Tip("相对'null'的反义，即左值为非null值时检测规则成立;此时不对右值做任何处理")]
        [Abbreviation("非Null")]
        NotNull,

        /// <summary>
        /// Null或空:左值为Null或空值时检测规则成立，用于检测空字符串、空数组、空链表;此时不对右值做任何处理
        /// </summary>
        [Name("Null或空")]
        [Tip("左值为Null或空值时检测规则成立，用于检测空字符串、空数组、空链表;此时不对右值做任何处理")]
        [Abbreviation("Null或空")]
        NullOrEmpty,

        /// <summary>
        /// 非Null或空:相对'Null或空'的反义，即左值非Null且非空值时检测规则成立，用于检测非空字符串、非空数组、非空链表;此时不对右值做任何处理
        /// </summary>
        [Name("非Null或空")]
        [Tip("相对'Null或空'的反义，即左值非Null且非空值时检测规则成立，用于检测非空字符串、非空数组、非空链表;此时不对右值做任何处理")]
        [Abbreviation("非Null或空")]
        NotNullOrEmpty,

        /// <summary>
        /// 小于:左值小于右值时检测规则成立
        /// </summary>
        [Name("小于")]
        [Tip("左值小于右值时检测规则成立")]
        [Abbreviation("＜")]
        Less,

        /// <summary>
        /// 小于等于:左值小于等于右值时检测规则成立
        /// </summary>
        [Name("小于等于")]
        [Tip("左值小于等于右值时检测规则成立")]
        [Abbreviation("≤")]
        LessEqual,

        /// <summary>
        /// 等于:左值等于右值时检测规则成立
        /// </summary>
        [Name("等于")]
        [Tip("左值等于右值时检测规则成立")]
        [Abbreviation("＝")]
        Equal,

        /// <summary>
        /// 不等于:相对'等于'的反义，即左值不等于右值时检测规则成立
        /// </summary>
        [Name("不等于")]
        [Tip("相对'等于'的反义，即左值不等于右值时检测规则成立")]
        [Abbreviation("≠")]
        NotEqual,

        /// <summary>
        /// 大于:左值大于右值时检测规则成立
        /// </summary>
        [Name("大于")]
        [Tip("左值大于右值时检测规则成立")]
        [Abbreviation("＞")]
        Greater,

        /// <summary>
        /// 大于等于:左值大于等于右值时检测规则成立
        /// </summary>
        [Name("大于等于")]
        [Tip("左值大于等于右值时检测规则成立")]
        [Abbreviation("≥")]
        GreaterEqual,

        /// <summary>
        /// 元素数目小于：左值对应的字符串、数组、链表中元素数目需小于右值时检测规则成立
        /// </summary>
        [Name("元素数目小于")]
        [Tip("左值对应的字符串、数组、链表中元素数目需小于右值时检测规则成立")]
        [Abbreviation(".数目＜")]
        ElementCountLess,

        /// <summary>
        /// 元素数目大于：左值对应的字符串、数组、链表中元素数目需大于右值时检测规则成立
        /// </summary>
        [Name("元素数目大于")]
        [Tip("左值对应的字符串、数组、链表中元素数目需大于右值时检测规则成立")]
        [Abbreviation(".数目＞")]
        ElementCountGreater,
    }
}
