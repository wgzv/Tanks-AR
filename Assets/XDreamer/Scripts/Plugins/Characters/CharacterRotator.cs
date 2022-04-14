using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Characters.Base;

namespace XCSJ.Extension.Characters
{
    /// <summary>
    /// 角色旋转器
    /// </summary>
    [Name("角色旋转器")]
    public class CharacterRotator : BaseCharacterRotator
    {
        /// <summary>
        /// 旋转
        /// </summary>
        /// <param name="value"></param>
        /// <param name="rotateMode"></param>
        public override void Rotate(Vector3 value, int rotateMode) => Rotate(value, (ERotateMode)rotateMode);

        /// <summary>
        /// 旋转
        /// </summary>
        /// <param name="value"></param>
        /// <param name="rotateMode"></param>
        public void Rotate(Vector3 value, ERotateMode rotateMode)
        {
            directions[rotateMode] = value;
        }

        private Dictionary<ERotateMode, Vector3> directions = new Dictionary<ERotateMode, Vector3>();

        /// <summary>
        /// 更新
        /// </summary>
        public virtual void Update()
        {
            foreach (var kv in directions)
            {
                switch (kv.Key)
                {
                    case ERotateMode.MoveDirection:
                        {
                            mainController.RotateTowardsMoveDirection();
                            break;
                        }
                    case ERotateMode.Velocity:
                        {
                            mainController.RotateTowardsVelocity();
                            break;
                        }
                    case ERotateMode.VectorDirection:
                        {
                            mainController.RotateTowards(kv.Value);
                            break;
                        }
                    case ERotateMode.Self_WorldY:
                        {
                            var rotation = characterTransform.rotation;
                            var targetRotation = rotation * Quaternion.Euler(kv.Value);
                            var dir = targetRotation * Vector3.forward;
                            mainController.RotateTowards(dir);
                            break;
                        }
                }
            }
            directions.Clear();
        }
    }

    /// <summary>
    /// 旋转模式
    /// </summary>
    [Name("旋转模式")]
    public enum ERotateMode
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 向移动方向向量（输入）旋转角色.即与<see cref="XCharacterController.moveDirection"/>方向一致；
        /// </summary>
        [Name("移动方向")]
        MoveDirection,

        /// <summary>
        /// 将角色朝其速度向量旋转.即与<see cref="CharacterMovement.velocity"/>方向一致；
        /// </summary>
        [Name("速度方向")]
        Velocity,

        /// <summary>
        /// 将角色朝参数指定向量方向旋转
        /// </summary>
        [Name("向量方向")]
        VectorDirection,

        /// <summary>
        /// 自身世界Y:以自身为旋转中心，世界坐标系的Y轴(上轴up对应Y轴)为旋转轴，执行旋转逻辑;
        /// </summary>
        [Name("自身世界Y")]
        [Tip("以自身为旋转中心，世界坐标系的Y轴(上轴up对应Y轴)为旋转轴，执行旋转逻辑;")]
        Self_WorldY,
    }
}
