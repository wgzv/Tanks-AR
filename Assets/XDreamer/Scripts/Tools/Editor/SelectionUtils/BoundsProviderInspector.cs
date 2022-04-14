using System;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.SelectionUtils;

namespace XCSJ.EditorTools.SelectionUtils
{
    /// <summary>
    /// 包围盒提供器检查器
    /// </summary>
    [CustomEditor(typeof(BoundsProvider))]
    public class BoundsProviderInspector : BaseInspector<BoundsProvider>
    {
        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }
    }
}
