using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.EditorExtension.XGUI;
using XCSJ.EditorSMS;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.EditorSMS.NodeKit;
using XCSJ.PluginCommonUtils;
using XCSJ.Attributes;
using XCSJ.EditorSMS.States.UGUI;

namespace XCSJ.PluginSMS.States.UGUI
{
    [CustomEditor(typeof(ToggleSwitchBatch))]
    public class ToggleSwitchBatchInspector : StateComponentInspector<ToggleSwitchBatch>
    {
        private SerializedProperty togglesSP = null;

        public override void OnEnable()
        {
            try
            {
                if (!target) return;

                base.OnEnable();
                togglesSP = serializedObject.FindProperty(nameof(ToggleSwitchBatch.toggles));
            }
            catch { }
        }

        private const float btnHeight = 18;

        [Name("所有Toggle")]
        [Tip("添加场景中所有Toggle")]
        [XCSJ.Attributes.Icon(EIcon.Add)]
        public static XGUIContent allButton { get; } = new XGUIContent(typeof(ToggleSwitchBatchInspector), nameof(allButton));

        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch(memberProperty.name)
            { 
                    case nameof(ToggleSwitchBatch.toggles):
                    {
                        if (arrayElementInfo.index < 0)
                        {
                            var isLock = EditorHandler.IsLockInspectorWindow();
                            if (GUILayout.Button(isLock ? CommonFun.NameTip(EIcon.Lock) : CommonFun.NameTip(EIcon.Unlock), EditorStyles.miniButtonLeft, GUILayout.Width(60), GUILayout.Height(btnHeight)))
                            {
                                EditorHandler.LockInspectorWindow(!isLock);
                            }

                            EditorGUI.BeginDisabledGroup(!EditorHandler.IsLockInspectorWindow());
                            if (GUILayout.Button(ButtonClickInspector.selectGameObjectGUIContent, EditorStyles.miniButtonMid, GUILayout.Width(70), GUILayout.Height(btnHeight)))
                            {
                                AddToggle(CommonFun.GetComponents<Toggle>(Selection.gameObjects));
                            }
                            EditorGUI.EndDisabledGroup();

                            if (GUILayout.Button(allButton, EditorStyles.miniButtonRight, GUILayout.Width(70), GUILayout.Height(btnHeight)))
                            {
                                AddToggle(CommonFun.GetComponentsInChildren<Toggle>(true));
                            }
                        }
                        break;
                    }
            }
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }

        protected void AddToggle(IEnumerable<Toggle> toggles)
        {
            if (togglesSP == null || toggles == null) return;

            foreach (var tg in toggles)
            {
                if (!tg) continue;
                if (stateComponent.toggles.Contains(tg)) continue;

                togglesSP.arraySize++;
                togglesSP.GetArrayElementAtIndex(togglesSP.arraySize - 1).objectReferenceValue = tg;
            }
        }
    }
}
