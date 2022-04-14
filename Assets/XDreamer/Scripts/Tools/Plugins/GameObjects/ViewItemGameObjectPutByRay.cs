using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.Puts;

namespace XCSJ.PluginTools.GameObjects
{
    /// <summary>
    /// 游戏对象摆放通过射线
    /// </summary>
    [Name("游戏对象摆放通过射线")]
    [Tool("游戏对象", disallowMultiple = true, rootType = typeof(ToolsManager))]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Put)]
    [RequireManager(typeof(ToolsManager))]
    public class ViewItemGameObjectPutByRay : ViewItemGameObjectPut
    {
        /// <summary>
        /// 选择集拖拽通过射线
        /// </summary>
        [Name("选择集拖拽通过射线")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public DragByRay _dragByRay = null;

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            InitDragByRay();
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        protected void Awake()
        {
            InitDragByRay();
        }

        private void InitDragByRay()
        {
            if (!_dragByRay)
            {
                var rays = CommonFun.GetComponentsInChildren<DragByRay>(true);
                if (rays.Length > 0)
                {
                    _dragByRay = rays[0];
                }
                else
                {
                    _dragByRay = gameObject.AddComponent<DragByRay>();
                    _dragByRay.enabled = false;
                }
            }
        }

        /// <summary>
        /// 开始拖拽
        /// </summary>
        /// <param name="gameObjects"></param>
        public override void OnGrab(params GameObject[] gameObjects)
        {
            base.OnGrab(gameObjects);

            _dragByRay.OnGrab(gameObjects);
        }

        /// <summary>
        /// 结束拖拽
        /// </summary>
        /// <param name="gameObjects"></param>
        public override void OnRelease(params GameObject[] gameObjects)
        {
            base.OnRelease(gameObjects);

            _dragByRay.OnRelease(gameObjects);
        }

        /// <summary>
        /// 更新拖拽游戏对象位置
        /// </summary>
        /// <param name="viewItemGameObjectPutEvent"></param>
        protected override void UpdateDragGameObjectPosition(EViewItemGameObjectPutEvent viewItemGameObjectPutEvent)
        {
            dragGameObject.transform.position = _dragByRay.GetPositionByRay(dragGameObject.transform);
        }

        private Dictionary<GameObject, IGameObjectViewItemData> instanceInfo = new Dictionary<GameObject, IGameObjectViewItemData>();

        /// <summary>
        /// 获取拖拽实例游戏对象
        /// </summary>
        /// <param name="gameObjectViewItemData"></param>
        protected override GameObject GetDragInstanceGameObject(IGameObjectViewItemData gameObjectViewItemData)
        {
            var go = gameObjectViewItemData.prototype;
            var newGO = go.XCloneObject();
            if (newGO)
            {
                --gameObjectViewItemData.count;
                newGO.XSetParent(go.transform.parent);
                newGO.XSetUniqueName(go.name);
                newGO.transform.position = _dragByRay.GetPositionByRay(newGO.transform);

                newGO.SetActive(true);

                instanceInfo.Add(newGO, gameObjectViewItemData);
                instanceGameObjects.Add(newGO, this);
            }
            return newGO;
        }

        /// <summary>
        /// 销毁实例对象
        /// </summary>
        /// <param name="instanceGameObject"></param>
        /// <returns></returns>
        public override bool OnDestroyInstanceGameObject(GameObject instanceGameObject)
        {
            instanceGameObjects.Remove(instanceGameObject);
            if (instanceInfo.TryGetValue(instanceGameObject, out var data))
            {
                ++data.count;
                instanceInfo.Remove(instanceGameObject);
                UnityObjectHelper.XDestoryObject(instanceGameObject);
                return true;
            }
            return true;
        }

        public override DragGameObjectViewItemEventArgs GetDragGameObjectViewItemData(GameObject instanceGameObject, EViewItemGameObjectPutEvent viewItemGameObjectPutEvent, IGameObjectViewItemData gameObjectViewItemData = null)
        {
            var data = base.GetDragGameObjectViewItemData(instanceGameObject, viewItemGameObjectPutEvent, gameObjectViewItemData);
            if (gameObjectViewItemData == null && instanceInfo.TryGetValue(instanceGameObject, out var tmpData))
            {
                data.prototype = tmpData.prototype;
                data.count = tmpData.count;
            }
            return data;
        }
    }
}
