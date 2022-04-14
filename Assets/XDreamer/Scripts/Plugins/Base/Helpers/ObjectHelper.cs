using System;

namespace XCSJ.Extension.Base.Helpers
{
    /// <summary>
    /// 对象辅助类
    /// </summary>
    public static class ObjectHelper
    {
        /// <summary>
        /// 对象相等
        /// </summary>
        /// <param name="objA"></param>
        /// <param name="objB"></param>
        /// <returns></returns>
        public static bool ObjectEquals(this object objA, object objB)
        {
            if (objA == objB) return true;
            return objA != null ? objA.Equals(objB) : objB.Equals(objA);
        }

        /// <summary>
        /// 判断对象是否为Null
        /// </summary>
        /// <param name="objectValue"></param>
        /// <returns></returns>
        public static bool ObjectIsNull(this object objectValue)
        {
            return Equals(objectValue, null) || objectValue.Equals(null);
        }
    }
}
