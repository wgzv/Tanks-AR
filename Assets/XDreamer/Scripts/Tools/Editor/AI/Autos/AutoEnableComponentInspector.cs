using System;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.AI.Autos;

namespace XCSJ.EditorTools.AI.Autos
{
    [CustomEditor(typeof(AutoEnableComponent), true)]
    public class AutoEnableComponentInspector : AutoWaitInspector<AutoEnableComponent>
    {
    }
}
