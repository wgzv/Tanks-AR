
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginRepairman.Devices;
using XCSJ.PluginRepairman.Machines;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Dataflows.DataModel;
using XCSJ.PluginSMS.States.Show;
using XCSJ.PluginTools.GameObjects;
using static XCSJ.PluginSMS.States.Show.Step;

namespace XCSJ.PluginRepairman.Task
{
    /// <summary>
    /// 拆装步骤通过匹配位置：使用步骤中的零件生成对应的匹配槽对象，使用拖拽工具将所有零件拖拽至与槽重合的位置时，认为步骤完成
    /// </summary>
    [ComponentMenu(RepairmanHelperExtension.StepStateLibName + "/" + Title, typeof(RepairmanManager))]
    [Name(Title, nameof(RepairStepByMatchPosition))]
    [XCSJ.Attributes.Icon(EIcon.Step)]
    [DisallowMultipleComponent]
    [Tip("使用步骤中的零件生成对应的匹配槽对象，使用拖拽工具将所有零件拖拽至与槽重合的位置时，认为步骤完成")]
    [RequireComponent(typeof(RepairStep))]
    public class RepairStepByMatchPosition : StateComponent<RepairStepByMatchPosition>, ITrigger
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "拆装步骤通过位置匹配";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(RepairmanHelperExtension.StepStateLibName, typeof(RepairmanManager))]
        [StateComponentMenu(RepairmanHelperExtension.StepStateLibName + "/" + Title, typeof(RepairmanManager))]
        [Name(Title, nameof(RepairStepByMatchPosition))]
        [Tip("使用步骤中的零件生成对应的匹配槽对象，使用拖拽工具将所有零件拖拽至与槽重合的位置时，认为步骤完成")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj)
        {
            return obj?.CreateNormalState(CommonFun.Name(typeof(RepairStepByMatchPosition)), null, typeof(RepairStepByMatchPosition));
        }

        /// <summary>
        /// 关联的修理步骤
        /// </summary>
        public RepairStep repairStep
        {
            get
            {
                if (!_repairStep)
                {
                    _repairStep = GetComponent<RepairStep>();
                }
                return _repairStep;
            }
        }
        private RepairStep _repairStep;

        public override void OnCreated()
        {
            base.OnCreated();

            repairStep._finishRule = EFinishRule.ExtensionCondition;
        }

        /// <summary>
        /// 预进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnBeforeEntry(StateData stateData)
        {
            base.OnBeforeEntry(stateData);

            repairStep.extensionFinishCondition += FinishCondition;
        }

        /// <summary>
        /// 预退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnAfterExit(StateData stateData)
        {
            base.OnAfterExit(stateData);

            repairStep.extensionFinishCondition -= FinishCondition;
        }

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            CreateSockets();
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            base.OnExit(data);

            DeleteSockets();
        }

        private bool FinishCondition() => _partSockets.Count > 0 && !_partSockets.Exists(s => s.empty);

        /// <summary>
        /// 完成
        /// </summary>
        /// <returns></returns>
        public override bool Finished() => FinishCondition();

        /// <summary>
        /// 有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return repairStep.selectedParts.Count > 0;
        }

        ///// <summary>
        ///// 帮助
        ///// </summary>
        //public override void Help()
        //{

        //}

        /// <summary>
        /// 零件插槽，用于拖拽拼装
        /// </summary>
        private List<PartSorket> _partSockets = new List<PartSorket>();

        private void CreateSockets()
        {
            _partSockets.Clear();

            foreach (var p in repairStep.selectedParts)
            {
                if (p)
                {
                    var ps = p.GetComponent<PartSocket>();
                    var socket = ps && ps.socket ? ps.socket : p.gameObject.transform;
                    _partSockets.Add(new PartSorket(p, socket, ESocketMatchRule.DisplaySocketSelfGameObject));
                }
            }
            GameObjectSocketCache.RegisterSockets(_partSockets);
        }

        private void DeleteSockets()
        {
            GameObjectSocketCache.UnregisterSockets(_partSockets);
            _partSockets.Clear();
        }
    }

    /// <summary>
    /// 零件插槽
    /// </summary>
    public class PartSorket : GameObjectSocket
    {
        /// <summary>
        /// 零件
        /// </summary>
        public Part part { get; private set; }

        /// <summary>
        /// 空
        /// </summary>
        public override bool empty 
        { 
            get => base.empty; 
            set
            {
                base.empty = value;
                // 插槽状态
                part.state = value ? EPartState.Disassembly : EPartState.Assembly; 
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="part"></param>
        /// <param name="socket"></param>
        /// <param name="matchRule"></param>
        public PartSorket(Part part, Transform socket, ESocketMatchRule matchRule) : base(part.gameObject, socket, matchRule)
        {
            this.part = part;

            var groupClip = part.GetComponent<SingleGroupMember>();
            if (groupClip)
            {
                group = groupClip.group;
            }
        }
    }
}
