using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.Extension.OSInteracts
{
    public class ForOSToUnityMsgCmdXScriptParam : EnumScriptParam<EOSToUnityMsgCmd>
    {
        public const int ScriptParamType = ForSceneHandleRuleWhenFail.ScriptParamType + 2;

        public override int GetParamType() => ScriptParamType;
    }

    /// <summary>
    /// OS向Uinty发送的各种消息命令
    /// </summary>
    public enum EOSToUnityMsgCmd
    {
        [Name("无")]
        None = 0,

        [Name("导入并加载场景")]
        ImportAndLoadScene,

        [Name("导入场景")]
        ImportScene,

        [Name("加载场景")]
        LoadScene,

        [Name("加载或导入并加载场景")]
        LoadOrImportAndLoadScene,

        [Name("卸载子场景")]
        UnloadSubScene,

        [Name("卸载子场景(通过索引)")]
        UnloadSubSceneByIndex,

        [Name("卸载全部子场景")]
        UnloadAllSubScene,

        [Name("请求场景名称列表")]
        RequestSceneNameList,

        [Name("用户自定义")]
        UserDefine,

        [Name("调用自定义函数")]
        CallUserDefineFun,

        [Name("执行XCSJ脚本")]
        RunXCSJScript,

        [Name("执行单句XCSJ脚本并返回结果")]
        RunSingleXCSJScriptAndReturnResult,

        [Name("请求图片二维码扫描")]
        RequestImageQRCodeScan,
    }
}
