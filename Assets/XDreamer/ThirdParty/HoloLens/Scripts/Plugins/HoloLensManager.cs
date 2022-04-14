using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.PluginHoloLens
{
    [Serializable]
    [DisallowMultipleComponent]
    [ComponentKit(EKit.Peripheral)]
    [ComponentOption(EComponentOptionType.Optional)]
    [Name("HoloLens")]
    [Tip("微软HoloLens设备")]
    [Guid("F5DB5EE3-6824-46E9-894A-65405A68561E")]
    [Version("22.301")]
    public class HoloLensManager : BaseManager<HoloLensManager>
    {
        public override List<Script> GetScripts()
        {
            return Script.GetScriptsOfEnum<HoloLensScriptID>(this);
        }

        public override ReturnValue RunScript(int id, ScriptParamList param)
        {
            //switch ((HoloLensScriptID)id)
            //{
                
            //}
            return ReturnValue.No;
        }
    }
}

