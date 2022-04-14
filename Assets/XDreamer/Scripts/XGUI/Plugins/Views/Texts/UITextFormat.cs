using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Models;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginDataflows.Converters;

namespace XCSJ.PluginXGUI.Views.Texts
{
    /// <summary>
    /// 文本格式化组件：使用格式化组件来设置UI-Text内容
    /// </summary>
    [Name("文本格式化组件")]
    [RequireComponent(typeof(TextFormat))]
    public class UITextFormat : BaseText
    {
        private TextFormat textFormat;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            textFormat = GetComponent<TextFormat>();
        }

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            textFormat.sendEvent += OnTextFormatValueChanged;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();

            textFormat.sendEvent -= OnTextFormatValueChanged;
        }

        /// <summary>
        /// 文本格式值监听函数
        /// </summary>
        /// <param name="eventArg"></param>
        private void OnTextFormatValueChanged(XValueEventArg eventArg)
        {
            if (text && eventArg.hasValue)
            {
                text.text = eventArg.value as string;
            }
        }
    }
}
