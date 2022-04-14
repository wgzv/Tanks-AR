using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.CNScripts;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;

namespace XCSJ.PluginsCameras.Legacies.CNScripts
{
    /// <summary>
    /// 锁定相机(UGUI专用)脚本事件
    /// </summary>
    [Serializable]
    [RequireComponent(typeof(RectTransform))]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.UGUIMenu + Title)]
    [Obsolete("产品功能升级，不推荐用户再使用本功能组件，请使用新的功能组件替代！", false)]
    public class LockCameraForUGUIScriptEvent : BaseScriptEvent<ELockCameraForUGUIScriptEventType, LockCameraForUGUIScriptEventSet>, IPointerUpHandler, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "锁定相机(UGUI专用)脚本事件";

        /// <summary>
        /// 锁定相机
        /// </summary>
        public BaseCamera lockedCamera { get; private set; }

        /// <summary>
        /// 临时锁定相机
        /// </summary>
        public bool tmpLockCamera { get; private set; }

        private int lockCount = 0;

        private void LockCameraWithCount()
        {
            if (++lockCount > 1) return;//1+次锁定，不执行锁定操作，因为已经锁定了！

            LockCamera();
        }

        private void LockCamera()
        {
            if (CameraManager.instance) lockedCamera = CameraManager.instance.GetCurrentCamera();
            if (lockedCamera)
            {
                tmpLockCamera = lockedCamera.lockCamera;
                lockedCamera.lockCamera = true;
            }
        }

        private void UnlockCameraWithCount()
        {
            if (--lockCount > 0) return;

            UnlockCamera();
        }

        private void UnlockCamera()
        {
            if (lockedCamera)
            {
                lockedCamera.lockCamera = tmpLockCamera;
                lockedCamera = null;
            }
        }

        private void OnCameraChanged()
        {
            if (lockCount > 0)
            {
                //对旧相机执行解锁操作
                if (lockedCamera) lockedCamera.lockCamera = tmpLockCamera;

                LockCamera();
            }
        }

        /// <summary>
        /// 启动
        /// </summary>
        public override void Start()
        {
            RunScriptEvent(ELockCameraForUGUIScriptEventType.Start);
        }

        /// <summary>
        /// 启用时
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            LegacyCameraManagerProvider.onChangedCurrentCamera += OnCameraChanged;
        }

        /// <summary>
        /// 禁用时
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            LegacyCameraManagerProvider.onChangedCurrentCamera -= OnCameraChanged;
        }

        /// <summary>
        /// 当指针按下
        /// </summary>
        /// <param name="data"></param>
        public void OnPointerDown(PointerEventData data)
        {
            LockCameraWithCount();

            RunScriptEvent(ELockCameraForUGUIScriptEventType.OnPointerDown);
        }

        /// <summary>
        /// 当指针抬起
        /// </summary>
        /// <param name="data"></param>
        public void OnPointerUp(PointerEventData data)
        {
            UnlockCameraWithCount();

            RunScriptEvent(ELockCameraForUGUIScriptEventType.OnPointerUp);
        }

        /// <summary>
        /// 当开始拖拽
        /// </summary>
        /// <param name="eventData"></param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            LockCameraWithCount();

            RunScriptEvent(ELockCameraForUGUIScriptEventType.OnBeginDrag);
        }

        /// <summary>
        /// 当结束拖拽
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            UnlockCameraWithCount();

            RunScriptEvent(ELockCameraForUGUIScriptEventType.OnEndDrag);
        }

        /// <summary>
        /// 当拖拽中
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            RunScriptEvent(ELockCameraForUGUIScriptEventType.OnDrag);
        }
    }

    /// <summary>
    /// 锁定相机(UGUI专用)脚本事件类型
    /// </summary>
    public enum ELockCameraForUGUIScriptEventType
    {
        /// <summary>
        /// 启动时执行
        /// </summary>
        [Name("启动时执行")]
        Start,

        /// <summary>
        /// 指针按下时执行
        /// </summary>
        [Name("指针按下时执行")]
        OnPointerDown,

        /// <summary>
        /// 指针抬起时执行
        /// </summary>
        [Name("指针抬起时执行")]
        OnPointerUp,

        /// <summary>
        /// 开始拖拽时执行
        /// </summary>
        [Name("开始拖拽时执行")]
        OnBeginDrag,

        /// <summary>
        /// 结束拖拽时执行
        /// </summary>
        [Name("结束拖拽时执行")]
        OnEndDrag,

        /// <summary>
        /// 拖拽时执行
        /// </summary>
        [Name("拖拽时执行")]
        OnDrag,
    }

    /// <summary>
    /// 锁定相机(UGUI专用)脚本事件集合
    /// </summary>
    [Serializable]
    public class LockCameraForUGUIScriptEventSet : BaseScriptEventSet<ELockCameraForUGUIScriptEventType> { }
}
