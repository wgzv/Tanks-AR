using System.Reflection;

namespace XCSJ.PluginSMS.States.Dataflows.FieldBinds.Base
{
    /// <summary>
    /// 字段绑定接口
    /// </summary>
    public interface IFieldBind
    {
        /// <summary>
        /// 变量
        /// </summary>
        string variable { get; }

        /// <summary>
        /// 对象
        /// </summary>
        object obj { get; }

        /// <summary>
        /// 字段名
        /// </summary>
        string fieldName { get; }

        /// <summary>
        /// 字段信息
        /// </summary>
        FieldInfo fieldInfo { get; }
    }
}
