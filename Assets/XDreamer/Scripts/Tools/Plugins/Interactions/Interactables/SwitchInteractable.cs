using System;

namespace XCSJ.PluginTools.Interactions.Interactables
{
    /// <summary>
    /// 开关态可交互对象
    /// </summary>
    public abstract class SwitchInteractable : BaseInteractable
    {
        /// <summary>
        /// 开关态
        /// </summary>
        public bool isOn
        {
            get => _isOn;
            set
            {
                _isOn = value;
                onValueChanged?.Invoke(this, _isOn);
            }
        }
        private bool _isOn;

        /// <summary>
        /// 值变化回调
        /// </summary>
        public static event Action<SwitchInteractable, bool> onValueChanged;
    }

}
