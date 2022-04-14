using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;

namespace XCSJ.Extension.CNScripts.Inputs
{
    public enum ESingleKeyCodeScriptEventType
    {
        /// <summary>
        /// 按下时
        /// </summary>
        [Name("按下时执行")]
        Down,

        /// <summary>
        /// 按下中时
        /// </summary>
        [Name("按下中时执行")]
        Pressed,

        /// <summary>
        /// 弹起时
        /// </summary>
        [Name("弹起时执行")]
        Up,

        [Name("启动时执行")]
        Start,
    }

    /// <summary>
    /// 脚本SingleKeyCode事件集合类
    /// </summary>
    [Serializable]
    public class SingleKeyCodeScriptEventSet : BaseScriptEventSet<ESingleKeyCodeScriptEventType>
    {
        //
    }

    [Serializable]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.InputMenu + Title)]
    public class SingleKeyCodeScriptEvent : BaseScriptEvent<ESingleKeyCodeScriptEventType, SingleKeyCodeScriptEventSet>
    {
        public const string Title = "单一按键脚本事件";

        [Name("按键码")]
        [Tip("单一按键码脚本事件检测的按键码类型")]
        public KeyCode keyCode = KeyCode.None;

        public override void Start()
        {
            //base.Start();
            RunScriptEvent(ESingleKeyCodeScriptEventType.Start);
        }

        public override void Update()
        {
            if (Input.GetKeyDown(keyCode))
            {
                RunScriptEvent(ESingleKeyCodeScriptEventType.Down, keyCode.ToString());
            }
            else if (Input.GetKey(keyCode))
            {
                RunScriptEvent(ESingleKeyCodeScriptEventType.Pressed, keyCode.ToString());
            }
            else if (Input.GetKeyUp(keyCode))
            {
                RunScriptEvent(ESingleKeyCodeScriptEventType.Up, keyCode.ToString());
            }
            //base.Update();
        }
    }
}
