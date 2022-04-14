using System;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.EditorSMS.Utils;
using XCSJ.EditorExtension;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Attributes;
using XCSJ.PluginSMS.States.Components;
using XCSJ.EditorSMS.States.Base;

namespace XCSJ.EditorSMS.States.Components
{
    [CustomEditor(typeof(ComponentSet))]
    public class ComponentSetInspector : ObjectSetInspector<ComponentSet>
    {
        private static GameObject curAddGameObject = null;

        private Color orgColor;

        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(ComponentSet.includeSelf):
                    {
                        return false;
                    }
                case ObjectsString:
                    {
                        if (arrayElementInfo.index < 0)
                        {
                            GUILayout.BeginHorizontal();
                            {
                                curAddGameObject = EditorGUILayout.ObjectField("游戏对象", curAddGameObject, typeof(GameObject), true) as GameObject;

                                EditorGUI.BeginDisabledGroup(!curAddGameObject);
                                if (GUILayout.Button(new GUIContent("添加", EditorIconHelper.GetIconInLib(EIcon.Add)), EditorStyles.miniButtonRight, GUILayout.Width(60), GUILayout.Height(18)))
                                {
                                    AddComponent(objectsSP);
                                }
                                EditorGUI.EndDisabledGroup();
                            }
                            GUILayout.EndHorizontal();
                        }
                        break;
                    }
            }
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }

        public SerializedProperty objectsSP;

        public override void OnEnable()
        {
            if (!target) return;

            base.OnEnable();
            objectsSP = serializedObject.FindProperty(ObjectsString);
        }

        private void AddComponent(SerializedProperty memberProperty) 
        {            
            Component[] components = curAddGameObject.GetComponents<Component>();

            MenuHelper.DrawMenu(curAddGameObject.name, m =>
            {
                for (int i=0; i< components.Length; ++i)
                {
                    var component = components[i];

                    m.AddMenuItem((i+1).ToString() + "." + component.GetType().Name, (c) =>
                    {
                        var obj = c as UnityEngine.Object;
                        for (int j = 0; j < memberProperty.arraySize; ++j)
                        {
                            var sp = memberProperty.GetArrayElementAtIndex(j);
                            if (sp.objectReferenceValue == obj)
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

        [Name("无效对象")]
        [Tip("移除无效对象")]
        [XCSJ.Attributes.Icon(EIcon.Delete)]
        public static XGUIContent deleteButton { get; } = new XGUIContent(typeof(ComponentSetInspector), nameof(deleteButton));

        [Name("去除重复")]
        [Tip("去除重复对象")]
        [XCSJ.Attributes.Icon(EIcon.Delete)]
        public static XGUIContent distinctButton { get; } = new XGUIContent(typeof(ComponentSetInspector), nameof(distinctButton));

        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case ObjectsString:
                    {
                        if (arrayElementInfo.index < 0)
                        {
                            EditorGUI.BeginDisabledGroup(arrayElementInfo.isReadonly);

                            if (GUILayout.Button(distinctButton, EditorStyles.miniButtonLeft, UICommonOption.Width60, UICommonOption.Height18))
                            {
                                SerializedObjectHelper.ArrayElementDistinct(memberProperty, (x, y) => x.serializedProperty.objectReferenceValue == y.serializedProperty.objectReferenceValue);
                                GUI.FocusControl("");
                            }

                            if (GUILayout.Button(deleteButton, EditorStyles.miniButtonRight, UICommonOption.Width60, UICommonOption.Height18))
                            {
                                UICommonFun.DeleteArrayInvalidElements(objectsSP);
                                GUI.FocusControl("");
                            }

                            EditorGUI.EndDisabledGroup();
                        }
                        break;
                    }
                default: break;
            }
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }

        private bool setErrorColor = false;

        public override void OnBeforePropertyFieldVertical(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case ObjectsString:
                    {
                        if (!setErrorColor && !stateComponent.DataValidity())
                        {
                            setErrorColor = true;
                            orgColor = GUI.backgroundColor;
                            GUI.backgroundColor = Color.red;
                        }
                        break;
                    }
            }
            base.OnBeforePropertyFieldVertical(type, memberProperty, arrayElementInfo);
        }

        public override void OnAfterPropertyFieldVertical(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {                
                case ObjectsString:
                    {
                        if (setErrorColor)
                        {
                            setErrorColor = false;
                            GUI.backgroundColor = orgColor;
                        }
                        break;
                    }
            }
            base.OnAfterPropertyFieldVertical(type, memberProperty, arrayElementInfo);
        }
    }
}
