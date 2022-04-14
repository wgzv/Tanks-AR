using UnityEngine;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Kernel;
using XCSJ.Extension.CNScripts;
using XCSJ.Extension.GenericStandards;
using XCSJ.Extension.GenericStandards.Gif;

namespace XCSJ.Extension.Base
{
    /// <summary>
    /// 插件组手类
    /// </summary>
    public static class PlguinsHelper
    {
        /// <summary>
        /// UnityEngine前缀
        /// </summary>
        public const string UnityEngine_Prefix = nameof(UnityEngine) + ".";

        /// <summary>
        /// UnityEngineInternal前缀
        /// </summary>
        public const string UnityEngineInternal_Prefix = nameof(UnityEngineInternal) + ".";

        /// <summary>
        /// UnityEngine.EventSystems前缀
        /// </summary>
        public const string UnityEngine_EventSystems_Prefix = UnityEngine_Prefix + nameof(UnityEngine.EventSystems) + ".";

        /// <summary>
        /// UnityEngine.UI前缀
        /// </summary>
        public const string UnityEngine_UI_Prefix = UnityEngine_Prefix + nameof(UnityEngine.UI) + ".";

        private static bool initialized = false;

        /// <summary>
        /// 初始化
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            if (initialized) return;
            initialized = true;

            PluginsHandlerExtension.Init();
            HelperInit();
        }

        private static void HelperInit()
        {
            ComponentManagerExtension.Init();
            GifHelper.Init();
            TypeCacheExtension.Init();
            CNScriptHelper.Init();
        }
    }
}
