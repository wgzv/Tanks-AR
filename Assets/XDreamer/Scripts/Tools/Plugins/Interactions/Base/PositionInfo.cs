using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Interactions.Base
{
    /// <summary>
    /// 位置信息
    /// </summary>
    [Serializable]
    [Name("位置信息")]
    public class PositionInfo
    {
        /// <summary>
        /// 参考点类型
        /// </summary>
        [Name("参考点类型")]
        [EnumPopup]
        public EReferenceRule _referencePointType = EReferenceRule.Camera;

        /// <summary>
        /// 变换
        /// </summary>
        [Name("变换")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_referencePointType), EValidityCheckType.NotEqual, EReferenceRule.Transform)]
        public Transform _transform;

        /// <summary>
        /// 位置
        /// </summary>
        [Name("位置")]
        [HideInSuperInspector(nameof(_referencePointType), EValidityCheckType.NotEqual, EReferenceRule.Vector3)]
        public Vector3PropertyValue _position;

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public bool DataValidity()
        {
            switch (_referencePointType)
            {
                case EReferenceRule.Camera: 
                case EReferenceRule.CameraScreenMousePositionRay: return cam;
                case EReferenceRule.Transform: return _transform;
                case EReferenceRule.Vector3:
                default: return true;
            }
        }

        private Vector3 GetPosition(Transform transform) => transform ? transform.position : Vector3.zero;

        private Camera cam => Camera.main ? Camera.main : Camera.current;

        /// <summary>
        /// 获取位置信息值
        /// </summary>
        /// <returns></returns>
        public Vector3 GetValue()
        {
            if (DataValidity())
            {
                switch (_referencePointType)
                {
                    case EReferenceRule.Camera:
                    case EReferenceRule.CameraScreenMousePositionRay: return GetPosition(cam.transform);
                    case EReferenceRule.Transform: return GetPosition(_transform);
                    case EReferenceRule.Vector3: return _position.GetValue();
                }
            }
            return Vector3.zero;
        }
    }
}
