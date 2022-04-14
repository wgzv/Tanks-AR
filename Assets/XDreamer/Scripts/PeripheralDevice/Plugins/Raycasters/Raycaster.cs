using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginPeripheralDevice.Raycasters
{
    /// <summary>
    /// 继承自UnityEngine.EventSystems.BaseRaycaster，主要用于设置RaycastResult的参数，不然，UGUI计算有些问题，主要提供eventCamera的设置
    /// </summary>
    public class Raycaster : UnityEngine.EventSystems.BaseRaycaster
    {
        /// <summary>
        /// 射线检测相机
        /// </summary>
        private Camera _eventCamera;
        public override Camera eventCamera  => _eventCamera;

        /// <summary>
        /// 射线检测
        /// </summary>
        /// <param name="eventData">事件数据</param>
        /// <param name="resultAppendList">结果集</param>
        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            
        }

        /// <summary>
        /// 设置射线检测相机
        /// </summary>
        /// <param name="camera">相机</param>
        public void SetRaycasterCamera(Camera camera)
        {
            _eventCamera = camera;
        }

        protected override void OnEnable()
        {
            
        }

        protected override void OnDisable()
        {
            
        }
    }
}
