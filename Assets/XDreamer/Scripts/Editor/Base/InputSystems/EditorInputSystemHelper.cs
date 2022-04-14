namespace XCSJ.EditorExtension.Base
{
    /// <summary>
    /// 输入系统辅助类
    /// </summary>
    public static class EditorInputSystemHelper
    {
        /// <summary>
        /// 输入系统Android宏
        /// </summary>
        public static readonly Macro XDREAMER_INPUT_SYSTEM_ANDROID = new Macro(nameof(XDREAMER_INPUT_SYSTEM_ANDROID), XDreamerEditor.DefaultBuildTargetGroups);

        /// <summary>
        /// 输入系统iOS宏
        /// </summary>
        public static readonly Macro XDREAMER_INPUT_SYSTEM_IOS = new Macro(nameof(XDREAMER_INPUT_SYSTEM_IOS), XDreamerEditor.DefaultBuildTargetGroups);

        /// <summary>
        /// 输入系统WebGL宏
        /// </summary>
        public static readonly Macro XDREAMER_INPUT_SYSTEM_WEBGL = new Macro(nameof(XDREAMER_INPUT_SYSTEM_WEBGL), XDreamerEditor.DefaultBuildTargetGroups);

        /// <summary>
        /// 输入系统Standalone宏
        /// </summary>
        public static readonly Macro XDREAMER_INPUT_SYSTEM_STANDALONE = new Macro(nameof(XDREAMER_INPUT_SYSTEM_STANDALONE), XDreamerEditor.DefaultBuildTargetGroups);

        /// <summary>
        /// 输入系统Switch宏
        /// </summary>
        public static readonly Macro XDREAMER_INPUT_SYSTEM_SWITCH = new Macro(nameof(XDREAMER_INPUT_SYSTEM_SWITCH), XDreamerEditor.DefaultBuildTargetGroups);

        /// <summary>
        /// 输入系统XInput宏
        /// </summary>
        public static readonly Macro XDREAMER_INPUT_SYSTEM_XINPUT = new Macro(nameof(XDREAMER_INPUT_SYSTEM_XINPUT), XDreamerEditor.DefaultBuildTargetGroups);

        /// <summary>
        /// 初始化
        /// </summary>
        [Macro]
        public static void Init()
        {
#if ENABLE_INPUT_SYSTEM //输入系统启用情况下

#if UNITY_ANDROID
            XDREAMER_INPUT_SYSTEM_ANDROID.DefineIfNoDefined();
#else
            XDREAMER_INPUT_SYSTEM_ANDROID.UndefineWithSelectedBuildTargetGroup();
#endif

#if UNITY_IOS
            XDREAMER_INPUT_SYSTEM_IOS.DefineIfNoDefined();
#else
            XDREAMER_INPUT_SYSTEM_IOS.UndefineWithSelectedBuildTargetGroup();
#endif

#if UNITY_WEBGL
            XDREAMER_INPUT_SYSTEM_WEBGL.DefineIfNoDefined();
#else
            XDREAMER_INPUT_SYSTEM_WEBGL.UndefineWithSelectedBuildTargetGroup();
#endif

#if UNITY_STANDALONE
            XDREAMER_INPUT_SYSTEM_STANDALONE.DefineIfNoDefined();
            XDREAMER_INPUT_SYSTEM_XINPUT.DefineIfNoDefined();
#else
            XDREAMER_INPUT_SYSTEM_STANDALONE.UndefineWithSelectedBuildTargetGroup();
            XDREAMER_INPUT_SYSTEM_XINPUT.UndefineWithSelectedBuildTargetGroup();
#endif

#if UNITY_SWITCH
            XDREAMER_INPUT_SYSTEM_SWITCH.DefineIfNoDefined();
#else
            XDREAMER_INPUT_SYSTEM_SWITCH.UndefineWithSelectedBuildTargetGroup();
#endif

#else

            XDREAMER_INPUT_SYSTEM_ANDROID.UndefineWithSelectedBuildTargetGroup();
            XDREAMER_INPUT_SYSTEM_IOS.UndefineWithSelectedBuildTargetGroup();
            XDREAMER_INPUT_SYSTEM_WEBGL.UndefineWithSelectedBuildTargetGroup();
            XDREAMER_INPUT_SYSTEM_STANDALONE.UndefineWithSelectedBuildTargetGroup();
            XDREAMER_INPUT_SYSTEM_XINPUT.UndefineWithSelectedBuildTargetGroup();
            XDREAMER_INPUT_SYSTEM_SWITCH.UndefineWithSelectedBuildTargetGroup();

#endif

        }
    }
}
