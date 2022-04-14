using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginART.Base;
using XCSJ.PluginART.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginART.States
{
    /// <summary>
    /// Flystick按钮事件:用于监听并捕获ART输入设备Flystick的事件
    /// </summary>
    [ComponentMenu(ARTHelper.Title + "/" + Title, typeof(ARTManager))]
    [Name(Title, nameof(FlystickButtonEvent))]
    [Tip("用于监听并捕获ART输入设备Flystick的事件")]
    public class FlystickButtonEvent : Trigger<FlystickButtonEvent>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Flystick按钮事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(FlystickButtonEvent))]
#if UNITY_EDITOR
        [StateLib(ARTHelper.Title, typeof(ARTManager))]
        [StateComponentMenu(ARTHelper.Title + "/" + Title, typeof(ARTManager))]
#endif
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// ART流客户端:用于与ART进行数据流通信的ART流客户端
        /// </summary>
        [Name("ART流客户端")]
        [Tip("用于与ART进行数据流通信的ART流客户端")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public ARTStreamClient _streamClient;

        /// <summary>
        /// ART流客户端
        /// </summary>
        public ARTStreamClient streamClient => this.XGetComponentInGlobal(ref _streamClient, true);

        /// <summary>
        /// Flystick按钮
        /// </summary>
        [Name("Flystick按钮")]
        public FlystickButton _flystickButton = new FlystickButton();

        /// <summary>
        /// 被按压的Flystick编号变量名
        /// </summary>
        [Name("被按压的Flystick编号变量名")]
        [GlobalVariable]
        public string _variableName0fPressedFlystickID;

        /// <summary>
        /// 被按压的Flystick按钮变量名
        /// </summary>
        [Name("被按压的Flystick按钮变量名")]
        [GlobalVariable]
        public string _variableName0fPressedFlystickSwitchs;

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            if (!streamClient) return;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnUpdate(StateData stateData)
        {
            base.OnUpdate(stateData);
            if (finished) return;
            finished = _flystickButton.IsPressed(_streamClient, out int pressedFlystickID, out int pressedFlystickSwitchs);
            if (finished)
            {
                ScriptManager.TrySetGlobalVariableValue(_variableName0fPressedFlystickID, pressedFlystickID.ToString());
                ScriptManager.TrySetGlobalVariableValue(_variableName0fPressedFlystickSwitchs, _flystickButton._flystick.GetFlystickSwitchs(pressedFlystickSwitchs).ToString());
            }
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            //return base.ToFriendlyString();
            return _flystickButton.ToFriendlyString();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (!streamClient) { }
        }
    }
}
