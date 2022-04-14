using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.Puts;

namespace XCSJ.PluginTools.GameObjects
{
    /// <summary>
    /// 游戏对象摆放通过插槽
    /// </summary>
    [Name("游戏对象摆放通过插槽")]
    [Tool("游戏对象", disallowMultiple = true, rootType = typeof(ToolsManager))]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Put)]
    [RequireManager(typeof(ToolsManager))]
    [RequireComponent(typeof(GameObjectSocketCache))]
    public class ViewItemGameObjectPutBySocket : ViewItemGameObjectPut
    {
        private IGameObjectSocket currentSocket = null;

        private GameObjectSocketCache gameObjectSocketCache;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            GameObjectSocketCache.onCurrentSocketChanged += OnCurrentSocketChanged;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();

            GameObjectSocketCache.onCurrentSocketChanged -= OnCurrentSocketChanged;
        }

        protected void Start()
        {
            gameObjectSocketCache = GetComponent<GameObjectSocketCache>();
        }

        private void OnCurrentSocketChanged(GameObjectSocketCache gameObjectSocketCache, IGameObjectSocket oldSocket, IGameObjectSocket newSocket, bool isEndDrag)
        {
            currentSocket = newSocket;
            dragGameObject = currentSocket?.target;
        }

        /// <summary>
        /// 获取拖拽实例游戏对象
        /// </summary>
        /// <param name="gameObjectViewItemData"></param>
        /// <returns></returns>
        protected override GameObject GetDragInstanceGameObject(IGameObjectViewItemData gameObjectViewItemData)
        {
            return gameObjectViewItemData.prototype;
        }

        /// <summary>
        /// 更新拖拽游戏对象位置
        /// </summary>
        /// <param name="viewItemGameObjectPutEvent"></param>
        protected override void UpdateDragGameObjectPosition(EViewItemGameObjectPutEvent viewItemGameObjectPutEvent)
        {
            if (TryGetDragPosition(out Vector3 pos))
            {
                dragGameObject.transform.position = pos;
            }
        }

        /// <summary>
        /// 放置游戏对象
        /// </summary>
        /// <param name="mousePositon"></param>
        private bool TryGetDragPosition(out Vector3 positon)
        {
            if (putCamera && currentSocket != null)
            {
                // 参考点，作为参考系，求出屏幕点对应的世界坐标系
                positon = XInput.mousePosition;
                var t = putCamera.transform;
                positon.z = Mathf.Abs(Vector3.Dot(t.forward, currentSocket.socketPosition - t.position));
                positon = putCamera.ScreenToWorldPoint(positon);
                return true;
            }
            positon = Vector3.zero;
            return false;
        }

        /// <summary>
        /// 开始拖拽
        /// </summary>
        /// <param name="pointerEventData"></param>
        /// <param name="viewItemDataGameObject"></param>
        protected override void OnViewItemBeginDrag(PointerEventData pointerEventData, IGameObjectViewItemData viewItemDataGameObject)
        {
            base.OnViewItemBeginDrag(pointerEventData, viewItemDataGameObject);

            gameObjectSocketCache.BeginDrag(dragGameObject);
        }

        /// <summary>
        /// 拖拽中
        /// </summary>
        /// <param name="pointerEventData"></param>
        /// <param name="viewItemDataGameObject"></param>
        protected override void OnViewItemDraging(PointerEventData pointerEventData, IGameObjectViewItemData viewItemDataGameObject)
        {
            base.OnViewItemDraging(pointerEventData, viewItemDataGameObject);

            gameObjectSocketCache.Dragging(dragGameObject);
        }

        /// <summary>
        /// 结束拖拽
        /// </summary>
        /// <param name="pointerEventData"></param>
        /// <param name="viewItemDataGameObject"></param>
        protected override void OnViewItemEndDrag(PointerEventData pointerEventData, IGameObjectViewItemData viewItemDataGameObject)
        {
            base.OnViewItemEndDrag(pointerEventData, viewItemDataGameObject);

            gameObjectSocketCache.EndDrag(dragGameObject);
        }
    }

}
