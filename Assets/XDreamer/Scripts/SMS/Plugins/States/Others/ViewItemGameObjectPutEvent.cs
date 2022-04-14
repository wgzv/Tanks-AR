using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginTools;
using XCSJ.PluginTools.GameObjects;

namespace XCSJ.PluginSMS.States.Others
{
    /// <summary>
    /// 视图项游戏对象摆放事件:用于监听图项游戏对象摆放的开始拖拽、拖拽中和结束拖拽事件
    /// </summary>
    [ComponentMenu("其它/" + Title, typeof(ToolsManager))]
    [Name(Title, nameof(ViewItemGameObjectPutEvent))]
    [Tip("用于监听图项游戏对象摆放的开始拖拽、拖拽中和结束拖拽事件")]
    [XCSJ.Attributes.Icon(EIcon.Drag)]
    [DisallowMultipleComponent]
    public class ViewItemGameObjectPutEvent : Trigger<ViewItemGameObjectPutEvent>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "视图项游戏对象摆放事件";

        /// <summary>
        /// 创建状态
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("其它", typeof(SMSManager))]
        [StateComponentMenu("其它/" + Title, typeof(SMSManager), categoryIndex = IndexAttribute.DefaultIndex + 1)]
        [Name(Title, nameof(ViewItemGameObjectPutEvent))]
        [Tip("用于监听图项游戏对象摆放的开始拖拽、拖拽中和结束拖拽事件")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 视图项游戏对象摆放组件
        /// </summary>
        [Name("视图项游戏对象摆放组件")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public ViewItemGameObjectPut _viewItemGameObjectPut = null;

        /// <summary>
        /// 摆放事件
        /// </summary>
        [Name("摆放事件")]
        [EnumPopup]
        public EViewItemGameObjectPutEvent _viewItemGameObjectPutEvent = EViewItemGameObjectPutEvent.BeginDrag;

        /// <summary>
        /// 拖拽游戏对象原型路径变量
        /// </summary>
        [Name("拖拽原型游戏对象路径变量")]
        [GlobalVariable]
        public string _dragGameObjectPrototypePathVariable = "";

        /// <summary>
        /// 拖拽实例游戏对象路径变量
        /// </summary>
        [Name("拖拽实例游戏对象路径变量")]
        [Tip("拖拽实例游戏对象可能是原型也可能是克隆对象")]
        [GlobalVariable]
        public string _currentDragGameObjectPathVariable = "";

        /// <summary>
        /// 视图项数量变量
        /// </summary>
        [Name("视图项当前数量变量")]
        [GlobalVariable]
        public string _viewItemCurrentCountVariable = "";

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);

            //注册拖拽事件
            ViewItemGameObjectPut.onDragEvent += OnDragEvent;
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);

            //取消注册拖拽事件
            ViewItemGameObjectPut.onDragEvent -= OnDragEvent;
        }

        /// <summary>
        /// 视图项游戏对象摆放事件
        /// </summary>
        /// <param name="pointerEventData"></param>
        /// <param name="viewItemDataGameObject"></param>
        private void OnDragEvent(ViewItemGameObjectPut viewItemGameObjectPut, DragGameObjectViewItemEventArgs dragGameObjectViewItemData)
        {
            if (_viewItemGameObjectPut != viewItemGameObjectPut || _viewItemGameObjectPutEvent != dragGameObjectViewItemData.viewItemGameObjectPutEvent) return;

            ScriptManager.TrySetGlobalVariableValue(_dragGameObjectPrototypePathVariable, CommonFun.GameObjectToStringWithoutAlias(dragGameObjectViewItemData.prototype));

            ScriptManager.TrySetGlobalVariableValue(_currentDragGameObjectPathVariable, CommonFun.GameObjectToStringWithoutAlias(dragGameObjectViewItemData.instance));

            ScriptManager.TrySetGlobalVariableValue(_viewItemCurrentCountVariable, dragGameObjectViewItemData.count.ToString());

            finished = true;
        }

        /// <summary>
        /// 提示字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return CommonFun.Name(_viewItemGameObjectPutEvent);
        }
    }
}
