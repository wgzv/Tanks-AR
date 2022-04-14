using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
using XCSJ.Tools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginsCameras.Tools.Controllers
{
    /// <summary>
    /// 相机移动通过目标
    /// </summary>
    [Name("相机移动通过目标")]
    [Tool(CameraHelperExtension.ControllersCategoryName, /*nameof(CameraController),*/ nameof(CameraMover)/*, nameof(CameraTargetController)*/)]
    [XCSJ.Attributes.Icon(EIcon.Move)]
    public class CameraMoveByTarget : BaseCameraMoveController
    {
        /// <summary>
        /// 上次目标位置
        /// </summary>
        [Name("上次目标位置")]
        [Readonly]
        public Vector3 lastTargetPosition = new Vector3();

        /// <summary>
        /// 使用速度
        /// </summary>
        [Name("使用速度")]
        public bool _useSpeed = false;

        /// <summary>
        /// 鼠标按钮处理器
        /// </summary>
        [Name("鼠标按钮处理器")]
        public MouseButtonHandler _mouseButtonHandler = new MouseButtonHandler();

        private void UpdateCache()
        {
            lastTargetPosition = cameraController.cameraTargetController.targetPosition;
        }

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            lastTargetPosition = cameraController.cameraTargetController.targetPosition;
        }

        /// <summary>
        /// 更新
        /// </summary>
        public override void Update()
        {
            if (!_mouseButtonHandler.CanContinue(UpdateCache))
            {
                return;
            }
            var targetPosition = cameraController.cameraTargetController.targetPosition;
            _offset = targetPosition - lastTargetPosition;

            if (_useSpeed)
            {
                base.Update();
                //var speedRealtime = this.speedRealtime;

                _offset = Vector3.Scale(_offset, speedRealtime);
                lastTargetPosition += _offset;
            }
            else
            {
                lastTargetPosition = targetPosition;
            }

            Move();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            _moveMode = EMoveMode.Self_World;

            _mouseButtonHandler._mouseButtons.Add(EMouseButton.Always);
            _mouseButtonHandler._ruleOnAnyMouseButton = MouseButtonHandler.ERule.None;
        }
    }

    /// <summary>
    /// 鼠标按钮处理器
    /// </summary>
    [Name("鼠标按钮处理器")]
    [Serializable]
    public class MouseButtonHandler
    {
        /// <summary>
        /// 鼠标按钮列表:列表中任意一项满足鼠标按键事件成立条件，则可继续处理；如果列表为空或无任何事件触发时，继续后续的处理；
        /// </summary>
        [Name("鼠标按钮列表")]
        [Tip("列表中任意一项满足鼠标按键事件成立条件，则可继续处理；如果列表为空或无任何事件触发时，继续后续的处理；")]
        [EnumPopup(typeof(EMouseButton))]
        public List<EMouseButton> _mouseButtons = new List<EMouseButton>();

        /// <summary>
        /// 规则:鼠标按钮列表中任何一个鼠标按钮按下事件触发时的处理规则；
        /// </summary>
        [Name("规则")]
        [Tip("鼠标按钮列表中任何一个鼠标按钮按下事件触发时的处理规则；")]
        [EnumPopup]
        public ERule _ruleOnAnyMouseButton = ERule.UpdateCacheAndReturn;

        /// <summary>
        /// 输入
        /// </summary>
        [Name("输入")]
        [EnumPopup]
        public EInput _input = EInput.XInput;

        /// <summary>
        /// 能否继续处理
        /// </summary>
        /// <param name="updateCache">更新缓存</param>
        /// <returns></returns>
        public bool CanContinue(Action updateCache)
        {            
            if (_mouseButtons.GetAnyMouseButton(_input.GetInput()))
            {
                switch (_ruleOnAnyMouseButton)
                {
                    case ERule.Return: return false;
                    case ERule.UpdateCacheAndReturn:
                        {
                            updateCache?.Invoke();
                            return false;
                        }
                    case ERule.UpdateCacheAndContinue:
                        {
                            updateCache?.Invoke();
                            break;
                        }
                }
            }
            return true;
        }

        /// <summary>
        /// 鼠标按钮事件的规则枚举
        /// </summary>
        [Name("规则")]
        public enum ERule
        {
            /// <summary>
            /// 无：当前不做任何操作，并继续后续的处理；
            /// </summary>
            [Name("无")]
            [Tip("当前不做任何操作，并继续后续的处理；")]
            None,

            /// <summary>
            /// 返回：逻辑直接返回，不做任何后续处理；
            /// </summary>
            [Name("返回")]
            [Tip("逻辑直接返回，不做任何后续处理；")]
            Return,

            /// <summary>
            /// 更新缓存并返回：更新缓存后，逻辑直接返回，不再做任何后续的处理；
            /// </summary>
            [Name("更新缓存并返回")]
            [Tip("更新缓存后，逻辑直接返回，不再做任何后续的处理；")]
            UpdateCacheAndReturn,

            /// <summary>
            /// 更新缓存并继续：更新缓存后，继续后续的处理；
            /// </summary>
            [Name("更新缓存并继续")]
            [Tip("更新缓存后，继续后续的处理；")]
            UpdateCacheAndContinue,
        }
    }
}
