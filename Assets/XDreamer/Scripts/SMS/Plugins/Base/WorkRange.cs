using System;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Maths;
using XCSJ.Attributes;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginSMS.Base
{
    [Serializable]
    public class WorkRange : ITimeClip, IPercentClip, ITTL
    {
        static WorkRange()
        {
            Converter.instance.Register<WorkRange, string>(i => i.ToString());
            Converter.instance.Register<string, WorkRange>(i => StringToWorkRange(i));
        }

        [OnlyMemberElements]
        [Name("百分比区间")]
        public PercentRange percentRange = new PercentRange();

        [OnlyMemberElements]
        [Name("时间区间")]
        public TimeRange timeRange = new TimeRange();

        public WorkRange() { }

        public WorkRange(Vector4 range)
        {
            percentRange.percentRange = new Vector2(range.x, range.y);
            timeRange.timeRange = new Vector2(range.z, range.w);
        }

        public float totalTimeLength
        {
            get => MathX.Scale(timeRange.length, percentRange.length);
            set => timeRange.timeRange = value * percentRange.percentRange;
        }

        public float beginTime { get => timeRange.beginTime; set => timeRange.beginTime = value; }
        public float endTime { get => timeRange.endTime; set => timeRange.endTime = value; }
        public float timeLength { get => timeRange.timeLength; set => timeRange.timeLength = value; }

        public float beginPercent { get => percentRange.beginPercent; set => percentRange.beginPercent = value; }
        public float endPercent { get => percentRange.endPercent; set => percentRange.endPercent = value; }
        public float percentLength { get => percentRange.percentLength; set => percentRange.percentLength = value; }

        public override string ToString() => string.Format("{0}/{1}", percentRange.ToString(), timeRange.ToString());

        public static string WorkRangeToString(WorkRange workRange) => workRange != null ? workRange.ToString() : "";

        public static WorkRange StringToWorkRange(string value) => new WorkRange(CommonFun.StringToVector4(value));
    }
}
