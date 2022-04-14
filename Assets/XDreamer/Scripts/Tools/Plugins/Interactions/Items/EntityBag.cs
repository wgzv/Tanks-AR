﻿using System;
using System.Collections.Generic;
using System.Linq;
using XCSJ.Attributes;
using UnityEngine;
using XCSJ.Extension.Base.Interactions;

namespace XCSJ.PluginTools.Interactions.Interactables.Items
{
    /// <summary>
    /// 实体背包
    /// </summary>
    [Name("实体背包")]
    public abstract class EntityBag : EntityBagItem, IBag
    {
        #region IBag接口实现

        public abstract bool CanAdd(IBagItem bagItem, int count);

        public abstract bool TryAdd(IBagItem bagItem, int count);

        public abstract bool CanRemove(IBagItem bagItem, int count);

        public abstract bool TryRemove(IBagItem bagItem, int count);

        /// <summary>
        /// 获取默认分类用途
        /// </summary>
        /// <returns></returns>
        public abstract string GetDefaultCategoryUsage();

        /// <summary>
        /// 获取分类用途
        /// </summary>
        /// <returns></returns>
        public abstract List<string> GetCategoryUsages();

        /// <summary>
        /// 尝试通过用途获取背包物品
        /// </summary>
        /// <param name="usage"></param>
        /// <param name="bagItems"></param>
        /// <returns></returns>
        public abstract bool TryGetBagItems(string usage, out List<IBagItem> bagItems);

        /// <summary>
        /// 尝试处理交互
        /// </summary>
        /// <param name="bagItem"></param>
        /// <param name="itemContext"></param>
        /// <param name="itemInteractor"></param>
        /// <param name="itemInput"></param>
        /// <param name="handleInteractableResult"></param>
        /// <returns></returns>
        public virtual bool TryHandleBagItemInteractable(IBagItem bagItem, IItemContext itemContext, IItemInteractor itemInteractor, IItemInput itemInput, out IHandleInteractableResult handleInteractableResult)
        {
            if (bagItem == null)
            {
                handleInteractableResult = default;
                return false;
            }
            return bagItem.TryHandleItemInteractable(itemContext, itemInteractor, itemInput, out handleInteractableResult);
        }

        /// <summary>
        /// 获取背包物品的交互用途
        /// </summary>
        /// <param name="bagItem"></param>
        /// <param name="itemContext"></param>
        /// <returns></returns>
        public virtual List<string> GetBagItemInteractableUsages(IBagItem bagItem, IItemContext itemContext) => bagItem?.GetItemInteractableUsages(itemContext);

        #endregion
    }
}
