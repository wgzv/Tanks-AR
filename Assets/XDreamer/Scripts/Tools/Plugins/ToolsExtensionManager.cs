using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.PluginTools
{
    /// <summary>
    /// 工具库扩展:基于Unity游戏对象与组件开发的各种功能的集合扩展库
    /// </summary>
    [Name("工具库扩展")]
    [Tip("基于Unity游戏对象与组件开发的各种功能的集合扩展库")]
    [ComponentKit(EKit.Advanced)]
    [ComponentOption(EComponentOptionType.Optional)]
    [Guid("24E6703E-2B74-4F32-9D94-856A416F6934")]
    [Version("22.301")]
    [Index(index = 200)]
    public class ToolsExtensionManager : BaseManager<ToolsExtensionManager>
    {
        /// <summary>
        /// 获取脚本
        /// </summary>
        /// <returns></returns>
        public override List<Script> GetScripts() => Script.GetScriptsOfEnum<EExtensionScriptID>(this);

        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override ReturnValue RunScript(int id, ScriptParamList param)
        {
            return ReturnValue.No;
        }
    }
}
