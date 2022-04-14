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
    [Name(Title, nameof(PartSocket))]
    [XCSJ.Attributes.Icon(EIcon.Part)]
    [DisallowMultipleComponent]
    [RequireManager(typeof(RepairmanManager))]
    [Tip("用于放置零件的位置对象")]
    [RequireComponent(typeof(Part))]
    public class PartSocket : StateComponent<PartSocket>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public const string Title = "零件插槽";

        [Name(Title, nameof(PartSocket))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [StateLib(RepairmanHelperExtension.DataModelStateLibName, typeof(RepairmanManager))]
        [StateComponentMenu(RepairmanHelperExtension.DataModelStateLibName + "/" + Title, typeof(RepairmanManager))]
        [Tip("用于放置零件的位置对象")]
        public static State CreatePart(IGetStateCollection obj)
        {
            return obj?.CreateNormalState(CommonFun.Name(typeof(PartSocket)), null, typeof(PartSocket));
        }

        /// <summary>
        /// 插槽对象
        /// </summary>
        [Name("插槽对象")]
        public Transform socket;

        private Part part = null;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool Init(StateData data)
        {
            part = GetComponent<Part>();
            return base.Init(data);
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => base.DataValidity() && socket;

        /// <summary>
        /// 完成
        /// </summary>
        /// <returns></returns>
        public override bool Finished() => true;
    }
}
