using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.CNScripts;

namespace XCSJ.Extension.CNScripts.Inputs
{
    /// <summary>
    /// 鼠标按钮类型
    /// </summary>
    public enum EMouseButtonType
    {
        /// <summary>
        /// 左键
        /// </summary>
        [Name("左键")]
        Left = 0,

        /// <summary>
        /// 右键
        /// </summary>
        [Name("右键")]
        Right,

        /// <summary>
        /// 中键
        /// </summary>
        [Name("中键")]
        Middle,
    }

    public enum EMouseButtonScriptEventType
    {
        /// <summary>
        /// 左键按下时
        /// </summary>
        [Name("左键按下时执行")]
        LeftDown,

        /// <summary>
        /// 左键按下中时
        /// </summary>
        [Name("左键按下中时执行")]
        Left,

        /// <summary>
        /// 左键弹起时
        /// </summary>
        [Name("左键弹起时执行")]
        LeftUp,

        /// <summary>
        /// 右键按下时
        /// </summary>
        [Name("右键按下时执行")]
        RightDown,

        /// <summary>
        /// 右键按下中时
        /// </summary>
        [Name("右键按下中时执行")]
        Right,

        /// <summary>
        /// 右键弹起时
        /// </summary>
        [Name("右键弹起时执行")]
        RightUp,

        /// <summary>
        /// 中键按下时
        /// </summary>
        [Name("中键按下时执行")]
        MiddleDown,

        /// <summary>
        /// 中键按下中时
        /// </summary>
        [Name("中键按下中时执行")]
        Middle,

        /// <summary>
        /// 中键弹起时
        /// </summary>
        [Name("中键弹起时执行")]
        MiddleUp,

        [Name("启动时执行")]
        Start,
    }

    /// <summary>
    /// 脚本MouseButton事件集合类
    /// </summary>
    [Serializable]
    public class MouseButtonScriptEventSet : BaseScriptEventSet<EMouseButtonScriptEventType>
    {
        //
    }

    /// <summary>
    /// 脚本MouseButton事件集合类
    /// </summary>
    [Serializable]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.InputMenu + Title)]
    public class MouseButtonScriptEvent : BaseScriptEvent<EMouseButtonScriptEventType, MouseButtonScriptEventSet>
    {
        public const string Title = "鼠标按钮脚本事件";

        public override void Start()
        {
            RunScriptEvent(EMouseButtonScriptEventType.Start);
        }

        public override void Update()
        {
            if (!Input.anyKey && !Input.anyKeyDown) return;

            if (Input.GetMouseButtonDown((int)EMouseButtonType.Left))
            {
                RunScriptEvent(EMouseButtonScriptEventType.LeftDown);
            }
            else if (Input.GetMouseButton((int)EMouseButtonType.Left))
            {
                RunScriptEvent(EMouseButtonScriptEventType.Left);
            }
            else if (Input.GetMouseButtonUp((int)EMouseButtonType.Left))
            {
                RunScriptEvent(EMouseButtonScriptEventType.LeftUp);
            }

            if (Input.GetMouseButtonDown((int)EMouseButtonType.Right))
            {
                RunScriptEvent(EMouseButtonScriptEventType.RightDown);
            }
            else if (Input.GetMouseButton((int)EMouseButtonType.Right))
            {
                RunScriptEvent(EMouseButtonScriptEventType.Right);
            }
            else if (Input.GetMouseButtonUp((int)EMouseButtonType.Right))
            {
                RunScriptEvent(EMouseButtonScriptEventType.RightUp);
            }

            if (Input.GetMouseButtonDown((int)EMouseButtonType.Middle))
            {
                RunScriptEvent(EMouseButtonScriptEventType.MiddleDown);
            }
            else if (Input.GetMouseButton((int)EMouseButtonType.Middle))
            {
                RunScriptEvent(EMouseButtonScriptEventType.Middle);
            }
            else if (Input.GetMouseButtonUp((int)EMouseButtonType.Middle))
            {
                RunScriptEvent(EMouseButtonScriptEventType.MiddleUp);
            }
        }
    }
}
