using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Input;
using XCSJ.EditorSMS.States;
using XCSJ.EditorSMS.States.Nodes;
using XCSJ.EditorSMS.States.Show;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginRepairman.Devices;
using XCSJ.PluginRepairman.Task;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.EditorRepairman.Inspectors
{
    /// <summary>
    /// 拆装步骤属性
    /// </summary>
    [CustomEditor(typeof(RepairStep), true)]
    public class RepairStepInspector : StepInspector
    {
        /// <summary>
        /// 拆装步骤
        /// </summary>
        public RepairStep repairStep => target as RepairStep;

        /// <summary>
        /// 后属性渲染编辑
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberProperty"></param>
        /// <param name="arrayElementInfo"></param>
        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            if (arrayElementInfo.index < 0)
            {
                switch (memberProperty.name)
                {
                    case nameof(RepairStep.selectedParts):
                        {
                            DrawSelectComponent<Part>(serializedObject.FindProperty(nameof(RepairStep.selectedParts)));
                            break;
                        }
                    case nameof(RepairStep.selectedTools):
                        {
                            DrawSelectComponent<XCSJ.PluginRepairman.Devices.Tool>(serializedObject.FindProperty(nameof(RepairStep.selectedTools)));
                            break;
                        }
                }
            }

            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }

        private void DrawSelectComponent<T>(SerializedProperty memberProperty) where T : StateComponent
        {
            SMTreeEditorWindow.SelectStateComponentWithButton<T>((win, sc) => SetMemberPropertyList(memberProperty, sc));

            if (GUILayout.Button(new GUIContent(EditorIconHelper.GetIconInLib(EIcon.Add), "添加选中对象到列表中"), EditorStyles.miniButtonLeft, UICommonOption.WH20x16))
            {
                int addObjectCount = 0;
                foreach (var node in NodeSelection.selections)
                {
                    if (node is StateNode sn)
                    {
                        var targetObj = sn.state.GetComponent<T>();
                        if (targetObj)
                        {
                            ++addObjectCount;
                            SetMemberPropertyList(memberProperty, targetObj);
                        }
                    }
                }
                if (addObjectCount == 0)
                {
                    Debug.LogWarningFormat("添加失败，请选择状态机中的[{0}]对象!", CommonFun.Name(typeof(T)));
                }
            }

            if (GUILayout.Button(new GUIContent(EditorIconHelper.GetIconInLib(EIcon.Delete), "清除列表中所有对象"), EditorStyles.miniButtonRight, UICommonOption.WH20x16))
            {
                memberProperty.ClearArray();
                memberProperty.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
