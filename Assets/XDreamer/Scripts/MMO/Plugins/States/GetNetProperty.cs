using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Kernel;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginMMO.NetSyncs;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.CNScripts;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;
using XCSJ.Tools;

namespace XCSJ.PluginMMO.States
{
    /// <summary>
    /// 获取网络属性的值；
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [ComponentMenu(MMOHelper.CategoryName + "/" + Title, typeof(MMOManager))]
    [Name(Title, nameof(GetNetProperty))]
    [Tip("获取网络属性的值并赋值给全局变量")]
    [RequireManager(typeof(MMOManager))]
    public class GetNetProperty : StateComponent<GetNetProperty>, IDropdownPopupAttribute
    {
        public const string Title = "获取网络属性";

        [StateLib(MMOHelper.CategoryName, typeof(MMOManager))]
        [StateComponentMenu(MMOHelper.CategoryName + "/" + Title, typeof(MMOManager))]
        [Name(Title, nameof(GetNetProperty))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("获取网络属性的值并赋值给全局变量")]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

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
        [Tip("期望获取属性的名称")]
        [NetPropertyName]
        [FormerlySerializedAs(nameof(propertyName))]
        public string _propertyName;

        /// <summary>
        /// 属性名
        /// </summary>
        public string propertyName { get => _propertyName; set => _propertyName = value; }

        [Name("变量名")]
        [Tip("将获取到的属性值存储在变量名对应的全局变量中")]
        [GlobalVariable(true)]
        [FormerlySerializedAs(nameof(variableName))]
        public string _variableName;

        public string variableName { get => _variableName; set => _variableName = value; }

        [Name("获取时机")]
        [Tip("获取网络属性在组件生命周期事件中的执行时机;即在何种事件发生时获取网络属性;")]
        [FormerlySerializedAs(nameof(getTime))]
        [EnumPopup]
        public ELifecycleEvent _getTime = ELifecycleEvent.OnEntry;

        public ELifecycleEvent getTime { get => _getTime; set => _getTime = value; }

        private void GetPropertyValue(ELifecycleEvent lifecycleEvent)
        {
            if (netProperty && lifecycleEvent == getTime && netProperty.GetProperty(propertyName) is Property property)
            {
                ScriptManager.TrySetGlobalVariableValue(variableName, property.value);
            }
        }

        public override bool Finished() => true;

        public override void OnBeforeEntry(StateData data)
        {
            base.OnBeforeEntry(data);
            GetPropertyValue(ELifecycleEvent.OnBeforeEntry);
        }

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);
            GetPropertyValue(ELifecycleEvent.OnEntry);
        }

        public override void OnAfterEntry(StateData data)
        {
            base.OnAfterEntry(data);
            GetPropertyValue(ELifecycleEvent.OnAfterEntry);
        }

        public override void OnUpdate(StateData data)
        {
            base.OnUpdate(data);
            GetPropertyValue(ELifecycleEvent.OnUpdate);
        }

        public override void OnBeforeExit(StateData data)
        {
            base.OnBeforeExit(data);
            GetPropertyValue(ELifecycleEvent.OnBeforeExit);
        }

        public override void OnExit(StateData data)
        {
            base.OnExit(data);
            GetPropertyValue(ELifecycleEvent.OnExit);
        }

        public override void OnAfterExit(StateData data)
        {
            base.OnAfterExit(data);
            GetPropertyValue(ELifecycleEvent.OnAfterExit);
        }

        public override string ToFriendlyString()
        {
            return ScriptOption.VarFlag + variableName + VariableCompareHelper.ToAbbreviations(ECompareType.Equal) + propertyName + ".属性值";
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
