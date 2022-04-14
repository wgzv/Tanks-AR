using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.Extension.CNScripts.Base
{
    public enum EAnimationScriptEventType
    {
        [Name("布尔型切换事件")]
        BoolChange = 0,

        [Name("浮点数型切换事件")]
        FloatChange,

        [Name("启动时执行")]
        Start,
    }

    [Serializable]
    public class AnimationScriptEventSet : BaseScriptEventSet<EAnimationScriptEventType>
    {

    }

    [Serializable]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.CNScriptMenu + Title)]
    public class AnimationScriptEvent : BaseScriptEvent<EAnimationScriptEventType, AnimationScriptEventSet>
    {
        public const string Title = "动画脚本事件";

        [Name("布尔标识")]
        public bool boolFlag;

        public bool _boolFlag { get; private set; }

        [Name("浮点数标识")]
        public float floatFlag;

        public float _floatFlag { get; private set; }

        public override void Start()
        {
            base.Start();
            _boolFlag = boolFlag;
            _floatFlag = floatFlag;
            RunScriptEvent(EAnimationScriptEventType.Start);
        }

        public override void Update()
        {
            base.Update();
            if (_boolFlag != boolFlag)
            {
                //Debug.Log(name + " , b :  " + boolFlag.ToString());
                _boolFlag = boolFlag;
                RunScriptEvent(EAnimationScriptEventType.BoolChange, ScriptOption.ReturnValueFlag + boolFlag.ToString());
            }

            if (!Mathf.Approximately(_floatFlag, floatFlag))
            {
                //Debug.Log(name + " , b :  " + floatFlag.ToString());
                _floatFlag = floatFlag;
                RunScriptEvent(EAnimationScriptEventType.FloatChange, floatFlag.ToString());
            }
        }

    }
}
