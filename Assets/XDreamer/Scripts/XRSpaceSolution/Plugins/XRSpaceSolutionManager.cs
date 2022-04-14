using System;
using System.Runtime.InteropServices;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Safety;
using XCSJ.PluginXRSpaceSolution.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.PluginXRSpaceSolution
{
    /// <summary>
    /// XR空间解决方案:用于统一整合常用外设型插件并配合XR配置工具使用的插件
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [ComponentKit(EKit.Professional)]
    [ComponentOption(EComponentOptionType.Optional)]
    [Name(XRSpaceSolutionHelper.Title)]
    [Tip("用于统一整合常用外设型插件并配合XR配置工具使用的插件")]
    [Guid("DFF3C70C-D71D-49F8-BD55-355E5E7CB4DD")]
    [Version("22.301")]
    public class XRSpaceSolutionManager : BaseManager<XRSpaceSolutionManager, EScriptsID>, IXRAccess
    {
        /// <summary>
        /// 运行脚本
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override ReturnValue RunScript(EScriptsID id, ScriptParamList param)
        {
            return ReturnValue.No;
        }
    }
}
