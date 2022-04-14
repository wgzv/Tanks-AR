using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using UnityEngine.UI;

namespace XCSJ.PluginTools.Puts.TRSHandles
{
    /// <summary>
    /// 平移旋转缩放工具UI控制器:用于控制工具操作类型与变换空间
    /// </summary>
    [Name("平移旋转缩放工具UI控制器")]
    [RequireManager(typeof(ToolsManager))]
    public class TRSHandleUIOperation : SingleInstanceMB<TRSHandleUIOperation>
    {
        /// <summary>
        /// 平移
        /// </summary>
        [Name("平移")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Readonly(EEditorMode.Runtime)]
        public Toggle _positionToggle = null;

        /// <summary>
        /// 旋转
        /// </summary>
        [Name("旋转")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Readonly(EEditorMode.Runtime)]
        public Toggle _rotateToggle = null;

        /// <summary>
        /// 缩放
        /// </summary>
        [Name("缩放")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Readonly(EEditorMode.Runtime)]
        public Toggle _scaleToggle = null;

        /// <summary>
        /// 变换空间：本地或世界
        /// </summary>
        [Name("变换空间")]
        [Tip("勾选时为本地；不勾选时为世界")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Readonly(EEditorMode.Runtime)]
        public Toggle _spaceToggle = null;

        /// <summary>
        /// 切换空快捷键
        /// </summary>
        [Name("切换空快捷键")]
        public KeyCode _switchNoneKeyCode = KeyCode.Q;

        /// <summary>
        /// 切换平移快捷键
        /// </summary>
        [Name("切换平移快捷键")]
        public KeyCode _switchTranslateKeyCode = KeyCode.W;

        /// <summary>
        /// 切换旋转快捷键
        /// </summary>
        [Name("切换旋转快捷键")]
        public KeyCode _switchRotateKeyCode = KeyCode.E;

        /// <summary>
        /// 切换缩放快捷键
        /// </summary>
        [Name("切换缩放快捷键")]
        public KeyCode _switchScaleKeyCode = KeyCode.R;

        private TRSHandle _TRSHandle = null;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            _TRSHandle = FindObjectOfType<TRSHandle>();
            if (_TRSHandle)
            {
                OnTRSHandleSpaceChanged(_TRSHandle, _TRSHandle.space);
                OnTRSHandleToolTypeChanged(_TRSHandle, _TRSHandle.trsToolType);// 初始化

                TRSHandle.onTRSHandleSpaceChanged += OnTRSHandleSpaceChanged;
                TRSHandle.onTRSHandleToolTypeChanged += OnTRSHandleToolTypeChanged;

                if (_positionToggle) _positionToggle.onValueChanged.AddListener(OnPositionChanged);
                if (_rotateToggle) _rotateToggle.onValueChanged.AddListener(OnRotateChanged);
                if (_scaleToggle) _scaleToggle.onValueChanged.AddListener(OnScaleChanged);
                if (_spaceToggle) _spaceToggle.onValueChanged.AddListener(OnSpaceChanged);
            }
            else
            {
                enabled = false;
                return;
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();

            if (_TRSHandle)
            {
                TRSHandle.onTRSHandleSpaceChanged -= OnTRSHandleSpaceChanged;
                TRSHandle.onTRSHandleToolTypeChanged -= OnTRSHandleToolTypeChanged;

                if (_positionToggle) _positionToggle.onValueChanged.RemoveListener(OnPositionChanged);
                if (_rotateToggle) _rotateToggle.onValueChanged.RemoveListener(OnRotateChanged);
                if (_scaleToggle) _scaleToggle.onValueChanged.RemoveListener(OnScaleChanged);
                if (_spaceToggle) _spaceToggle.onValueChanged.RemoveListener(OnSpaceChanged);
            }
        }
       
        private void OnTRSHandleSpaceChanged(TRSHandle handle, TRSHandle.ESpace oldSpace)
        {
            if (_spaceToggle) _spaceToggle.isOn = handle.space == TRSHandle.ESpace.Local;
        }

        private void OnTRSHandleToolTypeChanged(TRSHandle handle, ETRSToolType oldTRSToolType)
        {
            if (_positionToggle) _positionToggle.isOn = false;
            if (_rotateToggle) _rotateToggle.isOn = false;
            if (_scaleToggle) _scaleToggle.isOn = false;

            switch (handle.trsToolType)
            {
                case ETRSToolType.Position:
                    {
                        if (_positionToggle) _positionToggle.isOn = true;
                        break;
                    }
                case ETRSToolType.Rotate:
                    {
                        if (_rotateToggle) _rotateToggle.isOn = true;
                        break;
                    }
                case ETRSToolType.Scale:
                    {
                        if (_scaleToggle) _scaleToggle.isOn = true;
                        break;
                    }
            }
        }

        private void OnPositionChanged(bool isOn)
        {
            if (isOn)
            {
                _TRSHandle.trsToolType = ETRSToolType.Position;
            }
            else 
            {
                StartCoroutine(DelaySetTRSHandleToolTypeNone());
            }
        }

        private void OnRotateChanged(bool isOn)
        {
            if (isOn)
            {
                _TRSHandle.trsToolType = ETRSToolType.Rotate;
            }
            else
            {
                StartCoroutine(DelaySetTRSHandleToolTypeNone());
            }
        }

        private void OnScaleChanged(bool isOn)
        {
            if (isOn)
            {
                _TRSHandle.trsToolType = ETRSToolType.Scale;
            }
            else
            {
                StartCoroutine(DelaySetTRSHandleToolTypeNone());
            }
        }

        /// <summary>
        /// 延帧调用
        /// </summary>
        /// <returns></returns>
        private IEnumerator DelaySetTRSHandleToolTypeNone()
        {
            yield return new WaitForEndOfFrame();
            if (_positionToggle && !_positionToggle.isOn && _rotateToggle && !_rotateToggle.isOn && _scaleToggle && !_scaleToggle.isOn)
            {
                _TRSHandle.trsToolType = ETRSToolType.None;
            }
        }

        private void OnSpaceChanged(bool isOn)
        {
            _TRSHandle.space = isOn ? TRSHandle.ESpace.Local : TRSHandle.ESpace.World;
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected void Update()
        {
            if (!_TRSHandle) return;

            if (Input.GetKeyUp(_switchNoneKeyCode))
            {
                _TRSHandle.trsToolType = ETRSToolType.None;
            }

            if (Input.GetKeyUp(_switchTranslateKeyCode))
            {
                _TRSHandle.trsToolType = ETRSToolType.Position;
            }

            if (Input.GetKeyUp(_switchRotateKeyCode))
            {
                _TRSHandle.trsToolType = ETRSToolType.Rotate;
            }

            if (Input.GetKeyUp(_switchScaleKeyCode))
            {
                _TRSHandle.trsToolType = ETRSToolType.Scale;
            }
        }
    }
}
