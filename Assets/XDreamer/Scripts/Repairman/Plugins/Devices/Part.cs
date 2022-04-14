using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginRepairman.Machines;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;

namespace XCSJ.PluginRepairman.Devices
{
    [ComponentMenu(RepairmanHelperExtension.DataModelStateLibName + "/" + Title, typeof(RepairmanManager))]
    [Name(Title, nameof(Part))]
    [XCSJ.Attributes.Icon(EIcon.Part)]
    [DisallowMultipleComponent]
    [RequireManager(typeof(RepairmanManager))]
    [Tip("零件组件用于关联一个三维模型和图片的容器。用状态来实现。是一个数据组织对象、其中数据提供给其他状态组件使用。零件不能放在设备和模块之外。零件不能包含零件。")]
    public class Part : Item
    {
        /// <summary>
        /// 名称
        /// </summary>
        public const string Title = "零件";

        [Name(Title, nameof(Part))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [StateLib(RepairmanHelperExtension.DataModelStateLibName, typeof(RepairmanManager))]
        [StateComponentMenu(RepairmanHelperExtension.DataModelStateLibName + "/" + Title, typeof(RepairmanManager))]
        [Tip("零件组件用于关联一个三维模型和图片的容器。用状态来实现。是一个数据组织对象、其中数据提供给其他状态组件使用。零件不能放在设备和模块之外。零件不能包含零件。")]
        public static State CreatePart(IGetStateCollection obj)
        {
            return obj?.CreateNormalState(CommonFun.Name(typeof(Part)), null, typeof(Part));
        }

        /// <summary>
        /// 零件状态
        /// </summary>
        public virtual EPartState state 
        { 
            get => _state; 
            set
            {
                if (_state != value)
                {
                    var old = _state;
                    _state = value;
                    onPartStateChanged?.Invoke(this, old);
                }
            }
        }
        [Name("零件状态")]
        private EPartState _state = EPartState.None;

        private EPartState _recordState = EPartState.None;

        /// <summary>
        /// 零件状态变化事件：参数1=零件，参数2=旧状态
        /// </summary>
        public static event Action<Part, EPartState> onPartStateChanged = null;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool Init(StateData data)
        {
            RecordPartState();
            return base.Init(data);
        }

        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="data"></param>
        public override void Reset(ResetData data)
        {
            base.Reset(data);

            RecoverPartState();
        }

        /// <summary>
        /// 记录零件状态
        /// </summary>
        public void RecordPartState() => _recordState = state;

        /// <summary>
        /// 还原零件状态
        /// </summary>
        public void RecoverPartState() => state = _recordState;

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => base.DataValidity() && GetParentItem();

        /// <summary>
        /// 友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return GetParentItem() ? "" : "需放在模块和设备中!";
        }
    }
}
