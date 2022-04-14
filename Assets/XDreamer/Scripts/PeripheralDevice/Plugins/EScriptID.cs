using XCSJ.Extension;
using XCSJ.Scripts;

namespace XCSJ.PluginPeripheralDevice
{
    /// <summary>
    /// ID区间
    /// </summary>
    public static class IDRange
    {
        /// <summary>
        /// 开始35200
        /// </summary>
        public const int Begin = (int)EExtensionID._0x13;

        /// <summary>
        /// 结束35328-1=35327
        /// </summary>
        public const int End = (int)EExtensionID._0x14 - 1;

        public const int Fragment = 0x18;//24

        public const int Common = Begin + Fragment * 0;//35200
        public const int MonoBehaviour = Begin + Fragment * 1;//35224
        public const int StateLib = Begin + Fragment * 2;//35248
        public const int Tools = Begin + Fragment * 3;//35272
        public const int Editor = Begin + Fragment * 4;//35296
    }

    /// <summary>
    /// 脚本ID
    /// </summary>
    public enum EScriptID
    {
        /// <summary>
        /// 开始
        /// </summary>
        _Begin = IDRange.Begin,

        #region 外部设备输入-目录
        /// <summary>
        /// 拆装修理扩展
        /// </summary>
        [ScriptName("外部设备输入", "", EGrammarType.Category)]
        [ScriptDescription("外部设备输入的相关脚本目录；")]
        #endregion
        PeripheralDevice,

        #region 按键是否有操作
        [ScriptName("按键是否有操作", "Get Peripheral Device Button")]
        [ScriptDescription("获取按键是否有操作")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.String, "按键名称:")]
        #endregion
        GetButton,

        #region 按键是否有操作
        [ScriptName("指定设备按键是否有操作", "Get Appointed Peripheral Device Button")]
        [ScriptDescription("获取指定设备按键是否有操作")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.String, "设备名称:")]
        [ScriptParams(2, EParamType.String, "按键名称:")]
        #endregion
        GetPeripheralDeviceButton,

        #region 按键是否按下
        [ScriptName("按键是否按下", "Get Peripheral Device Button Down")]
        [ScriptDescription("获取按键是否按下")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.String, "按键名称:")]
        #endregion
        GetButtonDown,

        #region 按键是否按下
        [ScriptName("指定设备按键是否按下", "Get Appointed Peripheral Device Bitton Down")]
        [ScriptDescription("获取指定设备按键是否按下")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.String, "设备名称:")]
        [ScriptParams(2, EParamType.String, "按键名称:")]
        #endregion
        GetPeripheralDeviceButtonDown,

        #region 按键是否弹起
        [ScriptName("按键是否弹起", "Get Peripheral Device Button Up")]
        [ScriptDescription("获取按键是否弹起")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.String, "按键名称:")]
        #endregion
        GetButtonUp,

        #region 按键是否弹起
        [ScriptName("指定设备按键是否弹起", "Get Appointed Peripheral Device Button Up")]
        [ScriptDescription("获取指定设备按键是否弹起")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.String, "设备名称:")]
        [ScriptParams(2, EParamType.String, "按键名称:")]
        #endregion
        GetPeripheralDeviceButtonUp,

        #region 获取指定设备坐标轴值
        [ScriptName("获取指定设备坐标轴值", "Get Appointed Peripheral Device Axis Value")]
        [ScriptDescription("获取指定设备指定坐标轴值")]
        [ScriptReturn("成功返回坐标轴值 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.String, "设备名称:")]
        [ScriptParams(2, EParamType.String, "轴名称:")]
        #endregion
        GetPeripheralDeviceAxisValue,

        #region 获取射线原点
        [ScriptName("获取射线原点", "Get Ray Orgin")]
        [ScriptDescription("获取射线原点")]
        [ScriptReturn("成功返回射线原点 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObject, "输入源:", typeof(BaseInputSource))]
        #endregion
        GetRayOrgin,

        #region 获取射线方向
        [ScriptName("获取射线方向", "Get Ray Direction")]
        [ScriptDescription("获取射线方向")]
        [ScriptParams(1, EParamType.GameObject, "输入源:", typeof(BaseInputSource))]
        [ScriptReturn("成功返回射线方向 ;失败返回 #False;")]
        #endregion
        GetRayDirection,

        #region 获取射线长度
        [ScriptName("获取射线长度", "Get Ray End Lenth")]
        [ScriptDescription("获取射线长度")]
        [ScriptParams(1, EParamType.GameObject, "输入源:", typeof(BaseInputSource))]
        [ScriptReturn("成功返回射线长度 ;失败返回 #False;")]
        #endregion
        GetRayLenth,

        #region 获取射线终点
        [ScriptName("获取射线终点", "Get Ray End Point")]
        [ScriptDescription("获取射线终点")]
        [ScriptParams(1, EParamType.GameObject, "输入源:", typeof(BaseInputSource))]
        [ScriptReturn("成功返回射线终点 ;失败返回 #False;")]
        #endregion
        GetRayEndPoint,

        #region 设置射线显示隐藏
        [ScriptName("设置射线显示隐藏", "Set Ray Show or Hide")]
        [ScriptDescription("获取射线终点")]
        [ScriptParams(1, EParamType.GameObject, "输入源:", typeof(BaseInputSource))]
        [ScriptParams(2, EParamType.Bool, "显示:")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        #endregion
        SetRayShowOrHide,

        /// <summary>
        /// 当前已使用的脚本最大ID
        /// </summary>
        MaxCurrent
    }
}
