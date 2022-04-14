using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Components
{
    /// <summary>
    /// 碰撞体点击:碰撞体点击组件是Unity碰撞体点击事件的触发器。当鼠标点击其中一个游戏对象上的碰撞体，组件切换为完成态。
    /// </summary>
    [Serializable]
    [ComponentMenu(SMSHelperExtension.ComponentCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(ColliderClick))]
    [Tip("碰撞体点击组件是Unity碰撞体点击事件的触发器。当鼠标点击其中一个游戏对象上的碰撞体，组件切换为完成态。")]
    [XCSJ.Attributes.Icon(index = 33600)]
    public class ColliderClick : Trigger<ColliderClick>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "碰撞体点击";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.CommonUseCategoryName, typeof(SMSManager))]
        [StateLib(SMSHelperExtension.ComponentCategoryName, typeof(SMSManager))]
        [StateComponentMenu(SMSHelperExtension.CommonUseCategoryName + "/" + Title, typeof(SMSManager))]
        [StateComponentMenu(SMSHelperExtension.ComponentCategoryName + "/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(ColliderClick))]
        [Tip("碰撞体点击组件是Unity碰撞体点击事件的触发器。当鼠标点击其中一个游戏对象上的碰撞体，组件切换为完成态。")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("游戏对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]        
        public GameObject go;

        [Name("点击类型")]
        [EnumPopup]
        public EClickType clickType = EClickType.DownAndUp;

        [Name("点击识别限定距离")]
        [Tip("点击按下和弹起的屏幕坐标差值;在这个限定数值之内认为是点击，否则为拖动")]
        [Range(0, 1000)]
        [HideInSuperInspector(nameof(clickType), EValidityCheckType.NotEqual, EClickType.DownAndUp)]
        public float limitDistanceAsClick = 5;

        [Name("包含子对象")]
        public bool includeChildren = false;

        [Name("点击UI时无效")]
        public bool invalidOnGUI = true;

        [Name("自动添加碰撞体")]
        [Tip("没有碰撞体，点击事件就不会产生！")]
        public bool addCollider = true;

        [Name("点击配置")]
        [HideInInspector]
        public ColliderClickHandle colliderClickHandle = new ColliderClickHandle();

        public override bool Init(StateData data)
        {
            if (DataValidity())
            {
                SetColliderClickHandleData();
                colliderClickHandle.Init(go);                
            }
            return base.Init(data);
        }

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            SetColliderClickHandleData();
            colliderClickHandle.OnEntry();
        }

        public override void OnUpdate(StateData data)
        {
            base.OnUpdate(data);

            if (finished) return;

            SetColliderClickHandleData();
            finished = colliderClickHandle.IsTrigger();
        }

        public override bool DataValidity() => go;

        public override string ToFriendlyString() => go ? go.name : "";

        private void SetColliderClickHandleData()
        {
            colliderClickHandle.clickType = clickType;
            colliderClickHandle.limitDistanceAsClick = limitDistanceAsClick;
            colliderClickHandle.includeChildren = includeChildren;
            colliderClickHandle.invalidOnGUI = invalidOnGUI;
            colliderClickHandle.addCollider = addCollider;
        }
    }

    [Serializable]
    public class ColliderClickHandle
    {
        [Name("点击类型")]
        [EnumPopup]
        public EClickType clickType = EClickType.DownAndUp;

        /// <summary>
        /// 最大距离:射线检测的最大距离
        /// </summary>
        [Name("最大距离")]
        [Tip("射线检测的最大距离")]
        [Min(0.01f)]
        public float _maxDistance = 1000f;

        /// <summary>
        /// 图层遮罩:射线检测时的图层遮罩
        /// </summary>
        [Name("图层遮罩")]
        [Tip("射线检测时的图层遮罩")]
        public LayerMask _layerMask = Physics.DefaultRaycastLayers;

        [Name("点击识别限定距离")]
        [Tip("点击按下和弹起的屏幕坐标差值;在这个限定数值之内认为是点击，否则为拖动")]
        [Range(0.1f, 1000)]
        [HideInSuperInspector(nameof(clickType), EValidityCheckType.NotEqual, EClickType.DownAndUp)]
        public float limitDistanceAsClick = 5;

        [Name("包含子对象")]
        public bool includeChildren = false;

        [Name("点击UI时无效")]
        public bool invalidOnGUI = true;

        [Name("自动添加碰撞体")]
        [Tip("没有碰撞体，点击事件就不会产生！")]
        public bool addCollider = true;

        private float sqrLimitDistanceAsClick = 0;

        private bool isDown = false;

        private List<Collider> colliders = new List<Collider>();

        private List<GameObject> goList = new List<GameObject>();

        public GameObject lastClickGameObject { get; protected set; }

        public void Init(GameObject go) => Init((new GameObject[] { go }).ToList());

        public void Init(List<GameObject> goList)
        {
            this.goList = goList;

            sqrLimitDistanceAsClick = limitDistanceAsClick * limitDistanceAsClick;

            InitCollider();
        }

        public void OnEntry()
        {
            isDown = false;
            lastClickGameObject = null;
        }

        public bool IsTrigger()
        {
            if (XInput.GetMouseButtonDown(0))
            {
                return OnMouseDown();
            }
            else if (XInput.GetMouseButtonUp(0))
            {
                return OnMouseUp();
            }
            return false;
        }

        private Vector3 mousePositionWhenDown = Vector3.zero;

        private bool OnMouseDown()
        {
            bool trigger = false;
            switch (clickType)
            {
                case EClickType.DownAndUp:
                    {
                        isDown = IsOnCollider();
                        mousePositionWhenDown = XInput.mousePosition;
                        break;
                    }
                case EClickType.Down:
                    {
                        trigger = IsOnCollider();
                        break;
                    }
            }
            return trigger;
        }

        private bool OnMouseUp()
        {
            bool trigger = false;
            switch (clickType)
            {
                case EClickType.DownAndUp:
                    {
                        trigger = isDown && IsOnCollider() && ((XInput.mousePosition - mousePositionWhenDown).sqrMagnitude < sqrLimitDistanceAsClick);
                        break;
                    }
                case EClickType.Up:
                    {
                        trigger = IsOnCollider();
                        break;
                    }
            }
            return trigger;
        }

        private void InitCollider()
        {
            if (goList.Count==0) return;

            goList.ForEach(go=> colliders.AddRange(go.GetComponentsInChildren<Collider>()));

            if (colliders.Count == 0 && addCollider)
            {
                goList.ForEach(go =>
                {
                    var renderers = go.GetComponentsInChildren<MeshRenderer>();
                    if (renderers.Length > 0)
                    {
                        foreach (var r in renderers)
                        {
                            colliders.Add(r.gameObject.AddComponent<MeshCollider>());
                        }
                    }
                    else
                    {
                        colliders.Add(go.AddComponent<BoxCollider>());
                    }
                });                
            }
        }

        private bool IsOnCollider()
        {
            if (colliders.Count == 0) return false;

            if (invalidOnGUI && CommonFun.IsOnUINow()) return false;

            var cam = Camera.main ? Camera.main : Camera.current;
            if (!cam)
            {
                Log.Warning("相机缺失!");
                return false;
            }

            if (Physics.Raycast(cam.ScreenPointToRay(XInput.mousePosition), out var hitInfo, _maxDistance, _layerMask))
            {
                lastClickGameObject = hitInfo.collider.gameObject;
                return colliders.Contains(hitInfo.collider);
            }
            return false;
        }
    }
}
