using UnityEngine;
using XCSJ.Attributes;

namespace XCSJ.Extension.Characters.Base
{
    /// <summary>
    /// 基础角色旋转器
    /// </summary>
    [Name("基础角色旋转器")]
    public abstract class BaseCharacterRotator : BaseCharacterCoreController
    {
        /// <summary>
        /// 旋转
        /// </summary>
        /// <param name="value"></param>
        /// <param name="rotateMode"></param>
        public abstract void Rotate(Vector3 value, int rotateMode);
    }
}
