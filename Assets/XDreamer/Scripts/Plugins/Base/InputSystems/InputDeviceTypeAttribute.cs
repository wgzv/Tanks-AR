using System;
using XCSJ.Caches;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.LowLevel;
#endif

namespace XCSJ.Extension.Base.InputSystems
{
    /// <summary>
    /// 输入设备类型特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class InputDeviceTypeAttribute : Attribute
    {
        private Type _type = null;

        /// <summary>
        /// 类型
        /// </summary>
        public Type type
        {
            get
            {
                if (_type == null && !string.IsNullOrEmpty(_typeFullName))
                {
                    _type = TypeCache.Get(_typeFullName);
                }
                return _type;
            }
            private set => this._type = value;
        }

        private string _typeFullName = "";

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="type"></param>
        public InputDeviceTypeAttribute(Type type) { this.type = type; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="typeFullName"></param>
        public InputDeviceTypeAttribute(string typeFullName) { this._typeFullName = typeFullName; }

        /// <summary>
        /// 获取输入设备类型
        /// </summary>
        /// <param name="e"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static Type GetInputDeviceType(Enum e, Type defaultValue = default) => AttributeCache<InputDeviceTypeAttribute>.GetOfField(e) is InputDeviceTypeAttribute attribute ? attribute.type : defaultValue;
    }

#if ENABLE_INPUT_SYSTEM

    /// <summary>
    /// 游戏手柄按钮特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class GamepadButtonAttribute : Attribute
    {
        /// <summary>
        /// 按钮
        /// </summary>
        public GamepadButton button { get; private set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="button"></param>
        public GamepadButtonAttribute(GamepadButton button) { this.button = button; }

        /// <summary>
        /// 获取键码
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static GamepadButton GetGamepadButton(Enum e, GamepadButton defaultGamepadButton = (GamepadButton)(-1)) => AttributeCache<GamepadButtonAttribute>.GetOfField(e) is GamepadButtonAttribute attribute ? attribute.button : defaultGamepadButton;
    }

#endif
}
