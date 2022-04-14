using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginTools.SelectionUtils;

namespace XCSJ.PluginTools.GameObjects
{
    /// <summary>
    /// 视图项游戏对象摆放
    /// </summary>
    [Name("视图项游戏对象摆放")]
    [RequireManager(typeof(ToolsManager))]
    public abstract class ViewItemGameObjectPut : BaseDragger 
    {
        /// <summary>
        /// 克隆游戏对象集
        /// </summary>
        public static Dictionary<GameObject, ViewItemGameObjectPut> instanceGameObjects = new Dictionary<GameObject, ViewItemGameObjectPut>();

        /// <summary>
        /// 销毁实例游戏对象
        /// </summary>
        /// <param name="instanceGameObject"></param>
        /// <returns></returns>
        public static bool DestroyInstanceGameObject(GameObject instanceGameObject)
        {
            if (!instanceGameObject) return false;

            if (instanceGameObjects.TryGetValue(instanceGameObject, out var component))
            {
                var data0 = component.GetDragGameObjectViewItemData(instanceGameObject, EViewItemGameObjectPutEvent.BeforeDestroy);
                onDragEvent?.Invoke(component, data0);

                var data1 = component.GetDragGameObjectViewItemData(instanceGameObject, EViewItemGameObjectPutEvent.AfterDestroy);

                data1.handleResult = component.OnDestroyInstanceGameObject(instanceGameObject);
                if (data1.handleResult)
                {
                    ++data1.count;
                }
                onDragEvent?.Invoke(component, data1);

                return data1.handleResult;
            }

            return false;
        }

        /// <summary>
        /// 销毁实例对象回调
        /// </summary>
        /// <param name="instanceGameObject"></param>
        /// <returns></returns>
        public virtual bool OnDestroyInstanceGameObject(GameObject instanceGameObject) => true;

        /// <summary>
        /// 获取拖拽游戏对象视图项数据
        /// </summary>
        /// <param name="instanceGameObject"></param>
        /// <param name="viewItemGameObjectPutEvent"></param>
        /// <param name="gameObjectViewItemData"></param>
        /// <returns></returns>
        public virtual DragGameObjectViewItemEventArgs GetDragGameObjectViewItemData(GameObject instanceGameObject, EViewItemGameObjectPutEvent viewItemGameObjectPutEvent, IGameObjectViewItemData gameObjectViewItemData = null)
        {
            return new DragGameObjectViewItemEventArgs(instanceGameObject, viewItemGameObjectPutEvent, gameObjectViewItemData);
        }

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset()
        {
            _activeOnEnable = false;
        }

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            GameObjectViewItem.onBeginDrag += OnViewItemBeginDrag;
            GameObjectViewItem.onDraging += OnViewItemDraging;
            GameObjectViewItem.onEndDrag += OnViewItemEndDrag;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();

            GameObjectViewItem.onBeginDrag -= OnViewItemBeginDrag;
            GameObjectViewItem.onDraging -= OnViewItemDraging;
            GameObjectViewItem.onEndDrag -= OnViewItemEndDrag;
        }

        #region 视图项拖拽触发

        protected virtual GameObject dragGameObject
        {
            get => _dragGameObject;
            set
            {
                Selection.selection = _dragGameObject = value;
            }
        }
        private GameObject _dragGameObject;

        /// <summary>
        /// UI视图项触发拖拽
        /// </summary>
        /// <param name="pointerEventData"></param>
        /// <param name="viewItemDataGameObject"></param>
        protected virtual void OnViewItemBeginDrag(PointerEventData pointerEventData, IGameObjectViewItemData gameObjectViewItemData)
        {
            // 拖拽时激活拖拽器
            SetActiveDragger(true);

            isBeginDrag = true;

            dragGameObject = GetDragInstanceGameObject(gameObjectViewItemData);
            onDragEvent?.Invoke(this, GetDragGameObjectViewItemData(dragGameObject, EViewItemGameObjectPutEvent.BeginDrag, gameObjectViewItemData));
        }

        /// <summary>
        /// 拖拽进行中
        /// </summary>
        /// <param name="pointerEventData"></param>
        /// <param name="viewItemDataGameObject"></param>
        protected virtual void OnViewItemDraging(PointerEventData pointerEventData, IGameObjectViewItemData gameObjectViewItemData)
        {
            isDragging = true;
            onDragEvent?.Invoke(this, GetDragGameObjectViewItemData(dragGameObject, EViewItemGameObjectPutEvent.Draging, gameObjectViewItemData));
        }

        /// <summary>
        /// 结束拖拽
        /// </summary>
        /// <param name="pointerEventData"></param>
        /// <param name="viewItemDataGameObject"></param>
        protected virtual void OnViewItemEndDrag(PointerEventData pointerEventData, IGameObjectViewItemData gameObjectViewItemData)
        {
            isDragging = false;
            isEndDrag = true;
            onDragEvent?.Invoke(this, GetDragGameObjectViewItemData(dragGameObject, EViewItemGameObjectPutEvent.EndDrag, gameObjectViewItemData));
        }

        #endregion

        #region 拖拽动作

        public static event Action<ViewItemGameObjectPut, DragGameObjectViewItemEventArgs> onDragEvent;

        protected bool isDragging = false;

        /// <summary>
        /// 是否开始拖拽
        /// </summary>
        /// <returns></returns>
        public override bool Grab() => isBeginDrag;

        /// <summary>
        /// 开始拖拽
        /// </summary>
        private bool isBeginDrag = false;

        /// <summary>
        /// 开始拖拽
        /// </summary>
        public override void OnGrab(params GameObject[] gameObjects) 
        {
            isBeginDrag = false;

            if (dragGameObject)
            {
                UpdateDragGameObjectPosition(EViewItemGameObjectPutEvent.BeginDrag);
            }
        }

        /// <summary>
        /// 是否拖拽中
        /// </summary>
        /// <returns></returns>
        public override bool Hold() => true;

        /// <summary>
        /// 拖拽中
        /// </summary>
        public override void OnHold(params GameObject[] gameObjects)
        {
            if (dragGameObject)
            {
                UpdateDragGameObjectPosition(EViewItemGameObjectPutEvent.Draging);
            }
        }

        /// <summary>
        /// 延迟渲染
        /// </summary>
        /// <returns></returns>
        public IEnumerator DelayRecoverLastDragger()
        {
            yield return new WaitForEndOfFrame();

            // 恢复上一个激活的拖拽器
            RecoverLastDragger();
        }

        /// <summary>
        /// 是否拖拽结束
        /// </summary>
        /// <returns></returns>
        public override bool Release() => isEndDrag;

        private bool isEndDrag = false;

        /// <summary>
        /// 结束拖拽
        /// </summary>
        public override void OnRelease(params GameObject[] gameObjects) 
        {
            isEndDrag = false;

            if (dragGameObject)
            {
                UpdateDragGameObjectPosition(EViewItemGameObjectPutEvent.EndDrag);
            }

            StartCoroutine(DelayRecoverLastDragger());
        }

        #endregion

        #region 拖拽对象操作

        /// <summary>
        /// 获取拖拽对象
        /// </summary>
        /// <param name="gameObjectViewItemData"></param>
        /// <returns></returns>
        protected abstract GameObject GetDragInstanceGameObject(IGameObjectViewItemData gameObjectViewItemData);

        /// <summary>
        /// 更新拖拽游戏对象位置
        /// </summary>
        /// <param name="mousePositon"></param>
        /// <returns></returns>
        protected abstract void UpdateDragGameObjectPosition(EViewItemGameObjectPutEvent viewItemGameObjectPutEvent);

        /// <summary>
        /// 放置相机
        /// </summary>
        protected virtual Camera putCamera => Camera.main ? Camera.main : Camera.current;

        #endregion
    }

    /// <summary>
    /// 拖拽游戏对象视图项数据
    /// </summary>
    public class DragGameObjectViewItemEventArgs : EventArgs
    {
        /// <summary>
        /// 原型
        /// </summary>
        public GameObject prototype { get; set; }

        /// <summary>
        /// 实例
        /// </summary>
        public GameObject instance { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int count { get; set; }

        public IGameObjectViewItemData gameObjectViewItemData { get; private set; }

        /// <summary>
        /// 拖拽事件
        /// </summary>
        public EViewItemGameObjectPutEvent viewItemGameObjectPutEvent { get; private set; }

        public bool handleResult { get; set; } = false;

        public DragGameObjectViewItemEventArgs(GameObject instance, EViewItemGameObjectPutEvent viewItemGameObjectPutEvent, IGameObjectViewItemData gameObjectViewItemData)
        {
            this.instance = instance;
            this.viewItemGameObjectPutEvent = viewItemGameObjectPutEvent;
            SetGameObjectViewItemData(gameObjectViewItemData);
        }

        public void SetGameObjectViewItemData(IGameObjectViewItemData gameObjectViewItemData)
        {
            this.gameObjectViewItemData = gameObjectViewItemData;
            if (gameObjectViewItemData != null)
            {
                this.prototype = gameObjectViewItemData.prototype;
                this.count = gameObjectViewItemData.count;
            }
        }
    }

    /// <summary>
    /// 摆放事件
    /// </summary>
    public enum EViewItemGameObjectPutEvent
    {
        [Name("无")]
        None,

        [Name("开始拖拽")]
        BeginDrag,

        [Name("拖拽中")]
        Draging,

        [Name("结束拖拽")]
        EndDrag,

        [Name("销毁前")]
        BeforeDestroy,

        [Name("销毁后")]
        AfterDestroy,
    }
}
