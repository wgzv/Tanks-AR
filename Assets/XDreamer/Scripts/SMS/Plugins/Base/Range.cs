using UnityEngine;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.Base
{
    public abstract class Range
    {
        public abstract Vector2 range { get; }

        public float left => range.x;

        public float right => range.y;

        public float length => range.y - range.x;

        public float NormalizeOfRelativeLeft(float value)
        {
            if (value <= 0) return 0;
            var len = length;
            if (MathX.ApproximatelyZero(len)) return 0;
            if (value >= len) return 1;
            return value / len;
        }

        public float Normalize(float value)
        {
            return NormalizeOfRelativeLeft(value - range.x);
        }

        public float ScaleOfRelativeLeft(float value)
        {
            return MathX.Scale(value, length);
        }

        public float Scale(float value)
        {
            return ScaleOfRelativeLeft(value - range.x);
        }

        public bool In(float value, float epsilon = SMSHelperExtension.Epsilon)
        {
            return value >= range.x - epsilon && value <= range.y + epsilon;
        }

        public bool Left(float value, float epsilon = SMSHelperExtension.Epsilon)
        {
            return value < range.x - epsilon;

        }

        public bool Right(float value, float epsilon = SMSHelperExtension.Epsilon)
        {
            return value > range.y + epsilon;
        }

        public override string ToString() => CommonFun.Vector2ToString(range);
    }
}
