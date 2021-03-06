using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginsCameras.Base
{
    /// <summary>
    /// 基础相机移动器
    /// </summary>
    public abstract class BaseCameraMover : BaseCameraCoreController, ICameraMover
    {
        /// <summary>
        /// 速度系数
        /// </summary>
        [Name("速度系数")]
        public Vector3 _speedCoefficient = Vector3.one;

        /// <summary>
        /// 速度系数
        /// </summary>
        public Vector3 speedCoefficient
        {
            get => _speedCoefficient;
            set => this.XModifyProperty(ref _speedCoefficient, value);
        }

        /// <summary>
        /// 阻尼系数
        /// </summary>
        [Name("阻尼系数")]
        [Range(0, CameraHelperExtension.MaxDampingCoefficient)]
        public float _dampingCoefficient = 1;

        /// <summary>
        /// 阻尼系数
        /// </summary>
        public float dampingCoefficient
        {
            get => _dampingCoefficient;
            set => this.XModifyProperty(ref _dampingCoefficient, value);
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="value"></param>
        /// <param name="moveMode"></param>
        public abstract void Move(Vector3 value, int moveMode);
    }
}
