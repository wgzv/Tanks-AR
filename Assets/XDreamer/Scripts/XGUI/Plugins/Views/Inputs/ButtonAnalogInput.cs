﻿using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginXGUI.Views.Inputs
{
    /// <summary>
    /// 按钮模拟输入:通过UGUI模拟输入按钮的状态或轴的值
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Button)]
    [Name("按钮模拟输入")]
    [Tip("通过UGUI模拟输入按钮的状态或轴的值")]
    [Tool(AnalogInputHelper.Category, nameof(XGUIManager))]
    public class ButtonAnalogInput : BaseAnalogInput, IPointerUpHandler, IPointerDownHandler
    {
        [Name("按钮输入")]
        [Input]
        public string buttonInput;

        [Name("更新按钮状态或值")]
        [Tip("为True时，指针按下或抬起时更新虚拟按钮的按压状态；为False时，指针按下或抬起时更新虚拟轴的值；")]
        public bool stateOrValue = true;

        [HideInSuperInspector(nameof(stateOrValue), EValidityCheckType.True)]
        [Name("按下时的值")]
        [Range(-1, 1)]
        public float downValue = 1;

        [HideInSuperInspector(nameof(stateOrValue), EValidityCheckType.True)]
        [Name("抬起时的值")]
        [Range(-1, 1)]
        public float upValue = 0;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (stateOrValue)
            {
                UpdateButton(buttonInput, true);
            }
            else
            {
                UpdateAxis(buttonInput, downValue);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (stateOrValue)
            {
                UpdateButton(buttonInput, false);
            }
            else
            {
                UpdateAxis(buttonInput, upValue);
            }
        }
    }
}
