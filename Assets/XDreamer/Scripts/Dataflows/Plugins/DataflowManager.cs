using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.PluginDataflows
{
    /// <summary>
    /// 数据流：基于别名机制的数据流处理管理器,可实现跨插件的数据交互、界面层与控制层数据的分离等功能
    /// </summary>
    [Name("数据流")]
    [Tip("基于别名机制的数据流处理管理器,可实现跨插件的数据交互、界面层与控制层数据的分离等功能")]
    [ComponentKit(EKit.Advanced)]
    [ComponentOption(EComponentOptionType.Optional)]
    [Guid("610F183B-7343-4087-B322-57192080DE2D")]
    [Version("22.301")]
    public class DataflowManager : BaseManager<DataflowManager>
    {
        /// <summary>
        /// 获取脚本
        /// </summary>
        /// <returns></returns>
        public override List<Script> GetScripts()
        {
            return new List<Script>();
        }

        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override ReturnValue RunScript(int id, ScriptParamList param)
        {
            return ReturnValue.Create(false);
        }
    }
}
