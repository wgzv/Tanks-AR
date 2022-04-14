using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Extension;

namespace XCSJ.PluginDataflows
{
    /// <summary>
    /// 数据流ID
    /// </summary>
    class IDRange
    {
        /// <summary>
        /// 开始值
        /// </summary>
        public const int Begin = (int)EExtensionID._0x1f;

        /// <summary>
        /// 结束值
        /// </summary>
        public const int End = ((int)EExtensionID._0x20 - 1);
    }
}
