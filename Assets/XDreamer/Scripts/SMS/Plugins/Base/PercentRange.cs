using System;
using UnityEngine;
using XCSJ.Maths;
using XCSJ.Attributes;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginSMS.Base
{
    [Serializable]
    public class PercentRange : Range, IPercentClip
    {
        [LimitRange(0, 1)]
        [Name("百分比区间")]
        [Tip("当前逻辑执行的百分比区间")]
        public Vector2 percentRange = new Vector2(0, 1);

        public override Vector2 range => percentRange;

        public float beginPercent { get => percentRange.x; set => percentRange.x = value; }
        public float endPercent { get => percentRange.y; set => percentRange.y = value; }
        public float percentLength { get => percentRange.y - percentRange.x; set => percentRange.y = MathX.Clamp01(percentRange.x + value); }
    }
}
