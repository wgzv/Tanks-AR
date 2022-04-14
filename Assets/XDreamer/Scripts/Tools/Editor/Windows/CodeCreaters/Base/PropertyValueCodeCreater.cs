using System;
using UnityEditor;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorTools.Windows.CodeCreaters.Base
{
    /// <summary>
    /// 基础属性值代码生成器
    /// </summary>
    public abstract class BasePropertyValueCodeCreater : BaseCodeCreater
    {
        /// <summary>
        /// 当开始生成代码
        /// </summary>
        /// <param name="codeWirter"></param>
        protected override void OnBeginCreateCode(ICodeWirter codeWirter)
        {
            base.OnBeginCreateCode(codeWirter);
            AddUsedType(typeof(SerializableAttribute));
        }

        /// <summary>
        /// 当开始类型定义
        /// </summary>
        /// <param name="codeWirter"></param>
        protected override void OnBeginTypeDefine(ICodeWirter codeWirter)
        {
            codeWirter.WriteSummary(CommonFun.Name(targetObjectType) + "属性值");
            codeWirter.WriteFormat("[Serializable]");
            base.OnBeginTypeDefine(codeWirter);
        }
    }

    /// <summary>
    /// 基础属性值泛型代码生成器
    /// </summary>
    public abstract class BasePropertyValue_T_CodeCreater : BasePropertyValueCodeCreater
    {

    }
}
