using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Characters;
using XCSJ.Extension.Characters.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXBox.Base;

namespace XCSJ.PluginXBox.Tools
{
    /// <summary>
    /// 角色移动通过XBox
    /// </summary>
    [Name("角色移动通过XBox")]
    [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
    [Tool(CharacterHelper.ControllersCategoryName, /*nameof(XCharacterController),*/ nameof(CharacterMover))]
    [Tool(XBoxHelper.Title)]
    [RequireManager(typeof(XBoxManager))]
    public class CharacterMoveByXBox : BaseCharacterToolController, IXBox
    {
        /// <summary>
        /// 移动模式
        /// </summary>
        [Name("移动模式")]
        [EnumPopup]
        public EMoveMode _moveMode = EMoveMode.Local;

        /// <summary>
        /// 左移:对应X轴移动，X减
        /// </summary>
        [Name("左移")]
        [Tip("对应X轴移动，X减")]
        [EnumPopup]
        public EXBoxAxisAndButton _left = EXBoxAxisAndButton.LeftStickLeft;

        /// <summary>
        /// 右移:对应X轴移动，X增
        /// </summary>
        [Name("右移")]
        [Tip("对应X轴移动，X增")]
        [EnumPopup]
        public EXBoxAxisAndButton _right = EXBoxAxisAndButton.LeftStickRight;

        /// <summary>
        /// 右移:对应X轴移动，X增
        /// </summary>
        [Name("跳跃")]
        [Tip("对应角色的Y轴变化控制")]
        [EnumPopup]
        public EXBoxAxisAndButton _jump = EXBoxAxisAndButton.A;

        /// <summary>
        /// 前进:对应Z轴移动，Z增
        /// </summary>
        [Name("前进")]
        [Tip("对应Z轴移动，Z增")]
        [EnumPopup]
        public EXBoxAxisAndButton _forward = EXBoxAxisAndButton.LeftStickUp;

        /// <summary>
        /// 后退:对应Z轴移动，Z减
        /// </summary>
        [Name("后退")]
        [Tip("对应Z轴移动，Z减")]
        [EnumPopup]
        public EXBoxAxisAndButton _back = EXBoxAxisAndButton.LeftStickDown;

        /// <summary>
        /// 死区
        /// </summary>
        [Name("死区")]
        [Range(0, 1)]
        public float _deadZone = 0.125f;

        /// <summary>
        /// 按压死区
        /// </summary>
        [Name("按压死区")]
        [Range(0, 1)]
        public float _pressDeadZone = 0.5f;

        private float GetValue(EXBoxAxisAndButton axisAndButton)
        {
            var value = axisAndButton.GetValue();
            return value < _deadZone ? 0 : value;
        }

        /// <summary>
        /// 更新
        /// </summary>
        public virtual void Update()
        {
            var moveDirection = Vector3.zero;
            moveDirection.x = -GetValue(_left) + GetValue(_right);

            moveDirection.y = _jump.IsPressed(_pressDeadZone) ? CharacterMover.JumpValue : 0;

            moveDirection.z = GetValue(_forward) - GetValue(_back);

            if (moveDirection != Vector3.zero)
            {
                mainController.Move(moveDirection, (int)_moveMode);
            }
        }
    }
}
