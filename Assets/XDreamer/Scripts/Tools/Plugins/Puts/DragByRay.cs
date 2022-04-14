using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.SelectionUtils;

namespace XCSJ.PluginTools.Puts
{
    /// <summary>
    /// 选择集拖拽通过射线
    /// </summary>
    [Name("选择集拖拽摆放通过射线")]
    [Tool("选择集", disallowMultiple = true, rootType = typeof(ToolsManager))]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Select)]
    [RequireManager(typeof(ToolsManager))]
    public class DragByRay : BaseDragger
    {
        /// <summary>
        /// 射线拾取层：选择对象属于当前设定的层才能被射线检测拾取
        /// </summary>
        [Name("射线拾取层")]
        [Tip("选择对象属于当前设定的层才能被射线检测拾取;默认为缺省层")]
        public LayerMask _rayLayer = 1;

        /// <summary>
        /// 射线拾取最大距离
        /// </summary>
        [Name("射线拾取最大距离")]
        [Min(0.01f)]
        public float _rayPickMaxDistance = 1000;

        /// <summary>
        /// 放置规则
        /// </summary>
        [Name("放置规则")]
        [EnumPopup]
        public EPutRule _putRule = EPutRule.BoundsTangent;

        /// <summary>
        /// 使用射线拾取对象面中心
        /// </summary>
        [Name("使用射线拾取对象面中心")]
        [Tip("启用时，碰撞点会被换算为碰撞面的中心；否则为碰撞点")]
        public bool _useHitObjectFaceCenter = false;

        /// <summary>
        /// 当射线命中无效时处理规则
        /// </summary>
        public enum EHandleRuleOnRayHitInvalid
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 智能
            /// </summary>
            [Name("智能")]
            [Tip("上次位置有效时使用上次位置；否则使用固定距离")]
            Smart,

            /// <summary>
            /// 固定距离
            /// </summary>
            [Name("固定距离")]
            FixedDistance,

            /// <summary>
            /// 上次距离
            /// </summary>
            [Name("上次距离")]
            LastDistance,
        }

        /// <summary>
        /// 射线命中无效处理规则
        /// </summary>
        [Name("射线命中无效处理规则")]
        [EnumPopup]
        public EHandleRuleOnRayHitInvalid _handleRuleOnRayHitInvalid = EHandleRuleOnRayHitInvalid.Smart;

        /// <summary>
        /// 射线未命中时拾取距离
        /// </summary>
        [Name("射线未命中时拾取距离")]
        [Tip("当射线未命中时，本值作为使用鼠标位置的Z分量值，计算得出一个基于相机的世界坐标；")]
        [HideInSuperInspector(nameof(_handleRuleOnRayHitInvalid), EValidityCheckType.Equal, EHandleRuleOnRayHitInvalid.LastDistance)]
        [Min(0)]
        public float _distanceOnRayHitInvalid = 100;

        /// <summary>
        /// 上次距离
        /// </summary>
        //[Name("上次距离")]
        //[Readonly]
        public float _lastDistance { get; private set; } = -1;

        /// <summary>
        /// 摆放包围盒
        /// </summary>
        protected Bounds dragObjectBounds = new Bounds();

        /// <summary>
        /// 拖拽输入
        /// </summary>
        public IGrabAction dragTrigger
        {
            get
            {
                if (_dragTrigger == null)
                {
                    _dragTrigger = new SelectionDragByMouse();
                }
                return _dragTrigger;
            }
            set
            {
                _dragTrigger = value;
            }
        }
        private IGrabAction _dragTrigger;

        /// <summary>
        /// 是否开始拖拽
        /// </summary>
        /// <returns></returns>
        public override bool Grab() => dragTrigger.Grab();

        /// <summary>
        /// 是否拖拽中
        /// </summary>
        /// <returns></returns>
        public override bool Hold() => dragTrigger.Hold();

        /// <summary>
        /// 是否结束拖拽
        /// </summary>
        /// <returns></returns>
        public override bool Release() => dragTrigger.Release();

        private Vector3 lastRayPosition = Vector3.zero;

        /// <summary>
        /// 开始拖拽
        /// </summary>
        /// <param name="gameObjects"></param>
        public override void OnGrab(params GameObject[] gameObjects)
        {
            if (gameObjects == null || gameObjects.Length == 0 || !gameObjects[0]) return;

            var dragGameObject = gameObjects[0];
            if (dragGameObject)
            {
                CommonFun.GetBounds(out dragObjectBounds, dragGameObject, true, false, false);
                var dragTransform = dragGameObject.transform;
                lastRayPosition = GetPositionByRay(dragTransform);

                // 监测拖拽对象的Y值与射线求解的Y值不相等时，将强行把游戏对象设置到射线定位高度上
                var position = dragTransform.position;
                if (!Mathf.Approximately(lastRayPosition.y, position.y))
                {
                    position.y = lastRayPosition.y;
                    dragTransform.position = position;
                }
            }
        }

        /// <summary>
        /// 拖拽中
        /// </summary>
        /// <param name="gameObjects"></param>
        public override void OnHold(params GameObject[] gameObjects)
        {
            if (gameObjects == null || gameObjects.Length == 0 || !gameObjects[0]) return;

            var dragGameObject = gameObjects[0];
            if (dragGameObject)
            {
                var rayPosition = GetPositionByRay(dragGameObject.transform);
                var offset = rayPosition - lastRayPosition;
                dragGameObject.transform.position += offset;
                lastRayPosition = rayPosition;
            }
        }

        /// <summary>
        /// 通过射线获取位置
        /// </summary>
        /// <param name="dragTransform"></param>
        /// <returns></returns>
        public Vector3 GetPositionByRay(Transform dragTransform)
        {
            var mousePos = Input.mousePosition;

            var hits = Physics.RaycastAll(cam.ScreenPointToRay(mousePos), _rayPickMaxDistance, _rayLayer);
            if (hits != null && hits.Length > 0)
            {
                var hitList = hits.ToList().OrderBy(h => h.distance);// 升序排列

                foreach (var hitInfo in hitList)
                {
                    var pickPoint = hitInfo.point;

                    // 排除拾取对象为拖拽对象或其子对象
                    if (hitInfo.transform != dragTransform && !hitInfo.transform.IsChildOf(dragTransform))
                    {
                        _lastDistance = hitInfo.distance;//记录上次距离

                        // 射线点转换为拾取碰撞对象面中心
                        if (_useHitObjectFaceCenter)
                        {
                            pickPoint = PutHelper.GetRayHitObjectFaceCenter(hitInfo, dragObjectBounds);
                        }
                        PutHelper.TryGetPutPosition(dragTransform, hitInfo.transform, dragObjectBounds, _putRule, pickPoint, hitInfo.normal, out var position);

                        return position;
                    }
                }
            }

            switch (_handleRuleOnRayHitInvalid)
            {
                case EHandleRuleOnRayHitInvalid.Smart:
                    {
                        mousePos.z = _lastDistance > 0 ? _lastDistance : _distanceOnRayHitInvalid;
                        return cam.ScreenToWorldPoint(mousePos);
                    }
                case EHandleRuleOnRayHitInvalid.FixedDistance:
                    {
                        mousePos.z = _distanceOnRayHitInvalid;
                        return cam.ScreenToWorldPoint(mousePos);
                    }
                case EHandleRuleOnRayHitInvalid.LastDistance:
                    {
                        mousePos.z = _lastDistance;
                        return cam.ScreenToWorldPoint(mousePos);
                    }
                default:
                    {
                        return dragTransform.position;
                    }
            }
        }
    }
}
