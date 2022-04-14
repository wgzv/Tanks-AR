using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Characters.Base;

namespace XCSJ.Extension.Characters
{
    /// <summary>
    /// 角色移动器
    /// </summary>
    [Name("角色移动器")]
    public class CharacterMover : BaseCharacterMover
    {
        private Vector3 _moveDirection = new Vector3();

        private bool jump = false;

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="value"></param>
        /// <param name="moveMode"></param>
        public override void Move(Vector3 value, int moveMode) => Move(value, (EMoveMode)moveMode);

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="value"></param>
        /// <param name="moveMode"></param>
        public void Move(Vector3 value, EMoveMode moveMode)
        {
            if (value.y > JumpThresholdValue)
            {
                value.y = 0;
                jump = true;
            }
            else
            {
                jump = false;
            }
            switch (moveMode)
            {
                case EMoveMode.Local:
                    {
                        _moveDirection += characterTransform.TransformDirection(value);
                        break;
                    }
                case EMoveMode.World:
                    {
                        _moveDirection += value;
                        break;
                    }
            }
        }

        /// <summary>
        /// 计算所需的移动速度。
        /// 例如：将输入（移动方向）转换为运动速度矢量， 如使用导航网格代理所需的速度等。   
        /// 如组件未启用，返回默认值；
        /// </summary>
        public override Vector3 CalcDesiredVelocity()
        {
            if (!enabled) return Vector3.zero;
            try
            {
                var mainController = this.mainController;
                mainController.moveDirection = _moveDirection;

                // 如果正在应用根运动和根运动（例如：接地）
                // 使用动画速度作为动画完全控制
                if (mainController.useRootMotion && mainController.applyRootMotion)
                {
                    return mainController.rootMotionController.animVelocity;
                }

                // 将输入（移动方向）转换为速度矢量
                return _moveDirection * mainController.speed;
            }
            finally
            {
                _moveDirection = Vector3.zero;
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public virtual void Update()
        {
            mainController.jump = jump;
        }

        /// <summary>
        /// 跳跃阈值
        /// </summary>
        public const float JumpThresholdValue = 1000;

        /// <summary>
        /// 跳跃值
        /// </summary>
        public const float JumpValue = JumpThresholdValue + 1;
    }

    /// <summary>
    /// 移动模式
    /// </summary>
    public enum EMoveMode
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 本地
        /// </summary>
        [Name("本地")]
        Local,

        /// <summary>
        /// 世界
        /// </summary>
        [Name("世界")]
        World,
    }
}
