using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Views.ScrollViews;

namespace XCSJ.PluginTools.GameObjects
{
    /// <summary>
    /// 游戏对象视图项数据组件
    /// </summary>
    [Name("游戏对象视图项数据组件")]
    [Tool("游戏对象", rootType = typeof(ToolsManager))]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Model)]
    [RequireManager(typeof(ToolsManager))]
    public class GameObjectViewItemDataMB : ViewItemDataMB<GameObjectViewItemData>
    {
        /// <summary>
        /// 游戏对象视图项数据
        /// </summary>
        [Name("游戏对象视图项数据")]
        [OnlyMemberElements]
        public GameObjectViewItemData _gameObjectViewItemData = new GameObjectViewItemData();

        /// <summary>
        /// 数据
        /// </summary>
        public override GameObjectViewItemData data => _gameObjectViewItemData;

        /// <summary>
        /// 重置
        /// </summary>
        protected void Reset()
        {
            InitGameObject();
        }

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            InitGameObject();

            base.OnEnable();
        }

        private void InitGameObject()
        {
            if (!_gameObjectViewItemData.prototype)
            {
                _gameObjectViewItemData.prototype = gameObject;
            }
        }
    }
}
