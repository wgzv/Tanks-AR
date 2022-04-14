using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Interactions.Base
{
    /// <summary>
    /// 方向信息
    /// </summary>
    [Serializable]
    [Name("方向信息")]
    public class DirectionInfo
    {
        /// <summary>
        /// 方向参考规则
        /// </summary>
        [Name("方向参考规则")]
        [EnumPopup]
        public EReferenceRule _forceDirectionRule;

        /// <summary>
        /// 变换
        /// </summary>
        [Name("变换")]
        [HideInSuperInspector(nameof(_forceDirectionRule), EValidityCheckType.NotEqual, EReferenceRule.Transform)]
        public Transform _transform;

        /// <summary>
        /// 三维向量
        /// </summary>
        [Name("三维向量")]
        [HideInSuperInspector(nameof(_forceDirectionRule), EValidityCheckType.NotEqual, EReferenceRule.Vector3)]
        public Vector3PropertyValue _vector3;

        /// <summary>
        /// 获取方向值
        /// </summary>
        /// <returns></returns>
        public Vector3 GetValue()
        {
            switch (_forceDirectionRule)
            {
                case EReferenceRule.Camera:
                    {
                        if (cam)
                        {
                            return cam.transform.forward;
                        }
                        break;
                    }
                case EReferenceRule.CameraScreenMousePositionRay:
                    {
                        if (cam)
                        {
                            return cam.ScreenPointToRay(Input.mousePosition).direction;
                        }
                        break;
                    }
                case EReferenceRule.Transform:
                    {
                        if (_transform)
                        {
                            return _transform.forward;
                        }
                        break;
                    }
                case EReferenceRule.Vector3:
                    {
                        return _vector3.GetValue();
                    }
            }
            return Vector3.zero;
        }

        private Camera cam => Camera.main ? Camera.main : Camera.current;
    }
}
