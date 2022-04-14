using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.CNScripts;

namespace XCSJ.Extension.CNScripts.UGUI
{
    /// <summary>
    /// UGUI按钮脚本事件类型 
    /// </summary>
    public enum EUGUIButtonScriptEventType
    {
        /// <summary>
        /// 选择时
        /// </summary>
        [Name("点击时执行")]
        OnClick,

        [Name("启动时执行")]
        Start,
    }

    /// <summary>
    /// UGUI按钮脚本事件集合类
    /// </summary>
    [Serializable]
    public class UGUIButtonScriptEventSet : BaseScriptEventSet<EUGUIButtonScriptEventType>
    {
        //
    }

    /// <summary>
    /// UGUI按钮脚本事件
    /// </summary>
    [Serializable]
    [RequireComponent(typeof(Button))]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.UGUIMenu + Title)]
    public class UGUIButtonScriptEvent : BaseScriptEvent<EUGUIButtonScriptEventType, UGUIButtonScriptEventSet>
    {
        public const string Title = "UGUI按钮脚本事件";

        public Button button { get; protected set; }

        public override void OnEnable()
        {
            base.OnEnable();
            button = gameObject.GetComponent<Button>();
            if (button)
            {
                button.onClick.AddListener(this.OnClick);
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (button)
            {
                button.onClick.RemoveListener(this.OnClick);
            }
        }

        public override void Start()
        {
            RunScriptEvent(EUGUIButtonScriptEventType.Start);
        }

        public void OnClick()
        {
            //Log.Info(gameObject.name);
            RunScriptEvent(EUGUIButtonScriptEventType.OnClick);
        }
    }
}
