using System;
using UnityEngine;
using XCSJ.Attributes;

namespace XCSJ.PluginTools.Interactions.Base
{
    /// <summary>
    /// 射线拾取器
    /// </summary>
    [Serializable]
    [Name("射线拾取器")]
    public class RayPicker : ReferencePoint
    {
        /// <summary>
        /// 射线最大距离
        /// </summary>
        [Name("射线最大距离")]
        public float _maxDistance = 1000;

        /// <summary>
        /// 射线层
        /// </summary>
        [Name("射线层")]
        public LayerMask _layerMask = 1;

        /// <summary>
        /// 射线产生碰撞，获取碰撞体的刚体
        /// </summary>
        /// <returns></returns>
        public Rigidbody Hit()
        {
            if (Physics.Raycast(GetPosition(), GetDirection(), out RaycastHit hit, _maxDistance, _layerMask))
            {
                return hit.rigidbody;
            }
            return null;
        }
    }
}
