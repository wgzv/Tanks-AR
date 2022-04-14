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
    /// 碰撞体点击批量:碰撞体点击批量组件是多个Unity碰撞体之中任意一个发生点击事件的触发器。当鼠标点击若干游戏对象中任意一个时，组件切换为完成态。
    /// </summary>
    [Serializable]
    [ComponentMenu(SMSHelperExtension.ComponentCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(ColliderClickBatch))]
    [Tip("碰撞体点击批量组件是多个Unity碰撞体之中任意一个发生点击事件的触发器。当鼠标点击若干游戏对象中任意一个时，组件切换为完成态。")]
    [XCSJ.Attributes.Icon(nameof(ColliderClick))]
    [RequireComponent(typeof(GameObjectSet))]
    public class ColliderClickBatch : Trigger<ColliderClickBatch>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "碰撞体点击批量";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.ComponentCategoryName, typeof(SMSManager))]
        [StateComponentMenu(SMSHelperExtension.ComponentCategoryName + "/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(ColliderClickBatch))]
        [Tip("碰撞体点击批量组件是多个Unity碰撞体之中任意一个发生点击事件的触发器。当鼠标点击若干游戏对象中任意一个时，组件切换为完成态。")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreateColliderClickBatch(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("点击配置")]
        [OnlyMemberElements]
        public ColliderClickHandle colliderClickHandle = new ColliderClickHandle();

        [Name("变量")]
        [Tip("被点击碰撞体对象的保存变量")]
        [GlobalVariable(true)]
        public string variable;

        public GameObjectSet gameObjectSet => GetComponent<GameObjectSet>(true);

        public override bool Init(StateData data)
        {
            colliderClickHandle.Init(gameObjectSet.objects);
            return base.Init(data);
        }

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);
            colliderClickHandle.OnEntry();
        }

        public override void OnUpdate(StateData data)
        {
            base.OnUpdate(data);

            if (finished) return;

            finished = colliderClickHandle.IsTrigger();
            if (finished && colliderClickHandle.lastClickGameObject)
            {
                ScriptManager.TrySetGlobalVariableValue(variable, CommonFun.GameObjectToString(colliderClickHandle.lastClickGameObject));
            }
        }

        public override bool DataValidity() => gameObjectSet.objects.Count>0;

        public override string ToFriendlyString()
        {
            return gameObjectSet.objects.Count.ToString();
        }
    }
}
