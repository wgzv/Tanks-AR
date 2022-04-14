using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Extension;
using XCSJ.Scripts;

namespace XCSJ.PluginHoloLens
{
    public static class IDRange
    {
        public const int Begin = (int)EExtensionID._0x11;//34944
        public const int End = (int)EExtensionID._0xa - 1;

        public const int Fragment = 0x18;//24

        public const int Common = Begin + Fragment * 0;//34944
        public const int MonoBehaviour = Begin + Fragment * 1;//34968
        public const int StateLib = Begin + Fragment * 2;//34992
        public const int Tools = Begin + Fragment * 3;//35016
        public const int Editor = Begin + Fragment * 4;//35,040
    }

    public enum HoloLensScriptID
    {
        _Begin = IDRange.Begin,

        #region HoloLens-目录
        /// <summary>
        /// HoloLens脚本
        /// </summary>
        [ScriptName("HoloLens", "", EGrammarType.Category)]
        #endregion
        HoloLensCategory,

        MaxCurrent
    }
}
