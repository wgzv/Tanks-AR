using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginsCameras.States
{
    /// <summary>
    /// 切换相机控制器
    /// </summary>
    [Name(Title, nameof(SwitchCameraController))]
    [ComponentMenu(CameraHelperExtension.StateLibCategoryName + "/" + Title, typeof(CameraManager))]
    [XCSJ.Attributes.Icon(EIcon.Switch)]
    public class SwitchCameraController : LifecycleExecutor<SwitchCameraController>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "切换相机控制器";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(CameraHelperExtension.StateLibCategoryName, typeof(CameraManager))]
        [Name(Title, nameof(SwitchCameraController))]
        [StateComponentMenu(CameraHelperExtension.StateLibCategoryName + "/" + Title, typeof(CameraManager))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 切换规则
        /// </summary>
        public enum ESwitchRule
        {
            /// <summary>
            /// 无：不做任何切换操作，会导致当前状态组件一直处于为完成态；
            /// </summary>
            [Name("无")]
            [Tip("不做任何切换操作，会导致当前状态组件一直处于为完成态；")]
            [Abbreviation("")]
            None,

            /// <summary>
            /// 相机控制器：切换到参数指定的相机控制器
            /// </summary>
            [Name("相机控制器")]
            [Tip("切换到参数指定的相机控制器")]
            CameraController,

            /// <summary>
            /// 上一个：切换到相机控制器列表中基于当前相机控制器的上一个相机控制器
            /// </summary>
            [Name("上一个")]
            [Tip("切换到相机控制器列表中基于当前相机控制器的上一个相机控制器")]
            [Abbreviation("<<<")]
            Previous,

            /// <summary>
            /// 下一个：切换到相机控制器列表中基于当前相机控制器的下一个相机控制器
            /// </summary>
            [Name("下一个")]
            [Tip("切换到相机控制器列表中基于当前相机控制器的下一个相机控制器")]
            [Abbreviation(">>>")]
            Next,
        }

        /// <summary>
        /// 切换规则
        /// </summary>
        [Name("切换规则")]
        [EnumPopup]
        public ESwitchRule _switchRule = ESwitchRule.CameraController;

        /// <summary>
        /// 相机控制器
        /// </summary>
        [Name("相机控制器")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_switchRule), EValidityCheckType.NotEqual, ESwitchRule.CameraController)]
        public BaseCameraMainController _cameraController;

        /// <summary>
        /// 持续时间
        /// </summary>
        [Name("持续时间")]
        [Tip("切换相机控制器的过渡时间，会自动进行补间动画；如果时间过短，会不做任何补间直接切换；")]
        [Range(0, 10f)]
        public float _duration = 1f;

        /// <summary>
        /// 强制切换：如果当前有相机控制器正在切换中，是否中断该切换并强制切换到期望的相机控制器；
        /// </summary>
        [Name("强制切换")]
        [Tip("如果当前有相机控制器正在切换中，是否中断该切换并强制切换到期望的相机控制器；")]
        public bool _museSwitch = false;

        /// <summary>
        /// 等待结束切换
        /// </summary>
        [Name("等待结束切换")]
        [Tip("标识是否等待相机控制器切换完成之后，状态组件方才标记完成态；仅针执行模式包含预进入、进入、已经入、更新的任意一个执行模式时本参数方才有效；")]
        public bool _waitEndSwitch = false;

        private bool hasEndSwitch = false;

        /// <summary>
        /// 预进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnBeforeEntry(StateData stateData)
        {
            hasEndSwitch = false;
            base.OnBeforeEntry(stateData);
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            
            switch (_switchRule)
            {
                case ESwitchRule.CameraController:
                    {
                        var manager = CameraManager.instance;
                        if (manager)
                        {
                            manager.GetProvider().SwitchCameraController(_cameraController, _duration, () => hasEndSwitch = true, _museSwitch);
                        }
                        break;
                    }
                case ESwitchRule.Previous:
                    {
                        CameraHelperExtension.SwitchPreviousCamera(_duration, () => hasEndSwitch = true, _museSwitch);
                        break;
                    }
                case ESwitchRule.Next:
                    {
                        CameraHelperExtension.SwitchNextCamera(_duration, () => hasEndSwitch = true, _museSwitch);
                        break;
                    }
            }
        }

        /// <summary>
        /// 标记完成
        /// </summary>
        /// <returns></returns>
        public override bool Finished()
        {
            if (_waitEndSwitch && CanExecute(EExecuteMode.OnEntry | EExecuteMode.OnBeforeEntry | EExecuteMode.OnAfterEntry | EExecuteMode.OnUpdate))
            {
                return hasEndSwitch;
            }
            return base.Finished();
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            switch (_switchRule)
            {
                case ESwitchRule.CameraController: return _cameraController ? _cameraController.name : "";
                default: return AbbreviationAttribute.GetAbbreviation(_switchRule);
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            switch (_switchRule)
            {
                case ESwitchRule.CameraController: return _cameraController;
            }
            return base.DataValidity();
        }
    }

}
