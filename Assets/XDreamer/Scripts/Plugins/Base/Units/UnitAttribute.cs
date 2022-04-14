using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCSJ.Extension.Base.Units
{
    /// <summary>
    /// 单位特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class UnitAttribute : Attribute
    {
        /// <summary>
        /// 单位
        /// </summary>
        public string unit { get; private set; } = "";

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="unit"></param>
        public UnitAttribute(string unit)
        {
            this.unit = unit;
        }
    }
}
