using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Attributes;

namespace XCSJ.PluginPeripheralDevice.Raycasters
{
    /// <summary>
    /// 用于检测3D的射线检测
    /// </summary>
    public class PhysicsRaycaster : BaseRaycaster
    {
        /// <summary>
        /// 射线检测
        /// </summary>
        /// <param name="eventData">事件数据</param>
        /// <param name="resultAppendList">结果集</param>
        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            if (eventCamera == null)
                return;

            if (origin == null) origin = transform;
            var ray = isCamera ? eventCamera.ScreenPointToRay(eventData.position) : new Ray(origin.position, origin.forward);
            float dist = eventCamera.farClipPlane - eventCamera.nearClipPlane;

            var hits =  Physics.RaycastAll(ray, distance);

            if (hits.Length > 1)
                System.Array.Sort(hits, (r1, r2) => r1.distance.CompareTo(r2.distance));

            if (hits.Length != 0)
            {
                for (int b = 0, bmax = hits.Length; b < bmax; ++b)
                {
                    var result = new RaycastResult
                    {
                        gameObject = hits[b].collider.gameObject,
                        module = Raycaster(),
                        distance = hits[b].distance,
                        worldPosition = hits[b].point,
                        worldNormal = hits[b].normal,
                        screenPosition = eventCamera.WorldToScreenPoint(hits[b].point),
                        index = resultAppendList.Count,
                        sortingLayer = 0,
                        sortingOrder = 0
                    };
                    resultAppendList.Add(result);
                }
            }
        }

        #region Unity 函数

        //protected void Start()
        //{
        //    InitRayCaster();
        //}
        /// <summary>
        /// 射线长度
        /// </summary>
        private float distance = 1000;

        /// <summary>
        /// 初始化射线检测
        /// </summary>
        /// <param name="isCam">是否相机检测</param>
        public override void InitRayCaster(bool isCam = false)
        {
            baseInputSource?.raycasterCantainer.AddRaycaster(this);
            isCamera = isCam;
            _eventCamera = baseInputSource?.eventCamera;
            if (_origin == null) _origin = baseInputSource?.origin;
            distance = baseInputSource.rayDistance;
        }

        #endregion  Unity 函数
    }
}
