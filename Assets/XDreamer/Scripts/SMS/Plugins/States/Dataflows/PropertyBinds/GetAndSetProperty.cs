using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.Dataflows.PropertyBinds
{
    /// <summary>
    /// 获取并设置属性:获取对象中指定成员的属性值(字段、属性),同时将获取到的值设置到对象中指定成员的属性值(字段、属性)
    /// </summary>
    [ComponentMenu(DataflowHelper.PropertyBinds + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(GetAndSetProperty))]
    [Tip("获取对象中指定成员的属性值(字段、属性),同时将获取到的值设置到对象中指定成员的属性值(字段、属性)")]
    [XCSJ.Attributes.Icon(EIcon.Property)]
    public class GetAndSetProperty : BaseGetProperty<GetAndSetProperty>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "获取并设置属性";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib(DataflowHelper.PropertyBinds, typeof(SMSManager))]
        [StateComponentMenu(DataflowHelper.PropertyBinds + "/" + Title, typeof(SMSManager))]
#endif
        [Name(Title, nameof(GetAndSetProperty))]
        [Tip("获取对象中指定成员的属性值(字段、属性),同时将获取到的值设置到对象中指定成员的属性值(字段、属性)")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 设置绑定器:用于设置属性时使用的绑定对象的字段或属性信息的对象列表
        /// </summary>
        [Name("设置绑定器")]
        [Tip("用于设置属性时使用的绑定对象的字段或属性信息的对象列表")]
        public List<FieldOrPropertyBinder> _setBinders = new List<FieldOrPropertyBinder>();

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            var binderValue = _binder.memberValue;

            //base.Execute(stateData, executeMode);
            SetToVariable(binderValue);
            foreach (var setBinder in _setBinders)
            {
                setBinder.SetMemberValue(binderValue);
            }
        }

        private static readonly StringBuilder friendlyString = new StringBuilder();

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            friendlyString.Clear();
            var getString = _binder.ToFriendlyString();
            if (!string.IsNullOrEmpty(_variableName))
            {
                friendlyString.AppendLine(ScriptOption.VarFlag + _variableName + "=" + getString);
            }
            foreach (var setBinder in _setBinders)
            {
                friendlyString.AppendLine(setBinder.ToFriendlyString() + "=" + getString);
            }
            return friendlyString.ToString();
        }

        /// <summary>
        /// 判断数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return base.DataValidity() && _setBinders.All(binder => binder.DataValidity());
        }

        #region IDropdownPopupAttribute实现

        /// <summary>
        /// 尝试获取选项文本列表；
        /// </summary>
        /// <param name="purpose">目标用途</param>
        /// <param name="propertyPath">属性路径</param>
        /// <param name="options">选项文本列表；如果期望下拉式弹出菜单出现层级，需要数组元素中有'/'</param>
        /// <returns></returns>
        public override bool TryGetOptions(string purpose, string propertyPath, out string[] options)
        {
            switch (purpose)
            {
                case nameof(MemberNamePopupAttribute):
                    {
                        if (propertyPath.Contains(nameof(_setBinders)))
                        {
                            var match = Regex.Match(propertyPath, @"\d+");
                            if (match.Success && _setBinders.ElementAtOrDefault(Converter.instance.ConvertTo<int>(match.Value)) is FieldOrPropertyBinder setBinder)
                            {
                                options = FieldOrPropertyBinder.GetMemberNames(setBinder.targetType, setBinder.bindField, setBinder.bindingFlags, setBinder.includeBaseType);
                                return true;
                            }
                        }
                        else
                        {
                            options = FieldOrPropertyBinder.GetMemberNames(_binder.targetType, _binder.bindField, _binder.bindingFlags, _binder.includeBaseType);
                            return true;
                        }
                        break;
                    }
                case nameof(TypeFullNamePopupAttribute):
                    {
                        if (propertyPath.Contains(nameof(_setBinders)))
                        {
                            var match = Regex.Match(propertyPath, @"\d+");
                            if (match.Success && _setBinders.ElementAtOrDefault(Converter.instance.ConvertTo<int>(match.Value)) is FieldOrPropertyBinder setBinder)
                            {
                                options = FieldOrPropertyBinder.GetTypeFullNames(setBinder.bindField, setBinder.bindingFlags, setBinder.includeBaseType);
                                return true;
                            }
                        }
                        else
                        {
                            options = FieldOrPropertyBinder.GetTypeFullNames(_binder.bindField, _binder.bindingFlags, _binder.includeBaseType);
                            return true;
                        }
                        break;
                    }
            }
            options = default;
            return false;
        }

        /// <summary>
        /// 尝试获取文本选项
        /// </summary>
        /// <param name="purpose">目标用途</param>
        /// <param name="propertyPath">属性路径</param>
        /// <param name="propertyValue">属性值</param>
        /// <param name="option">选项文本</param>
        /// <returns></returns>
        public override bool TryGetOption(string purpose, string propertyPath, object propertyValue, out string option)
        {
            switch (purpose)
            {
                case nameof(MemberNamePopupAttribute):
                    {
                        option = (propertyValue as string) ?? "";
                        return true;
                    }
                case nameof(TypeFullNamePopupAttribute):
                    {
                        option = (propertyValue as string) ?? "";
                        return true;
                    }
            }
            option = default;
            return false;
        }

        /// <summary>
        /// 尝试获取属性值
        /// </summary>
        /// <param name="purpose">目标用途</param>
        /// <param name="propertyPath">属性路径</param>
        /// <param name="option">选项文本</param>
        /// <param name="propertyValue">属性值</param>
        /// <returns></returns>
        public override bool TryGetPropertyValue(string purpose, string propertyPath, string option, out object propertyValue)
        {
            switch (purpose)
            {
                case nameof(MemberNamePopupAttribute):
                    {
                        propertyValue = option;
                        return true;
                    }
                case nameof(TypeFullNamePopupAttribute):
                    {
                        propertyValue = option;
                        return true;
                    }
            }
            propertyValue = default;
            return false;
        }

        #endregion
    }
}
