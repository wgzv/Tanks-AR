using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginSMS.Base
{
    [Serializable]
    public class TimeRange : Range, ITimeClip
    {
        public const float DefaultMaxTimeLength = 3600;
        public const float DefaultTimeLength = 3;

        [LimitRange(0, DefaultMaxTimeLength)]
        [Name("时间区间")]
        [Tip("当前逻辑执行的时间区间")]
        public Vector2 timeRange = new Vector2(0, DefaultTimeLength);

        public override Vector2 range => timeRange;

        public float beginTime { get => timeRange.x; set => timeRange.x = value; }
        public float endTime { get => timeRange.y; set => timeRange.y = value; }
        public float timeLength { get => timeRange.y - timeRange.x; set => timeRange.y = timeRange.x + value; }
    }
}
