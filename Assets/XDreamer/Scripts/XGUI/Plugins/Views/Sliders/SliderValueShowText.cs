using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Views.Sliders
{
    /// <summary>
    /// 滑动条值显示文本
    /// </summary>
    [Tool(XGUICategory.Component, nameof(XGUIManager), rootType = typeof(ToolsManager))]
    [Name("滑动条值显示文本")]
    [XCSJ.Attributes.Icon(EIcon.Slider)]
    [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
    public class SliderValueShowText : View
    {
        [Name("滑动条")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public Slider slider;

        [Name("文本")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public Text text;

        [Name("显示文本小数位数")]
        [Range(0, 5)]
        public int showTextDecimalDigitNumber = 1;

        protected void Awake()
        {
            if (!slider) slider = GetComponent<Slider>();
            if (!text) text = GetComponent<Text>();

            SetText();
        }

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            if (slider)
            {
                slider.onValueChanged.AddListener(OnSliderValueChanged);
            }
        }
        
        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            if (slider)
            {
                slider.onValueChanged.RemoveListener(OnSliderValueChanged);
            }
        }

        protected virtual void OnSliderValueChanged(float value)
        {
            SetText();
        }

        private void SetText()
        {
            if (slider && text)
            {
                text.text = slider.value.ToString("F" + showTextDecimalDigitNumber.ToString());
            }
        }
    }
}
