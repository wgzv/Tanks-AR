using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;

namespace XCSJ.PluginRepairman.Devices
{
    [ComponentMenu(RepairmanHelperExtension.DataModelStateLibName + "/" + Title, typeof(RepairmanManager))]    
    [Name(Title, nameof(Device))]
    [XCSJ.Attributes.Icon(index = 34481)]
    [DisallowMultipleComponent]
    [RequireState(typeof(SubStateMachine))]
    [RequireManager(typeof(RepairmanManager))]
    [Tip("设备组件是包含零件组件与模块组件的容器。是一个数据组织对象、其中数据提供给其他状态组件使用。")]
    public class Device : Module
    {
        /// <summary>
        /// 名称
        /// </summary>
        public new const string Title = "设备";

        [Name(Title, nameof(Device))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [StateLib(RepairmanHelperExtension.DataModelStateLibName, typeof(RepairmanManager), stateType = EStateType.SubStateMachine)]
        [StateComponentMenu(RepairmanHelperExtension.DataModelStateLibName + "/" + Title, typeof(RepairmanManager))]
        [Tip("设备组件是包含零件组件与模块组件的容器。是一个数据组织对象、其中数据提供给其他状态组件使用。")]
        public static State CreateDevice(IGetStateCollection obj)
        {
            return obj?.CreateSubStateMachine(CommonFun.Name(typeof(Device)), null, typeof(Device));
        }

        public override ETreeNodeType nodeType => ETreeNodeType.Root;

        public override bool DataValidity() => go && !GetParentItem();

        public override string ToFriendlyString()
        {
            return GetParentItem()?"父容器不能是物品":"";
        }
    }
}
