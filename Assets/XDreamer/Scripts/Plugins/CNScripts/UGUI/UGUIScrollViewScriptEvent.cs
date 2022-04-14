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
    /// UGUI滚动视图脚本事件类型 
    /// </summary>
    public enum EUGUIScrollViewScriptEventType
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
    /// UGUI滚动视图脚本事件集合类
    /// </summary>
    [Serializable]
    public class UGUIScrollViewScriptEventSet : BaseScriptEventSet<EUGUIScrollViewScriptEventType>
    {
        //
    }

    /// <summary>
    /// UGUI滚动视图脚本事件
    /// </summary>
    [Serializable]
    [RequireComponent(typeof(ScrollRect))]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.UGUIMenu + Title)]
    public class UGUIScrollViewScriptEvent : BaseScriptEvent<EUGUIScrollViewScriptEventType, UGUIScrollViewScriptEventSet>
    {
        public const string Title = "UGUI滚动视图脚本事件";

        public ScrollRect scrollView { get; protected set; }

        public override void OnEnable()
        {
            base.OnEnable();
            scrollView = gameObject.GetComponent<ScrollRect>();
            if (scrollView)
            {
                scrollView.onValueChanged.AddListener(this.OnValueChanged);
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (scrollView)
            {
                scrollView.onValueChanged.RemoveListener(this.OnValueChanged);
            }
        }

        public override void Start()
        {
            RunScriptEvent(EUGUIScrollViewScriptEventType.Start);
        }

        public void OnValueChanged(Vector2 obj)
        {
            RunScriptEvent(EUGUIScrollViewScriptEventType.OnValueChanged, CommonFun.Vector2ToString(obj));
        }
    }
}
