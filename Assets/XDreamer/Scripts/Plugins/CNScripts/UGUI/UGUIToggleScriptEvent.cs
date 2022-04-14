using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.Extension.CNScripts.UGUI
{
    /// <summary>
    /// UGUI切换脚本事件类型 
    /// </summary>
    public enum EUGUIToggleScriptEventType
    {
        /// <summary>
        /// 值变动时执行
        /// </summary>
        [Name("值变动时执行")]
        OnValueChanged,

        /// <summary>
        /// 启动时执行
        /// </summary>
        [Name("启动时执行")]
        Start,
    }

    /// <summary>
    /// UGUI切换脚本事件集合类
    /// </summary>
    [Serializable]
    public class UGUIToggleScriptEventSet : BaseScriptEventSet<EUGUIToggleScriptEventType>
    {
        //
    }

    /// <summary>
    /// UGUI切换脚本事件
    /// </summary>
    [Serializable]
    [RequireComponent(typeof(Toggle))]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.UGUIMenu + Title)]
    public class UGUIToggleScriptEvent : BaseScriptEvent<EUGUIToggleScriptEventType, UGUIToggleScriptEventSet>
    {
        public const string Title = "UGUI切换脚本事件";

        public Toggle toggle { get; protected set; }

        public override void OnEnable()
        {
            base.OnEnable();
            toggle = gameObject.GetComponent<Toggle>();
            if (toggle)
            {
                toggle.onValueChanged.AddListener(this.OnValueChanged);
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (toggle)
            {
                toggle.onValueChanged.RemoveListener(this.OnValueChanged);
            }
        }

        public override void Start()
        {
            RunScriptEvent(EUGUIToggleScriptEventType.Start);
        }

        public void OnValueChanged(bool obj)
        {
            RunScriptEvent(EUGUIToggleScriptEventType.OnValueChanged, ScriptOption.ReturnValueFlag + obj.ToString());
        }
    }
}
