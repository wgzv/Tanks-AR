using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginRepairman.Machines;
using XCSJ.PluginSMS;
using XCSJ.PluginTools.SelectionUtils;

namespace XCSJ.PluginRepairman
{
    /// <summary>
    /// 拆装拾取组件
    /// </summary>
    [Name("零件拾取器")]
    [Tip("用于识别零件或物品的选择工具")]
    [XCSJ.Attributes.Icon(EIcon.Click)]
    [DisallowMultipleComponent]
    [RequireManager(typeof(RepairmanManager))]
    [Tool(category = RepairmanHelper.PluginName, purposes = new string[] { nameof(RepairmanManager) }, groupRule = EToolGroupRule.None)]
    public class RepairmanInputSystem : SelectionModify
    {
        private List<Item> items = new List<Item>();

        private void Awake()
        {
            items = SMSHelper.GetStateComponents<Item>();
        }

        /// <summary>
        /// 尝试获取选择对象
        /// </summary>
        /// <param name="selection"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected override bool TryGetSelectionGameObject(GameObject selection, out GameObject result)
        {
            // 优先使用零件
            var item = items.Find(i => i.IsSelected(selection));
            if (item)
            {
                result = item.gameObject;
                return true;
            }
            return base.TryGetSelectionGameObject(selection, out result);
        }
    }
}
