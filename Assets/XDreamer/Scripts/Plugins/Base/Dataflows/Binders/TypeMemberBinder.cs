using System;
using System.Linq;
using System.Reflection;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.DataBinders;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Dataflows.Binders
{
    /// <summary>
    /// 类型成员绑定器
    /// </summary>
    [Serializable]
    public class TypeMemberBinder : TypeBinder, ITypeMemberBinder
    {
        /// <summary>
        /// 成员名称
        /// </summary>
        [Name("成员名称")]
        [Tip("期望绑定的成员名称")]
        [MemberNamePopup]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public string _memberName = "";

        /// <summary>
        /// 成员名称
        /// </summary>
        public override string memberName { get => _memberName; set => _memberName = value; }

        #region ITypeMemberBinder

        /// <summary>
        /// 有成员
        /// </summary>
        public override bool hasMember => true;

        /// <summary>
        /// 成员信息对象
        /// </summary>
        public override MemberInfo memberInfo
        {
            get
            {
                return mainType?.GetMember(memberName, bindingFlags).FirstOrDefault();
            }
        }

        /// <summary>
        /// 成员类型
        /// </summary>
        public override Type memberType => TypeHelper.GetMemberType(memberInfo);

        /// <summary>
        /// 成员值，当成员信息类型为字段或属性时本参数有意义；
        /// </summary>
        public override object memberValue { get => GetMemberValue(); set => SetMemberValue(value); }

        /// <summary>
        /// 实体类型
        /// </summary>
        public override Type entityType { get => memberType; }

        /// <summary>
        /// 实体对象
        /// </summary>
        public override object entityObject { get => memberValue; set => memberValue = value; }

        #endregion

        /// <summary>
        /// 获取成员值
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual object GetMemberValue(object[] index = null)
        {
            DataBinderHelper.TryGetValue(mainType, mainObject, memberName, out object value, index);
            return value;
        }

        /// <summary>
        /// 设置成员值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        public virtual void SetMemberValue(object value, object[] index = null)
        {
            DataBinderHelper.TrySetValue(mainType, mainObject, memberName, value, index);
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => base.ToFriendlyString() + "." + memberName;

        /// <summary>
        /// 数据有效性；对当前对象的数据进行有效性判断；仅判断，不做其它处理；
        /// </summary>
        /// <returns></returns>
        public virtual bool DataValidity() => memberInfo != null;
    }
}
