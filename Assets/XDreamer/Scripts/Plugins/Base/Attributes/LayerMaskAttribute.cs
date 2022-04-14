using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.Extension.Base.Maths;
using XCSJ.Helper;
using XCSJ.Maths;

namespace XCSJ.Extension.Base.Attributes
{
    /// <summary>
    /// 图层蒙版特性；用于修饰Int类型的字段，可用于选择多个图层；
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class LayerMaskAttribute : PropertyAttribute
    {
        /// <summary>
        /// 获取层名称列表
        /// </summary>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        public static string[] GetLayerNames(int layerMask)
        {
            var names = new List<string>();
            foreach(int i in layerMask.ToMaskIndexes())
            {
                var name = LayerMask.LayerToName(i);
                if (!string.IsNullOrEmpty(name))
                {
                    names.Add(name);
                }
            }
            return names.ToArray();
        }
    }
    /// <summary>
    /// 图层特性；用于修饰Int类型的字段，可用于选择单个图层；
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class LayerAttribute: PropertyAttribute
    {

    }
}
