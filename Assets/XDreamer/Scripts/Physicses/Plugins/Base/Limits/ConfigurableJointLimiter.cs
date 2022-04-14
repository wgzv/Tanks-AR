using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginPhysicses.Base.Limits
{
    /// <summary>
    /// 可配置关节限定器:实现位移限定
    /// </summary>
    [Name("可配置关节限定器")]
    [Serializable]
    public class ConfigurableJointLimiter : Limiter
    {
        /// <summary>
        /// 可配置关节
        /// </summary>
        [Name("可配置关节")]
        [Tip("")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public ConfigurableJoint _configurableJoint;

        /// <summary>
        /// 当前值
        /// </summary>
        public override float currentValue
        {
            get
            {
                if (!valid) return 0;

                var currPos = GetLocalPositionWithMoveAxis();

                // 正负定义
                //var positive = (startPosition.x < currPos.x || startPosition.y < currPos.y || startPosition.z < currPos.z) ? false : true;

                var value = Vector3.Distance(startPosition, currPos) / _configurableJoint.linearLimit.limit;

                //if (!positive) value *= -1;

                return value;
            }
        }

        private Vector3 startPosition = Vector3.zero;

        /// <summary>
        /// 移动轴
        /// </summary>
        protected Vector3 moveAxis
        {
            get
            {
                if (valid)
                {
                    return new Vector3(_configurableJoint.xMotion == ConfigurableJointMotion.Locked ? 0 : 1,
                        _configurableJoint.yMotion == ConfigurableJointMotion.Locked ? 0 : 1,
                        _configurableJoint.zMotion == ConfigurableJointMotion.Locked ? 0 : 1);
                }
                return Vector3.zero;
            }
        }

        /// <summary>
        /// 数据有效
        /// </summary>
        public bool valid => _configurableJoint;

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Init()
        {
            base.Init();

            startPosition = GetLocalPositionWithMoveAxis();
        }

        private Vector3 GetLocalPositionWithMoveAxis()
        {
            return _configurableJoint ? Vector3.Scale(_configurableJoint.transform.localPosition, moveAxis) : Vector3.zero;
        }
    }
}
