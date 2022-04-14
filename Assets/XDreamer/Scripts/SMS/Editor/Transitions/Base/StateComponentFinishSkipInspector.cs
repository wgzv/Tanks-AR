using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.Transitions.Base;

namespace XCSJ.EditorSMS.Transitions.Base
{
    [CustomEditor(typeof(StateComponentFinishSkip))]
    public class StateComponentFinishSkipInspector : TransitionComponentInspector
    {
        public SerializedProperty objectsSP;

        public override void OnEnable()
        {
            if (!target) return;
            base.OnEnable();
            objectsSP = serializedObject.FindProperty(nameof(StateComponentFinishSkip.componentsInState));
        }

        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            if (arrayElementInfo.index < 0)
            {
                switch (memberProperty.name)
                {
                    case nameof(StateComponentFinishSkip.componentsInState):
                        {
                            if (GUILayout.Button(new GUIContent(" 添加", EditorIconHelper.GetIconInLib(EIcon.Add)), EditorStyles.miniButtonRight, GUILayout.Width(60), GUILayout.Height(18)))
                            {
                                AddStateComponent(objectsSP);
                            }
                            break;
                        }
                }
            }

            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }

        private void AddStateComponent(SerializedProperty memberProperty)
        {
            try
            {
                State inState = (target as TransitionComponent).parent.inState;

                MenuHelper.DrawMenu(memberProperty.name, m =>
                {
                    for (int i = 0; i < inState.components.Length; ++i)
                    {
                        var component = inState.components[i];
                        m.AddMenuItem((i + 1).ToString() + "." + component.GetType(), (c) =>
                        {
                            var obj = c as UnityEngine.Object;

                            // 已存在
                            for (int j = 0; j < memberProperty.arraySize; ++j)
                            {
                                var sp = memberProperty.GetArrayElementAtIndex(j);
                                if (sp != null && sp.objectReferenceValue == obj)
                                {
                                    return;
                                }
                            }

                            memberProperty.arraySize++;
                            memberProperty.GetArrayElementAtIndex(memberProperty.arraySize - 1).objectReferenceValue = obj;
                            memberProperty.serializedObject.ApplyModifiedProperties();
                        }, component);
                    }
                });
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}

