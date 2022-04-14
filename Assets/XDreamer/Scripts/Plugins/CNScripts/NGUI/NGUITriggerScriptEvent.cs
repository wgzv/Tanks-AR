using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;

namespace XCSJ.Extension.CNScripts.NGUI
{
    /// <summary>
    /// 专门处理NGUI触发的各种事件的脚本类
    /// </summary>
    [Serializable]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.NGUIMenu + Title)]
    [Obsolete("因NGUI不再提供更新,本组件不推荐用户使用")]
    public class NGUITriggerScriptEvent : BaseScriptEvent<ENGUITriggerScriptEventType, NGUITriggerScriptEventSet>
    {
        public const string Title = "NGUI触发脚本事件";

        public override void Start()
        {
            RunScriptEvent(ENGUITriggerScriptEventType.Start);
        }

        /// <summary>
        /// 选择时
        /// </summary>
        /// <param name="obj"></param>
        public void OnSelect(bool obj)
        {
            RunScriptEvent(obj ? ENGUITriggerScriptEventType.OnSelect : ENGUITriggerScriptEventType.OnDeSelect, obj.ToString());
        }

        /// <summary>
        /// 滚轮滚动时
        /// </summary>
        /// <param name="obj"></param>
        public void OnScroll(float obj)
        {
            RunScriptEvent(ENGUITriggerScriptEventType.OnScroll, obj.ToString());
        }

        /// <summary>
        /// 悬停时
        /// </summary>
        /// <param name="obj"></param>
        public void OnHover(bool obj)
        {
            RunScriptEvent(obj ? ENGUITriggerScriptEventType.OnHover : ENGUITriggerScriptEventType.OnDeHover, obj.ToString());
        }

        /// <summary>
        /// 键盘按下时
        /// </summary>
        /// <param name="obj"></param>
        public void OnKey(KeyCode obj)
        {
            RunScriptEvent(ENGUITriggerScriptEventType.OnKey, obj.ToString());
        }

        /// <summary>
        /// 按压时
        /// </summary>
        /// <param name="obj"></param>
        public void OnPress(bool obj)
        {
            RunScriptEvent(obj ? ENGUITriggerScriptEventType.OnPress : ENGUITriggerScriptEventType.OnDePress, obj.ToString());
        }

        /// <summary>
        /// 开始拖动时
        /// </summary>
        public void OnDragStart()
        {
            RunScriptEvent(ENGUITriggerScriptEventType.OnDragStart);
        }

        /// <summary>
        /// 拖动移过时
        /// </summary>
        /// <param name="obj"></param>
        public void OnDragOver(GameObject obj)
        {
            RunScriptEvent(ENGUITriggerScriptEventType.OnDragOver, CommonFun.GameObjectToString(obj));
        }

        /// <summary>
        /// 拖动移出时
        /// </summary>
        /// <param name="obj"></param>
        public void OnDragOut(GameObject obj)
        {
            RunScriptEvent(ENGUITriggerScriptEventType.OnDragOut, CommonFun.GameObjectToString(obj));
        }

        /// <summary>
        /// 拖动时
        /// </summary>
        /// <param name="obj"></param>
        public void OnDrag(Vector2 obj)
        {
            RunScriptEvent(ENGUITriggerScriptEventType.OnDrag, CommonFun.Vector2ToString(obj));
        }

        /// <summary>
        /// 完成拖动时
        /// </summary>
        public void OnDragEnd()
        {
            RunScriptEvent(ENGUITriggerScriptEventType.OnDragEnd);
        }

        /// <summary>
        /// 单击时
        /// </summary>
        public void OnClick()
        {
            RunScriptEvent(ENGUITriggerScriptEventType.OnClick);
        }

        /// <summary>
        /// 双击时
        /// </summary>
        public void OnDoubleClick()
        {
            RunScriptEvent(ENGUITriggerScriptEventType.OnDoubleClick);
        }

        /// <summary>
        /// 消息提示时
        /// </summary>
        /// <param name="obj"></param>
        public void OnTooltip(bool obj)
        {
            RunScriptEvent(obj ? ENGUITriggerScriptEventType.OnTooltip : ENGUITriggerScriptEventType.OnDeTooltip, obj.ToString());
        }
    }
}
