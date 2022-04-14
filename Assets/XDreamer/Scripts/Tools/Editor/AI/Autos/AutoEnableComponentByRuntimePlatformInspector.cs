using System;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorTools.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.AI.Autos;

namespace XCSJ.EditorTools.AI.Autos
{
    [CustomEditor(typeof(AutoEnableComponentByRuntimePlatform), true)]
    public class AutoEnableComponentByRuntimePlatformInspector : ToolMBInspector<AutoEnableComponentByRuntimePlatform>
    {
        
    }
}
