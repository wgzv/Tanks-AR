using System;
using UnityEngine.Events;
using XCSJ.Attributes;

namespace XCSJ.Extension.Base.Dataflows.Base
{
    /// <summary>
    /// 实参类型枚举
    /// </summary>
    [Name("实参类型")]
    public enum EArgumentType
    {
        /// <summary>
        /// 空
        /// </summary>
        [Name("空")]
        [ArgumentType(typeof(void))]
        Void,

        /// <summary>
        /// 变量:中文脚本中的全局变量
        /// </summary>
        [Name("变量")]
        [Tip("中文脚本中的全局变量")]
        [ArgumentType(typeof(string))]
        Variable,

        /// <summary>
        /// Unity对象
        /// </summary>
        [Name("Unity对象")]
        [ArgumentType(typeof(UnityEngine.Object))]
        UnityObject,

        /// <summary>
        /// 布尔型
        /// </summary>
        [Name("布尔型")]
        [ArgumentType(typeof(bool))]
        Bool,

        /// <summary>
        /// 整形
        /// </summary>
        [Name("整形")]
        [ArgumentType(typeof(int))]
        Int,

        /// <summary>
        /// 长整型
        /// </summary>
        [Name("长整型")]
        [ArgumentType(typeof(long))]
        Long,

        /// <summary>
        /// 浮点型
        /// </summary>
        [Name("浮点型")]
        [ArgumentType(typeof(float))]
        Float,

        /// <summary>
        /// 双精度浮点型
        /// </summary>
        [Name("双精度浮点型")]
        [ArgumentType(typeof(double))]
        Double,

        /// <summary>
        /// 字符串
        /// </summary>
        [Name("字符串")]
        [ArgumentType(typeof(string))]
        String,

        /// <summary>
        /// 枚举长整型
        /// </summary>
        [Name("枚举长整型")]
        [ArgumentType(typeof(long))]
        EnumLong,

        /// <summary>
        /// 枚举字符串
        /// </summary>
        [Name("枚举字符串")]
        [ArgumentType(typeof(string))]
        EnumString,

        /// <summary>
        /// Unity事件
        /// </summary>
        [Name("Unity事件")]
        [ArgumentType(typeof(UnityEvent))]
        UnityEvent,

        /// <summary>
        /// 链表接口类型
        /// </summary>
        [Name("链表接口类型")]
        [ArgumentType(typeof(System.Collections.IList))]
        IList,

        /// <summary>
        /// 颜色
        /// </summary>
        [Name("颜色")]
        [ArgumentType(typeof(UnityEngine.Color))]
        Color,
    }

    /// <summary>
    /// 实参类型特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ArgumentTypeAttribute : Attribute
    {
        /// <summary>
        /// 类型
        /// </summary>
        public Type type { get; private set; } = null;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="type"></param>
        public ArgumentTypeAttribute(Type type)
        {
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }
    }
}
