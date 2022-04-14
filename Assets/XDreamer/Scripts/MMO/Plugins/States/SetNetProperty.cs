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
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.CNScripts;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;
using XCSJ.Tools;

namespace XCSJ.PluginMMO.States
{
    /// <summary>
    /// 设置网络属性的值；设置之后会执行网络同步，本地到网络；
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [ComponentMenu(MMOHelper.CategoryName + "/" + Title, typeof(MMOManager))]
    [Name(Title, nameof(SetNetProperty))]
    [Tip("设置网络属性的值；设置之后会执行网络同步，本地到网络；")]
    [RequireManager(typeof(MMOManager))]
    public class SetNetProperty : StateComponent<SetNetProperty>, IDropdownPopupAttribute
    {
        public const string Title = "设置网络属性";

        [StateLib(MMOHelper.CategoryName, typeof(MMOManager))]
        [StateComponentMenu(MMOHelper.CategoryName + "/" + Title, typeof(MMOManager))]
        [Name(Title, nameof(SetNetProperty))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("设置网络属性的值；设置之后会执行网络同步，本地到网络；")]
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
        [Tip("期望设置属性的名称")]
        [NetPropertyName]
        [FormerlySerializedAs(nameof(propertyName))]
        public string _propertyName;

        /// <summary>
        /// 属性名
        /// </summary>
        public string propertyName { get => _propertyName; set => _propertyName = value; }

        [Name("值类型")]
        [Tip("属性值的类型")]
        [FormerlySerializedAs(nameof(valueType))]
        [EnumPopup]
        public EValueType _valueType = EValueType.Value;

        public EValueType valueType { get => _valueType; set => _valueType = value; }

        [Name("属性值")]
        [GlobalVariable(nameof(_valueType), EValidityCheckType.Equal, EValueType.Variable, enumMemberName = nameof(propertyName))]
        [HideInSuperInspector(nameof(_valueType), EValidityCheckType.NotEqual | EValidityCheckType.And, EValueType.Value, nameof(_valueType), EValidityCheckType.NotEqual, EValueType.Variable)]
        [FormerlySerializedAs(nameof(propertyValue))] 
        public string _propertyValue;

        [Name("尝试状态切换")]
        [Tip("尝试状态切换；会将属性进行状态切换操作；即 0 与 1、#True 与 #False、True 与 False 、Yes 与 No 、Y 与 N 字符串信息互相切换；如果属性为空字符串或为不可作为状态状态的则使用输入的属性值进行设置；")]
        public bool _tryStateSwitch = false;

        public string propertyValue { get => _propertyValue; set => _propertyValue = value; }

        [Name("游戏对象")]
        [Tip("属性值游戏对象")]
        [HideInSuperInspector(nameof(_valueType), EValidityCheckType.Less | EValidityCheckType.Or, EValueType.GameOjbectName, nameof(_valueType), EValidityCheckType.Greater, EValueType.GameOjbectAlias)]
        [FormerlySerializedAs(nameof(go))]
        public GameObject _go = null;

        public GameObject go { get => _go; set => _go = value; }

        [Name("设置时机")]
        [Tip("设置网络属性在组件生命周期事件中的执行时机;即在何种事件发生时设置网络属性;")]
        [FormerlySerializedAs(nameof(setTime))]
        [EnumPopup]
        public ELifecycleEvent _setTime = ELifecycleEvent.OnEntry;

        public ELifecycleEvent setTime { get => _setTime; set => _setTime = value; }

        [Name("强制设置")]
        [Tip("新值与旧值相同时，是否强制设置网络属性")]
        [FormerlySerializedAs(nameof(mustSet))]
        public bool _mustSet = false;
        public bool mustSet { get => _mustSet; set => _mustSet = value; }

        public string propertyValueString => valueType.GetValueString(propertyValue, go);

        private void SetPropertyValue(ELifecycleEvent lifecycleEvent)
        {
            if (netProperty && lifecycleEvent == setTime)
            {
                netProperty.SetProperty(propertyName, _tryStateSwitch ? SwitchState() : valueType.GetValue(propertyValue, go), mustSet);
            }
        }

        private string SwitchState()
        {
            if (netProperty.GetProperty(propertyName) is Property property)
            {
                string rs = property.value;
                switch (rs)
                {
                    case "0": rs = "1"; break;
                    case "1": rs = "0"; break;
                    case "#True": rs = "#False"; break;
                    case "#False": rs = "#True"; break;
                    case "True": rs = "False"; break;
                    case "False": rs = "True"; break;
                    case "Yes": rs = "No"; break;
                    case "No": rs = "Yes"; break;
                    case "Y": rs = "N"; break;
                    case "N": rs = "Y"; break;
                    default: rs = valueType.GetValue(propertyValue, go); break;
                }
                return rs;
            }
            return "";
        }

        public override bool Finished() => true;

        public override void OnBeforeEntry(StateData data)
        {
            base.OnBeforeEntry(data);
            SetPropertyValue(ELifecycleEvent.OnBeforeEntry);
        }

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);
            SetPropertyValue(ELifecycleEvent.OnEntry);
        }

        public override void OnAfterEntry(StateData data)
        {
            base.OnAfterEntry(data);
            SetPropertyValue(ELifecycleEvent.OnAfterEntry);
        }

        public override void OnUpdate(StateData data)
        {
            base.OnUpdate(data);
            SetPropertyValue(ELifecycleEvent.OnUpdate);
        }

        public override void OnBeforeExit(StateData data)
        {
            base.OnBeforeExit(data);
            SetPropertyValue(ELifecycleEvent.OnBeforeExit);
        }

        public override void OnExit(StateData data)
        {
            base.OnExit(data);
            SetPropertyValue(ELifecycleEvent.OnExit);
        }

        public override void OnAfterExit(StateData data)
        {
            base.OnAfterExit(data);
            SetPropertyValue(ELifecycleEvent.OnAfterExit);
        }

        public override string ToFriendlyString()
        {
            return propertyName + ".属性值" + (_tryStateSwitch ? ":状态切换" : (VariableCompareHelper.ToAbbreviations(ECompareType.Equal) + propertyValueString));
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
