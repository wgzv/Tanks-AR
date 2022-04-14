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
    /// UGUI滑动条脚本事件类型 
    /// </summary>
    public enum EUGUISliderScriptEventType
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
    /// UGUI滑动条脚本事件集合类
    /// </summary>
    [Serializable]
    public class UGUISliderScriptEventSet : BaseScriptEventSet<EUGUISliderScriptEventType>
    {
        //
    }

    /// <summary>
    /// UGUI滑动条脚本事件
    /// </summary>
    [Serializable]
    [RequireComponent(typeof(Slider))]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.UGUIMenu + Title)]
    public class UGUISliderScriptEvent : BaseScriptEvent<EUGUISliderScriptEventType, UGUISliderScriptEventSet>
    {
        public const string Title = "UGUI滑动条脚本事件";

        public Slider slider { get; protected set; }

        public override void OnEnable()
        {
            base.OnEnable();
            slider = gameObject.GetComponent<Slider>();
            if (slider)
            {
                slider.onValueChanged.AddListener(this.OnValueChanged);
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (slider)
            {
                slider.onValueChanged.RemoveListener(this.OnValueChanged);
            }
        }

        public override void Start()
        {
            RunScriptEvent(EUGUISliderScriptEventType.Start);
        }

        public void OnValueChanged(float obj)
        {
            RunScriptEvent(EUGUISliderScriptEventType.OnValueChanged, obj.ToString());
        }
    }
}
