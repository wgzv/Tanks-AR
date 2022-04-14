using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginPeripheralDevice.Raycasters
{
    /// <summary>
    /// 基础Raycaster类，模仿UnityEngine.EventSystems.BaseRaycaster，处理Raycast
    /// </summary>
    [RequireManager(typeof(PeripheralDeviceInputManager))]
    public abstract class BaseRaycaster : MB
    {
        public abstract void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList);

        /// <summary>
        /// 是否是相机射线检测
        /// </summary>
        [SerializeField]
        [Name("相机射线检测")]
        private bool _isCamera = false;
        public virtual bool isCamera { get => _isCamera; set => _isCamera = value; }

        /// <summary>
        /// 射线原点
        /// </summary>
        [SerializeField]
        [Name("射线原点")]
        protected Transform _origin;
        public virtual Transform origin { get => _origin ? _origin : _origin = transform; set => _origin = value; }

        /// <summary>
        /// 事件相机
        /// </summary>
        protected Camera _eventCamera;
        public virtual Camera eventCamera { get => _eventCamera != null ? _eventCamera : Camera.main; set => _eventCamera = value; }

        /// <summary>
        /// 输入源
        /// </summary>
        protected BaseInputSource _baseInputSource;
        public virtual BaseInputSource baseInputSource
        {
            get => _baseInputSource != null ? _baseInputSource : _baseInputSource = GetComponent<BaseInputSource>();
            set => _baseInputSource = value;
        }

        /// <summary>
        /// 射线检测
        /// </summary>
        private Raycaster _raycaster;

        /// <summary>
        /// 获取射线检测
        /// </summary>
        /// <returns></returns>
        public Raycaster Raycaster()
        {
            if (_raycaster == null)
                _raycaster = GetComponent<Raycaster>();
            if (_raycaster == null)
                _raycaster = gameObject.AddComponent<Raycaster>();
            _raycaster.SetRaycasterCamera(eventCamera);
            return _raycaster;
        }

        #region Unity 函数

        /// <summary>
        /// 可用时执行
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            baseInputSource?.raycasterCantainer?.AddRaycaster(this);
        }

        /// <summary>
        /// 不可用时执行
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            baseInputSource?.raycasterCantainer?.RemoveRaycasters(this);
        }

        /// <summary>
        /// 是否激活
        /// </summary>
        /// <returns>布尔值</returns>
        public virtual bool IsActive()
        {
            return gameObject.activeInHierarchy;
        }

        /// <summary>
        /// 初始化射线检测
        /// </summary>
        /// <param name="isCam">是否相机检测</param>
        public virtual void InitRayCaster(bool isCam = false)
        {

        }

        #endregion Unity 函数
    }
}
