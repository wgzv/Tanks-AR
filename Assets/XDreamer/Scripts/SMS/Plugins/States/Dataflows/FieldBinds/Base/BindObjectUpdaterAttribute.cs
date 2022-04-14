using System;
using XCSJ.Algorithms;

namespace XCSJ.PluginSMS.States.Dataflows.FieldBinds.Base
{
    /// <summary>
    /// 绑定对象更新器特性
    /// </summary>
    public class BindObjectUpdaterAttribute : LinkedTypeAttribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">绑定的类型</param>
        /// <param name="forChildClasses">可用于子类</param>
        public BindObjectUpdaterAttribute(Type type, bool forChildClasses = false)
           : base(type, forChildClasses, nameof(BindObjectUpdaterAttribute))
        { }
    }

    /// <summary>
    /// 对象更新器
    /// </summary>
    public interface IObjectUpdater : IObjectUpdate { }

    /// <summary>
    /// 对象更新
    /// </summary>
    public interface IObjectUpdate
    {
        /// <summary>
        /// 对象更新函数
        /// </summary>
        /// <param name="fieldBind">字段绑定的信息</param>
        /// <param name="objFieldValue">待更新对象的字段值</param>
        /// <param name="args">额外参数信息</param>
        void ObjectUpdate(IFieldBind fieldBind, object objFieldValue, params object[] args);
    }
}
