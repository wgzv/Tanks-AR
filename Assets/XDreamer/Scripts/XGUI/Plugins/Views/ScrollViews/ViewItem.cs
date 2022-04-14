using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Views.ScrollViews
{
    /// <summary>
    /// 视图项接口
    /// </summary>
    public interface IViewItem
    {
        /// <summary>
        /// 数据
        /// </summary>
        IViewItemData data { get; }

        /// <summary>
        /// 有效性
        /// </summary>
        bool valid { get; }
    }

    /// <summary>
    /// 视图项
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [Name("视图项")]
    public class ViewItem : View, IViewItem, IPointerDownHandler, IDragHandler, IPointerUpHandler, IScrollHandler
    {
        /// <summary>
        /// 视图项数据
        /// </summary>
        public virtual IViewItemData data 
        { 
            get => _data; 
            set
            {
                if (value!=null)
                {
                    valid = true;
                }
                RemoveDataChangedListener();
                _data = value;
                UpdateData();
                AddDataChangedListener();
            }
        }
        protected IViewItemData _data;

        /// <summary>
        /// 标题
        /// </summary>
        [Name("标题")]
        [Readonly(EEditorMode.Runtime)]
        public Text _titleText = null;

        /// <summary>
        /// 描述
        /// </summary>
        [Name("描述")]
        [Readonly(EEditorMode.Runtime)]
        public Text _descriptionText = null;

        /// <summary>
        /// 项视图图像
        /// </summary>
        [Name("项视图图像")]
        [Readonly(EEditorMode.Runtime)]
        public Button _button = null;

        /// <summary>
        /// 背景图像
        /// </summary>
        [Name("背景边框")]
        [Readonly(EEditorMode.Runtime)]
        public Image _border = null;

        /// <summary>
        /// 可用性效果
        /// </summary>
        [Name("可用性效果")]
        [EnumPopup]
        public EEnableEffect _enableEffect = EEnableEffect.Transparent;

        /// <summary>
        /// 透明度
        /// </summary>
        [Name("透明度")]
        public float _alpha = 0.5f;

        /// <summary>
        /// 可用性效果
        /// </summary>
        public enum EEnableEffect
        {
            [Name("透明度")]
            Transparent,
        }

        /// <summary>
        /// 项选中色
        /// </summary>
        public Color selectedColor { get; set; }

        protected List<Graphic> graphics = new List<Graphic>();
        private GraphicRecorder _graphicRecorder = new GraphicRecorder();

        /// <summary>
        /// 点击回调
        /// </summary>
        public static event Action<IViewItemData> onClick = null;

        private GraphicRecorder _borderGraphicRecorder = new GraphicRecorder();

        private Image buttonImage
        {
            get
            {
                if (!_buttonImage && _button)
                {
                    _buttonImage = _button.GetComponent<Image>();
                }
                return _buttonImage;
            }
        }

        public virtual bool valid { get; private set; } = true;

        private Image _buttonImage = null;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            graphics.AddRange(GetComponentsInChildren<Graphic>());
            graphics.Remove(GetComponent<Graphic>());
            _graphicRecorder.Record(graphics);

            if (_button)
            {
                _buttonImage = _button.GetComponent<Image>();
                _button.onClick.AddListener(OnClick);
            }

            _borderGraphicRecorder.Record(_border);

            AddDataChangedListener();

            if (!_parentScrollRect) _parentScrollRect = GetComponentInParent<ScrollRect>();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            if (_button)
            {
                _button.onClick.RemoveListener(OnClick);
            }
            _borderGraphicRecorder.Recover();

            RemoveDataChangedListener();
        }

        private void AddDataChangedListener()
        {
            if (data != null)
            {
                data.onDatachanged -= OnDataChanged;
                data.onDatachanged += OnDataChanged;
            }
        }

        private void RemoveDataChangedListener()
        {
            if (data != null)
            {
                data.onDatachanged -= OnDataChanged;
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected void Start()
        {
            UpdateData(); // 主动刷新数据
        }

        /// <summary>
        /// 固定更新
        /// </summary>
        protected void FixedUpdate()
        {
            if (data != null && data.selected) { }
        }

        /// <summary>
        /// 点击
        /// </summary>
        public void OnClick()
        {
            data?.OnClick();

            onClick?.Invoke(this.data);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        public virtual void UpdateData()
        {
            if (data == null) return;

            // 设置透明度
            if (data.enable)
            {
                _graphicRecorder.Recover();
            }
            else
            {
                foreach (var g in graphics)
                {
                    var c = g.color;
                    c.a = _alpha;
                    g.color = c;
                }
            }

            // 选择
            if (data.selected)
            {
                _borderGraphicRecorder._records.ForEach(r => r.SetColor(selectedColor));
            }
            else
            {
                _borderGraphicRecorder.Recover();
            }
            
            // 标题
            if (_titleText)
            {
                _titleText.text = data.title;
            }

            // 描述
            if (_descriptionText)
            {
                _descriptionText.text = data.description;
            }

            // 按钮贴图
            if (buttonImage)
            {
                buttonImage.sprite = data.sprite;
            }

            // 按钮交互性
            if (_button)
            {
                _button.interactable = data.enable;
            }
        }

        private void OnDataChanged(IViewItemData data)
        {
            if (this.data == data)
            {
                UpdateData();
            }
        }

        /// <summary>
        /// 父对象滚动矩形
        /// </summary>
        private ScrollRect _parentScrollRect = null;

        /// <summary>
        /// 指针按下
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (_parentScrollRect)
            {
                _parentScrollRect.OnBeginDrag(eventData);
            }
        }

        /// <summary>
        /// 拖动
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (_parentScrollRect && _parentScrollRect.enabled)
            {
                _parentScrollRect.OnDrag(eventData);

                // 求与垂直的夹角，判断拖动方向，防止水平与垂直方向同时响应导致的拖动时整个界面都会动
                float angle = Vector2.Angle(eventData.delta, Vector2.up);

                // 视图项正在水平运动时禁用垂直运行
                if (angle > 45f && angle < 135f) _parentScrollRect.enabled = false;
            }
        }

        /// <summary>
        /// 指针弹起
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (_parentScrollRect)
            {
                _parentScrollRect.enabled = true;
                _parentScrollRect.OnEndDrag(eventData);
            }
        }

        /// <summary>
        /// 响应滚轮滚动
        /// </summary>
        /// <param name="eventData"></param>
        public void OnScroll(PointerEventData eventData)
        {
            if (_parentScrollRect)
            {
                _parentScrollRect.OnScroll(eventData);
            }
        }
    }

}
