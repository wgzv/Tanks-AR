using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.Components
{
    /// <summary>
    /// 碰撞体启用:碰撞体启用组件是控制Unity碰撞体组件启用禁用的执行体。随着状态生命周期发生的事件（进入和退出），启用和禁用设置的Unity碰撞体组件，组件激活后即刻切换为完成态。
    /// </summary>
    [ComponentMenu(SMSHelperExtension.ComponentCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(ColliderEnable))]
    [Tip("碰撞体启用组件是控制Unity碰撞体组件启用禁用的执行体。随着状态生命周期发生的事件（进入和退出），启用和禁用设置的Unity碰撞体组件，组件激活后即刻切换为完成态。")]
    [XCSJ.Attributes.Icon(index = 33630)]
    [RequireComponent(typeof(GameObjectSet))]
    public class ColliderEnable : StateComponent<ColliderEnable>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "碰撞体启用";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.ComponentCategoryName, typeof(SMSManager))]
        [StateComponentMenu(SMSHelperExtension.ComponentCategoryName + "/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(ColliderEnable))]
        [Tip("碰撞体启用组件是控制Unity碰撞体组件启用禁用的执行体。随着状态生命周期发生的事件（进入和退出），启用和禁用设置的Unity碰撞体组件，组件激活后即刻切换为完成态。")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreateColliderEnable(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("初始化")]
        [EnumPopup]
        public EBool initEnable = EBool.None;

        [Name("进入")]
        [EnumPopup]
        public EBool entryEnable = EBool.Yes;

        [Name("退出")]
        [EnumPopup]
        public EBool exitEnable = EBool.No;

        private GameObjectSet gameObjectSet = null;

        private List<Collider> colliders = new List<Collider>();

        public override bool Init(StateData data)
        {
            InitGameObjectSet();
            SetEnable(initEnable);
            return base.Init(data);
        }

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            SetEnable(entryEnable);
        }

        public override void OnExit(StateData data)
        {
            base.OnExit(data);

            SetEnable(exitEnable);
        }

        public override bool Finished()
        {
            return true;
        }

        private void InitGameObjectSet()
        {
            gameObjectSet = GetComponent<GameObjectSet>();
            foreach (var obj in gameObjectSet.objects)
            {
                var c = obj.GetComponent<Collider>();
                if (c)
                {
                    colliders.Add(c);
                }
            }
        }

        public void SetEnable(EBool enable)
        {
            foreach (var c in colliders)
            {
                c.XSetEnable(enable);
            }
        }
    }
}
