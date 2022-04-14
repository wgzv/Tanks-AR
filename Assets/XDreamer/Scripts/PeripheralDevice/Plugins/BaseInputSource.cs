///////////////////////////////////////////////////////////////////////////////////////////////////////////////
///InputSource用以处理Unity中所有交互事件
///
///1. 按键类操作，返回按键的操作（按下，弹起事件）
///   GetButton：
///   GetButtonDown：
///   GetButtonUp：
///   GetAxis：
///   GetRawAxis：
///   GetKey：
///   GetKeyDown：
///   GetKeyUp：
///   GetMouseButton：
///   GetMouseButtonDown：
///   GetMouseButtonUp：
///   GetTriggerValue：  Pico手柄接口
///   GetTouchPadPosition：  Pico手柄接口
///2. MonoBehavior 事件， 用gameObject.SendMessage()实现
///   OnMouseEnter：当鼠标进入GUI元素或Collider时被触发，每次进入只会出发一次
///   OnMouseOver：当鼠标悬浮在GUI元素或Collider时被触发，若没离开则每一帧被触发一次
///   OnMouseExit：当鼠标离开GUI元素或Collider时被触发
///   OnMouseDown：当鼠标在GUI元素或Collider上点击时触发
///   OnMouseUp：当释放鼠标时调用
///   OnMouseDrag：当鼠标拖拽GUI元素或Collider时触发 
///   OnMouseUpAsButton：当鼠标在同一个GUI元素或Collider上点击并释放时触发
///3. EventTrigger 事件，用ExecuteEvents.Execute(gameObject, currentPointerData, ExecuteEvents)来调用
///   OnPointerEnter(PointerEventData): 当“鼠标”进入UGUI元素或Collider时被触发，每次进入只会出发一次
///   OnPointerExit(PointerEventData): 当“鼠标”离开UGUI元素或Collider时被触发
///   OnPointerDown(PointerEventData): 当“鼠标”在GUI元素或Collider上点击时触发
///   OnPointerUp(PointerEventData): 当释放“鼠标”时调用
///   OnPointerClick(PointerEventData): 当“鼠标”在GUI元素或Collider上点击时触发
///   OnDrag(PointerEventData): 当“鼠标”在GUI元素或Collider上拖动时触发
///   OnDrop(PointerEventData): 当对象接受丢弃时触发
///   OnScroll(PointerEventData): 当Scroll事件发生时触发
///   OnUpdateSelected(PointerEventData): 更新与此EventTrigger关联的对象时触发
///   OnSelect(PointerEventData): 当Select事件发生时触发
///   OnDeselect(PointerEventData): 在选择新对象时触发
///   OnMove(PointerEventData): 当Move事件发生时触发
///   OnInitializePotentialDrag(PointerEventData): 在找到拖动对象时，但在有效开始拖动之前触发
///   OnBeginDrag(PointerEventData): 在拖动开始之前触发
///   OnEndDrag(PointerEventData): 拖动结束后触发
///   OnSubmit(PointerEventData): 发生Submit事件时触发
///   OnCancel(PointerEventData): 发生Cancel事件时触发
/// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginPeripheralDevice.Raycasters;
using BaseInputModule = XCSJ.PluginPeripheralDevice.Modules.BaseInputModule;
using Physics2DRaycaster = XCSJ.PluginPeripheralDevice.Raycasters.Physics2DRaycaster;
using PhysicsRaycaster = XCSJ.PluginPeripheralDevice.Raycasters.PhysicsRaycaster;

namespace XCSJ.PluginPeripheralDevice
{
    /// <summary>
    /// 基础输入源
    /// </summary>
    [RequireManager(typeof(PeripheralDeviceInputManager))]
    public abstract class BaseInputSource : MB, IProcess
    {
        #region IProcess
        public virtual void Process()
        {
            //处理函数，添加函数，处理按键，鼠标点击等事件处理
        }
        #endregion IProcess

        /// <summary>
        /// 输入
        /// </summary>
        protected BaseInput _baseInput;
        public BaseInput baseInput
        {
            get { return _baseInput; }
            set { _baseInput = value; }
        }

        /// <summary>
        /// 输入的射线容器
        /// </summary>
        private RaycasterContainer _raycasterCantainer;
        public RaycasterContainer raycasterCantainer => _raycasterCantainer;

        /// <summary>
        /// 发送Unity默认事件
        /// </summary>
        [Name("发送Unity默认事件")]
        [SerializeField]
        private bool _sendUnity = false;
        public bool sendUnity => _sendUnity;

        /// <summary>
        /// BaseInputModule,用以处理一系列事件
        /// </summary>
        protected BaseInputModule _baseInputModule;
        public BaseInputModule GetBaseInputModule()
        {
            return _baseInputModule;
        }

        #region Unity 函数

        /// <summary>
        /// 设备ID，用以区分不同的设备
        /// </summary>
        [Name("设备ID")]
        public int deviceIndex = 0;

        #region 射线类信息

        [SerializeField]
        [Name("自定义射线检测")]
        private bool _useDefinedRay = false;
        public bool useDefinedRay { get => _useDefinedRay; set => UseDefinedRay(value); }


        [Name("显示射线")]
        [SerializeField]
        private bool _showRay = false;
        public bool showRay { get => _showRay; set => ShowRay(value); }

        /// <summary>
        /// 射线渲染展示
        /// </summary>
        protected BaseRayRenderer _rayRenderer;
        public BaseRayRenderer rayRenderer => _rayRenderer;

        [SerializeField]
        [Name("相机射线检测")]
        private bool _isCamera = false;
        public virtual bool isCamera { get => _isCamera; set => _isCamera = value; }

        [SerializeField]
        [Name("事件相机")]
        private Camera _eventCamera;
        public virtual Camera eventCamera { get => _eventCamera != null ? _eventCamera : Camera.main; set => _eventCamera = value; }

        [SerializeField]
        [Name("射线原点")]
        private Transform _origin;
        public virtual Transform origin { get => _origin ? _origin : _origin = transform; set => _origin = value; }

        [SerializeField]
        [Name("射线长度")]
        protected float _rayLenth = 100;
        public float rayLenth => _rayLenth;

        [SerializeField]
        [Name("射线检测距离")]
        protected float _rayDistance = 1000;
        public float rayDistance { get => _rayDistance; set => _rayDistance = value; }

        [SerializeField]
        [Name("Canvas画布")]
        private Canvas _canvas;
        public Canvas canvas => _canvas ? _canvas : _canvas = GameObject.FindObjectOfType<Canvas>();

        #endregion 射线类信息

        /// <summary>
        /// 按键类数据
        /// </summary>
        protected Dictionary<string, ButtonData> _buttonDataDic = new Dictionary<string, ButtonData>();
        public Dictionary<string, ButtonData> buttonDataDictionary  => _buttonDataDic; 

        /// <summary>
        /// 键值类数据
        /// </summary>
        protected Dictionary<string, KeyData> _keyDataDic = new Dictionary<string, KeyData>();
        public Dictionary<string, KeyData> keyDataDictionary => _keyDataDic;

        /// <summary>
        /// Axis数据
        /// </summary>
        protected Dictionary<string, AxisData> _axisDataDic = new Dictionary<string, AxisData>();
        public Dictionary<string, AxisData> axisDataDictioanry => _axisDataDic;

        /// <summary>
        /// RawAxis数据
        /// </summary>
        protected Dictionary<string, AxisData> _rawAxisDataDic = new Dictionary<string, AxisData>();
        public Dictionary<string, AxisData> rawAxisDataDictioanry => _rawAxisDataDic;

        /// <summary>
        /// Awake函数,唤醒时执行
        /// </summary>
        protected virtual void Awake()
        {
            _raycasterCantainer = new RaycasterContainer(this);
            _rayRenderer = GetComponent<BaseRayRenderer>();
            _rayRenderer?.InitRayLine(_showRay);

            if (_useDefinedRay) UseDefinedRay(_useDefinedRay);
        }

        /// <summary>
        /// OnEnable函数，可用时执行
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            ActivateInputSource();
        }

        /// <summary>
        /// OnDisable函数，不可用时执行
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            DeactivateInputSource();
        }

        /// <summary>
        /// 激活外设输入
        /// </summary>
        public virtual void ActivateInputSource()
        {
            _baseInput = PeripheralDeviceInputManager.Instance().currentPeripheralDevice;
            _baseInput?.inputSourceCantainer.AddInputSource(this);
        }

        /// <summary>
        /// 停止使用外设输入
        /// </summary>
        public virtual void DeactivateInputSource()
        {
            _baseInput?.inputSourceCantainer.RemoveInputSource(this);
            _baseInput = null;
        }

        /// <summary>
        /// 绘制射线
        /// </summary>
        /// <param name="distance"></param>
        public virtual void DrawLine(float distance)
        {
            
        }

        /// <summary>
        /// 使用自定义射线
        /// </summary>
        /// <param name="useDefined">是否自定义</param>
        protected virtual void UseDefinedRay(bool useDefined)
        {
            _useDefinedRay = useDefined;
            //PhysicsRaycaster physicsRaycaster = GetOrAddComponent<PhysicsRaycaster>();
            //physicsRaycaster.InitRayCaster(_isCamera);
            //physicsRaycaster.enabled = useDefined;

            //Physics2DRaycaster physics2DRaycaster = GetOrAddComponent<Physics2DRaycaster>();
            //physics2DRaycaster.InitRayCaster(_isCamera);
            //physicsRaycaster.enabled = useDefined;

            //GraphicRaycaster graphicRaycaster = GetOrAddComponent<GraphicRaycaster>();
            //graphicRaycaster.InitRayCaster(_isCamera);
            //graphicRaycaster.enabled = useDefined;
            Raycasters.BaseRaycaster[] baseRaycasters = GetComponents<Raycasters.BaseRaycaster>();
            for (int i = 0; i < baseRaycasters.Length; i++)
            {
                baseRaycasters[i].InitRayCaster(_isCamera);
            }
        }

        /// <summary>
        /// 显示射线
        /// </summary>
        /// <param name="showray">是否显示</param>
        protected virtual void ShowRay(bool showray)
        {
            _showRay = showray;
            _rayRenderer?.ShowRayLine(_showRay);
        }

        /// <summary>
        /// 获取或者添加组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private T GetOrAddComponent<T>() where T : MB
        {
            T t = GetComponent<T>();
            if (t) return t;
            else
            {
                t = gameObject.AddComponent<T>();
                return t;
            }
        }

        /// <summary>
        /// 处理射线事件
        /// </summary>
        protected virtual void ProcessPoniterEvent()
        {

        }

        /// <summary>
        /// 预处理按键事件
        /// </summary>
        protected virtual void PreProcessKey()
        {
            //处理按钮的状态
            foreach (var button in _buttonDataDic)
            {
                button.Value.ProcessData();
            }

            //处理按键的状态
            foreach (var key in _keyDataDic)
            {
                key.Value.ProcessData();
            }
        }

        /// <summary>
        /// 删除无效健值
        /// </summary>
        protected virtual void DeleteInvalidKey()
        {
            List<string> deleteKeys = new List<string>();
            //删除无效按钮
            foreach (var button in _buttonDataDic)
            {
                if (button.Value.buttonPressState == ButtonPressState.None)
                    deleteKeys.Add(button.Key);
            }
            for (int i = 0; i < deleteKeys.Count; i++)
            {
                _buttonDataDic.Remove(deleteKeys[i]);
            }

            deleteKeys.Clear();
            //删除无效按键
            foreach (var key in _keyDataDic)
            {
                if (key.Value.buttonPressState == ButtonPressState.None)
                    deleteKeys.Add(key.Key);
            }
            for (int i = 0; i < deleteKeys.Count; i++)
            {
                _keyDataDic.Remove(deleteKeys[i]);
            }

            deleteKeys.Clear();
            //删除无效轴
            foreach (var axis in _axisDataDic)
            {
                if (Mathf.Approximately(axis.Value.axisValue, 0.0f))
                    deleteKeys.Add(axis.Key);
            }
            for (int i = 0; i < deleteKeys.Count; i++)
            {
                _axisDataDic.Remove(deleteKeys[i]);
            }

            deleteKeys.Clear();
            //删除无效轴
            foreach (var rawAxis in _rawAxisDataDic)
            {
                if (Mathf.Approximately(rawAxis.Value.axisValue, 0.0f))
                    deleteKeys.Add(rawAxis.Key);
            }
            for (int i = 0; i < deleteKeys.Count; i++)
            {
                _rawAxisDataDic.Remove(deleteKeys[i]);
            }
        }

        #endregion Unity 函数
    }
}
