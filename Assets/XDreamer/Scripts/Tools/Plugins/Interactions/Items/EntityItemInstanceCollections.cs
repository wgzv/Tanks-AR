﻿using System.Collections.Generic;
using XCSJ.Attributes;

namespace XCSJ.PluginTools.Interactions.Interactables.Items
{
    /// <summary>
    /// 实实体物品例集合列表
    /// </summary>
    [Name("实实体物品例集合列表")]
    public class EntityItemInstanceCollections : ToolMB
    {
        /// <summary>
        /// 实体物品实例集合列表
        /// </summary>
        public List<EntityItemInstanceCollection> singleInstanceEntityItemCollections { get; } = new List<EntityItemInstanceCollection>();

        /// <summary>
        /// 实例实体物品列表
        /// </summary>
        public List<EntityItem> instanceEntityItems
        {
            get
            {
                List<EntityItem> entityItems = new List<EntityItem>();
                singleInstanceEntityItemCollections.ForEach(c => entityItems.AddRange(c.instanceEntityItems));
                return entityItems;
            }
        }
    }
}
