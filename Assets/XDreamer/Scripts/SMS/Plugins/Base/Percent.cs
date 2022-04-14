using System;
using XCSJ.Maths;

namespace XCSJ.PluginSMS.Base
{
    /// <summary>
    /// 百分比：用于工作剪辑计算包括经过循环类型、工作曲线等处理后的百分比数值；
    /// </summary>
    public class Percent
    {
        /// <summary>
        /// 状态的百分比进度(范围[-∞,+∞])
        /// </summary>
        public float percentOfState { get; private set; }

        /// <summary>
        /// 状态组件百分比进度(范围[-∞,+∞]):相对当前状态组件有效工作区间的百分比进度
        /// </summary>
        public float percent { get; private set; }

        /// <summary>
        /// 循环类型计算后的状态组件百分比进度(范围[0,1])
        /// </summary>
        public float percentOfLoopType { get; private set; }

        /// <summary>
        /// 状态组件百分比进度(范围[0,1]):对percentOfLoopType根据前状态组件有效工作区间规则进行计算纠正
        /// </summary>
        public float percent01 { get; private set; }

        /// <summary>
        /// 工作曲线百分比进度(理论范围[0,1]),使用工作曲线对percent01计算后的值；
        /// </summary>
        public float percentOfWorkCurve { get; private set; }

        /// <summary>
        /// 工作曲线百分比进度(范围[0,1])
        /// </summary>
        public float percent01OfWorkCurve => MathX.Clamp01(percentOfWorkCurve);

        /// <summary>
        /// 循环工作剪辑
        /// </summary>
        public ILoopWorkClip loopWorkClip { get; private set; }

        /// <summary>
        /// 标识当前百分比是否在区间左
        /// </summary>
        public bool leftRange => percent < -SMSHelperExtension.Epsilon;

        /// <summary>
        /// 标识当前百分比是否在区间右
        /// </summary>
        public bool rightRange => percent > loopWorkClip.loopCount + SMSHelperExtension.Epsilon;

        /// <summary>
        /// 标识当前百分比是否在区间内
        /// </summary>
        public bool inRange => percent >= -SMSHelperExtension.Epsilon && percent <= loopWorkClip.loopCount + SMSHelperExtension.Epsilon;

        /// <summary>
        /// 零百分比对象
        /// </summary>
        public static Percent Zero => new Percent();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="loopWorkClip"></param>
        public void Init(ILoopWorkClip loopWorkClip)
        {
            this.loopWorkClip = loopWorkClip ?? throw new ArgumentNullException(nameof(loopWorkClip));
        }

        /// <summary>
        /// 更新百分比数据
        /// </summary>
        /// <param name="percentOfState">状态的百分比</param>
        /// <returns>返回当前百分比对象</returns>
        public Percent Update(float percentOfState)
        {
            this.percentOfState = percentOfState;
            percent = MathX.Scale(percentOfState - loopWorkClip.beginPercent, loopWorkClip.oncePercentLength);
            switch (loopWorkClip.loopType)
            {
                case ELoopType.None:
                    {
                        percent01 = percentOfLoopType = MathX.Clamp01(percent);
                        percentOfWorkCurve = loopWorkClip.workCurve.Evaluate(percent01);
                        break;
                    }
                case ELoopType.Loop:
                    {
                        percentOfLoopType = Loop01(percent);
                        if (leftRange)
                        {
                            percent01 = 0;
                        }
                        else if (rightRange)
                        {
                            if (loopWorkClip.continueLoopAfterWorkRange)
                            {
                                percent01 = percentOfLoopType;
                            }
                            else
                            {
                                percent01 = Loop01(loopWorkClip.percentOnAfterWorkRange);
                            }
                        }
                        else//in
                        {
                            percent01 = percentOfLoopType;
                        }
                        percentOfWorkCurve = loopWorkClip.workCurve.Evaluate(percent01);
                        break;
                    }
                case ELoopType.PingPong:
                    {
                        percentOfLoopType = PingPong01(percent);
                        if (leftRange)
                        {
                            percent01 = 0;
                        }
                        else if (rightRange)
                        {
                            if (loopWorkClip.continueLoopAfterWorkRange)
                            {
                                percent01 = percentOfLoopType;
                            }
                            else
                            {
                                percent01 = PingPong01(loopWorkClip.percentOnAfterWorkRange);
                            }
                        }
                        else//in
                        {
                            percent01 = percentOfLoopType;
                        }
                        percentOfWorkCurve = loopWorkClip.workCurve.Evaluate(percent01);
                        break;
                    }
            }
            return this;
        }

        /// <summary>
        /// 重置：将所有数据信息清零
        /// </summary>
        public void Reset()
        {
            percentOfState = 0;
            percent = 0;
            percentOfLoopType = 0;
            percent01 = 0;
            percentOfWorkCurve = 0;
        }

        /// <summary>
        /// 以Loop模式计算的百分比值
        /// </summary>
        /// <param name="f"></param>
        /// <returns>范围[0,1]</returns>
        public static float Loop01(float f)
        {
            var endFlag = f >= 1 || MathX.Approximately(f, 1);
            var value = MathX.DecimalPart(f);
            return endFlag && MathX.ApproximatelyZero(value) ? 1 : value;
        }

        /// <summary>
        /// 以PingPong模式计算的百分比值
        /// </summary>
        /// <param name="f"></param>
        /// <returns>范围[0,1]</returns>
        public static float PingPong01(float f)
        {
            var percent01 = MathX.DecimalPart(f);
            return (MathX.FloorToInt(f) % 2 == 0) ? percent01 : (1 - percent01);
        }
    }

}
