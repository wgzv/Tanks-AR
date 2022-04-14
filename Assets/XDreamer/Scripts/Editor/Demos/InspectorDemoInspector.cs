using System;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.Demos;

namespace XCSJ.EditorExtension.Demos
{
    [CustomEditor(typeof(InspectorDemo))]
    [Serializable]
    public class InspectorDemoInspector : BaseInspector<InspectorDemo>
    {
        public override void OnAfterScript()
        {
            if (targetObject.drawOnAfterScript) GUILayout.Button("OnBeforeVertical");
            base.OnAfterScript();
        }

        public override void OnBeforeVertical()
        {
            if (targetObject.drawOnBeforeVertical) GUILayout.Button("OnBeforeVertical");
            base.OnBeforeVertical();
        }

        public override void OnAfterVertical()
        {
            if (targetObject.drawOnAfterVertical) GUILayout.Button("OnAfterVertical");
            base.OnAfterVertical();
        }

        public override void OnBeforePropertyFieldVertical(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            if (targetObject.drawOnBeforePropertyFieldVertical) GUILayout.Button("Up");
            base.OnBeforePropertyFieldVertical(type, memberProperty, arrayElementInfo);
        }

        public override void OnAfterPropertyFieldVertical(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            if (targetObject.drawOnAfterPropertyFieldVertical) GUILayout.Button("Down");
            base.OnAfterPropertyFieldVertical(type, memberProperty, arrayElementInfo);
        }

        public override void OnBeforePropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            if (targetObject.drawOnBeforePropertyFieldHorizontal) GUILayout.Button("Left", GUILayout.Width(40));
            base.OnBeforePropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }

        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            if (targetObject.drawOnAfterPropertyFieldHorizontal) GUILayout.Button("Right", GUILayout.Width(40));
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }

        public override void OnBeforeGroupVertical(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            if (targetObject.drawOnBeforeGroupVertical) GUILayout.Button("Group Up");
            base.OnBeforeGroupVertical(type, memberProperty, arrayElementInfo);
        }

        public override void OnAfterGroupHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            if (targetObject.drawOnAfterGroupHorizontal) GUILayout.Button("Group Right", GUILayout.Width(80));
            base.OnAfterGroupHorizontal(type, memberProperty, arrayElementInfo);
        }

        public override void OnAfterUnityEngineObjectHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            if (targetObject.drawOnAfterUnityEngineObjectHorizontal) GUILayout.Button("Object Right", GUILayout.Width(80));
            base.OnAfterUnityEngineObjectHorizontal(type, memberProperty, arrayElementInfo);
        }
    }
}
