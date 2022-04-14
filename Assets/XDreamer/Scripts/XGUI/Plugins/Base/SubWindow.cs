using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.Extension.Base.Recorders;

namespace XCSJ.PluginXGUI.Base
{
    /// <summary>
    /// 窗口接口
    /// </summary>
    public interface IWindow
    {
        /// <summary>
        /// 内容全屏
        /// </summary>
        bool fullScreen { get; set; }

        /// <summary>
        /// 尺寸
        /// </summary>
        Vector2 size { get; set; }

        /// <summary>
        /// 展开
        /// </summary>
        bool expand { get; set; }

        /// <summary>
        /// 窗口最大化
        /// </summary>
        bool maxSize { get; set; }

        /// <summary>
        /// 宽
        /// </summary>
        float width { get; set; }

        /// <summary>
        /// 高
        /// </summary>
        float height { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        RectTransform title { get; }

        /// <summary>
        /// 内容
        /// </summary>
        RectTransform content { get; }

        /// <summary>
        /// 显示
        /// </summary>
        bool display { get; set; }

        /// <summary>
        /// 打开
        /// </summary>
        void Open();

        /// <summary>
        /// 关闭
        /// </summary>
        void Close();

        /// <summary>
        /// 缩放
        /// </summary>
        /// <param name="scaleValue"></param>
        void Scale(float scaleValue);

        /// <summary>
        /// 缩放：三轴缩放
        /// </summary>
        /// <param name="scale"></param>
        void Scale(Vector3 scale);
    }

    /// <summary>
    /// 子窗口：可嵌套
    /// </summary>
    [Name("子窗口")]
    [Tip("窗口基类；子窗口之间可嵌套")]
    [DisallowMultipleComponent]
    public class SubWindow : DraggableView, IWindow
    {
        [Name("拖拽锁定")]
        [Tip("勾选，窗口不可拖拽;不勾选，窗口可拖拽")]
        public bool _lockDrag = true;

        /// <summary>
        /// 锁定拖拽
        /// </summary>
        public virtual bool lockDrag { get => _lockDrag; set => _lockDrag = value; }

        /// <summary>
        /// 切换锁定
        /// </summary>
        public void SwitchLockDrag() => _lockDrag = !_lockDrag;

        /// <summary>
        /// 可拖拽
        /// </summary>
        public override bool canDrag
        {
            get => !_lockDrag;
            set
            {
                if (canDrag != value)
                {
                    _lockDrag = !value;
                    SendSubWindowEvent();
                }
            }
        }

        /// <summary>
        /// 展开
        /// </summary>
        public bool expand
        {
            get => _expand;
            set
            {
                var old = _expand;
                _expand = value;

                if (_content)
                {
                    if (_content.gameObject.activeSelf)
                    {
                        _expandSize = size;
                    }
                    _content.gameObject.SetActive(_expand);
                }

                if (_expand != old)
                {
                    if (!_expand)
                    {
                        _expandSize = size;
                        switch (_titleDirection)
                        {
                            case EFourDirection.Top:
                            case EFourDirection.Bottom:
                                {
                                    size = new Vector2(size.x, title.rect.height);
                                    break;
                                }
                            case EFourDirection.Left:
                            case EFourDirection.Right:
                                {
                                    size = new Vector2(title.rect.width, size.y);
                                    break;
                                }
                        }
                    }
                    else
                    {
                        size = _expandSize;
                    }
                }

                SendSubWindowEvent();
            }
        }

        private Vector2 _expandSize = new Vector2();

        /// <summary>
        /// 展开
        /// </summary>
        [Name("展开")]
        public bool _expand = true;

        /// <summary>
        /// 全屏
        /// </summary>
        public bool fullScreen
        {
            get => _fullScreen;
            set
            {
                if (content)
                {
                    maxSize = value;
                    _fullScreen = value;
                    if (_fullScreen)
                    {
                        contentRecorder.Record(content);
                        content.SetFullScreen();
                    }
                    else
                    {
                        contentRecorder.Recover();
                    }
                    SendSubWindowEvent();
                }
            }
        }
        /// <summary>
        /// 全屏
        /// </summary>
        [Name("全屏")]
        [HideInSuperInspector(nameof(_expand), EValidityCheckType.Equal, false)]
        public bool _fullScreen = false;

        private RectTransformRecorder contentRecorder = new RectTransformRecorder();

        public bool maxSize
        {
            get => _maxSize;
            set
            {
                if (_maxSize != value)
                {
                    _maxSize = value;

                    if (_maxSize)
                    {
                        windowMaxSizeRecorder.Record(rectTransform);
                        rectTransform.SetFullScreen();
                    }
                    else
                    {
                        windowMaxSizeRecorder.Recover();
                    }
                    LayoutTitleBar(_titleDirection == EFourDirection.Top || _titleDirection == EFourDirection.Bottom);
                    SendSubWindowEvent();
                }
            }
        }
        private bool _maxSize = false;
        private RectTransformRecorder windowMaxSizeRecorder = new RectTransformRecorder();

        /// <summary>
        /// 宽
        /// </summary>
        public virtual float width { get => size.x; set => size = new Vector2(value, size.y); }

        /// <summary>
        /// 高
        /// </summary>
        public virtual float height { get => size.y; set => size = new Vector2(size.x, value); }

        public virtual Vector2 positon
        {
            get => rectTransform.anchoredPosition;
            set => rectTransform.anchoredPosition = value;
        }

        /// <summary>
        /// 尺寸
        /// </summary>
        public virtual Vector2 size
        {
            get => rectTransform.sizeDelta;
            set => rectTransform.sizeDelta = value;
        }

        /// <summary>
        /// 标题与内容间隙
        /// </summary>
        [Name("标题与内容间隙")]
        [Min(0)]
        public float _titleAndContentSpace = 2;

        #region 标题设置

        /// <summary>
        /// 标题
        /// </summary>
        [Name("标题")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Group("标题栏设置")]
        public RectTransform _title = null;

        /// <summary>
        /// 标题
        /// </summary>
        public RectTransform title { get => _title; set => _title = value; }

        /// <summary>
        /// 标题方位
        /// </summary>
        [Name("标题方位")]
        [EnumPopup]
        public EFourDirection _titleDirection = EFourDirection.Top;

        #region 标题水平布局

        /// <summary>
        /// 宽度规则
        /// </summary>
        public enum EWidthRule
        {
            /// <summary>
            /// 固定
            /// </summary>
            [Name("固定")]
            Fixed,

            /// <summary>
            /// 窗口宽度
            /// </summary>
            [Name("窗口宽度")]
            WindowWidth,
        }

        /// <summary>
        /// 宽度规则
        /// </summary>
        [Name("宽度规则")]
        [HideInSuperInspector(nameof(_titleDirection), EValidityCheckType.NotEqual | EValidityCheckType.And, EFourDirection.Top,
            nameof(_titleDirection), EValidityCheckType.NotEqual, EFourDirection.Bottom)]
        [EnumPopup]
        public EWidthRule _widthRule = EWidthRule.WindowWidth;

        /// <summary>
        /// 水平标题宽度
        /// </summary>
        [Name("水平标题宽度")]
        [Tip("水平布局时标题的宽度")]
        public float _titleWidthOnHorizontal = 200;

        /// <summary>
        /// 水平标题高度
        /// </summary>
        [Name("水平标题高度")]
        [Tip("水平布局时标题的高度")]
        [HideInSuperInspector(nameof(_titleDirection), EValidityCheckType.NotEqual | EValidityCheckType.And, EFourDirection.Top,
            nameof(_titleDirection), EValidityCheckType.NotEqual, EFourDirection.Bottom)]
        public float _titleHeightOnHorizontal = 40;

        /// <summary>
        /// 水平方位
        /// </summary>
        [Name("水平方位")]
        [EnumPopup]
        public EHorizontalPosition _horizontalPosition = EHorizontalPosition.Left;

        #endregion

        #region 标题垂直布局

        /// <summary>
        /// 高度规则
        /// </summary>
        public enum EHeightRule
        {
            /// <summary>
            /// 固定
            /// </summary>
            [Name("固定")]
            Fixed,

            /// <summary>
            /// 窗口高度
            /// </summary>
            [Name("窗口高度")]
            WindowHeight,
        }

        /// <summary>
        /// 高度规则
        /// </summary>
        [Name("高度规则")]
        [HideInSuperInspector(nameof(_titleDirection), EValidityCheckType.NotEqual | EValidityCheckType.And, EFourDirection.Left,
            nameof(_titleDirection), EValidityCheckType.NotEqual, EFourDirection.Right)]
        [EnumPopup]
        public EHeightRule _heightRule = EHeightRule.WindowHeight;

        /// <summary>
        /// 垂直标题宽度
        /// </summary>
        [Name("垂直标题宽度")]
        [Tip("垂直布局时标题的宽度")]
        [HideInSuperInspector(nameof(_titleDirection), EValidityCheckType.NotEqual | EValidityCheckType.And, EFourDirection.Left,
            nameof(_titleDirection), EValidityCheckType.NotEqual, EFourDirection.Right)]
        public float _titleWidthOnVertical = 40;

        /// <summary>
        /// 垂直标题高度
        /// </summary>
        [Name("垂直标题高度")]
        [Tip("垂直布局时标题的高度")]
        public float _titleHeightOnVertical = 200;

        /// <summary>
        /// 垂直方位
        /// </summary>
        [Name("垂直方位")]
        [EnumPopup]
        public EVerticalPosition _verticalPosition = EVerticalPosition.Top;

        #endregion

        #endregion

        /// <summary>
        /// 内容
        /// </summary>
        [Name("内容")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Group("内容区域设置")]
        public RectTransform _content = null;

        /// <summary>
        /// 内容
        /// </summary>
        public RectTransform content { get => _content; set => _content = value; }

        /// <summary>
        /// 显示
        /// </summary>
        public bool display { get => gameObject.activeSelf; set => gameObject.SetActive(value); }

        /// <summary>
        /// 尺寸变化
        /// </summary>
        public static Action<SubWindow, Rect> onSizeChanged;

        /// <summary>
        /// 打开
        /// </summary>
        public void Open()
        {
            display = true;
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            display = false;
        }

        /// <summary>
        /// 缩放
        /// </summary>
        /// <param name="scaleValue"></param>
        public void Scale(float value)
        {
            Scale(Vector3.one * value);
        }

        /// <summary>
        /// 缩放：三轴缩放
        /// </summary>
        /// <param name="scale"></param>
        public void Scale(Vector3 scale)
        {
            transform.localScale = scale;
        }

        /// <summary>
        /// 子窗口事件回调函数
        /// </summary>
        public static event Action<SubWindow, WindowEventArgs> onSubWindowEventCallback = null;

        /// <summary>
        /// 开始
        /// </summary>
        protected virtual void Start()
        {
            Invoke(nameof(Init), 0.5f);
        }

        private void Init()
        {
            expand = _expand;
            fullScreen = _fullScreen;
        }

        /// <summary>
        /// 发送子窗口事件
        /// </summary>
        protected void SendSubWindowEvent()
        {
            onSubWindowEventCallback?.Invoke(this, new WindowEventArgs(fullScreen, expand, canDrag, maxSize));
        }
        
        /// <summary>
        /// 设置标题栏的布局
        /// </summary>
        /// <param name="isHorizontal"></param>
        public void LayoutTitleBar(bool isHorizontal)
        {
            var titleBar = _title.GetComponent<TitleBar>();
            if (titleBar)
            {
                titleBar.Layout(isHorizontal, isHorizontal ? _titleHeightOnHorizontal : _titleWidthOnVertical);
                titleBar.SetExpandRotation(_titleDirection);
            }
        }
    }

}
