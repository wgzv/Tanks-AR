using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.Notes
{
    [Tool("标注")]
    [Tip("弹出式的提示界面")]
    [Name("提示(碰撞体)")]
    [XCSJ.Attributes.Icon(EIcon.Tip)]
    [RequireComponent(typeof(Collider))]
    [RequireManager(typeof(ToolsManager))]
    public class ColliderTip : Tip
    {
        /// <summary>
        /// 鼠标移入
        /// </summary>
        protected void OnMouseEnter() => OnEnter();

        /// <summary>
        /// 鼠标移出
        /// </summary>
        protected void OnMouseExit() => OnExit();

        /// <summary>
        /// 点击
        /// </summary>
        protected void OnMouseUpAsButton()
        {
            if (!CommonFun.IsOnUGUI())
            {
                OnClick();
            }
        }

        protected override Vector3 GetTipPosition()
        {
            var cam = Camera.main ? Camera.main : Camera.current;
            if (!cam) return Vector2.zero;

            var canvas = ugui.GetComponentInParent<Canvas>();
            if (!canvas) return Vector2.zero;

            // 计算提示出现的位置
            Vector3 viewPoint = cam.WorldToViewportPoint(transform.position);
            Vector3 screenPoint = cam.ViewportToScreenPoint(new Vector3(viewPoint.x, viewPoint.y, 0));
            Vector3 pointer = (screenPoint == Vector3.zero) ? Input.mousePosition : screenPoint;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, pointer, canvas.worldCamera, out Vector2 localPointInRectangle))
            {
                return localPointInRectangle;
            }

            return Vector2.zero;
        }

        protected override void SetTipPosition(Vector3 position) => ugui.anchoredPosition = position;
    }
}
