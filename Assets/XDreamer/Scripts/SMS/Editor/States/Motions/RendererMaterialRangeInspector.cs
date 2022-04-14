using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    [CustomEditor(typeof(RendererMaterialRange))]
    public class RendererMaterialRangeInspector : RendererRangeHandleInspector<RendererMaterialRange>
    {
        public override GUIContent OnGetPrefixLabel(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo, out FieldInfo memberFieldInfo)
        {
            var label = base.OnGetPrefixLabel(type, memberProperty, arrayElementInfo, out memberFieldInfo);
            switch (memberProperty.name)
            {
                case nameof(workClip.leftValue):
                    {
                        label.text = "区间左材质列表";
                        break;
                    }
                case nameof(workClip.inValue):
                    {
                        label.text = "区间内材质列表";
                        break;
                    }
                case nameof(workClip.rightValue):
                    {
                        label.text = "区间右材质列表";
                        break;
                    }
            }
            return label;
        }
    }
}
