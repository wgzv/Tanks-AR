using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginPhysicses.Base.Limits
{
    /// <summary>
    /// 铰链限定器：使用物理引擎铰链组件来限制旋转量，并提供计算数据
    /// </summary>
    [Name("铰链限定器")]
    [Serializable]
    public class HingeJointLimiter : Limiter
    {
        /// <summary>
        /// 铰链
        /// </summary>
        [Name("铰链")]
        [Tip("")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public HingeJoint _hingeJoint;

        /// <summary>
        /// 当前值
        /// </summary>
        public override float currentValue
        {
            get
            {
                if (dataValid && range!=0)
                {
                    return (_hingeJoint.angle - _hingeJoint.limits.min) / range;
                }
                return 0;
            }
        }

        /// <summary>
        /// 数据有效
        /// </summary>
        public bool dataValid => _hingeJoint;

        private float range = 0;

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Init()
        {
            base.Init();

            if (Mathf.Approximately(_hingeJoint.limits.max, _hingeJoint.limits.min))
            {
                range = 0;
            }
            else
            {
                range = _hingeJoint.limits.max - _hingeJoint.limits.min;
            }
        }
    }
}
