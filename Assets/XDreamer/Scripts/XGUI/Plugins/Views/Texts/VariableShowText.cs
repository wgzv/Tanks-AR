using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginXGUI.Views.Texts
{
    [Name("变量显示文本")]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Variable)]
    [Tip("将变量值显示在Text上，文本会随着变量变化而变化")]
    [Tool(XGUICategory.Component, nameof(XGUIManager))]
    [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
    public class VariableShowText : View
    {
        [Name("变量")]
        [GlobalVariable(true)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public string variable;

        [Name("文本")]
        [ComponentPopup]
        public Text text;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            if (!text) text = GetComponent<Text>();

            if (text)
            {
                Variable.onValueChanged += OnVariableValueChanged;
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override  void OnDisable()
        {
            base.OnDisable();
            Variable.onValueChanged -= OnVariableValueChanged;
        }

        protected void OnVariableValueChanged(Variable var)
        {
            if (var.varScope == EVarScope.Global && var.name == variable)
            {
                text.text = var.value;
            }
        }
    }
}