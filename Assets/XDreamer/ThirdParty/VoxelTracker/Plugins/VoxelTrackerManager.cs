﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.PluginVoxelTracker
{
    /// <summary>
    /// VoxelTracker:用于对接VoxelTracker官方插件包的管理器组件
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [ComponentKit(EKit.Peripheral)]
    [ComponentOption(EComponentOptionType.Optional)]
    [Name(VoxelTrackerHelper.Title)]
    [Tip("用于对接VoxelTracker官方插件包的管理器组件")]
    [Guid("D8753C32-6D60-468D-AD15-750FA2A321D4")]
    [Version("22.301")]
    public sealed class VoxelTrackerManager : BaseManager<VoxelTrackerManager>
    {
        /// <summary>
        /// 获取脚本
        /// </summary>
        /// <returns></returns>
        public override List<Script> GetScripts() => default;

        /// <summary>
        /// 运行脚本
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override ReturnValue RunScript(int id, ScriptParamList param) => ReturnValue.No;
    }
}
