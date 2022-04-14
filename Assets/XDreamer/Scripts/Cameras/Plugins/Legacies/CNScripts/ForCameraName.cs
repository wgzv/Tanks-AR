using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;
using IDRange = XCSJ.PluginCamera.IDRange;

namespace XCSJ.PluginsCameras.Legacies.CNScripts
{
    /// <summary>
    /// 相机名称专用
    /// </summary>
    public class ForCameraName : StringScriptParam
    {
        /// <summary>
        /// 脚本参数类型
        /// </summary>
        public const int ScriptParamType = IDRange.Begin;

        /// <summary>
        /// 获取扩展类型
        /// </summary>
        /// <returns></returns>
        public override int GetParamType() => ScriptParamType;
    }

    /// <summary>
    /// 相机聚焦模式专用
    /// </summary>
    public class ForCameraFocusType : EnumScriptParam<EBoundsAnchor>
    {
        /// <summary>
        /// 脚本参数类型
        /// </summary>
        public const int ScriptParamType = ForCameraName.ScriptParamType + 1;

        /// <summary>
        /// 获取扩展类型
        /// </summary>
        /// <returns></returns>
        public override int GetParamType() => ScriptParamType;
    }
}
