using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;

namespace XCSJ.Extension.Base.Components
{
    /// <summary>
    /// 基础控制器
    /// </summary>
    [Icon]
    public abstract class BaseController : ToolMB, IController { }

    /// <summary>
    /// 控制器
    /// </summary>
    public interface IController { }
}
