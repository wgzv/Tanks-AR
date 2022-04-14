using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Transitions.CNScripts;
using XCSJ.PluginRepairman.Task;

namespace XCSJ.EditorRepairman.Inspectors
{
    [CustomEditor(typeof(RepairStepSkipWhenButtonClick), true)]
    public class RepairStepSkipButtonClickInspector : ButtonClickSkipInspector
    {
        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(RepairStepSkipWhenButtonClick.step):
                {
                    if (GUILayout.Button(new GUIContent(" 输入状态", EditorIconHelper.GetIconInLib(EIcon.Add)), EditorStyles.miniButtonRight, GUILayout.Width(60)))
                    {
                        var t = targetObject as RepairStepSkipWhenButtonClick;
                        t.step = TaskHelper.FindStepInPreviousState(t.parent); 
                    }
                    break;
                }
            }
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }
    }
}
