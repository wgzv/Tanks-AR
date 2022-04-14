using System;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO.NetSyncs;

namespace XCSJ.EditorMMO.NetSyncs
{
    [CustomEditor(typeof(NetProperty), true)]
    public class NetPropertyInspector : NetPropertyInspector<NetProperty>
    {

    }

    public class NetPropertyInspector<T> : NetMBInspector<T> where T : NetProperty
    {
        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(NetProperty.propertys):
                    {
                        try
                        {
                            EditorGUI.BeginChangeCheck();
                            BeginSyncVar(type, memberProperty, arrayElementInfo);

                            memberProperty.isExpanded = UICommonFun.Foldout(memberProperty.isExpanded, CommonFun.NameTip(type, nameof(NetProperty.propertys)), () =>
                            {
                                if (GUILayout.Button(UICommonOption.Insert, EditorStyles.miniButtonMid, GUILayout.Width(20), UICommonOption.Height16))
                                {
                                    memberProperty.arraySize++;
                                    CommonFun.FocusControl();
                                }
                                if (GUILayout.Button(UICommonOption.Delete, EditorStyles.miniButtonRight, GUILayout.Width(20), UICommonOption.Height16) && memberProperty.arraySize > 0)
                                {
                                    memberProperty.arraySize--;
                                    CommonFun.FocusControl();
                                }
                            });
                            if (!memberProperty.isExpanded) return false;

                            CommonFun.BeginLayout();

                            EditorGUILayout.BeginHorizontal(GUI.skin.box);
                            EditorGUILayout.LabelField("NO.", UICommonOption.Width20);
                            EditorGUILayout.LabelField("名称", UICommonOption.Width120);
                            EditorGUILayout.LabelField("值");
                            EditorGUILayout.LabelField("操作", GUILayout.Width(40));
                            EditorGUILayout.EndHorizontal();

                            for (int i = 0; i < memberProperty.arraySize; i++)
                            {
                                var sp = memberProperty.GetArrayElementAtIndex(i);

                                EditorGUILayout.BeginHorizontal();

                                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width20);

                                var nameSP = sp.FindPropertyRelative(nameof(Property.name));
                                nameSP.stringValue = EditorGUILayout.DelayedTextField(nameSP.stringValue, UICommonOption.Width120);

                                var valueSP = sp.FindPropertyRelative(nameof(Property.value));
                                valueSP.stringValue = EditorGUILayout.DelayedTextField(valueSP.stringValue);

                                if (GUILayout.Button(UICommonOption.Insert, EditorStyles.miniButtonMid, GUILayout.Width(20), UICommonOption.Height16))
                                {
                                    memberProperty.InsertArrayElementAtIndex(i);
                                    CommonFun.FocusControl();
                                }
                                if (GUILayout.Button(UICommonOption.Delete, EditorStyles.miniButtonRight, GUILayout.Width(20), UICommonOption.Height16))
                                {
                                    memberProperty.DeleteArrayElementAtIndex(i);
                                    CommonFun.FocusControl();
                                }

                                EditorGUILayout.EndHorizontal();
                            }

                            CommonFun.EndLayout();
                        }
                        finally
                        {
                            EndSyncVar(type, memberProperty, arrayElementInfo);
                            if (EditorGUI.EndChangeCheck())
                            {
                                mb.MarkDirty();
                            }
                        }                      

                        return false;
                    }
            }
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }
    }
}
