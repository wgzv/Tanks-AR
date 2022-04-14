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
    /// UGUI滚动条脚本事件类型 
    /// </summary>
    public enum EUGUIScrollbarScriptEventType
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
    /// UGUI滚动条脚本事件集合类
    /// </summary>
    [Serializable]
    public class UGUIScrollbarScriptEventSet : BaseScriptEventSet<EUGUIScrollbarScriptEventType>
    {
        //
    }

    /// <summary>
    /// UGUI滚动条脚本事件
    /// </summary>
    [Serializable]
    [RequireComponent(typeof(Scrollbar))]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.UGUIMenu + Title)]
    public class UGUIScrollbarScriptEvent : BaseScriptEvent<EUGUIScrollbarScriptEventType, UGUIScrollbarScriptEventSet>
    {
        public const string Title = "UGUI滚动条脚本事件";

        public Scrollbar scrollbar { get; protected set; }

        public override void OnEnable()
        {
            base.OnEnable();
            scrollbar = gameObject.GetComponent<Scrollbar>();
            if (scrollbar)
            {
                scrollbar.onValueChanged.AddListener(this.OnValueChanged);
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (scrollbar)
            {
                scrollbar.onValueChanged.RemoveListener(this.OnValueChanged);
            }
        }

        public override void Start()
        {
            RunScriptEvent(EUGUIScrollbarScriptEventType.Start);
        }

        public void OnValueChanged(float obj)
        {
            RunScriptEvent(EUGUIScrollbarScriptEventType.OnValueChanged, obj.ToString());
        }
    }
}
