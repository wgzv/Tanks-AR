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
    /// 可配置关节限定触发器
    /// </summary>
    [Name("可配置关节限定触发器")]
    [Tip("用于移动限定或旋转限定")]
    [XCSJ.Attributes.Icon(EIcon.Button)]
    [Tool(PhysicsManager.Title, rootType = typeof(PhysicsManager))]
    public class ConfigurableJointLimitTrigger : LimitCalculatorTrigger
    {
        /// <summary>
        /// 可配置关节限定器
        /// </summary>
        [EndGroup]
        [Name("可配置关节限定器")]
        public ConfigurableJointLimiter _configurableJointLimiter = new ConfigurableJointLimiter();

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            _configurableJointLimiter._configurableJoint = GetComponent<ConfigurableJoint>();
        }

        /// <summary>
        /// 限定器
        /// </summary>
        protected override ILimiter limiter => _configurableJointLimiter;
    }
}
