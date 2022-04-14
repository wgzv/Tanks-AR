using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginPhysicses.Base;
using XCSJ.PluginTools;

namespace XCSJ.PluginPhysicses.Tools
{
    /// <summary>
    /// 基础物理组件
    /// </summary>
    [RequireManager(typeof(PhysicsManager))]
    public abstract class BasePhysicsMB : ToolMB
    {
    }
}
