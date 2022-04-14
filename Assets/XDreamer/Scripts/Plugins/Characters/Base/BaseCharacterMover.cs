using UnityEngine;
using XCSJ.Attributes;

namespace XCSJ.Extension.Characters.Base
{
    /// <summary>
    /// 基础角色移动器
    /// </summary>
    [Name("基础角色移动器")]
    public abstract class BaseCharacterMover: BaseCharacterCoreController
    {
        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="value"></param>
        /// <param name="moveMode"></param>
        public abstract void Move(Vector3 value, int moveMode);

        /// <summary>
        /// 计算期望的矢量速度
        /// </summary>
        /// <returns></returns>
        public abstract Vector3 CalcDesiredVelocity();
    }
}
