﻿using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
using XCSJ.Tools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginsCameras.Tools.Controllers
{
    /// <summary>
    /// 相机旋转通过键码：默认通过键盘光标键码控制相机的旋转
    /// </summary>
    [Name("相机旋转通过键码")]
    [Tip("默认通过键盘光标键码控制相机的旋转")]
    [Tool(CameraHelperExtension.ControllersCategoryName, /*nameof(CameraController),*/ nameof(CameraRotator))]
    [XCSJ.Attributes.Icon(EIcon.Rotate)]
    public class CameraRotateByKeyCode : BaseCameraRotateController
    {
        #region 键码

        /// <summary>
        /// 抬头仰视:对应X轴旋转，X减
        /// </summary>
        [Name("抬头仰视")]
        [Tip("对应X轴旋转，X减")]
        public KeyCode _up = KeyCode.UpArrow;

        /// <summary>
        /// 低头俯视:对应X轴旋转，X增
        /// </summary>
        [Name("低头俯视")]
        [Tip("对应X轴旋转，X增")]
        public KeyCode _down = KeyCode.DownArrow;

        /// <summary>
        /// 左转:对应Y轴旋转，Y减
        /// </summary>
        [Name("左转")]
        [Tip("对应Y轴旋转，Y减")]
        public KeyCode _left = KeyCode.LeftArrow;

        /// <summary>
        /// 右转:对应Y轴旋转，Y增
        /// </summary>
        [Name("右转")]
        [Tip("对应Y轴旋转，Y增")]
        public KeyCode _right = KeyCode.RightArrow;

        /// <summary>
        /// 左倾斜:对应Z轴旋转，逆时针旋转，Z增
        /// </summary>
        [Name("左倾斜")]
        [Tip("对应Z轴旋转，逆时针旋转，Z增")]
        public KeyCode _leftTilt = KeyCode.None;

        /// <summary>
        /// 右倾斜:对应Z轴旋转，顺时针旋转，Z
        /// </summary>
        [Name("右倾斜")]
        [Tip("对应Z轴旋转，顺时针旋转，Z减")]
        public KeyCode _rightTilt = KeyCode.None;

        #endregion

        /// <summary>
        /// 更新
        /// </summary>
        public override void Update()
        {
            base.Update();
            var speedRealtime = this.speedRealtime;

            var hasInput = false;

            if (Input.GetKey(_up))
            {
                _offset.x -= speedRealtime.x;
                hasInput = true;
            }
            if (Input.GetKey(_down))
            {
                _offset.x += speedRealtime.x;
                hasInput = true;
            }

            if (Input.GetKey(_left))
            {
                _offset.y -= speedRealtime.y;
                hasInput = true;
            }
            if (Input.GetKey(_right))
            {
                _offset.y += speedRealtime.y;
                hasInput = true;
            }

            if (Input.GetKey(_leftTilt))
            {
                _offset.z += speedRealtime.z;
                hasInput = true;
            }
            if (Input.GetKey(_rightTilt))
            {
                _offset.z -= speedRealtime.z;
                hasInput = true;
            }

            if (hasInput)
            {
                Rotate();
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
        }
    }
}
