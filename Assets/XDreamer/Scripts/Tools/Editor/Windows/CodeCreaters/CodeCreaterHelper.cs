using System;
using System.Reflection;
using XCSJ.Algorithms;
using XCSJ.EditorTools.Windows.CodeCreaters.Base;
using XCSJ.Helper;

namespace XCSJ.EditorTools.Windows.CodeCreaters
{
    /// <summary>
    /// 代码生成器辅助类
    /// </summary>
    public static class CodeCreaterHelper
    {
        private static string IndentText(int indent) => new string(' ', indent * 4);

        /// <summary>
        /// 获取代码缩进文本
        /// </summary>
        /// <param name="indent">缩进量</param>
        /// <returns></returns>
        public static string GetCodeIndentText(this int indent) => indent > 0 ? IndentText(indent) : "";

        /// <summary>
        /// 获取代码文本
        /// </summary>
        /// <param name="prefixIndent">前缀缩进量</param>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public static string GetCodeText(int prefixIndent, string text) => prefixIndent > 0 ? IndentText(prefixIndent) + text : text;

        /// <summary>
        /// 创建代码生成器
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static BaseCodeCreater CreateCodeCreater(this Enum enumValue) => CreateCodeCreater(enumValue.GetFieldInfo());

        /// <summary>
        /// 创建代码生成器
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static BaseCodeCreater CreateCodeCreater(this MemberInfo memberInfo) => CodeCreaterTypeAttribute.CreateCodeCreater(memberInfo);
    }

    /// <summary>
    /// 代码生成器类型特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class CodeCreaterTypeAttribute : BaseLinkTypeAttribute
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="linkType"></param>
        /// <param name="linkTypeMode"></param>
        public CodeCreaterTypeAttribute(string linkType, ELinkTypeMode linkTypeMode = ELinkTypeMode.TypeFullName) : base(linkType, linkTypeMode) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="type"></param>
        public CodeCreaterTypeAttribute(Type type) : base(type) { }

        /// <summary>
        /// 创建代码生成器
        /// </summary>
        /// <param name="codeCreaterType"></param>
        /// <returns></returns>
        public static BaseCodeCreater CreateCodeCreater(MemberInfo memberInfo) => TypeHelper.CreateInstance(GetLinkType<CodeCreaterTypeAttribute>(memberInfo)) as BaseCodeCreater;
    }
}
