using UnityEngine.Serialization;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.Dataflows.PropertyBinds
{
    /// <summary>
    /// 基础获取属性
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseGetProperty<T> : BasePropertyGet<T>, IDropdownPopupAttribute
        where T : BaseGetProperty<T>
    {
        /// <summary>
        /// 绑定器:用于绑定对象的字段或属性信息的对象
        /// </summary>
        [Name("绑定器")]
        [Tip("用于绑定对象的字段或属性信息的对象")]
        [FormerlySerializedAs(nameof(_binder))]
        public FieldOrPropertyBinder _binder = new FieldOrPropertyBinder();

        /// <summary>
        /// 绑定器
        /// </summary>
        public FieldOrPropertyBinder binder => _binder;

        /// <summary>
        /// 变量名:将获取到的属性值存储在变量名对应的全局变量中
        /// </summary>
        [Name("变量名")]
        [Tip("将获取到的属性值存储在变量名对应的全局变量中")]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        [GlobalVariable(true)]
        [FormerlySerializedAs(nameof(variableName))]
        public string _variableName;

        /// <summary>
        /// 变量名
        /// </summary>
        public string variableName => _variableName;

        /// <summary>
        /// 将值设置到变量
        /// </summary>
        /// <param name="value"></param>
        protected void SetToVariable(object value)
        {
            if (Converter.instance.TryConvertTo<string>(value, out string output))
            {
                ScriptManager.TrySetGlobalVariableValue(_variableName, output);
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode) => SetToVariable(_binder.memberValue);

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return ScriptOption.VarFlag + _variableName + "=" + _binder.ToFriendlyString();
        }

        /// <summary>
        /// 判断数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return base.DataValidity() && _binder.DataValidity();
        }

        #region IDropdownPopupAttribute实现

        /// <summary>
        /// 尝试获取选项文本列表；
        /// </summary>
        /// <param name="purpose">目标用途</param>
        /// <param name="propertyPath">属性路径</param>
        /// <param name="options">选项文本列表；如果期望下拉式弹出菜单出现层级，需要数组元素中有'/'</param>
        /// <returns></returns>
        public virtual bool TryGetOptions(string purpose, string propertyPath, out string[] options)
        {
            switch (purpose)
            {
                case nameof(MemberNamePopupAttribute):
                    {
                        options = FieldOrPropertyBinder.GetMemberNames(_binder.targetType, _binder.bindField, _binder.bindingFlags, _binder.includeBaseType);
                        return true;
                    }
                case nameof(TypeFullNamePopupAttribute):
                    {
                        options = FieldOrPropertyBinder.GetTypeFullNames(_binder.bindField, _binder.bindingFlags, _binder.includeBaseType);
                        return true;
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
        public virtual bool TryGetOption(string purpose, string propertyPath, object propertyValue, out string option)
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
        public virtual bool TryGetPropertyValue(string purpose, string propertyPath, string option, out object propertyValue)
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
