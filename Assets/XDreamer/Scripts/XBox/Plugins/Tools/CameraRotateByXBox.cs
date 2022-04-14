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
    /// 相机旋转通过XBox：默认通过XBox右摇杆控制相机的旋转
    /// </summary>
    [Name("相机旋转通过XBox")]
    [Tip("默认通过XBox右摇杆控制相机的旋转")]
    [Tool(CameraHelperExtension.ControllersCategoryName, /*nameof(CameraController),*/ nameof(CameraRotator))]
    [Tool(XBoxHelper.Title)]
    [XCSJ.Attributes.Icon(EIcon.Rotate)]
    [RequireManager(typeof(XBoxManager))]
    public class CameraRotateByXBox : BaseCameraRotateController, IXBox
    {
        /// <summary>
        /// 抬头仰视:对应X轴旋转，X减
        /// </summary>
        [Name("抬头仰视")]
        [Tip("对应X轴旋转，X减")]
        [EnumPopup]
        public EXBoxAxisAndButton _up = EXBoxAxisAndButton.RightStickUp;

        /// <summary>
        /// 低头俯视:对应X轴旋转，X增
        /// </summary>
        [Name("低头俯视")]
        [Tip("对应X轴旋转，X增")]
        [EnumPopup]
        public EXBoxAxisAndButton _down = EXBoxAxisAndButton.RightStickDown;

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
        /// 左倾斜:对应Z轴旋转，逆时针旋转，Z增
        /// </summary>
        [Name("左倾斜")]
        [Tip("对应Z轴旋转，逆时针旋转，Z增")]
        [EnumPopup]
        public EXBoxAxisAndButton _leftTilt = EXBoxAxisAndButton.None;

        /// <summary>
        /// 右倾斜:对应Z轴旋转，顺时针旋转，Z减
        /// </summary>
        [Name("右倾斜")]
        [Tip("对应Z轴旋转，顺时针旋转，Z减")]
        [EnumPopup]
        public EXBoxAxisAndButton _rightTilt = EXBoxAxisAndButton.None;

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

            _offset.x = -GetValue(_up) + GetValue(_down);
            _offset.x *= speedRealtime.x;

            _offset.y = -GetValue(_left) + GetValue(_right);
            _offset.y *= speedRealtime.y;

            _offset.z = GetValue(_leftTilt) - GetValue(_rightTilt);
            _offset.z *= speedRealtime.z;

            if (_offset != Vector3.zero)
            {
                Rotate();
            }
        }
    }
}
