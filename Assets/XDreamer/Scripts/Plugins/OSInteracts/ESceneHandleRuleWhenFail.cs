using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.Extension.OSInteracts
{
    public class ForSceneHandleRuleWhenFail : EnumScriptParam<ESceneHandleRuleWhenFail>
    {
        public const int ScriptParamType = (int)EOSInteractScriptID._Begin;

        public override int GetParamType() => ScriptParamType;
    }

    /// <summary>
    /// 场景加载失败时场景处理规则
    /// </summary>
    public enum ESceneHandleRuleWhenFail
    {
        [Name("无")]
        [Tip("不做任何操作")]
        None,

        [Name("加载之前场景")]
        [Tip("加载处于记录状态的之前场景；如果之前场景不存在则，触发'加载主场景并返回OS'操作；")]
        LoadPreviousScene,

        [Name("加载主场景并返回OS")]
        LoadMainSceneAndBackOS,

        [Name("返回OS")]
        BackOS,

        [Name("加载主场景")]
        LoadMainScene,

        [Name("关闭程序")]
        ApplicationQuit,
    }
}
