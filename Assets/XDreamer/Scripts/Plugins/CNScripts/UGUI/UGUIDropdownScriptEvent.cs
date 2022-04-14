using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.Extension.CNScripts.UGUI
{
    /// <summary>
    /// UGUI下拉选择框脚本事件类型 
    /// </summary>
    public enum EUGUIDropdownScriptEventType
    {
        /// <summary>
        /// 选择时
        /// </summary>
        [Name("值变动时执行")]
        OnValueChanged,

        [Name("启动时执行")]
        Start,
    }

    /// <summary>
    /// UGUI下拉选择框脚本事件集合类
    /// </summary>
    [Serializable]
    public class UGUIDropdownScriptEventSet : BaseScriptEventSet<EUGUIDropdownScriptEventType>
    {
        //
    }

    /// <summary>
    /// UGUI下拉选择框脚本事件
    /// </summary>
    [Serializable]
    [RequireComponent(typeof(Dropdown))]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.UGUIMenu + Title)]
    public class UGUIDropdownScriptEvent : BaseScriptEvent<EUGUIDropdownScriptEventType, UGUIDropdownScriptEventSet>
    {
        public const string Title = "UGUI下拉选择框脚本事件";

        public Dropdown dropdown { get; protected set; }

        public override void OnEnable()
        {
            base.OnEnable();
            dropdown = gameObject.GetComponent<Dropdown>();
            if (dropdown)
            {
                dropdown.onValueChanged.AddListener(this.OnValueChanged);
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (dropdown)
            {
                dropdown.onValueChanged.RemoveListener(this.OnValueChanged);
            }
        }

        public override void Start()
        {
            RunScriptEvent(EUGUIDropdownScriptEventType.Start);
        }

        public void OnValueChanged(int obj)
        {
            RunScriptEvent(EUGUIDropdownScriptEventType.OnValueChanged, obj.ToString());
        }
    }
}
