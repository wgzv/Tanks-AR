using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Views.Texts
{
    /// <summary>
    /// 打字效果组件，用于一个播放打字效果
    /// </summary>
    [RequireComponent(typeof(UnityEngine.UI.Text))]
    [DisallowMultipleComponent]
    [Name("打字特效")]
    [XCSJ.Attributes.Icon(EIcon.Text)]
    [Tip("渐次显示Text中的文本内容")]
    [Tool(XGUICategory.Component, nameof(XGUIManager))]
    [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
    public class TypewriterEffect : View
    {
        /// <summary>
        /// 播放时长（秒）从 0.1秒到10分钟
        /// </summary>
        [Name("播放时长（秒）", "PlayTime")]
        [Range(0.1f, 600)]
        public float PlayTime = 1;

        /// <summary>
        /// 播放完回调函数
        /// </summary>          
        [Name("播放完回调函数", "FinishCallBack")]
        [UserDefineFun]
        public string FinishCallBack;

        /// <summary>
        /// 启动后是否播放
        /// </summary>
        [Name("启动后播放", "PlayOnStart")]
        public bool PlayOnStart = true;

        /// <summary>
        /// 是否播放
        /// </summary>
        private bool m_isPlay = false;

        /// <summary>
        /// 内部计数器，用来计算打到第几个字
        /// </summary>
        private float m_count = 0;

        /// <summary>
        /// 完整的句子
        /// </summary>
        private string m_words;

        /// <summary>
        /// UGUI字组件
        /// </summary>
        private Text m_text;

        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            if (!m_text)
            {
                m_text = GetComponent<Text>();
                m_words = m_text.text;
                m_text.text = string.Empty;
            }

            if (PlayOnStart)
            {
                Play();
            }
        }

        /// <summary>
        /// 播放
        /// </summary>
        public void Play()
        {
            if (!m_text) return;

            if (m_isPlay == false)
            {
                m_isPlay = true;
                m_count = 0;
            }
        }

        /// <summary>
        /// 完成
        /// </summary>
        void OnFinish()
        {
            m_isPlay = false;
            m_text.text = m_words;
            try
            {
                if (ScriptManager.instance != null && string.IsNullOrEmpty(FinishCallBack) == false)
                {
                    ScriptManager.instance.CallUserDefineFun(FinishCallBack, "");
                }
            }
            catch (Exception e)
            {
                Debug.Log("TypewriterEffect " + e.ToString());
            }
        }

        /// <summary>
        /// 跟新
        /// </summary>
        public void Update()
        {
            if (m_isPlay == false)
            {
                return;
            }

            try
            {
                m_count += Time.deltaTime;
                int subTextLength = (int)(m_count * m_words.Length / PlayTime);
                if (subTextLength < m_words.Length)
                {
                    m_text.text = m_words.Substring(0, subTextLength);
                }
                else
                {
                    OnFinish();
                }
            }
            catch
            {
                OnFinish();
            }
        }
    }
}
