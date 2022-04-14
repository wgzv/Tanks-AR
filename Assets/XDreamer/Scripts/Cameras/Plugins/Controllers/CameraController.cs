using XCSJ.Attributes;
using XCSJ.PluginsCameras.Base;

namespace XCSJ.PluginsCameras.Controllers
{
    /// <summary>
    /// 相机控制器
    /// </summary>
    [Name("相机控制器")]
    public sealed class CameraController : BaseCameraMainController
    {

    }

    /// <summary>
    /// 相机版本规则枚举
    /// </summary>
    [Name("相机版本规则")]
    public enum ECameraVersionRule
    {
        /// <summary>
        /// 新版优先:如果新版可用，则优先使用新版；如果新版本不可用，则使用旧版本；
        /// </summary>
        [Name("新版优先")]
        [Tip("如果新版可用，则优先使用新版；如果新版本不可用，则使用旧版本；")]
        NewFirst = 0,

        /// <summary>
        /// 旧版
        /// </summary>
        [Name("旧版")]
        Legacy,

        /// <summary>
        /// 新版
        /// </summary>
        [Name("新版")]
        New,
    }
}
