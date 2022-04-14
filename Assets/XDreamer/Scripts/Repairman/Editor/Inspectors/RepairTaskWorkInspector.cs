using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.States.Show;
using XCSJ.EditorTools;
using XCSJ.PluginRepairman.Task;

namespace XCSJ.EditorRepairman.Inspectors
{
    /// <summary>
    /// 拆装修理任务检视器
    /// </summary>
    [CustomEditor(typeof(RepairTaskWork), true)]
    public class RepairTaskWorkInspector : StepGroupInspector
    {
        private string createStepButtonName;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            createStepButtonName = string.Format("创建[{0}]", XCSJ.EditorRepairman.Tools.ToolsMenu.RepairStepTreeViewName);
        }

        /// <summary>
        /// 垂直布局
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            if (GUILayout.Button(new GUIContent(createStepButtonName, EditorIconHelper.GetIconInLib(EIcon.Add)), UICommonOption.Height18))
            {
                XCSJ.EditorRepairman.Tools.ToolsMenu.CreateUITaskWorkTreeView(ToolContext.Get(typeof(XCSJ.EditorRepairman.Tools.ToolsMenu), nameof(XCSJ.EditorRepairman.Tools.ToolsMenu.CreateUITaskWorkTreeView)));
            }
        }
    }
}
