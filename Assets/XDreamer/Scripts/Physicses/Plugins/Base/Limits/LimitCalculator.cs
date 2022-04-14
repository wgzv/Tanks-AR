using System;
using UnityEngine;
using XCSJ.Attributes;

namespace XCSJ.PluginPhysicses.Base.Limits
{
    /// <summary>
    /// 限定计算器：使用限定对象的数值和特定算法进行计算
    /// </summary>
    public abstract class LimitCalculator
    {
        /// <summary>
        /// 限定对象类
        /// </summary>
        protected ILimiter limiter;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="limiter"></param>
        public LimitCalculator(ILimiter limiter)
        {
            this.limiter = limiter;
            this.limiter.Init();
        }

        /// <summary>
        /// 更新计算
        /// </summary>
        public abstract void Calculate();

        /// <summary>
        /// 获取限定百分比
        /// </summary>
        /// <returns></returns>
        protected float GetPercent()
        {
            var value = limiter.currentValue;
            if (Mathf.Abs(value) < limiter.deadZone)
            {
                value = 0;
            }
            return value;
        }
    }

    /// <summary>
    /// 开关限定：将状态分成开或关
    /// </summary>
    public class SwitchCalculator : LimitCalculator
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="limiter"></param>
        public SwitchCalculator(ILimiter limiter) : base(limiter) { }

        /// <summary>
        /// 按下状态
        /// </summary>
        public bool isOn { get; protected set; } = false;

        /// <summary>
        /// 值变化回调函数
        /// </summary>
        public static event Action<SwitchCalculator, bool> onValueChanged;

        /// <summary>
        /// 计算更新函数
        /// </summary>
        public override void Calculate()
        {
            var value = GetPercent();
            if (isOn)
            {
                // 偏移量小于最小值判定为关
                if (value < limiter.min)
                {
                    isOn = false;
                    OnValueChanged();
                    //Debug.Log("switch:" + value + ",isOn:" + isOn);
                }
            }
            else
            {
                // 偏移量大于最大值判定为开
                if (value > limiter.max)
                {
                    isOn = true;
                    OnValueChanged();
                    //Debug.Log("switch:" + value + ",isOn:" + isOn);
                }
            }
        }

        private void OnValueChanged()
        {
            onValueChanged?.Invoke(this, isOn);
        }
    }

    /// <summary>
    /// 开关状态
    /// </summary>
    public enum ESwitchState
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 开
        /// </summary>
        [Name("开")]
        On,

        /// <summary>
        /// 关
        /// </summary>
        [Name("关")]
        Off,
    }

    /// <summary>
    /// 小中大限定：将百分比数值状态分成小、中和大，三种区间状态
    /// </summary>
    public class MinMiddMaxCalculator : LimitCalculator
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="limiter"></param>
        public MinMiddMaxCalculator(ILimiter limiter) : base(limiter) { }

        /// <summary>
        /// 阈值为 0.1 
        /// 1、百分比[0, min]，为最小状态
        /// 2、百分比[0.4, 0.6]，为中间状态
        /// 3、百分比[max, 1]时，为最大状态
        /// </summary>
        public EMinMiddleMaxState minMiddleMaxState
        {
            get => _minMiddleMaxState;
            set
            {
                if (_minMiddleMaxState!= value)
                {
                    _minMiddleMaxState = value;
                    OnValueChanged();
                }
            }
        }
        private EMinMiddleMaxState _minMiddleMaxState = EMinMiddleMaxState.None;

        /// <summary>
        /// 值变化回调函数
        /// </summary>
        public static event Action<MinMiddMaxCalculator, EMinMiddleMaxState> onValueChanged;

        /// <summary>
        /// 更新函数
        /// </summary>
        public override void Calculate()
        {
            var value = GetPercent();

            var middle = new Vector2(0.4f, 0.6f);
            middle.x = Mathf.Max(middle.x, limiter.min);
            middle.y = Mathf.Min(middle.y, limiter.max);

            switch (minMiddleMaxState)
            {
                case EMinMiddleMaxState.None:// 中间
                    {
                        if (value <= middle.y && value >= middle.x)
                        {
                            minMiddleMaxState = EMinMiddleMaxState.Middle;
                        }
                        break;
                    }
                case EMinMiddleMaxState.Min:// 中间
                    {
                        if (value >= middle.x)
                        {
                            minMiddleMaxState = EMinMiddleMaxState.Middle;
                        }
                        break;
                    }
                case EMinMiddleMaxState.Middle:
                    {
                        // 最小
                        if (value < limiter.min)
                        {
                            minMiddleMaxState = EMinMiddleMaxState.Min;
                        }

                        // 最大
                        if (value >= limiter.max)
                        {
                            minMiddleMaxState = EMinMiddleMaxState.Max;
                        }
                        break;
                    }
                case EMinMiddleMaxState.Max:// 中间
                    {
                        if (value <= middle.y)
                        {
                            minMiddleMaxState = EMinMiddleMaxState.Middle;
                        }
                        break;
                    }
            }
        }

        private void OnValueChanged()
        {
            onValueChanged?.Invoke(this, minMiddleMaxState);
        }
    }

    /// <summary>
    /// 三态
    /// </summary>
    public enum EMinMiddleMaxState
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 最小
        /// </summary>
        [Name("最小")]
        Min,

        /// <summary>
        /// 中间
        /// </summary>
        [Name("中间")]
        Middle,

        /// <summary>
        /// 最大
        /// </summary>
        [Name("最大")]
        Max,
    }

    /// <summary>
    /// 限定计算器类型
    /// </summary>
    [Name("限定计算器类型")]
    public enum ELimitCalculatorType
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 开与关两种状态
        /// </summary>
        [Name("开关")]
        Switch,

        /// <summary>
        /// 小中大三种状态
        /// </summary>
        [Name("小中大")]
        MinMiddleMax,
    }
}
