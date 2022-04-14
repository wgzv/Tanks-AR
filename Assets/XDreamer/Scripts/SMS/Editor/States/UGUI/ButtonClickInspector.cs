using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.EditorExtension.XGUI;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.EditorSMS.NodeKit;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.UGUI;
using XCSJ.Attributes;
using XCSJ.EditorXGUI;

namespace XCSJ.EditorSMS.States.UGUI
{
    [CustomEditor(typeof(ButtonClick))]
    public class ButtonClickInspector : StateComponentInspector<ButtonClick>
    {
        private SerializedProperty buttonsSP = null;

        public override void OnEnable()
        {
            try
            {
                if (!target) return;

                base.OnEnable();
                buttonsSP = serializedObject.FindProperty(nameof(ButtonClick.buttons));
            }
            catch { }
        }

        private const float btnHeight = 18;

        [Name("选择对象")]
        [Tip("添加场景中选择的游戏对象\n(需锁定Inspector窗口)")]
        [XCSJ.Attributes.Icon(EIcon.Add)]
        public static XGUIContent selectGameObjectGUIContent { get; } = new XGUIContent(typeof(ButtonClickInspector), nameof(selectGameObjectGUIContent));

        [Name("所有按钮")]
        [Tip("添加场景中所有按钮")]
        [XCSJ.Attributes.Icon(EIcon.Add)]
        public static XGUIContent allButton { get; } = new XGUIContent(typeof(ButtonClickInspector), nameof(allButton));

        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(stateComponent.button):
                    {
                        EditorXGUIHelper.DrawCreateButton(stateComponent.button, () =>
                        {
                            ToolsMenu.CreateUIInCanvas(() =>
                            {
                                var btn = ToolsMenu.CreateUIWithStyle<Button>();
                                memberProperty.objectReferenceValue = btn;
                                return btn.gameObject;
                            });
                        });
                        break;
                    }
                case nameof(ButtonClick.buttons):
                    {
                        if(arrayElementInfo.index <0)
                        {
                            var isLock = EditorHandler.IsLockInspectorWindow();
                            if (GUILayout.Button(isLock ? CommonFun.NameTip(EIcon.Lock) : CommonFun.NameTip(EIcon.Unlock), EditorStyles.miniButtonLeft, GUILayout.Width(60), GUILayout.Height(btnHeight)))
                            {
                                EditorHandler.LockInspectorWindow(!isLock);
                            }

                            EditorGUI.BeginDisabledGroup(!EditorHandler.IsLockInspectorWindow());
                            if (GUILayout.Button(selectGameObjectGUIContent, EditorStyles.miniButtonMid, GUILayout.Width(60), GUILayout.Height(btnHeight)))
                            {
                                AddButton(CommonFun.GetComponents<Button>(Selection.gameObjects));
                            }
                            EditorGUI.EndDisabledGroup();

                            if (GUILayout.Button(allButton, EditorStyles.miniButtonRight, GUILayout.Width(60), GUILayout.Height(btnHeight)))
                            {
                                AddButton(CommonFun.GetComponentsInChildren<Button>(true));
                            }
                        }
                        break;
                    }
            }
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }

        protected void AddButton(IEnumerable<Button> buttons)
        {
            if (buttonsSP == null || buttons == null) return;

            foreach (var btn in buttons)
            {
                if (!btn) continue;
                if (stateComponent.buttons.Contains(btn)) continue;

                buttonsSP.arraySize++;
                buttonsSP.GetArrayElementAtIndex(buttonsSP.arraySize - 1).objectReferenceValue = btn;
            }
        }
    }
}
