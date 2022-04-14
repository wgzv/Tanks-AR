using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginMMO.NetSyncs;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.Transitions.Base;

namespace XCSJ.PluginMMO.Transitions
{
    /// <summary>
    /// 将网络属性的值与待比较值进行比较判断；
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [ComponentMenu(MMOHelper.CategoryName + "/" + Title, typeof(MMOManager))]
    [Name(Title, nameof(NetPropertyCompare))]
    [Tip("将网络属性的值与待比较值进行比较判断")]
    [RequireManager(typeof(MMOManager))]
    public class NetPropertyCompare : TransitionComponent, IDropdownPopupAttribute
    {
        public const string Title = "网络属性比较";

        [Name("网络属性")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [FormerlySerializedAs(nameof(netProperty))]
        public NetProperty _netProperty;

        /// <summary>
        /// 网络属性
        /// </summary>
        public NetProperty netProperty { get => _netProperty; set => _netProperty = value; }

        [Name("属性名")]
        [Tip("期望比较的网络属性的名称")]
        [NetPropertyName]
        [FormerlySerializedAs(nameof(propertyName))]
        public string _propertyName;

        /// <summary>
        /// 属性名
        /// </summary>
        public string propertyName { get => _propertyName; set => _propertyName = value; }

        [Name("比较类型")]
        [FormerlySerializedAs(nameof(compareType))]
        [EnumPopup]
        public ECompareType _compareType = ECompareType.Equal;

        public ECompareType compareType { get => _compareType; set => _compareType = value; }

        [Name("值类型")]
        [Tip("待比较值的类型")]
        [EnumPopup]
        public EValueType _valueType = EValueType.Value;

        public EValueType valueType { get => _valueType; set => _valueType = value; }

        [Name("待比较值")]
        [GlobalVariable(nameof(_valueType), EValidityCheckType.Equal, EValueType.Variable, enumMemberName = nameof(propertyName))]
        [HideInSuperInspector(nameof(_valueType), EValidityCheckType.NotEqual | EValidityCheckType.And, EValueType.Value, nameof(_valueType), EValidityCheckType.NotEqual, EValueType.Variable)]
        [FormerlySerializedAs(nameof(compareValue))]
        public string _compareValue;

        public string compareValue { get => _compareValue; set => _compareValue = value; }

        [Name("游戏对象")]
        [Tip("待比较值游戏对象")]
        [HideInSuperInspector(nameof(_valueType), EValidityCheckType.Less | EValidityCheckType.Or, EValueType.GameOjbectName, nameof(_valueType), EValidityCheckType.Greater, EValueType.GameOjbectAlias)]
        [FormerlySerializedAs(nameof(go))]
        public GameObject _go = null;

        public GameObject go { get => _go; set => _go = value; }

        [Name("比较规则")]
        [FormerlySerializedAs(nameof(compareRule))]
        [EnumPopup]
        public ECompareRule _compareRule = ECompareRule.String;

        public ECompareRule compareRule { get => _compareRule; set => _compareRule = value; }

        /// <summary>
        /// 待比较值字符串
        /// </summary>
        public string compareValueString => valueType.GetValueString(compareValue, go);

        public override bool Finished()
        {
            return base.Finished() || Check();
        }

        private bool Check()
        {
            if (netProperty && netProperty.GetProperty(propertyName) is Property property)
            {
                return VariableCompareHelper.ValueCompareValue(property.value, compareType, valueType.GetValue(compareValue, go), compareRule);
            }
            return false;
        }

        //public override void OnUpdate(StateData data)
        //{
        //    base.OnUpdate(data);

        //    if (canTrigger)
        //    {
        //        finished = Check();
        //    }
        //}

        public override string ToFriendlyString()
        {
            return propertyName + ".属性值" + VariableCompareHelper.ToAbbreviations(compareType) + compareValueString;
        }

        public override bool DataValidity()
        {
            return base.DataValidity() && netProperty;
        }

        bool IDropdownPopupAttribute.TryGetOptions(string purpose, string propertyPath, out string[] options)
        {
            switch (purpose)
            {
                case nameof(NetPropertyNameAttribute):
                    {
                        options = netProperty ? netProperty.propertys.Cast(p => p.name).ToArray() : Empty<string>.Array;
                        return true;
                    }
            }
            options = default;
            return false;
        }
        bool IDropdownPopupAttribute.TryGetOption(string purpose, string propertyPath, object propertyValue, out string option)
        {
            switch (purpose)
            {
                case nameof(NetPropertyNameAttribute):
                    {
                        option = (propertyValue as string) ?? "";
                        return true;
                    }
            }
            option = default;
            return false;
        }

        bool IDropdownPopupAttribute.TryGetPropertyValue(string purpose, string propertyPath, string option, out object propertyValue)
        {
            switch (purpose)
            {
                case nameof(NetPropertyNameAttribute):
                    {
                        propertyValue = option;
                        return true;
                    }
            }
            propertyValue = default;
            return false;
        }
    }
}
