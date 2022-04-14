using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.Show;

namespace XCSJ.EditorSMS.States.Show
{
    [CustomEditor(typeof(StepGroup), true)]
    public class StepGroupInspector : StateComponentInspector
    {
        private StepGroup stepGroup => target as StepGroup;

        private bool expanded = true;

        public override void OnAfterVertical()
        {
            expanded = UICommonFun.Foldout(expanded, "步骤列表");

            if (!expanded) return;

            CommonFun.BeginLayout();            
            {
                TreeView.Draw(StepGroupHelper.InitStepTree(stepGroup).ToArray(), TreeView.DefaultDrawExpandedFunc, TreeView.DefaultPrefixFunc, (node, content) =>
                {
                    var step = node as Step;

                    bool stepError = false;

                    if (!(node is StepGroup))
                    {
                        if (step)
                        {
                            var stepClips = step.parent.parent.GetComponentsInChildren<StepClip>();
                            stepError = !stepClips.FirstOrDefault(c => c.step == step);
                        }
                    }

                    Color orgColor = GUI.color;
                    
                    if (stepError)
                    {
                        GUI.color = Color.red;
                        content.tooltip = "["+ content.tooltip + "] 缺少步骤剪辑!";
                    }
                    
                    if (GUILayout.Button(content, GUI.skin.label))
                    {
                        if (node.enable && stepGroup.parent is SubStateMachine subSM && subSM && subSM.busy)
                        {
                            node.OnClick();
                        }
                        Selection.activeObject = step;
                    }

                    GUI.color = orgColor;
                });
            }
            CommonFun.EndLayout();

            base.OnAfterVertical();
        }       
    }
}