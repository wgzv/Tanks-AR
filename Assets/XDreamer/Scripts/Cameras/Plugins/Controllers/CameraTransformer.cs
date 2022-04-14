using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Tools.Base;

namespace XCSJ.PluginsCameras.Controllers
{
    /// <summary>
    /// 相机变换器
    /// </summary>
    [Name("相机变换器")]
    [DisallowMultipleComponent]
    public class CameraTransformer : BaseCameraTransformer
    {
        /// <summary>
        /// 相机切换补间器
        /// </summary>
        [Name("相机切换补间器")]
        [Tip("用于覆盖默认相机切换补间操作的相机切换补间器对象；如果本对象无效，则使用默认规则；仅影响当前相机控制器切换到其他相机控制器时的补间效果；")]
        [ComponentPopup]
        public BaseCameraSwitchTweener _cameraSwitchTweener;

        /// <summary>
        /// 相机切换补间器
        /// </summary>
        public BaseCameraSwitchTweener cameraSwitchTweener
        {
            get => _cameraSwitchTweener;
            set => this.XModifyProperty(ref _cameraSwitchTweener, value, nameof(cameraSwitchTweener));
        }

        /// <summary>
        /// 当将要开始强制切换时回调
        /// </summary>
        /// <param name="targetCameraController">期望切换到的目标相机控制器</param>
        /// <param name="directSwitch">标识是直接切换还是补间切换</param>
        /// <returns>支持强制切换返回True，不支持强制切换返回False;</returns>
        public override bool OnWillBeginMustSwitch(BaseCameraMainController targetCameraController, bool directSwitch)
        {
            if(cameraSwitchTweener)
            {
                return cameraSwitchTweener.OnWillBeginMustSwitch(targetCameraController, directSwitch);
            }
            else
            {
                return base.OnWillBeginMustSwitch(targetCameraController, directSwitch);
            }
        }

        /// <summary>
        /// 当开始切换补间时回调
        /// </summary>
        /// <param name="targetCameraController">期望切换到的目标相机控制器</param>
        /// <param name="duration">补间的持续时间</param>
        /// <param name="onCompleted">补间完成后的回调</param>
        public override void OnBeginSwitchTween(BaseCameraMainController targetCameraController, float duration, Action onCompleted)
        {
            if (cameraSwitchTweener)
            {
                cameraSwitchTweener.OnBeginSwitchTween(targetCameraController, duration, onCompleted);
            }
            else
            {
                base.OnBeginSwitchTween(targetCameraController, duration, onCompleted);
            }
        }

        /// <summary>
        /// 当结束切换补间后回调
        /// </summary>
        /// <param name="newCurrentCameraController">新的当前相机控制器</param>
        public override void OnEndSwitchTween(BaseCameraMainController newCurrentCameraController)
        {
            if (cameraSwitchTweener)
            {
                cameraSwitchTweener.OnEndSwitchTween(newCurrentCameraController);
            }
            else
            {
                base.OnEndSwitchTween(newCurrentCameraController);
            }
        }
    }
}
