using System;
using System.Collections.Generic;
using System.Linq;
using XCSJ.Attributes;
using UnityEngine;

namespace XCSJ.PluginTools.Interactions.Interactables.Items
{
    /// <summary>
    /// 背包物品
    /// </summary>
    [Name("背包物品")]
    [RequireComponent(typeof(EntityItem))]
    public class BagItem : BaseItem, IBagItem
    {
        /// <summary>
        /// 堆叠数目：在背包中时，允许堆叠的最大数目，即一个背包格内当前物品的最大堆叠最大值
        /// </summary>
        [Name("堆叠数目")]
        [Tip("在背包中时，允许堆叠的最大数目，即一个背包格内当前物品的最大堆叠最大值")]
        public int _pileCount = 1;

        /// <summary>
        /// 获取用法列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetUsages() => new List<string>() { nameof(OutputInfo) };

        /// <summary>
        /// 背包项用法
        /// </summary>
        [BagItemUsage]
        public void OutputInfo()
        {
            Debug.Log("背包物品:"+name+", K");
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
        }
    }

    /// <summary>
    /// 背包项用法特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class BagItemUsageAttribute : Attribute { }

    /// <summary>
    /// 背包项用法数据
    /// </summary>
    public class BagItemUsageData
    {
        public Bag bag => bagItemData._bag;

        public BagItemData bagItemData { get; set; }
    }
}
