using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginSMS.States.Show;

namespace XCSJ.EditorSMS.States.Show
{
    [CustomEditor(typeof(StepClip), true)]
    public class StepClipInspector : StateComponentInspector<StepClip>
    {
        public override GUIContent OnGetPrefixLabel(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo, out FieldInfo memberFieldInfo)
        {
            var label = base.OnGetPrefixLabel(type, memberProperty, arrayElementInfo, out memberFieldInfo);
            switch (memberProperty.name)
            {
                case nameof(StepClip.step):
                    {
                        if(stateComponent.step)
                        {
                            label.text = stateComponent.step.parent.name;
                        }
                        break;
                    }
            }
            return label;
        }
    }
}
