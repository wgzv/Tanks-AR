using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Views.ScrollViews;
using XCSJ.PluginsCameras;

namespace XCSJ.PluginTools.GameObjects
{
    /// <summary>
    /// 游戏对象视图数据
    /// </summary>
    public interface IGameObjectViewItemData : IViewItemData
    {
        /// <summary>
        /// 原型游戏对象
        /// </summary>
        GameObject prototype { get; }

        /// <summary>
        /// 数量
        /// </summary>
        int count { get; set; }

        /// <summary>
        /// 渲染游戏对象视图
        /// </summary>
        void RenderGameObjectView(Camera camera, Vector2Int size);
    }

    /// <summary>
    /// 游戏对象视图项
    /// </summary>
    [Serializable]
    [Name("游戏对象视图项")]
    public class GameObjectViewItemData : ViewItemData, IGameObjectViewItemData
    {
        /// <summary>
        /// 游戏对象
        /// </summary>
        [Name("游戏对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public GameObject _gameObject;

        /// <summary>
        /// 游戏对象数量
        /// </summary>
        [Name("游戏对象数量")]
        [Min(1)]
        [ValidityCheck(EValidityCheckType.NotZero)]
        public int _count = 1;

        /// <summary>
        /// 游戏对象数量
        /// </summary>
        public int count
        {
            get => _count;
            set
            {
                _count = value;
                SendDataChanged();
            }
        }

        /// <summary>
        /// 标题
        /// </summary>
        public override string title
        {
            get
            {
                return useGameObject ? prototype.name : base.title;
            }
            set
            {
                if (useGameObject)
                {
                    prototype.name = value;
                }
                else
                {
                    _title = value;
                }
            }
        }

        private bool useGameObject => prototype && string.IsNullOrEmpty(_title);

        /// <summary>
        /// 选中
        /// </summary>
        public override bool selected 
        { 
            get
            {
                if (prototype) selected = Selection.Contains(prototype);
                return base.selected;
            }
            set
            {
                if (prototype && base.selected != value)
                {
                    base.selected = value;

                    if (value)
                    {
                        Selection.selection = prototype;
                    }
                    else
                    {
                        Selection.Remove(prototype);
                    }
                }
            }
        }

        /// <summary>
        /// 原型
        /// </summary>
        public virtual GameObject prototype
        {
            get => _gameObject;
            set => _gameObject = value;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="gameObjectViewItemList"></param>
        /// <param name="camera"></param>
        /// <param name="size"></param>
        public void RenderGameObjectView(Camera camera, Vector2Int size)
        {
            if (_gameObject && !sprite)
            {
                sprite = XGUIHelper.Texture2DToSprite(GameOjectViewInfoMB.GetTexture(camera, size, _gameObject));
            }
        }

        /// <summary>
        /// 点击
        /// </summary>
        public override void OnClick()
        {
            base.OnClick();

            selected = true;
        }
    }
}
