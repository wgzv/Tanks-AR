using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
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
using XCSJ.PluginSMS.Transitions.Base;
using XCSJ.Scripts;
using XCSJ.Tools;

namespace XCSJ.PluginMMO.States
{
    /// <summary>
    /// 将网络状态的值与待比较值进行比较判断；
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [ComponentMenu(MMOHelper.CategoryName + "/" + Title, typeof(MMOManager))]
    [Name(Title, nameof(NetStateCompare))]
    [Tip("将网络状态的值与待比较值进行比较判断")]
    [RequireManager(typeof(MMOManager))]
    public class NetStateCompare : Trigger<NetStateCompare>
    {
        public const string Title = "网络状态比较";

        [StateLib(MMOHelper.CategoryName, typeof(MMOManager))]
        [StateComponentMenu(MMOHelper.CategoryName + "/" + Title, typeof(MMOManager))]
        [Name(Title, nameof(NetStateCompare))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("将网络状态的值与待比较值进行比较判断")]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("比较类型")]
        [EnumPopup]
        public ECompareType compareType = ECompareType.Equal;

        [Name("值类型")]
        [Tip("待比较值的类型;仅支持值与变量类型")]
        [EnumPopup]
        public EValueType valueType = EValueType.Value;

        [Name("待比较网络状态")]
        [EnumPopup]
        [HideInSuperInspector(nameof(valueType), EValidityCheckType.Equal, EValueType.Variable)]
        public ENetState compareNetState = ENetState.SyncRoomed;

        [Name("待比较变量")]
        [GlobalVariable(nameof(valueType), EValidityCheckType.Equal, EValueType.Variable)]
        [HideInSuperInspector(nameof(valueType), EValidityCheckType.NotEqual, EValueType.Variable)]
        public string variable;

        [Name("比较规则")]
        [EnumPopup]
        public ECompareRule compareRule = ECompareRule.String;

        /// <summary>
        /// 待比较值字符串
        /// </summary>
        public string compareValueString
        {
            get
            {
                switch (valueType)
                {
                    case EValueType.Variable: return ScriptOption.VarFlag + variable;
                    default: return compareNetState.ToString();
                }

            }
        }

        private bool Check()
        {
            var mmo = MMOManager.instance;
            if (!mmo) return false;
            var compareValue = "";
            switch (valueType)
            {
                case EValueType.Variable:
                    {
                        ScriptManager.TryGetGlobalVariableValue(variable, out compareValue);
                        break;
                    }
                default:
                    {
                        compareValue = compareNetState.ToString();
                        break;
                    }
            }
            return VariableCompareHelper.ValueCompareValue(mmo.netState.ToString(), compareType, compareValue, compareRule);
        }

        public override bool Finished()
        {
            return base.Finished() || Check();
        }

        public override string ToFriendlyString()
        {
            return "网络状态" + VariableCompareHelper.ToAbbreviations(compareType) + compareValueString;
        }
    }
}
