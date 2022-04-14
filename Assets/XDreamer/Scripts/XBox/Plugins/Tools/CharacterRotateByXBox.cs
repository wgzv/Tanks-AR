using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Characters;
using XCSJ.Extension.Characters.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXBox.Base;

namespace XCSJ.PluginXBox.Tools
{
    /// <summary>
    /// 角色旋转通过XBox
    /// </summary>
    [Name("角色旋转通过XBox")]
    [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
    [Tool(CharacterHelper.ControllersCategoryName, /*nameof(XCharacterController),*/ nameof(CharacterRotator))]
    [Tool(XBoxHelper.Title)]
    [RequireManager(typeof(XBoxManager))]
    public class CharacterRotateByXBox : BaseCharacterRotateController, IXBox
    {
        /// <summary>
        /// 左转:对应Y轴旋转，Y减
        /// </summary>
        [Name("左转")]
        [Tip("对应Y轴旋转，Y减")]
        [EnumPopup]
        public EXBoxAxisAndButton _left = EXBoxAxisAndButton.RightStickLeft;

        /// <summary>
        /// 右转:对应Y轴旋转，Y增
        /// </summary>
        [Name("右转")]
        [Tip("对应Y轴旋转，Y增")]
        [EnumPopup]
        public EXBoxAxisAndButton _right = EXBoxAxisAndButton.RightStickRight;

        /// <summary>
        /// 死区
        /// </summary>
        [Name("死区")]
        [Range(0, 1)]
        public float _deadZone = 0.125f;

        private float GetValue(EXBoxAxisAndButton axisAndButton)
        {
            var value = axisAndButton.GetValue();
            return value < _deadZone ? 0 : value;
        }

        /// <summary>
        /// 更新
        /// </summary>
        public override void Update()
        {
            base.Update();
            var speedRealtime = _speedInfo.valueRealtime;

            _offset.y = -GetValue(_left) + GetValue(_right);
            _offset.y *= speedRealtime.y;

            if (_offset != Vector3.zero)
            {
                Rotate();
            }
        }
    }
}
