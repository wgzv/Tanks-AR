using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Components
{
    /// <summary>
    /// 碰撞体触发器:碰撞体触发器组件是碰撞体与碰撞体之间发生碰撞的触发器。当碰撞发生时，组件切换为完成态。
    /// </summary>
    [Serializable]
    [ComponentMenu(SMSHelperExtension.ComponentCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(ColliderTrigger))]
    [Tip("碰撞体触发器组件是碰撞体与碰撞体之间发生碰撞的触发器。当碰撞发生时，组件切换为完成态。")]
    [XCSJ.Attributes.Icon(index = 33601)]
    public class ColliderTrigger : Trigger<ColliderTrigger>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "碰撞体触发器";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.ComponentCategoryName, typeof(SMSManager))]
        [StateComponentMenu(SMSHelperExtension.ComponentCategoryName + "/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(ColliderTrigger))]
        [Tip("碰撞体触发器组件是碰撞体与碰撞体之间发生碰撞的触发器。当碰撞发生时，组件切换为完成态。")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [ComponentPopup]
        [Name("碰撞体触发器")]
        [Tip("待碰撞的触发器对象;一般为静止不动的游戏对象;碰撞体触发器对应游戏对象最好具有刚体组件，且为触发器；")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Collider colliderTrigger;

        public enum ETriggerType
        {
            [Name("进入")]
            Enter,

            [Name("保留")]
            Stay,

            [Name("退出")]
            Exit,
        }

        [Name("触发类型")]
        [EnumPopup]
        public ETriggerType triggerType = ETriggerType.Enter;

        [Name("当前主相机")]
        public bool currentMainCamera = true;

        [Name("游戏对象")]
        [Tip("碰撞体所在游戏对象；一般为可发生移动的游戏对象;")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(currentMainCamera), EValidityCheckType.True)]
        public GameObject gameObject;

        [Name("自动添加碰撞体")]
        [Tip("当前主相机/游戏对象 没有碰撞体时动态添加碰撞体")]
        public bool autoAddCollider = true;

        public enum EColliderType
        {
            [Name("盒碰撞体")]
            BoxCollider = 0,

            [Name("球碰碰撞体")]
            SphereCollider,

            [Name("胶囊碰撞体")]
            CapsuleCollider,

            [Name("网格碰撞体")]
            MeshCollider,

            [Name("车轮碰撞体")]
            WheelCollider,

            [Name("地形碰撞体")]
            TerrainCollider,
        }

        [Name("碰撞体类型")]
        [HideInSuperInspector(nameof(autoAddCollider), EValidityCheckType.False)]
        [EnumPopup]
        public EColliderType colliderType = EColliderType.BoxCollider;

        private ColliderTriggerMB colliderTriggerMB;

        private GameObject colliderGO;

        private Collider collider;

        private bool colliderEnabled = true;

        public static Type GetType(EColliderType colliderType)
        {
            switch (colliderType)
            {
                case EColliderType.BoxCollider: return typeof(BoxCollider);
                case EColliderType.CapsuleCollider: return typeof(CapsuleCollider);
                case EColliderType.MeshCollider: return typeof(MeshCollider);
                case EColliderType.SphereCollider: return typeof(SphereCollider);
                case EColliderType.WheelCollider: return typeof(WheelCollider);
                case EColliderType.TerrainCollider: return typeof(TerrainCollider);
                default: return null;
            }
        }

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);
            var cam = Camera.main ? Camera.main : Camera.current;
            colliderGO = currentMainCamera ? (cam ? cam.gameObject : null) : gameObject;
            if (colliderGO)
            {
                AddTrigger();
            }
            else
            {
                collider = null;
                colliderTriggerMB = null;
            }
        }

        public override void OnUpdate(StateData stateData)
        {
            base.OnUpdate(stateData);

            // 相机碰撞触发改变
            var cam = Camera.main ? Camera.main : Camera.current;
            if (currentMainCamera && cam && cam.gameObject!= colliderGO)
            {
                RemoveTrigger();

                colliderGO = Camera.current.gameObject;

                AddTrigger();
            }
        }

        public override void OnExit(StateData data)
        {
            RemoveTrigger();

            base.OnExit(data);
        }

        private void AddTrigger()
        {
            if (autoAddCollider)
            {
                collider = CommonFun.GetOrAddComponent<Collider>(colliderGO, GetType(colliderType));
            }
            else
            {
                collider = colliderGO.GetComponent<Collider>();
            }
            colliderTriggerMB = CommonFun.GetOrAddComponent<ColliderTriggerMB>(colliderGO);

            if (collider)
            {
                colliderEnabled = collider.enabled;
                collider.enabled = true;
            }
            if (colliderTriggerMB)
            {
                colliderTriggerMB.enabled = true;
                colliderTriggerMB.onTriggerEnter += _OnTriggerEnter;
                colliderTriggerMB.onTriggerStay += _OnTriggerStay;
                colliderTriggerMB.onTriggerExit += _OnTriggerExit;
            }
        }

        private void RemoveTrigger()
        {
            if (colliderTriggerMB)
            {
                colliderTriggerMB.enabled = false;
                colliderTriggerMB.onTriggerEnter -= _OnTriggerEnter;
                colliderTriggerMB.onTriggerStay -= _OnTriggerStay;
                colliderTriggerMB.onTriggerExit -= _OnTriggerExit;
            }
            if (collider)
            {
                collider.enabled = colliderEnabled;
            }
        }

        private void _OnTriggerEnter(ColliderTriggerMB mb, Collider collider)
        {
            if (triggerType == ETriggerType.Enter && collider == colliderTrigger)
            {
                finished = true;
            }
        }

        private void _OnTriggerExit(ColliderTriggerMB mb, Collider collider)
        {
            if (triggerType == ETriggerType.Exit && collider == colliderTrigger)
            {
                finished = true;
            }
        }

        private void _OnTriggerStay(ColliderTriggerMB mb, Collider collider)
        {
            if (triggerType == ETriggerType.Stay && collider == colliderTrigger)
            {
                finished = true;
            }
        }

        public override bool DataValidity()
        {
            return colliderTrigger && (currentMainCamera || gameObject);
        }

        public override string ToFriendlyString()
        {
            return colliderTrigger ? colliderTrigger.name : "";
        }
    }
}
