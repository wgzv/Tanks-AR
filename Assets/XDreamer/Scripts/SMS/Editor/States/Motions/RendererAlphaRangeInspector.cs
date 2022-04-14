using System;
using System.Reflection;
using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    [CustomEditor(typeof(RendererAlphaRange))]
    public class RendererAlphaRangeInspector : RendererRangeHandleInspector<RendererAlphaRange>
    {
        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch(memberProperty.name)
            {
                case nameof(stateComponent.leftValue):
                case nameof(stateComponent.inValue):
                case nameof(stateComponent.rightValue):
                    {
                        memberProperty.floatValue = EditorGUILayout.Slider(OnGetPrefixLabel(type, memberProperty, arrayElementInfo, out FieldInfo memberFieldInfo), memberProperty.floatValue, 0, 1);
                        return false;
                    }
            }
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }
    }
}
