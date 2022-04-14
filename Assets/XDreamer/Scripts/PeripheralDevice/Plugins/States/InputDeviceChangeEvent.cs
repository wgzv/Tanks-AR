using XCSJ.Attributes;
using XCSJ.Extension.Base.InputSystems;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginCommonUtils;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace XCSJ.PluginPeripheralDevice.States
{
    /// <summary>
    /// 输入设备变更事件
    /// </summary>
    [Name(Title)]
    public class InputDeviceChangeEvent : Trigger<InputDeviceChangeEvent>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "输入设备变更事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(InputDeviceChangeEvent))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib(PeripheralDeviceInputHelper.Title, typeof(PeripheralDeviceInputManager))]
        [StateComponentMenu(PeripheralDeviceInputHelper.Title + "/" + Title, typeof(PeripheralDeviceInputManager))]
#endif
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 输入设备
        /// </summary>
        [Name("输入设备")]
        //[EnumPopup]
        public EInputDevice _inputDevice = EInputDevice.None;

#if ENABLE_INPUT_SYSTEM

        /// <summary>
        /// 输入设备变更
        /// </summary>
        [Name("输入设备变更")]
        public InputDeviceChange _inputDeviceChange = InputDeviceChange.Added;

#endif

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);

#if ENABLE_INPUT_SYSTEM
            InputSystem.onDeviceChange += OnDeviceChange;
#endif
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
#if ENABLE_INPUT_SYSTEM
            InputSystem.onDeviceChange -= OnDeviceChange;
#endif
        }


#if ENABLE_INPUT_SYSTEM
        private void OnDeviceChange(InputDevice inputDevice, InputDeviceChange inputDeviceChange)
        {
            if (finished) return;
            if (inputDeviceChange != _inputDeviceChange) return;
            var type = _inputDevice.GetInputDeviceType();
            if (type == null) return;
            finished = type.IsAssignableFrom(inputDevice.GetType());
        }
#endif

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
#if ENABLE_INPUT_SYSTEM
            return CommonFun.Name(_inputDevice) + "." + _inputDeviceChange;
#else
            return "未启用输入系统!";
#endif
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
#if ENABLE_INPUT_SYSTEM
            return base.DataValidity();
#else
            return false;
#endif
        }
    }
}
