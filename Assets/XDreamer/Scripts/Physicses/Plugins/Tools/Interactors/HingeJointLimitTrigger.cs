using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginPhysicses.Base.Limits;
using XCSJ.PluginTools;

namespace XCSJ.PluginPhysicses.Tools.Interactors
{
    /// <summary>
    /// 旋转限定触发器
    /// </summary>
    [Name("铰链限定触发器")]
    [XCSJ.Attributes.Icon(EIcon.Button)]
    [Tool(PhysicsManager.Title, rootType = typeof(PhysicsManager))]
    public class HingeJointLimitTrigger : LimitCalculatorTrigger
    {
        /// <summary>
        /// 铰链限定器
        /// </summary>
        [Name("铰链限定器")]
        [EndGroup]
        public HingeJointLimiter _hingeJointLimiter = new HingeJointLimiter();

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            _hingeJointLimiter._hingeJoint = GetComponent<HingeJoint>();
        }

        /// <summary>
        /// 限定器
        /// </summary>
        protected override ILimiter limiter => _hingeJointLimiter;
    }
}
