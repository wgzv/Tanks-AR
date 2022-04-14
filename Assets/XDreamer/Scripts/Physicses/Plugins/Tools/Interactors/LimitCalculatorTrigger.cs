using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginPhysicses.Base.Limits;
using XCSJ.PluginTools;

namespace XCSJ.PluginPhysicses.Tools.Interactors
{
    /// <summary>
    /// 限定计算触发器
    /// </summary>
    public abstract class LimitCalculatorTrigger : BasePhysicsMB
    {
        /// <summary>
        /// 限定计算器类型
        /// </summary>
        [Group("限定计算触发器")]
        [Name("限定计算器类型")]
        [EnumPopup]
        public ELimitCalculatorType _limitCalculatorType = ELimitCalculatorType.Switch;

        /// <summary>
        /// 开
        /// </summary>
        [Name("开")]
        [HideInSuperInspector(nameof(_limitCalculatorType), EValidityCheckType.NotEqual, ELimitCalculatorType.Switch)]
        public UnityEvent _on;

        /// <summary>
        /// 关
        /// </summary>
        [Name("关")]
        [HideInSuperInspector(nameof(_limitCalculatorType), EValidityCheckType.NotEqual, ELimitCalculatorType.Switch)]
        public UnityEvent _off;

        /// <summary>
        /// 最小值
        /// </summary>
        [Name("最小值")]
        [HideInSuperInspector(nameof(_limitCalculatorType), EValidityCheckType.NotEqual, ELimitCalculatorType.MinMiddleMax)]
        public UnityEvent _onMin;

        /// <summary>
        /// 中间值
        /// </summary>
        [Name("中间值")]
        [HideInSuperInspector(nameof(_limitCalculatorType), EValidityCheckType.NotEqual, ELimitCalculatorType.MinMiddleMax)]
        public UnityEvent _onMiddle;

        /// <summary>
        /// 最大值
        /// </summary>
        [Name("最大值")]
        [HideInSuperInspector(nameof(_limitCalculatorType), EValidityCheckType.NotEqual, ELimitCalculatorType.MinMiddleMax)]
        public UnityEvent _onMax;

        protected abstract ILimiter limiter { get; }

        /// <summary>
        /// 小中大计算器
        /// </summary>
        protected SwitchCalculator switchCalculator
        {
            get
            {
                if (_switchCalculator == null && limiter!=null)
                {
                    _switchCalculator = new SwitchCalculator(limiter);
                }
                return _switchCalculator;
            }
        }
        private SwitchCalculator _switchCalculator = null;

        /// <summary>
        /// 操纵杆状态变化回调函数
        /// </summary>
        public static event Action<LimitCalculatorTrigger, ESwitchState> onSwitchStateChanged;

        /// <summary>
        /// 小中大计算器
        /// </summary>
        protected MinMiddMaxCalculator minMiddMaxCalculator
        {
            get
            {
                if (_minMiddMaxCalculator == null && limiter != null)
                {
                    _minMiddMaxCalculator = new MinMiddMaxCalculator(limiter);
                }
                return _minMiddMaxCalculator;
            }
        }
        private MinMiddMaxCalculator _minMiddMaxCalculator = null;

        /// <summary>
        /// 操纵杆状态变化回调函数
        /// </summary>
        public static event Action<LimitCalculatorTrigger, EMinMiddleMaxState> onMinMiddleMaxStateChanged;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            SwitchCalculator.onValueChanged += OnValueChanged;
            MinMiddMaxCalculator.onValueChanged += OnMinMiddleMaxValueChanged;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();

            SwitchCalculator.onValueChanged -= OnValueChanged;
            MinMiddMaxCalculator.onValueChanged -= OnMinMiddleMaxValueChanged;
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected void Update()
        {
            switch (_limitCalculatorType)
            {
                case ELimitCalculatorType.Switch:
                    {
                        switchCalculator?.Calculate();
                        break;
                    }
                case ELimitCalculatorType.MinMiddleMax:
                    {
                        minMiddMaxCalculator?.Calculate();
                        break;
                    }
            }
        }

        private void OnValueChanged(SwitchCalculator toggleCalculator, bool isOn)
        {
            if (this.switchCalculator == toggleCalculator)
            {
                onSwitchStateChanged?.Invoke(this, isOn ? ESwitchState.On : ESwitchState.Off);
                if (isOn)
                {
                    _on?.Invoke();
                }
                else
                {
                    _off?.Invoke();
                }
            }
        }

        private void OnMinMiddleMaxValueChanged(MinMiddMaxCalculator minMiddMaxLimiter, EMinMiddleMaxState minMiddleMaxState)
        {
            if (this.minMiddMaxCalculator == minMiddMaxLimiter)
            {
                onMinMiddleMaxStateChanged?.Invoke(this, minMiddleMaxState);

                switch (minMiddleMaxState)
                {
                    case EMinMiddleMaxState.Min:
                        {
                            _onMin.Invoke();
                            //Debug.Log("min");
                            break;
                        }
                    case EMinMiddleMaxState.Middle:
                        {
                            _onMiddle.Invoke();
                            //Debug.Log("middle");
                            break;
                        }
                    case EMinMiddleMaxState.Max:
                        {
                            _onMax.Invoke();
                            //Debug.Log("max");
                            break;
                        }
                }
            }
        }
    }
}