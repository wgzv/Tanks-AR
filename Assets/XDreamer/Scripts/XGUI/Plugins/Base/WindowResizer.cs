using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using UnityEngine;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginXGUI.Base
{
    /// <summary>
    /// 窗口尺寸控制器
    /// </summary>
    [Name("窗口尺寸控制器")]
    public class WindowResizer : View, IDraggableView
    {
        /// <summary>
        /// 控制窗口
        /// </summary>
        [Name("控制窗口")]
        [Readonly(EEditorMode.Runtime)]
        public SubWindow _subWindow;

        private Rect screenRect = new Rect(0, 0, 0, 0);

        /// <summary>
        /// 最小尺寸
        /// </summary>
        [Name("最小尺寸")]
        public Vector2 _minSize = new Vector2(100, 100);

        /// <summary>
        /// 控制窗口
        /// </summary>
        [Name("最小尺寸")]
        public Vector2 _maxSize = new Vector2(10000, 10000);

        /// <summary>
        /// 能否拖拽
        /// </summary>
        public bool canDrag { get => true; set { } }

        /// <summary>
        /// 开始拖拽
        /// </summary>
        /// <param name="eventData"></param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            screenRect.width = Screen.width;
            screenRect.height = Screen.height;
        }

        /// <summary>
        /// 拖拽中
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            if (!_subWindow) return;

            _subWindow.rectTransform.position += (Vector3)eventData.delta * .5f;

            var flip = new Vector2(eventData.delta.x, -eventData.delta.y);

            _subWindow.rectTransform.sizeDelta += flip;

            var r = _subWindow.rectTransform.GetScreenRect();

            float w = _subWindow.rectTransform.rect.width, h = _subWindow.rectTransform.rect.height;

            if (w < _minSize.x || w > _maxSize.x || (r.x + r.width > screenRect.width))
            {
                var size = _subWindow.rectTransform.sizeDelta;
                size.x -= flip.x;
                _subWindow.rectTransform.sizeDelta = size;

                var pos = _subWindow.rectTransform.position;
                pos.x -= eventData.delta.x * .5f;
                _subWindow.rectTransform.position = pos;

                SubWindow.onSizeChanged?.Invoke(_subWindow, new Rect(pos, size));
            }

            if (h < _minSize.y || h > _maxSize.y || (r.y - r.height < 0))
            {
                var size = _subWindow.rectTransform.sizeDelta;
                size.y -= flip.y;
                _subWindow.rectTransform.sizeDelta = size;

                var pos = _subWindow.rectTransform.position;
                pos.y -= eventData.delta.y * .5f;
                _subWindow.rectTransform.position = pos;

                SubWindow.onSizeChanged?.Invoke(_subWindow, new Rect(pos, size));
            }
        }

        /// <summary>
        /// 结束拖拽
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            
        }
    }

    /// <summary>
    /// 窗口变换回调
    /// </summary>
    public interface IOnWindowResizeHandler
    {
        void OnWindowResize();
    }
}
