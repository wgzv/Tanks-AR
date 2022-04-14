using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.CNScripts
{
    /// <summary>
    /// 脚本组手
    /// </summary>
    public static class CNScriptHelper
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "中文脚本";

        /// <summary>
        /// 中文脚本菜单
        /// </summary>
        public const string CNScriptMenu = Product.Name + "/中文脚本/";

        /// <summary>
        /// 输入菜单
        /// </summary>
        public const string InputMenu = CNScriptMenu + "输入/";

        /// <summary>
        /// UGUI菜单
        /// </summary>
        public const string UGUIMenu = CNScriptMenu + "UGUI/";

        /// <summary>
        /// NGUI菜单
        /// </summary>
        public const string NGUIMenu = CNScriptMenu + "NGUI/";

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            HierarchyKeyExtensionHelper.Init();
        }
    }
}
