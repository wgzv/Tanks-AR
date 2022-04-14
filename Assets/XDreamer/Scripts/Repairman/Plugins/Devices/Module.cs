using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginRepairman.Machines;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Dataflows.DataModel;

namespace XCSJ.PluginRepairman.Devices
{
    /// <summary>
    /// 模块
    /// </summary>
    [ComponentMenu(RepairmanHelperExtension.DataModelStateLibName + "/" + Title, typeof(RepairmanManager))]
    [Name(Title, nameof(Module))]
    [XCSJ.Attributes.Icon(EIcon.Engine)]
    [DisallowMultipleComponent]
    [RequireState(typeof(SubStateMachine))]
    [RequireManager(typeof(RepairmanManager))]
    [Tip("模块组件包含多个零件组件的容器。用子状态机实现。是一个数据组织对象、其中数据提供给其他状态组件使用。模块是介于设备与零件之间的中间层概念，模块可以嵌套模块。")]
    public class Module : Part
    {
        /// <summary>
        /// 名称
        /// </summary>
        public new const string Title = "模块";

        /// <summary>
        /// 创建模块
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(Module))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [StateLib(RepairmanHelperExtension.DataModelStateLibName, typeof(RepairmanManager), stateType = EStateType.SubStateMachine)]
        [StateComponentMenu(RepairmanHelperExtension.DataModelStateLibName + "/" + Title, typeof(RepairmanManager))]
        [Tip("模块组件包含多个零件组件的容器。用子状态机实现。是一个数据组织对象、其中数据提供给其他状态组件使用。模块是介于设备与零件之间的中间层概念，模块可以嵌套模块。")]
        public static State CreateModule(IGetStateCollection obj)
        {
            return obj?.CreateSubStateMachine(CommonFun.Name(typeof(Module)), null, typeof(Module));
        }

        /// <summary>
        /// 子零件
        /// </summary>
        public Part[] childrenParts => GetChildrenItems().Where(i => i is Part).Cast(i => (Part)i).ToArray();

        /// <summary>
        /// 子模块
        /// </summary>
        public Module[] childrenModules => GetChildrenItems().Where(i => i is Module).Cast(i => (Module)i).ToArray();

        /// <summary>
        /// 节点类型
        /// </summary>
        public override ETreeNodeType nodeType => ETreeNodeType.Sub;

        /// <summary>
        /// 初始化时的子项链表
        /// </summary>
        public List<Item> itemListOnInit { get; private set; } = new List<Item>();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool Init(StateData data)
        {
            itemListOnInit.Clear();
            itemListOnInit.AddRange(GetChildrenItems());
            return base.Init(data);
        }

        /// <summary>
        /// 设置零件状态
        /// </summary>
        /// <param name="state"></param>
        public void SetPartState(EPartState state)
        {
            Item[] items = GetChildrenItems();

            foreach(var i in items)
            {
                if (i is Part p && p)
                {
                    p.state = state;
                }
            }
        }

        /// <summary>
        /// 状态
        /// </summary>
        public EPartState childrenState 
        { 
            get
            {
                foreach (var p in childrenParts)
                {
                    if (p.state == EPartState.Disassembly)
                    {
                        return EPartState.Disassembly;
                    }
                }
                foreach (var m in childrenModules)
                {
                    if (m.childrenState == EPartState.Disassembly)
                    {
                        return EPartState.Disassembly;
                    }
                }
                return EPartState.Assembly;
            }
            set
            {
                foreach (var p in childrenParts)
                {
                    p.state = value;
                }
                foreach (var m in childrenModules)
                {
                    m.childrenState = value;
                }
            }
        }
    }
}
