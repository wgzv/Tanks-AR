using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;

namespace XCSJ.PluginXGUI.Views.ScrollViews
{
    /// <summary>
    /// 精灵接口
    /// </summary>
    public interface ISprite
    {
        /// <summary>
        /// 精灵
        /// </summary>
        Sprite sprite { get; }
    }

    /// <summary>
    /// 视图项数据
    /// </summary>
    public interface IViewItemData : ISprite
    {
        /// <summary>
        /// 标题
        /// </summary>
        string title { get; }

        /// <summary>
        /// 描述
        /// </summary>
        string description { get; }

        /// <summary>
        /// 选中
        /// </summary>
        bool selected { get; set; }

        /// <summary>
        /// 启用
        /// </summary>
        bool enable { get; set; }

        /// <summary>
        /// 发送事件
        /// </summary>
        event Action<IViewItemData> onDatachanged;

        /// <summary>
        /// 点击
        /// </summary>
        void OnClick();
    }

    /// <summary>
    /// 视图项数据
    /// </summary>
    public abstract class ViewItemData : IViewItemData
    {
        /// <summary>
        /// 标题
        /// </summary>
        [Name("标题")]
        public string _title;

        /// <summary>
        /// 标题
        /// </summary>
        public virtual string title
        {
            get => _title; 
            set
            {
                _title = value;
                SendDataChanged();
            }
        }

        /// <summary>
        /// 描述
        /// </summary>
        public virtual string description { get; set; }

        /// <summary>
        /// 选择
        /// </summary>
        public virtual bool selected
        {
            get => _selected;
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    SendDataChanged();
                }
            }
        }
        private bool _selected;

        /// <summary>
        /// 有效性
        /// </summary>
        public virtual bool enable 
        { 
            get => _enable; 
            set
            {
                _enable = value;
                SendDataChanged();
            }
        }
        private bool _enable = true;

        private Sprite _sprite;

        /// <summary>
        /// 封面图片
        /// </summary>
        public Sprite sprite
        {
            get => _sprite;
            set
            {
                _sprite = value;
                SendDataChanged();
            }
        }

        /// <summary>
        /// 数据变化事件
        /// </summary>
        public event Action<IViewItemData> onDatachanged;

        /// <summary>
        /// 数据变化事件
        /// </summary>
        protected virtual void SendDataChanged()
        {
            onDatachanged?.Invoke(this);
        }

        /// <summary>
        /// 视图项数据被点击
        /// </summary>
        public static event Action<IViewItemData> onViewItemDataClick = null;

        /// <summary>
        /// 点击
        /// </summary>
        public virtual void OnClick()
        {
            onViewItemDataClick?.Invoke(this);
        }
    }
}
