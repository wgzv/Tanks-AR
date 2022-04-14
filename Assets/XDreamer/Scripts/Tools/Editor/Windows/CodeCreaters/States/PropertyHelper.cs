using System;
using System.Collections.Generic;
using UnityEngine;

namespace XCSJ.EditorTools.Windows.CodeCreaters.States
{
    /// <summary>
    /// 属性辅助类
    /// </summary>
    public static class PropertyHelper
    {
        /// <summary>
        /// 字段的第一个枚举值
        /// </summary>
        public const int FieldFirstEnumValue = 1;

        /// <summary>
        /// 属性的第一个枚举值
        /// </summary>
        public const int PropertyFirstEnumValue = 1000;

        /// <summary>
        /// 方法的第一个枚举值
        /// </summary>
        public const int MethodFirstEnumValue = 10000;

        /// <summary>
        /// 有效的属性类型
        /// </summary>
        public static HashSet<Type> validPropertyTypes = new HashSet<Type> {
            typeof(bool),
            typeof(int),
            typeof(float),
            typeof(double),
            typeof(string),
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector4),
            typeof(Vector2Int),
            typeof(Vector3Int),
            typeof(Rect),
            typeof(Color) };

        /// <summary>
        /// 有效属性类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool ValidPropertyType(this Type type)
        {
            if (type == null) return false;
            if (validPropertyTypes.Contains(type)) return true;
            if (typeof(UnityEngine.Object).IsAssignableFrom(type)) return true;
            if (type.IsEnum) return true;
            return false;
        }
    }
}
