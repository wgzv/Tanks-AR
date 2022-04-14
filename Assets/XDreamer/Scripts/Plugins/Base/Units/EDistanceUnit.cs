using System.Collections.Generic;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Extension.Base.Maths;

namespace XCSJ.Extension.Base.Units
{
    /// <summary>
    /// 距离单位;长度单位
    /// </summary>
    [Name("距离单位")]
    public enum EDistanceUnit
    {
        [Name("米")]
        [Tip("默认距离单位")]
        [Unit("m")]
        M = 0,

        [Name("分米")]
        [Unit("dm")]
        DM,

        [Name("厘米")]
        [Unit("cm")]
        CM,

        [Name("毫米")]
        [Unit("mm")]
        MM,

        [Name("千米")]
        [Unit("km")]
        KM,
    }

    /// <summary>
    /// 距离单位扩展
    /// </summary>
    public static class DistanceUnitExtension
    {
        /// <summary>
        /// 单位缓存
        /// </summary>
        public class UnitCache : TICache<UnitCache, EDistanceUnit, string>
        {
            protected override KeyValuePair<bool, string> CreateValue(EDistanceUnit key1)
            {
                return new KeyValuePair<bool, string>(true, AttributeCache<UnitAttribute>.Get(EnumFieldInfoCache.GetFieldInfo(key1))?.unit ?? "");
            }
        }

        /// <summary>
        /// 由from单位转换到to单位
        /// </summary>
        /// <param name="from"></param>
        /// <param name="fromUnit"></param>
        /// <param name="toUnit"></param>
        /// <returns></returns>
        public static double ConvetTo(this double from, EDistanceUnit fromUnit, EDistanceUnit toUnit)
        {
            return ConvetFromDefault(ConvetToDefault(from, fromUnit), toUnit);
        }

        /// <summary>
        /// 由from单位转换到默认单位
        /// </summary>
        /// <param name="from"></param>
        /// <param name="fromUnit"></param>
        /// <returns></returns>
        public static double ConvetToDefault(this double from, EDistanceUnit fromUnit)
        {
            switch (fromUnit)
            {
                case EDistanceUnit.DM: return from / 10;
                case EDistanceUnit.CM: return from / 100;
                case EDistanceUnit.MM: return from / 1000;
                case EDistanceUnit.KM: return from * 1000;
                default: return from;
            }
        }

        /// <summary>
        /// 由默认单位转换到to单位
        /// </summary>
        /// <param name="from"></param>
        /// <param name="toUnit"></param>
        /// <returns></returns>
        public static double ConvetFromDefault(this double from, EDistanceUnit toUnit)
        {
            switch (toUnit)
            {
                case EDistanceUnit.DM: return from * 10;
                case EDistanceUnit.CM: return from * 100;
                case EDistanceUnit.MM: return from * 1000;
                case EDistanceUnit.KM: return from / 1000;
                default: return from;
            }
        }

        /// <summary>
        /// 获取单位
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static string Unit(this EDistanceUnit unit) => UnitCache.GetCacheValue(unit, "");

        /// <summary>
        /// 转字符串
        /// </summary>
        /// <param name="from"></param>
        /// <param name="fromUnit"></param>
        /// <param name="toUnit"></param>
        /// <param name="decimalPlaces"></param>
        /// <param name="displayUnit"></param>
        /// <returns></returns>
        public static string ToString(this double from, EDistanceUnit fromUnit, EDistanceUnit toUnit, int decimalPlaces, bool displayUnit = true)
        {
            var value = ConvetTo(from, fromUnit, toUnit);
            return value.ToString(decimalPlaces.ToDecimalFormat()) + (displayUnit ? toUnit.Unit() : "");
        }
    }
}
