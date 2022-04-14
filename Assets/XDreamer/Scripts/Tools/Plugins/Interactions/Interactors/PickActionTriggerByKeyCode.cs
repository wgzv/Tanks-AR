using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginPhysicses.Base;

namespace XCSJ.PluginTools.Interactions.Interactors
{
    /// <summary>
    /// 拾取动作通过键盘
    /// </summary>
    [DisallowMultipleComponent]
    [Name("拾取动作通过键盘")]
    [Tool("交互器", rootType = typeof(ToolsManager))]
    [RequireManager(typeof(ToolsManager))]
    public class PickActionTriggerByKeyCode : ToolMB, IPickInput
    {
        /// <summary>
        /// 抓起
        /// </summary>
        [Name("抓起")]
        public KeyCode _pickKeyCode = KeyCode.F;

        /// <summary>
        /// 扔
        /// </summary>
        [Name("扔")]
        public KeyCode _throwKeyCode = KeyCode.T;

        /// <summary>
        /// 放下
        /// </summary>
        [Name("放下")]
        public KeyCode _dropKeyCode = KeyCode.F;

        /// <summary>
        /// 扔力
        /// </summary>
        [Name("扔力")]
        [Tip("将刚体对象扔出去所施加的力的模式和参数信息")]
        public ForceModeInfo _forceModeInfo = new ForceModeInfo();

        public ForceMode forceMode => _forceModeInfo._forceMode;

        /// <summary>
        /// 丢弃力
        /// </summary>
        public float dropForce
        {
            get
            {
                return _drop ? 0 : _forceModeInfo.GetValue();
            }
        }

        /// <summary>
        /// 交互用途
        /// </summary>
        public string usage { get => nameof(PickActionTriggerByKeyCode); set { } }

        private bool _drop = false;

        /// <summary>
        /// 是否拿起
        /// </summary>
        /// <returns></returns>
        public bool IsPickup() => Input.GetKeyUp(_pickKeyCode);

        /// <summary>
        /// 是否放下
        /// </summary>
        /// <returns></returns>
        public bool IsDrop()
        {
            if (Input.GetKeyUp(_dropKeyCode))
            {
                _drop = true;
                return true;
            }
            else if (Input.GetKeyUp(_throwKeyCode))
            {
                _drop = false;
                return true;
            }
            return false;
        }
    }
}
