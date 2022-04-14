using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.PluginTools.AI.Autos
{
    [XCSJ.Attributes.Icon(EIcon.Rotate)]
    [Name("自动旋转")]
    [DisallowMultipleComponent]
    public class AutoRotate : AutoWait
    {
        /// <summary>
        /// 旋转变换：用于控制自动旋转的对象；如本参数无效，则使用当前组件所在游戏对象上当前参数类型的组件对象
        /// </summary>
        [Group("旋转")]
        [Name("旋转变换")]
        [Tip("用于控制自动旋转的对象；如本参数无效，则使用当前组件所在游戏对象上当前参数类型的组件对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform _rotateTransform;

        /// <summary>
        /// 旋转变换
        /// </summary>
        public Transform rotateTransform
        {
            get
            {
                if (!_rotateTransform)
                {
                    _rotateTransform = transform;
                }
                return _rotateTransform;
            }
        }

        /// <summary>
        /// 旋转速度：游戏对象旋转的速度；单位：角度/秒；
        /// </summary>
        [Name("旋转速度")]
        [Tip("游戏对象旋转的速度；单位：角度/秒；")]
        public float rotateSpeed = 30;

        /// <summary>
        /// 旋转类型：游戏对象旋转的速度；单位：角度/秒；
        /// </summary>
        [Name("旋转类型")]
        [EnumPopup]
        public ERotateType rotateType = ERotateType.BoundsWorld;

        /// <summary>
        /// 旋转类型
        /// </summary>
        public enum ERotateType
        {
            /// <summary>
            /// 包围盒(世界坐标系)
            /// </summary>
            [Name("包围盒(世界坐标系)")]
            BoundsWorld,

            /// <summary>
            /// 包围盒(自身坐标系)
            /// </summary>
            [Name("包围盒(自身坐标系)")]
            BoundsLocal,

            /// <summary>
            /// 包围盒底面中心(世界坐标系)
            /// </summary>
            [Name("包围盒底面中心(世界坐标系)")]
            BoundsBottomCenterWorld,

            /// <summary>
            /// 包围盒底面中心(自身坐标系)
            /// </summary>
            [Name("包围盒底面中心(自身坐标系)")]
            BoundsBottomCenterLocal,

            /// <summary>
            /// 变换(世界坐标系)
            /// </summary>
            [Name("变换(世界坐标系)")]
            TransformWorld,

            /// <summary>
            /// 变换(自身坐标系)
            /// </summary>
            [Name("变换(自身坐标系)")]
            TransformLocal,
        }

        /// <summary>
        /// 旋转轴
        /// </summary>
        [Name("旋转轴")]
        [ValidityCheck(EValidityCheckType.NotDefault)]
        public Vector3 axis = Vector3.up;

        /// <summary>
        /// 包围盒中心
        /// </summary>
        private Bounds bounds;

        /// <summary>
        /// 临时变换
        /// </summary>
        private Transform tmpTransform;

        /// <summary>
        /// 启用时
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            if (!tmpTransform)
            {
                tmpTransform = new GameObject().transform;
                tmpTransform.SetParent(rotateTransform);
            }
            tmpTransform.localPosition = axis;

            // 求包围盒
            if (CommonFun.GetBounds(out bounds, this.gameObject) == false)
            {
                bounds = new Bounds(rotateTransform.position, Vector3.one);
            }
        }

        /// <summary>
        /// 更新时
        /// </summary>
        public override void Update()
        {
            base.Update();
            if (canUpdate)
            {
                Vector3 rotatePoint = Vector3.zero;
                Vector3 rotateAxis = axis;
                switch (rotateType)
                {
                    case ERotateType.TransformLocal:
                        {
                            rotatePoint = rotateTransform.position;
                            rotateAxis = tmpTransform.position - rotateTransform.position;
                            break;
                        }
                    case ERotateType.TransformWorld:
                        {
                            rotatePoint = rotateTransform.position;
                            break;
                        }
                    case ERotateType.BoundsWorld:
                        {
                            rotatePoint = bounds.center;
                            break;
                        }
                    case ERotateType.BoundsLocal:
                        {
                            rotatePoint = bounds.center;
                            rotateAxis = tmpTransform.position - rotateTransform.position;
                            break;
                        }
                    case ERotateType.BoundsBottomCenterWorld:
                        {
                            rotatePoint = bounds.center - new Vector3(0, bounds.extents.y, 0);
                            break;
                        }
                    case ERotateType.BoundsBottomCenterLocal:
                        {
                            rotatePoint = bounds.center - new Vector3(0, bounds.extents.y, 0);
                            rotateAxis = tmpTransform.position - rotateTransform.position;
                            break;
                        }
                }

                rotateTransform.RotateAround(rotatePoint, rotateAxis, rotateSpeed * Time.deltaTime);
            }
        }
    }
}
