using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginRepairman.Devices;
using XCSJ.PluginRepairman.Machines;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Show;
using XCSJ.PluginTools.SelectionUtils;
using EPartState = XCSJ.PluginRepairman.Machines.EPartState;

namespace XCSJ.PluginRepairman.Task
{
    [ComponentMenu(RepairmanHelperExtension.StepPath + "/" + Title, typeof(RepairmanManager))]
    [Name(Title, nameof(RepairStep))]
    [XCSJ.Attributes.Icon(index = 34484)]
    [KeyNode(nameof(ITrigger), "触发器")]
    [KeyNode(nameof(IStep), "步骤")]
    [DisallowMultipleComponent]
    [RequireManager(typeof(RepairmanManager))]
    [Tip("拆装步骤组件是零件选择、工具选择和动画组成的触发器。用状态来实现。是一个数据组织对象、其中数据提供给其他状态组件使用。用户在场景中选择对应的零件与工具即可触发状态完成。时间轴可播放修理步骤所关联的动画，并同步界面步骤的选中状态。")]
    public class RepairStep : BaseTask, IDisplayWhenMemberHasArrayElement, ITrigger
    {
        /// <summary>
        /// 名称
        /// </summary>
        public new const string Title = "拆装步骤";

        [Name(Title, nameof(RepairStep))]
        [StateLib(RepairmanHelperExtension.StepStateLibName, typeof(RepairmanManager))]
        [StateComponentMenu(RepairmanHelperExtension.StepPath + "/" + Title, typeof(RepairmanManager))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("拆装步骤组件是零件选择、工具选择和动画组成的触发器。用状态来实现。是一个数据组织对象、其中数据提供给其他状态组件使用。用户在场景中选择对应的零件与工具即可触发状态完成。时间轴可播放修理步骤所关联的动画，并同步界面步骤的选中状态。")]
        public static State CreateRepairStepTask(IGetStateCollection obj)
        {
            return obj?.CreateNormalState(CommonFun.Name(typeof(RepairStep)), null, typeof(RepairStep));
        }

        /// <summary>
        /// 零件列表
        /// </summary>
        [Name("零件列表")]
        [ValidityCheck(EValidityCheckType.ElementCountGreater, 0)]
        [DisallowResizeArray(DisallowResizeArrayAttribute.EDisallowResizeArrayType.CanDelete)]
        [Readonly(EEditorMode.Runtime)]
        public List<Part> selectedParts = new List<Part>();

        /// <summary>
        /// 工具列表
        /// </summary>
        [Name("工具列表")]
        [DisallowResizeArray(DisallowResizeArrayAttribute.EDisallowResizeArrayType.CanDelete)]
        public List<Tool> selectedTools = new List<Tool>();

        private int maxSelectionCount = 0;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool Init(StateData data)
        {
            // 移除空对象
            RepairmanHelperExtension.RemoveNullObject(selectedParts);
            RepairmanHelperExtension.RemoveNullObject(selectedTools);

            return base.Init(data);
        }

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            Selection.Clear();

            maxSelectionCount = LimitedSelection.maxCount;
            LimitedSelection.maxCount = selectedParts.Count;

            ToolSelection.selectionMaxCount = selectedTools.Count;
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            LimitedSelection.maxCount = maxSelectionCount;

            base.OnExit(data);
        }

        /// <summary>
        /// 完成
        /// </summary>
        /// <returns></returns>
        protected override bool DefaultFinish()
        {
            return ToolSelection.Contains(selectedTools.Cast<ITool>().ToList()) && RepairmanHelperExtension.IsPartsSelected(selectedParts);
        }

        /// <summary>
        /// 友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            var partCount = selectedParts.Count;
            if (partCount>0)
            {
                if (selectedParts[0])
                {
                    return selectedParts[0].showName;
                }
                else
                {
                    return partCount.ToString();
                }
            }
            return "";
        }

        public override bool Skip()
        {
            Help();

            return base.Skip();
        }

        /// <summary>
        /// 设置零件、动画对象和工具被选中，满足步骤条件
        /// </summary>
        public override void Help()
        {
            Selection.Clear();

            selectedParts.ForEach(p => Selection.Add(p.gameObject));

            selectedTools.ForEach(t => ToolSelection.AddTool(t));
        }

        /// <summary>
        /// 是否响应点击
        /// </summary>
        public static bool isOnClick = true; 

        public override void OnClick()
        {
            if (isOnClick)
            {
                base.OnClick();
            }            
        }

        public static bool autoSelectPartAndTool = false;

        public override bool selected
        {
            set
            {
                base.selected = value;

                // 自动选择（在播放模式下需要使用）
                if (value && autoSelectPartAndTool)
                {
                    Help();
                }
            }
        }

        #region IDisplayWhenMemberHasArrayElement

        /// <summary>
        /// GUI文字
        /// </summary>
        /// <param name="name"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetGUIContentText(string name, int index)
        {
            switch (name)
            {
                case nameof(selectedParts):
                    {
                        return (index + 1) + "." + selectedParts[index].parent.name;
                    }
                case nameof(selectedTools):
                    {
                        return (index + 1) + "." + selectedTools[index].parent.name;
                    }
                default: return "";
            }
        }

        public string GetGUIContentTooltip(string name, int index) => GetGUIContentText(name, index);

        #endregion

    }
}
