using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Interactions.Base
{
    /// <summary>
    /// 位置朝向参考点:提供位置与方向参考信息
    /// </summary>
    [Serializable]
    [Name("位置朝向参考点")]
    public class ReferencePoint
    {
        /// <summary>
        /// 参考点类型
        /// </summary>
        [Name("参考点类型")]
        [EnumPopup]
        public EReferenceRule _referencePointType = EReferenceRule.Camera;

        /// <summary>
        /// 变换：变换的朝向为方向
        /// </summary>
        [Name("变换")]
        [Tip("变换的朝向为方向")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_referencePointType), EValidityCheckType.NotEqual, EReferenceRule.Transform)]
        public Transform _transform;

        /// <summary>
        /// 位置
        /// </summary>
        [Name("位置")]
        [HideInSuperInspector(nameof(_referencePointType), EValidityCheckType.NotEqual, EReferenceRule.Vector3)]
        [OnlyMemberElements]
        public Vector3PropertyValue _position;

        /// <summary>
        /// 方向
        /// </summary>
        [Name("方向")]
        [HideInSuperInspector(nameof(_referencePointType), EValidityCheckType.NotEqual, EReferenceRule.Vector3)]
        [OnlyMemberElements]
        public Vector3PropertyValue _direction;

        /// <summary>
        /// 方向延伸距离：最终位置点=位置+方向法向量*方向延伸距离
        /// </summary>
        [Name("方向延伸距离")]
        [Tip("最终位置点=位置+方向法向量*方向延伸距离")]
        [Min(0)]
        [HideInSuperInspector(nameof(_referencePointType), EValidityCheckType.Equal, EReferenceRule.CameraScreenMousePositionRay)]
        public float _distanceAlongForward = 3;

        /// <summary>
        /// 获取位置
        /// </summary>
        /// <returns></returns>
        public Vector3 GetPosition()
        {
            if (TryGetPositionAndDirection(out Vector3 pos, out Vector3 dir))
            {
                return pos;
            }
            return Vector3.zero;
        }

        private Vector3 GetPosition(Transform transform) => GetPosition(transform.position, transform.forward);

        private Vector3 GetPosition(Vector3 position, Vector3 direction) => position + direction.normalized * _distanceAlongForward;

        /// <summary>
        /// 获取朝向
        /// </summary>
        /// <returns></returns>
        public Vector3 GetDirection()
        {
            if (TryGetPositionAndDirection(out Vector3 pos, out Vector3 dir))
            {
                return dir;
            }
            return Vector3.forward;
        }

        /// <summary>
        /// 获取旋转量
        /// </summary>
        /// <returns></returns>
        public Quaternion GetRotation()
        {
            switch (_referencePointType)
            {
                case EReferenceRule.Camera:
                    {
                        var cam = GetCamera();
                        if (cam)
                        {
                            return cam.transform.rotation;
                        }
                        break;
                    }
                case EReferenceRule.CameraScreenMousePositionRay:
                    {
                        var cam = GetCamera();
                        if (cam)
                        {
                            return Quaternion.LookRotation(cam.ScreenPointToRay(Input.mousePosition).direction);
                        }
                        break;
                    }
                case EReferenceRule.Transform:
                    {
                        if (_transform)
                        {
                            return _transform.rotation;
                        }
                        break;
                    }
                case EReferenceRule.Vector3: return Quaternion.LookRotation(_direction.GetValue());
            }
            return Quaternion.identity;
        }

        private Camera GetCamera() => Camera.main ? Camera.main : Camera.current;

        /// <summary>
        /// 尝试获取位置与旋转量
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public bool TryGetPositionAndDirection(out Vector3 position, out Vector3 direction)
        {
            switch (_referencePointType)
            {
                case EReferenceRule.Camera:
                    {
                        var cam = GetCamera();
                        if (cam)
                        {
                            direction = cam.transform.forward;
                            position = GetPosition(cam.transform);
                            return true;
                        }
                        break;
                    }
                case EReferenceRule.CameraScreenMousePositionRay:
                    {
                        var cam = GetCamera();
                        if (cam)
                        {
                            var ray = cam.ScreenPointToRay(Input.mousePosition);
                            direction = ray.direction;
                            position = ray.origin;
                            return true;
                        }
                        break;
                    }
                case EReferenceRule.Transform:
                    {
                        if (_transform)
                        {
                            direction = _transform.forward;
                            position = GetPosition(_transform);
                            return true;
                        }
                        break;
                    }
                case EReferenceRule.Vector3:
                    {
                        direction = _direction.GetValue();
                        position = GetPosition(_position.GetValue(), direction);
                        return true;
                    }
            }
            position = Vector3.zero;
            direction = Vector3.zero;
            return false;
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public bool DataValidity()
        {
            switch (_referencePointType)
            {
                case EReferenceRule.Transform: return _transform;
            }
            return true;
        }
    }

    /// <summary>
    /// 参考点规则
    /// </summary>
    public enum EReferenceRule
    {
        [Name("当前相机")]
        Camera,

        [Name("当前相机屏幕鼠标点射线")]
        CameraScreenMousePositionRay,

        [Name("变换")]
        Transform,

        [Name("三维向量")]
        Vector3,
    }
    
}
