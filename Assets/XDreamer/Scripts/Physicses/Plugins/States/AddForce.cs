using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginPhysicses.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginTools.Interactions.Base;

namespace XCSJ.PluginPhysicses.States
{
    /// <summary>
    /// 施力：用于对具有刚体的对象施加推力或扭矩力
    /// </summary>
    [ComponentMenu(PhysicsManager.Title + "/" + Title, typeof(PhysicsManager))]
    [Name(Title, nameof(AddForce))]
    [Tip("用于对具有刚体的对象施加推力或扭矩力")]
    [XCSJ.Attributes.Icon(EIcon.Target)]
    [DisallowMultipleComponent]
    public class AddForce : LifecycleExecutor<AddForce>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "施力";

        /// <summary>
        /// 创建施力组件
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(PhysicsManager.Title, typeof(PhysicsManager))]
        [StateComponentMenu(PhysicsManager.Title + "/" + Title, typeof(PhysicsManager))]
        [Name(Title, nameof(AddForce))]
        [Tip("用于对具有刚体的对象施加推力或扭矩力")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 力类型
        /// </summary>
        [Name("力类型")]
        [EnumPopup]
        public EForceType _forceRule = EForceType.Force;

        /// <summary>
        /// 施力坐标系
        /// </summary>
        [Name("施力坐标系")]
        [Tip("施力方向和位置所处的参考坐标系")]
        [HideInSuperInspector(nameof(_forceRule), EValidityCheckType.Equal, EForceType.ForceAtPosition)]
        public Space _space = Space.World;

        /// <summary>
        /// 目标类型
        /// </summary>
        [Name("目标")]
        public ForceTarget _forceTarget = new ForceTarget();

        /// <summary>
        /// 施力模式
        /// </summary>
        [Name("施力模式")]
        public ForceModeInfo _forceModeInfo = new ForceModeInfo();

        /// <summary>
        /// 施力方向
        /// </summary>
        [Name("施力方向")]
        public DirectionInfo _directionInfo = new DirectionInfo();

        /// <summary>
        /// 施力作用点
        /// </summary>
        [Name("施力作用点")]
        [Tip("当作用力坐标系等于Local时，当前值会被目标对象当做其本地坐标系下的点")]
        [HideInSuperInspector(nameof(_forceRule), EValidityCheckType.NotEqual, EForceType.ForceAtPosition)]
        public PositionInfo _positionInfo = new PositionInfo();

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            if (!DataValidity()) return;

            var target = GetTarget();
            if (!target) return;
                
            switch (_forceRule)
            {
                case EForceType.Force:
                    {
                        switch (_space)
                        {
                            case Space.World:
                                {
                                    target.AddForce(GetForce(), _forceModeInfo._forceMode);
                                    break;
                                }
                            case Space.Self:
                                {
                                    target.AddRelativeForce(GetForce(), _forceModeInfo._forceMode);
                                    break;
                                }
                        }
                        break;
                    }
                case EForceType.ForceAtPosition:
                    {
                        var pos = _positionInfo.GetValue();
                        switch (_space)
                        {
                            case Space.World:
                                {
                                    break;
                                }
                            case Space.Self:
                                {
                                    // 将获取的位置当作目标对象的本地坐标系下的点，转到世界坐标系中
                                    pos = target.transform.TransformPoint(pos);
                                    break;
                                }
                        }
                        target.AddForceAtPosition(GetForce(), pos, _forceModeInfo._forceMode);
                        break;
                    }
                case EForceType.Torque:
                    {
                        switch (_space)
                        {
                            case Space.World:
                                {
                                    target.AddTorque(GetForce(), _forceModeInfo._forceMode);
                                    break;
                                }
                            case Space.Self:
                                {
                                    target.AddRelativeTorque(GetForce(), _forceModeInfo._forceMode);
                                    break;
                                }
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return true;
        }

        /// <summary>
        /// 获取目标
        /// </summary>
        /// <returns></returns>
        protected virtual Rigidbody GetTarget()
        {
            return _forceTarget.GetObject();
        }

        /// <summary>
        /// 获取力
        /// </summary>
        /// <returns></returns>
        protected Vector3 GetForce()
        {
            return _directionInfo.GetValue() * _forceModeInfo.GetValue();
        }

        /// <summary>
        /// 提示字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return string.Format("{0}:{1}", CommonFun.Name(_forceRule), _forceModeInfo.GetValue());
        }

        /// <summary>
        /// 施力类型
        /// </summary>
        public enum EForceType
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 推力:推力通过刚体重心使物体移动
            /// </summary>
            [Name("推力")]
            [Tip("推力通过刚体重心使物体移动")]
            Force,

            /// <summary>
            /// 定点推力:在刚体某点上的作用力使物体同时移动和旋转
            /// </summary>
            [Name("定点推力")]
            [Tip("在刚体某点上的作用力使物体同时移动和旋转")]
            ForceAtPosition,

            /// <summary>
            /// 扭矩:扭矩使物体产生旋转的力
            /// </summary>
            [Name("扭矩")]
            [Tip("扭矩使物体产生旋转的力")]
            Torque,
        }

    }
}
