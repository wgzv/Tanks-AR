using System;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.CNScripts;

namespace XCSJ.Extension.CNScripts.NGUI
{
    public enum ENGUITriggerScriptEventType
    {
        /// <summary>
        /// 选择时
        /// </summary>
        [Name("选择时执行")]
        OnSelect,
        /// <summary>
        /// 取消选择时
        /// </summary>
        [Name("取消选择时执行")]
        OnDeSelect,
        /// <summary>
        /// 滚轮滚动时
        /// </summary>
        [Name("滚轮滚动时执行")]
        OnScroll,
        /// <summary>
        /// 悬停时
        /// </summary>
        [Name("悬停时执行")]
        OnHover,
        /// <summary>
        /// 取消悬停时
        /// </summary>
        [Name("取消悬停时执行")]
        OnDeHover,
        /// <summary>
        /// 键盘按下时
        /// </summary>
        [Name("键盘按下时执行")]
        OnKey,
        /// <summary>
        /// 按压时
        /// </summary>
        [Name("按压时执行")]
        OnPress,
        /// <summary>
        /// 取消按压时
        /// </summary>
        [Name("取消按压时执行")]
        OnDePress,
        /// <summary>
        /// 开始拖动时
        /// </summary>
        [Name("开始拖动时执行")]
        OnDragStart,
        /// <summary>
        /// 拖动移过时
        /// </summary>
        [Name("拖动移过时执行")]
        OnDragOver,
        /// <summary>
        /// 拖动移出时
        /// </summary>
        [Name("拖动移出时执行")]
        OnDragOut,
        /// <summary>
        /// 拖动时
        /// </summary>
        [Name("拖动时执行")]
        OnDrag,
        /// <summary>
        /// 完成拖动时
        /// </summary>
        [Name("完成拖动时执行")]
        OnDragEnd,
        /// <summary>
        /// 单击时
        /// </summary>
        [Name("单击时执行")]
        OnClick,
        /// <summary>
        /// 双击时
        /// </summary>
        [Name("双击时执行")]
        OnDoubleClick,
        /// <summary>
        /// 消息提示时
        /// </summary>
        [Name("消息提示时执行")]
        OnTooltip,
        /// <summary>
        /// 取消消息提示时
        /// </summary>
        [Name("取消消息提示时执行")]
        OnDeTooltip,

        [Name("启动时执行")]
        Start,
    }

    /// <summary>
    /// 脚本 NGUI触发事件集合类
    /// </summary>
    [Serializable]
    public class NGUITriggerScriptEventSet : BaseScriptEventSet<ENGUITriggerScriptEventType>
    {
        //
    }
}
