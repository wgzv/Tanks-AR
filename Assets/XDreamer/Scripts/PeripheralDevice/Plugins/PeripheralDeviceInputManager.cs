using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.PluginPeripheralDevice
{
    /// <summary>
    /// 外部设备输入:可用于管理外部设备(简称外设)输入；主要基于Unity新版输入系统与旧版输入管理器实现；
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [ComponentKit(EKit.Peripheral)]
    [ComponentOption(EComponentOptionType.Core)]
    [Name(PeripheralDeviceInputHelper.Title)]
    [Tip("可用于管理外部设备(简称外设)输入；主要基于Unity新版输入系统与旧版输入管理器实现；")]
    [Guid("7D480A49-79BE-42C7-81B7-425D5DFF44FE")]
    [Version("22.301")]
    public class PeripheralDeviceInputManager : BaseManager<PeripheralDeviceInputManager>
    {
        /// <summary>
        /// 获取脚本
        /// </summary>
        /// <returns>脚本列表</returns>
        public override List<Script> GetScripts()
        {
            return Script.GetScriptsOfEnum<EScriptID>(this);
        }

        /// <summary>
        /// 运行脚本
        /// </summary>
        /// <param name="id">脚本ID</param>
        /// <param name="param">脚本参数</param>
        /// <returns>ReturnValue</returns>
        public override ReturnValue RunScript(int id, ScriptParamList param)
        {
            switch ((EScriptID)id)
            {
                case EScriptID.GetButton:
                    {
                        string buttonName = param[1] as string;
                        return GetButton(buttonName, EButtonEvent.Both) ? ReturnValue.Yes : ReturnValue.No;
                    }
                case EScriptID.GetPeripheralDeviceButton:
                    {
                        string deviceName = param[1] as string;
                        string buttonName = param[2] as string;
                        return GetButton(deviceName, buttonName, EButtonEvent.Both) ? ReturnValue.Yes : ReturnValue.No;
                    }
                case EScriptID.GetButtonDown:
                    {
                        string buttonName = param[1] as string;
                        return GetButton(buttonName, EButtonEvent.Down) ? ReturnValue.Yes : ReturnValue.No;
                    }
                case EScriptID.GetPeripheralDeviceButtonDown:
                    {
                        string deviceName = param[1] as string;
                        string buttonName = param[2] as string;
                        return GetButton(deviceName, buttonName, EButtonEvent.Down) ? ReturnValue.Yes : ReturnValue.No;
                    }
                case EScriptID.GetButtonUp:
                    {
                        string buttonName = param[1] as string;
                        return GetButton(buttonName, EButtonEvent.Up) ? ReturnValue.Yes : ReturnValue.No;
                    }
                case EScriptID.GetPeripheralDeviceButtonUp:
                    {
                        string deviceName = param[1] as string;
                        string buttonName = param[2] as string;
                        return GetButton(deviceName, buttonName, EButtonEvent.Up) ? ReturnValue.Yes : ReturnValue.No;
                    }
                case EScriptID.GetPeripheralDeviceAxisValue:
                    {
                        string deviceName = param[1] as string;
                        string axisName = param[2] as string;
                        return new ReturnValue(true, GetAxis(deviceName, axisName));
                    }
                case EScriptID.GetRayOrgin:
                    {
                        var go = param[1] as GameObject;
                        BaseInputSource baseInputSource = go.GetComponent<BaseInputSource>();
                        if (baseInputSource)
                        {
                            return new ReturnValue(true, baseInputSource.origin.position);
                        }
                        return ReturnValue.No;
                    }
                case EScriptID.GetRayDirection:
                    {
                        var go = param[1] as GameObject;
                        BaseInputSource baseInputSource = go.GetComponent<BaseInputSource>();
                        if (baseInputSource)
                        {
                            return new ReturnValue(true, baseInputSource.origin.forward);
                        }
                        return ReturnValue.No;
                    }
                case EScriptID.GetRayEndPoint:
                    {
                        var go = param[1] as GameObject;
                        BaseInputSource baseInputSource = go.GetComponent<BaseInputSource>();
                        if (baseInputSource)
                        {
                            return new ReturnValue(true, baseInputSource.origin.position + baseInputSource.origin.forward * baseInputSource.rayLenth);
                        }
                        return ReturnValue.No;
                    }
                case EScriptID.GetRayLenth:
                    {
                        var go = param[1] as GameObject;
                        BaseInputSource baseInputSource = go.GetComponent<BaseInputSource>();
                        if(baseInputSource)
                        {
                            return new ReturnValue(true, baseInputSource.rayLenth);
                        }
                        return ReturnValue.No;
                    }
                case EScriptID.SetRayShowOrHide:
                    {
                        var go = param[1] as GameObject;
                        var show = (EBool)param[2];
                        BaseInputSource baseInputSource = go.GetComponent<BaseInputSource>();
                        if (baseInputSource)
                        {
                            if (show == EBool.Yes)
                                baseInputSource.showRay = true;
                            else if (show == EBool.No)
                                baseInputSource.showRay = false;
                            else if (show == EBool.Switch)
                                baseInputSource.showRay = !baseInputSource.showRay;
                            return ReturnValue.Yes;
                        }
                        return ReturnValue.No;
                    }
            }
            return ReturnValue.No;
        }

        #region Unity事件

        /// <summary>
        /// 输入设备列表
        /// </summary>
        private List<BaseInput> _peripheralDevices = new List<BaseInput>();

        /// <summary>
        /// 当前正在使用的外设配置
        /// </summary>
        private BaseInput _currentPeripheralDevice;
        public BaseInput currentPeripheralDevice => _currentPeripheralDevice;

        /// <summary>
        /// 外设列表中添加新设备
        /// </summary>
        /// <param name="baseInput"></param>
        public void AddPeripheralDeviceInput(BaseInput baseInput)
        {
            if (_peripheralDevices.Exists(device => device == baseInput)) return;
            _peripheralDevices.Add(baseInput);
        }

        /// <summary>
        /// 外射列表中移除某个设备
        /// </summary>
        /// <param name="baseInput">输入设备</param>
        public void RemovePeripheralDeviceInput(BaseInput baseInput)
        {
            if (_peripheralDevices.Exists(device => device == baseInput)) _peripheralDevices.Remove(baseInput);
        }

        /// <summary>
        /// 更改当前使用的外设
        /// </summary>
        /// <param name="input">输入设备</param>
        private void ChangeCurrentInput(BaseInput baseInput)
        {
            if (_currentPeripheralDevice == baseInput)
                return;

            if (_currentPeripheralDevice != null)
                _currentPeripheralDevice.DeactivateInput();

            if (baseInput != null)
                baseInput.ActivateInput();
            _currentPeripheralDevice = baseInput;
        }

        /// <summary>
        /// Awake
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
            if (eventSystem == null)
            {
                eventSystem = new GameObject("EventSystem").AddComponent<EventSystem>();
                eventSystem.gameObject.AddComponent<StandaloneInputModule>();
            }
        }

        /// <summary>
        /// 更新函数
        /// </summary>
        public override void Update()
        {
            //动态捕捉当前的外设输入模块
            bool changedInput = false;
            for (var i = 0; i < _peripheralDevices.Count; i++)
            {
                var input = _peripheralDevices[i];
                if (input.IsInputSupported() && input.ShouldActivateInput())
                {
                    if (_currentPeripheralDevice != input)
                    {
                        ChangeCurrentInput(input);
                        changedInput = true;
                    }
                    break;
                }
            }

            if (_currentPeripheralDevice == null)
            {
                for (var i = 0; i < _peripheralDevices.Count; i++)
                {
                    var input = _peripheralDevices[i];
                    if (input.IsInputSupported())
                    {
                        ChangeCurrentInput(input);
                        changedInput = true;
                        break;
                    }
                }
            }

            if (!changedInput && _currentPeripheralDevice != null)
                _currentPeripheralDevice.Process();
        }

        #endregion Unity事件

        #region 脚本函数

        public enum EButtonEvent
        {
            /// <summary>
            /// 所有事件
            /// </summary>
            Both,

            /// <summary>
            /// 按下事件
            /// </summary>
            Down,

            /// <summary>
            /// 弹起事件
            /// </summary>
            Up
        }

        /// <summary>
        /// 获取按键状态
        /// </summary>
        /// <param name="deviceName">设备名称</param>
        /// <param name="buttonName">按键名称</param>
        /// <param name="eButtonEvent">按键事件状态</param>
        /// <returns>布尔值</returns>
        public bool GetButton(string deviceName , string buttonName, EButtonEvent eButtonEvent)
        {
            if (string.IsNullOrEmpty(deviceName) || string.IsNullOrEmpty(buttonName)) return false;
            //添加Validate判断，预防在扩展子类中忘记加入有效性判断
            if (_currentPeripheralDevice.deviceName != deviceName) return false;
            switch(eButtonEvent)
            {
                case EButtonEvent.Both:
                    return _currentPeripheralDevice.GetButton(buttonName);
                case EButtonEvent.Down:
                    return _currentPeripheralDevice.GetButtonDown(buttonName);
                case EButtonEvent.Up:
                    return _currentPeripheralDevice.GetButtonUp(buttonName);
                default:
                    return false;
            }
        }

        /// <summary>
        /// 获取当前设备按键状态
        /// </summary>
        /// <param name="buttonName">按键名称</param>
        /// <param name="eButtonEvent">按键事件状态</param>
        /// <returns>布尔值</returns>
        public bool GetButton(string buttonName, EButtonEvent eButtonEvent)
        {
            if (string.IsNullOrEmpty(buttonName)) return false;

            switch (eButtonEvent)
            {
                case EButtonEvent.Both:
                    {
                        if (_currentPeripheralDevice.GetButton(buttonName)) return true;
                        break;
                    }
                case EButtonEvent.Down:
                    {
                        if (_currentPeripheralDevice.GetButtonDown(buttonName) ) return true;
                        break;
                    }
                case EButtonEvent.Up:
                    {
                        if (_currentPeripheralDevice.GetButtonUp(buttonName)) return true;
                        break;
                    }
            }

            return false;
        }

        /// <summary>
        /// 获取某个轴的数值
        /// </summary>
        /// <param name="deviceName">设备名称</param>
        /// <param name="axisName">轴名称</param>
        /// <returns>浮点数，轴对应的数值</returns>
        public float GetAxis(string deviceName, string axisName)
        {
            if (string.IsNullOrEmpty(deviceName) || string.IsNullOrEmpty(axisName)) return 0;
            if (_currentPeripheralDevice.deviceName != deviceName) return 0;
            return _currentPeripheralDevice.GetAxis(axisName);
        }

        #endregion 脚本函数
    }


}
