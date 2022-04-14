using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginXGUI.Views.Inputs
{
    /// <summary>
    /// 键码按钮模拟输入:通过UGUI模拟输入键码按钮的状态
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Keyboard)]
    [Name("键码按钮模拟输入")]
    [Tip("通过UGUI模拟输入键码按钮的状态")]
    [Tool(AnalogInputHelper.Category, nameof(XGUIManager))]
    public class KeyCodeButtonAnalogInput : BaseAnalogInput, IPointerUpHandler, IPointerDownHandler
    {
        [Name("键码")]
        [Input]
        public KeyCode keyCode;

        public void OnPointerDown(PointerEventData eventData)
        {
            UpdateButton(keyCode.ToString(), true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            UpdateButton(keyCode.ToString(), false);
        }
    }
}
