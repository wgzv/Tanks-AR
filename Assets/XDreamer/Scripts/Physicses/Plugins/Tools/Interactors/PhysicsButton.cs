using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginPhysicses.Base.Limits;
using XCSJ.PluginTools;

namespace XCSJ.PluginPhysicses.Tools.Interactors
{
    /// <summary>
    /// 物理按钮：使用可配置链接模拟按钮
    /// </summary>
    [Name("物理按钮")]
    [XCSJ.Attributes.Icon(EIcon.Button)]
    [Tool(PhysicsManager.Title, rootType = typeof(PhysicsManager))]
    public class PhysicsButton : BasePhysicsMB
    {
        /// <summary>
        /// 可配置节点限定器
        /// </summary>
        [Name("可配置节点限定器")]
        public ConfigurableJointLimiter _configurableJointLimiter = new ConfigurableJointLimiter();

        /// <summary>
        /// 按下回调函数
        /// </summary>
        [Name("按下")]
        public UnityEvent _onPressed;

        /// <summary>
        /// 弹起回调函数
        /// </summary>
        [Name("弹起")]
        public UnityEvent _onReleased;

        /// <summary>
        /// 切换计算器
        /// </summary>
        protected SwitchCalculator switchCalculator
        {
            get
            {
                if (_switchCalculator == null)
                {
                    _switchCalculator = new SwitchCalculator(_configurableJointLimiter);
                }
                return _switchCalculator;
            }
        }
        private SwitchCalculator _switchCalculator = null;

        /// <summary>
        /// 物理按钮按下回调事件
        /// </summary>
        public static event Action<PhysicsButton, bool> onPressed;

        /// <summary>
        /// 重置
        /// </summary>
        protected void Reset()
        {
            _configurableJointLimiter._configurableJoint = GetComponent<ConfigurableJoint>();
        }

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            SwitchCalculator.onValueChanged += OnToggleValueChanged;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();

            SwitchCalculator.onValueChanged -= OnToggleValueChanged;
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected void Update()
        {
            switchCalculator.Calculate();
        }

        private void OnToggleValueChanged(SwitchCalculator toggleLimiter, bool isOn)
        {
            if (this.switchCalculator == toggleLimiter)
            {
                onPressed?.Invoke(this, isOn);

                if (isOn)
                {
                    _onPressed.Invoke();
                }
                else
                {
                    _onReleased.Invoke();
                }
            }
        }
    }
}
