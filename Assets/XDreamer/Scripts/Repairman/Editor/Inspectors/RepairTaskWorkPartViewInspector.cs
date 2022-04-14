using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Input;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.EditorSMS.States.Nodes;
using XCSJ.EditorTools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginRepairman.Devices;
using XCSJ.PluginRepairman.Task;
using XCSJ.PluginTools.GameObjects;

namespace XCSJ.EditorRepairman.Inspectors
{
    /// <summary>
    /// 拆装任务零件视图检视器
    /// </summary>
    [CustomEditor(typeof(RepairTaskWorkPartView), true)]
    public class RepairTaskWorkPartViewInspector : StateComponentInspector<RepairTaskWorkPartView>
    {
        private string createStepButtonName;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            createStepButtonName = string.Format("添加选中的零件为[{0}]", RepairStepByMatchPosition.Title);
        }

        /// <summary>
        /// 属性字段水平布局后绘制
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberProperty"></param>
        /// <param name="arrayElementInfo"></param>
        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(RepairTaskWorkPartView._gameObjectViewItemDataList):
                    {
                        if (GUILayout.Button(new GUIContent(EditorIconHelper.GetIconInLib(EIcon.Add),"创建游戏对象列表"), UICommonOption.WH32x16))
                        {
                            var go = XCSJ.EditorXGUI.ToolsMenu.CreateGameObjectListWindow(ToolContext.Get(typeof(XCSJ.EditorXGUI.ToolsMenu), nameof(XCSJ.EditorXGUI.ToolsMenu.CreateGameObjectListWindow)));
                            if (go)
                            {
                                stateComponent.XModifyProperty(() => stateComponent._gameObjectViewItemDataList = go.GetComponentInChildren<GameObjectViewItemDataList>());
                            }
                        }
                        break;
                    }
            }
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }

        /// <summary>
        /// 垂直布局后绘制
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            // 根据选择零件动态添加步骤
            if (GUILayout.Button(new GUIContent(createStepButtonName, EditorIconHelper.GetIconInLib(EIcon.Add)), UICommonOption.Height18))
            {
                foreach (var node in NodeSelection.selections)
                {
                    if (node is StateNode sn)
                    {
                        var s = sn.state;
                        if (s)
                        {
                            var part = s.GetComponent<Part>();
                            if (part)
                            {
                                var stepState = RepairStepByMatchPosition.Create(stateComponent.parent);
                                var step = stepState.GetComponent<RepairStep>();
                                step.XModifyProperty(() => step.selectedParts.Add(part));
                            }
                        }
                    }
                }
            }
        }
    }
}
