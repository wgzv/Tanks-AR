using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.PluginPeripheralDevice;
using XCSJ.PluginPeripheralDevice.Modules;
using UInput = UnityEngine.Input;

namespace XCSJ.PluginPeripheralDevice.Standalones
{
    /// <summary>
    /// 鼠标键盘类输入操作
    /// </summary>
    public class StandaloneInputSource : BaseInputSource
    {
        /// <summary>
        /// 鼠标键盘一般使用的Axis名称数组
        /// </summary>
        private string[] _axisNames = new string[]{ "Mouse X", "Mouse Y", "Mouse ScrollWheel", "Vertical", "Horizontal" }; 

        /// <summary>
        /// 处理函数
        /// </summary>
        public override void Process()
        {
            // 预处理按键信息
            PreProcessKey();

            // 处理按键
            ProcessKeyboard();

            // 处理鼠标按键
            ProcessMouseButton();

            //处理Axis
            ProcessAxis();

            //处理AxisRaw
            ProcessAxisRaw();

            //处理移入事件
            ProcessPoniterEvent();

            // 删除无效信息
            DeleteInvalidKey();

        }

        /// <summary>
        /// 处理射线检测类事件
        /// </summary>
        protected override void ProcessPoniterEvent()
        {
            if (_baseInputModule == null)
            {
                _baseInputModule = new StandaloneInputModule();
            }
            _baseInputModule.UpdateModule();
            _baseInputModule?.Process();
        }

        /// <summary>
        /// 处理键盘按键事件
        /// </summary>
        private void ProcessKeyboard()
        {
            Type keyCode = typeof(KeyCode);
            Array keyArrays = Enum.GetValues(keyCode);
            for (int i = 0; i < keyArrays.LongLength; i++)
            {
                KeyCode curKey = (KeyCode)keyArrays.GetValue(i);
                string curKeyStr = curKey.ToString();
                if (UInput.GetKeyDown(curKey))
                {
                    if (_keyDataDic.ContainsKey(curKeyStr))
                    {
                        _keyDataDic[curKeyStr].buttonPressState = ButtonPressState.Pressed;
                    }
                    else
                    {
                        KeyData keyData = new KeyData(curKeyStr);
                        keyData.buttonPressState = ButtonPressState.Pressed;
                        _keyDataDic.Add(curKeyStr, keyData);
                    }
                }
                else if (UInput.GetKeyUp(curKey))
                {
                    if (_keyDataDic.ContainsKey(curKeyStr))
                    {
                        _keyDataDic[curKeyStr].buttonPressState = ButtonPressState.Released;
                    }
                }
            }
        }

        /// <summary>
        /// 处理鼠标按键事件
        /// </summary>
        private void ProcessMouseButton()
        {
            if (UInput.GetMouseButtonDown(0))
            {
                if (_buttonDataDic.ContainsKey("0"))
                {
                    _buttonDataDic["0"].buttonPressState = ButtonPressState.Pressed;
                }
                else
                {
                    ButtonData keyData = new ButtonData("0");
                    keyData.buttonPressState = ButtonPressState.Pressed;
                    _buttonDataDic.Add("0", keyData);
                }
            }
            else if (UInput.GetMouseButtonUp(0))
            {
                if (_buttonDataDic.ContainsKey("0"))
                {
                    _buttonDataDic["0"].buttonPressState = ButtonPressState.Released;
                }
            }

            if (UInput.GetMouseButtonDown(1))
            {
                if (_buttonDataDic.ContainsKey("1"))
                {
                    _buttonDataDic["1"].buttonPressState = ButtonPressState.Pressed;
                }
                else
                {
                    ButtonData keyData = new ButtonData("1");
                    keyData.buttonPressState = ButtonPressState.Pressed;
                    _buttonDataDic.Add("1", keyData);
                }
            }
            else if (UInput.GetMouseButtonUp(1))
            {
                if (_buttonDataDic.ContainsKey("1"))
                {
                    _buttonDataDic["1"].buttonPressState = ButtonPressState.Released;
                }
            }

            if (UInput.GetMouseButtonDown(2))
            {
                if (_buttonDataDic.ContainsKey("2"))
                {
                    _buttonDataDic["2"].buttonPressState = ButtonPressState.Pressed;
                }
                else
                {
                    ButtonData keyData = new ButtonData("2");
                    keyData.buttonPressState = ButtonPressState.Pressed;
                    _buttonDataDic.Add("2", keyData);
                }
            }
            else if (UInput.GetMouseButtonUp(1))
            {
                if (_buttonDataDic.ContainsKey("2"))
                {
                    _buttonDataDic["2"].buttonPressState = ButtonPressState.Released;
                }
            }
        }

        /// <summary>
        /// 处理轴事件
        /// </summary>
        private void ProcessAxis()
        {
            for(int i=0;i<_axisNames.Length;i++)
            {
                if (String.IsNullOrEmpty(_axisNames[i])) continue;
                if (_axisDataDic.ContainsKey(_axisNames[i]))
                {
                    _axisDataDic[_axisNames[i]].axisValue = UInput.GetAxis(_axisNames[i]);
                }
                else
                {
                    AxisData axisData = new AxisData(_axisNames[i]);
                    axisData.axisValue = UInput.GetAxis(_axisNames[i]);
                    _axisDataDic.Add(_axisNames[i], axisData);
                }
            }
        }

        /// <summary>
        /// 处理AxisRaw事件
        /// </summary>
        private void ProcessAxisRaw()
        {
            for (int i = 0; i < _axisNames.Length; i++)
            {
                if (String.IsNullOrEmpty(_axisNames[i])) continue;
                if (_rawAxisDataDic.ContainsKey(_axisNames[i]))
                {
                    _rawAxisDataDic[_axisNames[i]].axisValue = UInput.GetAxisRaw(_axisNames[i]);
                }
                else
                {
                    AxisData axisData = new AxisData(_axisNames[i]);
                    axisData.axisValue = UInput.GetAxisRaw(_axisNames[i]);
                    _rawAxisDataDic.Add(_axisNames[i], axisData);
                }  
            }
        }

        /// <summary>
        /// 激活输入源，向Input注册输入源
        /// </summary>
        public override void ActivateInputSource()
        {
            _baseInput = StandaloneInput.Instance();
            _baseInput?.inputSourceCantainer.AddInputSource(this);
        }

        /// <summary>
        /// 更新函数
        /// </summary>
        private void Update()
        {
            if(_baseInput == null)
            {
                _baseInput = StandaloneInput.Instance();
                _baseInput?.inputSourceCantainer.AddInputSource(this);
            }
        }
        /// <summary>
        /// 停止使用外设输入
        /// </summary>
        public override void DeactivateInputSource()
        {
            _baseInput?.inputSourceCantainer.RemoveInputSource(this);
            _baseInput = null;
        }
    }
}
