using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;

namespace XCSJ.PluginTools.Interactions.Interactables.Items
{
    /// <summary>
    /// 实体物品模版集合
    /// </summary>
    [Name("实体物品模版集合")]
    public class EntityItemTemplateCollection : ToolMB
    {
        /// <summary>
        /// 模版实体物品字典
        /// </summary>
        public Dictionary<string, EntityItem> _entityItems = new Dictionary<string, EntityItem>();

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            _entityItems.Clear();
            foreach (var item in GetComponentsInChildren<EntityItem>(true))
            {
                try
                {
                    _entityItems.Add(item._guid, item);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }
    }
}
