using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginsCameras;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
using XCSJ.PluginXBox.Base;

namespace XCSJ.PluginXBox.Tools
{
    /// <summary>
    /// 相机移动通过XBox:默认通过XBox左摇杆与Dpad控制相机的移动
    /// </summary>
    [Name("相机移动通过XBox")]
    [Tip("默认通过XBox左摇杆与Dpad控制相机的移动")]
    [Tool(CameraHelperExtension.ControllersCategoryName, /*nameof(CameraController),*/ nameof(CameraMover)/*, nameof(CameraTargetController)*/)]
    [Tool(XBoxHelper.Title)]
    [XCSJ.Attributes.Icon(EIcon.Move)]
    [RequireManager(typeof(XBoxManager))]
    public class CameraMoveByXBox : BaseCameraMoveController, IXBox
    {
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
        /// 上移:对应Y轴移动，Y增
        /// </summary>
        [Name("上移")]
        [Tip("对应Y轴移动，Y增")]
        [EnumPopup]
        public EXBoxAxisAndButton _up = EXBoxAxisAndButton.DpadUp;

        /// <summary>
        /// 下移:对应Y轴移动，Y减
        /// </summary>
        [Name("下移")]
        [Tip("对应Y轴移动，Y减")]
        [EnumPopup]
        public EXBoxAxisAndButton _down = EXBoxAxisAndButton.DpadDown;

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
            var speedRealtime = this.speedRealtime;

            _offset.x = -GetValue(_left) + GetValue(_right);
            _offset.x *= speedRealtime.x;

            _offset.y = GetValue(_up) - GetValue(_down);
            _offset.y *= speedRealtime.y;

            _offset.z = GetValue(_forward) - GetValue(_back);
            _offset.z *= speedRealtime.z;

            if (_offset != Vector3.zero)
            {
                Move();
            }
        }
    }
}
