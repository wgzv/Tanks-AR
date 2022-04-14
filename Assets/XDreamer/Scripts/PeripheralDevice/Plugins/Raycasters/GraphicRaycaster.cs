using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XCSJ.Attributes;
using static UnityEngine.UI.GraphicRaycaster;

namespace XCSJ.PluginPeripheralDevice.Raycasters
{
    /// <summary>
    /// 用于检测UGUI的射线检测
    /// </summary>
    public class GraphicRaycaster : BaseRaycaster
    {
        /// <summary>
        /// 检测的Canvas面板
        /// </summary>
        [SerializeField]
        [Name("Canvas面板")]
        private Canvas _canvas;
        public Canvas canvas { get => _canvas ? _canvas : _canvas = GameObject.FindObjectOfType<Canvas>(); set => _canvas = value; }

        /// <summary>
        /// 射线检测结果集
        /// </summary>
        private List<Graphic> _raycastResults = new List<Graphic>();

        /// <summary>
        /// 射线检测
        /// </summary>
        /// <param name="eventData">事件数据</param>
        /// <param name="resultAppendList">结果集</param>
        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            if (canvas == null)
                return;

            // Convert to view space
            Vector2 pos;
            if (eventCamera == null)
                pos = new Vector2(eventData.position.x / Screen.width, eventData.position.y / Screen.height);
            else
                pos = eventCamera.ScreenToViewportPoint(eventData.position);

            // If it's outside the camera's viewport, do nothing
            if (pos.x < 0f || pos.x > 1f || pos.y < 0f || pos.y > 1f)
                return;

            float hitDistance = float.MaxValue;

            var node = LineNodeHelper.GetIntersectWithLineAndPlane(origin.position, origin.forward, canvas.transform.forward, canvas.transform.position);

            var ray = isCamera ? eventCamera.ScreenPointToRay(eventData.position) : new Ray(origin.position, origin.forward);

            Vector2 vector = eventCamera.WorldToScreenPoint(node);

            _raycastResults.Clear();
            Raycast(canvas, eventCamera, out vector, _raycastResults, origin);

            for (var index = 0; index < _raycastResults.Count; index++)
            {
                var go = _raycastResults[index].gameObject;
                bool appendGraphic = true;

                if (appendGraphic)
                {
                    float distance = 0;

                    Plane plane = new Plane(go.transform.forward, go.transform.position);
                    Ray _ray = new Ray(origin.position, origin.forward);
                    //var node = Plane.//LineNodeHelper.GetIntersectWithLineAndPlane(origin.position, origin.forward, graphic.transform.forward, graphic.transform.position);

                    plane.Raycast(_ray, out distance);

                    //if (eventCamera == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                    //    distance = 0;
                    //else
                    //{
                    //    Transform trans = canvas.transform;
                    //    Vector3 transForward = trans.forward;
                    //    // http://geomalgorithms.com/a06-_intersect-2.html
                    //    distance = (Vector3.Dot(transForward, trans.position - ray.origin) / Vector3.Dot(transForward, ray.direction.normalized));

                    //    // Check to see if the go is behind the camera.
                    //    if (distance < 0)
                    //        continue;
                    //}

                    if (distance >= hitDistance)
                        continue;

                    var castResult = new RaycastResult
                    {
                        gameObject = go,
                        module = Raycaster(),
                        distance = distance,
                        screenPosition = vector,
                        index = resultAppendList.Count,
                        depth = _raycastResults[index].depth,
                        sortingLayer = canvas.sortingLayerID,
                        sortingOrder = canvas.sortingOrder
                    };
                    resultAppendList.Add(castResult);
                }
            }
        }

        /// <summary>
        /// 排序后的结果集
        /// </summary>
        static readonly List<Graphic> _sortedGraphics = new List<Graphic>();

        /// <summary>
        /// 对于Canvas面板的射线检测
        /// </summary>
        /// <param name="canvas">Canvas面板</param>
        /// <param name="eventCamera">事件相机</param>
        /// <param name="pointerPosition">碰撞点数据</param>
        /// <param name="results">结果集</param>
        /// <param name="origin">射线原点</param>
        private static void Raycast(Canvas canvas, Camera eventCamera, out Vector2 pointerPosition, List<Graphic> results, Transform origin)
        {
            pointerPosition = Vector2.zero;
            // Necessary for the event system
            var foundGraphics = GraphicRegistry.GetGraphicsForCanvas(canvas);
            for (int i = 0; i < foundGraphics.Count; ++i)
            {
                Graphic graphic = foundGraphics[i];

                // -1 means it hasn't been processed by the canvas, which means it isn't actually drawn
                if (graphic.depth == -1 || !graphic.raycastTarget)
                    continue;

                Plane plane = new Plane(graphic.transform.forward, graphic.transform.position);
                Ray ray = new Ray(origin.position, origin.forward);
                //var node = Plane.//LineNodeHelper.GetIntersectWithLineAndPlane(origin.position, origin.forward, graphic.transform.forward, graphic.transform.position);

                if(!plane.Raycast(ray, out float enter)) continue;
                Vector3 node = ray.GetPoint(enter);

                pointerPosition = eventCamera.WorldToScreenPoint(node);
                if (!RectTransformUtility.RectangleContainsScreenPoint(graphic.rectTransform, pointerPosition, eventCamera))
                    continue;

                if (graphic.Raycast(pointerPosition, eventCamera))
                {
                    _sortedGraphics.Add(graphic);
                }
            }

            _sortedGraphics.Sort((g1, g2) => g2.depth.CompareTo(g1.depth));
            for (int i = 0; i < _sortedGraphics.Count; ++i)
                results.Add(_sortedGraphics[i]);

            _sortedGraphics.Clear();
        }

        protected void Start()
        {
            //InitRayCaster();
            if (canvas == null)
            {
                this.enabled = false;
            }  
        }

        /// <summary>
        /// 初始化射线检测
        /// </summary>
        /// <param name="isCam"></param>
        public override void InitRayCaster(bool isCam = false)
        {
            baseInputSource?.raycasterCantainer.AddRaycaster(this);
            isCamera = isCam;
            _eventCamera = baseInputSource?.eventCamera;
            if (_origin == null) _origin = baseInputSource?.origin;
            if (_canvas == null) _canvas = baseInputSource?.canvas;
        }
    }
}
