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
    /// UGUI文本输入框脚本事件类型 
    /// </summary>
    public enum EUGUIInputFieldScriptEventType
    {
        /// <summary>
        /// 选择时
        /// </summary>
        [Name("值变动时执行")]
        OnValueChanged,

        /// <summary>
        /// 完成修改时
        /// </summary>
        [Name("完成修改时执行")]
        EndEdit,

        /// <summary>
        /// 完成修改时
        /// </summary>
        [Name("启动时执行")]
        Start,
    }

    /// <summary>
    /// UGUI文本输入框脚本事件集合类
    /// </summary>
    [Serializable]
    public class UGUIInputFieldScriptEventSet : BaseScriptEventSet<EUGUIInputFieldScriptEventType>
    {
        //
    }

    /// <summary>
    /// UGUI文本输入框脚本事件
    /// </summary>
    [Serializable]
    [RequireComponent(typeof(InputField))]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.UGUIMenu + Title)]
    public class UGUIInputFieldScriptEvent : BaseScriptEvent<EUGUIInputFieldScriptEventType, UGUIInputFieldScriptEventSet>
    {
        public const string Title = "UGUI文本输入框脚本事件";

        public InputField inputField { get; protected set; }

        public override void OnEnable()
        {
            base.OnEnable();
            inputField = gameObject.GetComponent<InputField>();
            if (inputField)
            {
                inputField.onValueChanged.AddListener(this.OnValueChanged);
                inputField.onEndEdit.AddListener(this.EndEdit);
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (inputField)
            {
                inputField.onValueChanged.RemoveListener(this.OnValueChanged);
                inputField.onEndEdit.RemoveListener(this.EndEdit);
            }
        }

        public override void Start()
        {
            RunScriptEvent(EUGUIInputFieldScriptEventType.Start);
        }

        public void OnValueChanged(string obj)
        {
            RunScriptEvent(EUGUIInputFieldScriptEventType.OnValueChanged, obj.ToString());
        }

        public void EndEdit(string obj)
        {
            RunScriptEvent(EUGUIInputFieldScriptEventType.EndEdit, obj.ToString());
        }
    }
}
