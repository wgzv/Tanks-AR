////////////////////////////////////////////////////////////////////////////////////////////
/// 按钮，按键，轴 无差别，只要某个InputSource的按钮，按键，轴有操作，即返回值
/// Input 可以对应多个 InputSource，例如HTC 对应两个手柄
/// 
///
////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginPeripheralDevice
{
    /// <summary>
    /// 输入外设基础类
    /// </summary>
    [RequireManager(typeof(PeripheralDeviceInputManager))]
    public abstract class BaseInput : MB, IInfo, IInput, IHapticPulse, IProcess
    {
        #region IInfo
        /// <summary>
        /// 继承的类，直接命名，表述输入的类型
        /// </summary>
        public virtual string deviceName { get => string.Format("外设"); }

        #endregion IInfo

        #region IInput

        public virtual float GetAxis(string axisName)
        {
            var inputSources = _inputSourceCantainer?.GetInputSources();
            for(int i=0;i<inputSources.Count;i++)
            {
                if (inputSources[i].axisDataDictioanry.ContainsKey(axisName))
                {
                    return inputSources[i].axisDataDictioanry[axisName].axisValue;
                }
            }
            return 0;
        }

        public virtual float GetRawAxis(string axisName)
        {
            var inputSources = _inputSourceCantainer?.GetInputSources();
            for (int i = 0; i < inputSources.Count; i++)
            {
                if (inputSources[i].rawAxisDataDictioanry.ContainsKey(axisName))
                {
                    return inputSources[i].rawAxisDataDictioanry[axisName].axisValue;
                }
            }
            return 0;
        }

        public virtual bool GetButton(string buttonName)
        {
            var inputSources = _inputSourceCantainer?.GetInputSources();
            for (int i = 0; i < inputSources.Count; i++)
            {
                if (inputSources[i].buttonDataDictionary.ContainsKey(buttonName))
                {
                    return true;
                }
            }
            return false;
        }

        public virtual bool GetButtonDown(string buttonName)
        {
            var inputSources = _inputSourceCantainer?.GetInputSources();
            for (int i = 0; i < inputSources.Count; i++)
            {
                if (inputSources[i].buttonDataDictionary.ContainsKey(buttonName))
                {
                    return inputSources[i].buttonDataDictionary[buttonName].buttonPressState == ButtonPressState.Pressed;
                }
            }
            return false;
        }

        public virtual bool GetButtonUp(string buttonName)
        {
            var inputSources = _inputSourceCantainer?.GetInputSources();
            for (int i = 0; i < inputSources.Count; i++)
            {
                if (inputSources[i].buttonDataDictionary.ContainsKey(buttonName))
                {
                    return inputSources[i].buttonDataDictionary[buttonName].buttonPressState == ButtonPressState.Released;
                }
            }
            return false;
        }

        public virtual bool GetKey(string keyName)
        {
            var inputSources = _inputSourceCantainer?.GetInputSources();
            for (int i = 0; i < inputSources.Count; i++)
            {
                if (inputSources[i].keyDataDictionary.ContainsKey(keyName))
                {
                    return true;
                }
            }
            return false;
        }

        public virtual bool GetKey(KeyCode key)
        {
            return GetKey(key.ToString());
        }

        public virtual bool GetKeyDown(string keyName)
        {
            var inputSources = _inputSourceCantainer?.GetInputSources();
            for (int i = 0; i < inputSources.Count; i++)
            {
                if (inputSources[i].keyDataDictionary.ContainsKey(keyName))
                {
                    return inputSources[i].keyDataDictionary[keyName].buttonPressState == ButtonPressState.Pressed;
                }
            }
            return false;
        }

        public virtual bool GetKeyDown(KeyCode key)
        {
            return GetKeyDown(key.ToString());
        }

        public virtual bool GetKeyUp(string keyName)
        {
            var inputSources = _inputSourceCantainer?.GetInputSources();
            for (int i = 0; i < inputSources.Count; i++)
            {
                if (inputSources[i].keyDataDictionary.ContainsKey(keyName))
                {
                    return inputSources[i].keyDataDictionary[keyName].buttonPressState == ButtonPressState.Released;
                }
            }
            return false;
        }

        public virtual bool GetKeyUp(KeyCode key)
        {
            return GetKeyUp(key.ToString());
        }

        public virtual bool GetMouseButton(int button)
        {
            return GetButton(button.ToString());
        }

        public virtual bool GetMouseButtonDown(int button)
        {
            return GetButtonDown(button.ToString());
        }

        public virtual bool GetMouseButtonUp(int button)
        {
            return GetButtonUp(button.ToString());
        }

        public virtual float GetTriggerValue(string triggerName)
        {
            return GetAxis(triggerName);
        }
        
        #endregion IInput

        #region IHapticPulse

        public virtual void TriggerHapticPulse(float amplitude, float duration)
        {
            
        }

        #endregion IHapticPulse

        #region IProcess

        /// <summary>
        /// 外设事件处理函数
        /// </summary>
        public virtual void Process()
        {
            var inputResources = _inputSourceCantainer.GetInputSources();
            for(int i=0; i < inputResources.Count; i++)
            {
                inputResources[i].Process();
            }
            ProcessPointerEvents();
        }

        #endregion IProcess

        #region Unity函数

        /// <summary>
        /// 输入的InputSource容器
        /// </summary>
        protected InputSourceContainer _inputSourceCantainer;
        public InputSourceContainer inputSourceCantainer => _inputSourceCantainer;

        private static BaseInput _instance;

        public static BaseInput Instance()
        {
            //if(!_instance)
            //{
            //    _instance = GameObject.FindObjectOfType<BaseInput>();
            //}
            return _instance;
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this && Application.isEditor && !Application.isPlaying)
            {
                Debug.LogError("重复定义: " +  GetType() + " 组件只可以被实例化一次！执行强制删除！");
                DestroyImmediate(this);
                return;
            }
            _inputSourceCantainer = new InputSourceContainer(this);
        }

        protected virtual void Start()
        {
            
        }

        protected virtual void Update()
        {
            
        }

        public override void OnEnable()
        {
            base.OnEnable();
            ActivateInput();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            DeactivateInput();
        }

        /// <summary>
        /// 激活外设输入
        /// </summary>
        public virtual void ActivateInput()
        {
            if (PeripheralDeviceInputManager.Instance()) PeripheralDeviceInputManager.Instance().AddPeripheralDeviceInput(this);
        }

        /// <summary>
        /// 停止使用外设输入
        /// </summary>
        public virtual void DeactivateInput()
        {
            if (PeripheralDeviceInputManager.Instance()) PeripheralDeviceInputManager.Instance().RemovePeripheralDeviceInput(this);
        }

        /// <summary>
        /// 检查外设输入是否支持，检测设备是否连接到系统
        /// </summary>
        /// <returns></returns>
        public virtual bool IsInputSupported()
        {
            return true;
        }

        /// <summary>
        /// 是否激活外设管理器, 可根据需要设定
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldActivateInput()
        {
            return enabled && gameObject.activeInHierarchy;
        }

        /// <summary>
        /// 处理Unity EventTrigger事件
        /// </summary>
        protected virtual void ProcessPointerEvents()
        {
            var inputSources = _inputSourceCantainer?.GetInputSources();
            for (int i = 0; i < inputSources.Count; i++)
            {
                
                var inputModule = inputSources[i].GetBaseInputModule();
                if (inputModule == null) continue;
                var pointerEvents = inputModule.pointerEvents;
                for (int j = 0; j < pointerEvents.Count; j++)
                {
                    if (inputSources[i].sendUnity) 
                        pointerEvents[j].ProcessPointerEvent();
                    //Debug.Log(pointerEvents[j].ToString());
                }
            }
        }

        #endregion Unity函数
    }
}
