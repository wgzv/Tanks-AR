﻿using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Recorders;
using XCSJ.Extension.Base.RichTexts;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Motions
{
    public abstract class Flash<T, TRecorder> : Motion<T, TRecorder>, IFlash
        where T : Flash<T, TRecorder>
        where TRecorder : class, IPercentRecorder<T>, new()
    {
        [Name("闪烁模式")]
        [EnumPopup]
        public EFlashMode flashMode = EFlashMode.Frequency;

        [Name("频率")]
        [Tip("单位时间（1秒）内的闪烁次数")]
        [Range(0, 100)]
        [HideInSuperInspector(nameof(flashMode), EValidityCheckType.NotEqual, EFlashMode.Frequency)]
        public float hz = 2;

        [Name("次数")]
        [Tip("在有效时长内的闪烁次数")]
        [Range(0, 100)]
        [HideInSuperInspector(nameof(flashMode), EValidityCheckType.NotEqual, EFlashMode.Count)]
        public float count = 6;

        public float flashCount
        {
            get
            {
                switch (flashMode)
                {
                    case EFlashMode.Frequency: return timeLength * hz;
                    case EFlashMode.Count: return count;
                    default: return 0;
                }
            }
        }

        protected override void OnSetPercent(Percent percent, StateData stateData)
        {
            inChangeArea = InChangeArea(percent.percent01OfWorkCurve);
            base.OnSetPercent(percent, stateData);
        }

        public bool inChangeArea { get; private set; } = false;

        protected bool InChangeArea(float percentage)
        {
            return Mathf.FloorToInt(flashCount * 2 * percentage) % 2 == 1;
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            //默认在闪烁结束时，恢复原状态
            percentOnExit = 0;
            base.Reset();
        }
    }

    /// <summary>
    /// 闪烁结果
    /// </summary>
    public interface IFlash : IMotion
    {
        /// <summary>
        /// 有效时长内的闪烁次数
        /// </summary>
        float flashCount { get; }
    }

    /// <summary>
    /// 闪烁模式
    /// </summary>
    public enum EFlashMode
    {
        /// <summary>
        /// 无处理
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 频率,即单位时间（1秒）内的闪烁次数
        /// </summary>
        [Name("频率")]
        [Tip("单位时间（1秒）内的闪烁次数")]
        Frequency,

        /// <summary>
        /// 次数,即在有效时长内的闪烁次数
        /// </summary>
        [Name("次数")]
        [Tip("在有效时长内的闪烁次数")]
        Count,
    }
}
