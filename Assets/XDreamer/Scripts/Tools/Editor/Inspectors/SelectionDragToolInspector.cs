using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorTools;
using XCSJ.PluginTools.Puts;

namespace XCSJ.EditorExtension.Tools
{
    [CustomEditor(typeof(DragByCameraView))]
    public class SelectionDragToolInspector : BaseInspectorWithLimit<DragByCameraView>
    {
        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(DragByCameraView.moveIcon):
                    {
                        EditorGUI.BeginDisabledGroup(targetObject.moveIcon);
                        if (GUILayout.Button(new GUIContent(" 添加", EditorIconHelper.GetIconInLib(EIcon.Add)), EditorStyles.miniButtonRight, GUILayout.Width(60), GUILayout.Height(18)))
                        {
                            var icon = LoadDefaultDragIcon();
                            if (icon)
                            {
                                memberProperty.objectReferenceValue = icon;
                                EditorGUIUtility.PingObject(icon.gameObject);
                            }
                        }
                        EditorGUI.EndDisabledGroup();
                        break;
                    }
            }
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }

        public static RectTransform LoadDefaultDragIcon()
        {
            return EditorXGUI.ToolsMenu.CreateUIInCanvas(() => EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("拖拽光标.prefab"));
        }
    }
}
