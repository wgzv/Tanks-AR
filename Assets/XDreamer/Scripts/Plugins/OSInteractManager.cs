using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.Extension.GenericStandards.Managers;
using XCSJ.Extension.OSInteracts;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.Extension
{
    /// <summary>
    /// 与OS交换的管理对象
    /// 1、明确通用的发送命令， 将抽象为通用的消息响应和发送函数
    /// 2、特定化的命令则使用用户自定义来交互
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [Name("OS交互")]
    [Tip("OS交互:用于Unity与iOS、安卓等平台交互的接口；")]
    [Guid("86A4A379-D8C6-4B61-B6BF-49808FF7EEFB")]
    [ComponentKit(EKit.Professional)]
    [ComponentOption(EComponentOptionType.Optional)]
    [Version("22.301")]
    [Index(index = IndexAttribute.DefaultIndex + 10)]
    public sealed class OSInteractManager : BaseManager<OSInteractManager>
    {
        public OSInteract interact { get; private set; }

#region Unity --> OS

#if !UNITY_EDITOR
#if UNITY_IPHONE
		[DllImport("__Internal")]
        public static extern void UnityToIOS(string jsonString);
#endif

#if UNITY_WEBGL && UNITY_5_6_OR_NEWER
        [DllImport("__Internal")]
        public static extern void UnityToJS(string jsonString);
#endif
#endif// !UNITY_EDITOR

        /// <summary>
        /// Unity --> OS 发送数据
        /// </summary>
        /// <param name="jsonString"></param>
        public void SendMessageToOS(string jsonString)
        {
#if !UNITY_5_6_OR_NEWER
            if (!runtimeInfo.NeedSendMessage()) return;
#endif
            try
            {
                if (showLogOfToOS) Log.DebugFormat("Unity --> OS: {0}", jsonString);
                interact.UnityToOS(jsonString);
            }
            catch (Exception ex)
            {
                Log.Exception(nameof(SendMessageToOS) + "异常: " + ex.ToString());
            }
        }

        private void SendMessageToOS(string msgCmd, JsonData msg)
        {
            //if (string.IsNullOrEmpty(msgCmd) || msg == null) return;
            var jsonData = new JsonData();
            jsonData[MsgCmd] = msgCmd;
            jsonData[Msg] = msg;
            SendMessageToOS(jsonData.ToJson());
        }

        private void SendMessageToOS(EUnityToOSMsgCmd msgCmd, params string[] paramArray)
        {
            var msg = new JsonData();
            msg[EVariableName.activeSceneName.ToString()] = activeSceneName;
            if (paramArray == null)
            {
                msg[EVariableName.paramCount.ToString()] = "0";
            }
            else
            {
                msg[EVariableName.paramCount.ToString()] = paramArray.Length.ToString();
                //索引由1开始
                for (int i = 1; i <= paramArray.Length; ++i)
                {
                    msg[string.Format("{0}{1}", EVariableName.param.ToString(), i.ToString())] = paramArray[i - 1];
                }
            }
            SendMessageToOS(msgCmd.ToString(), msg);
        }

#endregion

#region OS --> Unity

        /// <summary>
        /// 当收到 OS --> Unity 发送的数据时；
        /// </summary>
        /// <param name="jsonString">传递的信息</param>
        public void OnOSSendMessageToUnity(string jsonString)
        {
            if (showLogOfToUnity) Log.DebugFormat("OS --> Unity: {0}", jsonString);
            var scriptManager = ScriptManager.instance;
            if (!scriptManager)
            {
                Debug.LogErrorFormat("{0} 未启用或无效，无法处理OS传送到Unity的数据: {1}", CommonFun.Name(typeof(ScriptManager)), jsonString);
                return;
            }

            try
            {
                JsonData jsondata = JsonMapper.ToObject(jsonString);

                switch (CommonFun.StringToEnum<EOSToUnityMsgCmd>(jsondata.objectValue[MsgCmd].ToString(), EOSToUnityMsgCmd.None))
                {
                    case EOSToUnityMsgCmd.ImportAndLoadScene:
                        {
                            string scenePath = jsondata[Msg][EVariableName.scenePath.ToString()].ToString();
                            string sceneName = jsondata[Msg][EVariableName.sceneName.ToString()].ToString();
                            ImportAndLoadScene(scenePath, sceneName);
                            break;
                        }
                    case EOSToUnityMsgCmd.ImportScene:
                        {
                            string scenePath = jsondata[Msg][EVariableName.scenePath.ToString()].ToString();
                            string sceneName = jsondata[Msg][EVariableName.sceneName.ToString()].ToString();
                            ImportScene(scenePath, sceneName);
                            break;
                        }
                    case EOSToUnityMsgCmd.LoadScene:
                        {
                            string scenePath = jsondata[Msg][EVariableName.scenePath.ToString()].ToString();
                            string sceneName = jsondata[Msg][EVariableName.sceneName.ToString()].ToString();
                            LoadScene(scenePath, sceneName);
                            break;
                        }
                    case EOSToUnityMsgCmd.LoadOrImportAndLoadScene:
                        {
                            string scenePath = jsondata[Msg][EVariableName.scenePath.ToString()].ToString();
                            string sceneName = jsondata[Msg][EVariableName.sceneName.ToString()].ToString();
                            LoadOrImportAndLoadScene(scenePath, sceneName);
                            break;
                        }
                    case EOSToUnityMsgCmd.UnloadSubScene:
                        {
                            string sceneName = jsondata[Msg][EVariableName.sceneName.ToString()].ToString();
                            var ret = SceneAssetsManager.UnloadScene(sceneName);
                            SendMessageToOS(EUnityToOSMsgCmd.UnloadSubSceneFinish, sceneName, ret.ToString());
                            break;
                        }
                    case EOSToUnityMsgCmd.UnloadSubSceneByIndex:
                        {
                            var sceneIndex = CommonFun.StringToInt(jsondata[Msg][EVariableName.sceneIndex.ToString()].ToString()) - 1;
                            string sceneName;
                            if (SceneAssetsManager.TryGetSceneName(sceneIndex, out sceneName))
                            {
                                var ret = SceneAssetsManager.UnloadScene(sceneName);
                                SendMessageToOS(EUnityToOSMsgCmd.UnloadSubSceneFinish, sceneName, ret.ToString());
                            }
                            break;
                        }
                    case EOSToUnityMsgCmd.UnloadAllSubScene:
                        {
                            var ret = SceneAssetsManager.UnloadAllSubSceneWithoutActiveScene();
                            SendMessageToOS(EUnityToOSMsgCmd.UnloadAllSubSceneFinish, ret.ToString());
                            break;
                        }
                    case EOSToUnityMsgCmd.RequestSceneNameList:
                        {
                            List<string> sceneNameList = new List<string>(SceneAssetsManager.sceneNameAssetsDictionary.Keys);
                            SendMessageToOS(EUnityToOSMsgCmd.SceneNameList, sceneNameList.ToArray());
                            break;
                        }
                    case EOSToUnityMsgCmd.UserDefine:
                        {
                            string userDefine = jsondata[Msg][EVariableName.userDefine.ToString()].ToString();
                            scriptManager.CallUserDefineFun(userDefineCallbackFunc, userDefine);
                            break;
                        }
                    case EOSToUnityMsgCmd.CallUserDefineFun:
                        {
                            string userDefineFunName = jsondata[Msg][EVariableName.userDefineFunName.ToString()].ToString();
                            string param = jsondata[Msg][EVariableName.param.ToString()].ToString();
                            scriptManager.CallUserDefineFun(userDefineFunName, param);
                            break;
                        }
                    case EOSToUnityMsgCmd.RunXCSJScript:
                        {
                            string xcsjScript = jsondata[Msg][EVariableName.xcsjScript.ToString()].ToString();
                            scriptManager.RunScriptCmds(xcsjScript);
                            break;
                        }
                    case EOSToUnityMsgCmd.RunSingleXCSJScriptAndReturnResult:
                        {
                            string xcsjScript = jsondata[Msg][EVariableName.xcsjScript.ToString()].ToString();
                            SendMessageToOS(EUnityToOSMsgCmd.SingleXCSJScriptRunResult, xcsjScript, scriptManager.RunScriptCmd(xcsjScript).ToStringWithPrefix());
                            break;
                        }
                    case EOSToUnityMsgCmd.RequestImageQRCodeScan:
                        {
                            string imagePath = jsondata[Msg][EVariableName.imagePath.ToString()].ToString();
                            string otherInfo = jsondata[Msg][EVariableName.otherInfo.ToString()].ToString();
                            scriptManager.TrySetOrAddSetVarValue(EVariableName.imagePath.ToString(), imagePath);// 二维码图片文件名
                            scriptManager.TrySetOrAddSetVarValue(EVariableName.otherInfo.ToString(), otherInfo);// 其他信息
                            scriptManager.CallUserDefineFun(imageQRCodeScanFunc, jsonString);
                            break;
                        }
                    default:
                        {
                            //没有可处理消息命令时，调用缺省回调函数
                            scriptManager.CallUserDefineFun(defaultCallbackFunc, jsonString);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(nameof(OnOSSendMessageToUnity) + "异常: " + ex.ToString());
                //处理发生异常时，调用缺省回调函数
                scriptManager.CallUserDefineFun(defaultCallbackFunc, jsonString);
            }
        }

        private void AnalogOSSendMessageToUnity(string msgCmd, JsonData msg)
        {
            var jsonData = new JsonData();
            jsonData[MsgCmd] = msgCmd;
            jsonData[Msg] = msg;

            OnOSSendMessageToUnity(jsonData.ToJson());
        }

        private void AnalogOSSendMessageToUnity(EOSToUnityMsgCmd msgCmd, string key1 = "", string value1 = "", string key2 = "", string value2 = "")
        {
            var msg = new JsonData();
            if (!string.IsNullOrEmpty(key1)) msg[key1] = value1;
            if (!string.IsNullOrEmpty(key2)) msg[key2] = value2;

            AnalogOSSendMessageToUnity(msgCmd.ToString(), msg);
        }

#endregion

#region OS -> Unity 回调函数

        [Group("OS -> Unity 回调函数", tooltip = "OS调用Unity执行特定操作的回调函数")]
        [Name("缺省回调函数")]
        [Tip("用于处理程序中未内置的消息命令或无法转化为JSON的消息字符串时执行的回调函数,原始字符串信息存储在回调函数的传入参数中；")]
        [UserDefineFun]
        public string defaultCallbackFunc = "";

        [Name("用户自定义回调函数")]
        [Tip("当收到用户自定义消息时执行的回调函数,对应的用户自定义消息存储在回调函数的传入参数中；")]
        [UserDefineFun]
        public string userDefineCallbackFunc = "";

        [Name("图片二维码扫描函数")]
        [Tip("提交给Unity场景扫描图片的二维码信息,会将图片的路径存储在全局变量imagePath中，可能携带的其他信息存放在全局变量otherInfo中")]
        [UserDefineFun]
        public string imageQRCodeScanFunc = "";

#endregion

#region 场景处理

        private enum ESceneHandleState
        {
            Free = 0,
            ImportAndLoad,
            Import,
            Load,
        }

        private ESceneHandleState sceneHandleState = ESceneHandleState.Free;

        [Serializable]
        public class SceneHandleInfo
        {
            [Name("回调函数")]
            [Tip("会将场景文件的路径存储在全局变量scenePath中，场景名存储在全局变量sceneName中；")]
            [UserDefineFun]
            public string callbackFunc = "";

            [Name("自动处理")]
            [Tip("勾选后，将自动处理对应任务；否则需要用户自己编写中文脚本语句在回调函数中做后续处理；")]
            public bool autoHandle = true;

            [Name("发送定时消息")]
            [Tip("勾选后，在自动处理任务时会向OS发送定时消息；该定时消息包含进度[0,1]、场景名信息；")]
            [HideInSuperInspector(nameof(autoHandle), EValidityCheckType.False)]
            public bool sendTimedMessage = true;

            [Name("进度回调函数")]
            [Tip("自动处理任务时进度信息的回调函数；进度信息使用传入参数获取；进度范围[0,1]；")]
            [UserDefineFun]
            [HideInSuperInspector(nameof(autoHandle), EValidityCheckType.False)]
            public string progressCallbackFunc = "";

            [Name("完成时回调函数")]
            [Tip("自动处理任务完成后的回调函数;正常处理完成后，本回调函数的传入参数为#True; 当处理过程发生错误时，不会回调本函数；")]
            [UserDefineFun]
            [HideInSuperInspector(nameof(autoHandle), EValidityCheckType.False)]
            public string callbackFuncWhenFinish = "";

            [Name("失败时回调函数")]
            [Tip("自动处理任务过程中发生错误时的回调函数；当处理发生错误时，本回调函数的传入参数项为#False或具体的错误信息；")]
            [UserDefineFun]
            [HideInSuperInspector(nameof(autoHandle), EValidityCheckType.False)]
            public string callbackFuncWhenFail = "";

            /// <summary>
            /// 当前正在处理的场景名称
            /// </summary>
            internal string sceneName = "";
        }

#endregion

        [EndGroup]
        [Name("导入并加载场景")]
        [Tip("会将场景文件的路径存储在全局变量scenePath中，场景名存储在全局变量sceneName中；如果场景被成功加载,'完成时回调函数'不会被执行；")]
        public SceneHandleInfo importAndLoadScene = new SceneHandleInfo();

        [Name("导入场景")]
        [Tip("会将场景文件的路径存储在全局变量scenePath中，场景名存储在全局变量sceneName中；")]
        public SceneHandleInfo importScene = new SceneHandleInfo();

        [Name("加载场景")]
        [Tip("会将场景文件的路径存储在全局变量scenePath中，场景名存储在全局变量sceneName中；如果场景被成功加载,'完成时回调函数'不会被执行；")]
        public SceneHandleInfo loadScene = new SceneHandleInfo();

#region 主场景设置

        [Group("主场景设置", tooltip = "当前场景作为主场景时，额外需要设置的参数，推荐全部启用；作为子场景(分场景)时也可设置，但通常情况下不推荐对参数做修改；")]
        [Name("启动时发送unity引擎启动完成")]
        [Tip("**特别注意：普通场景切勿勾选！！当前场景加载完成后会向OS(Andriod的JAVA,IOS的Object-C) 发送Unity引擎启动完成的回调消息；**")]
        public bool sendUnityEngineLoadedFinishWhenStart = false;

        [Name("启动时卸载全部子场景")]
        [Tip("启动时会将除主场景(索引为0、程序启动时的默认场景)与当前场景外全部卸载,即将场景数据从Unity后台内存中卸载(会卸载该场景所有的资源对象信息);")]
        public bool unloadAllSubSceneWhenStart = false;

        [Name("启动时加载后续场景")]
        [Tip("普通场景中切勿勾选；用于完成中文脚本'通过主场景切换场景'的后续操作；当后续场景已被导入，则仅执行加载场景操作；否则执行导入并加载场景操作;")]
        public bool loadNextSceneWhenStart = false;

        [Name("失败时场景处理规则")]
        [Tip("当场景切换、打开后续场景等切换（加载）场景类型的任务处理失败时的处理规则；")]
        [EnumPopup]
        public ESceneHandleRuleWhenFail sceneHandleRuleWhenFail = ESceneHandleRuleWhenFail.None;

        [Name("发送定时消息")]
        [Tip("无导入或加载任务时，是否发送定时消息（即每过特定的间隔时间时间会向OS发送定时信息）;")]
        public bool sendTimedMessage = false;

        [Name("间隔时间")]
        [Tip("发送定时消息的间隔时间;单位为秒;")]
        [Range(0f, 5f)]
        [HideInSuperInspector(nameof(sendTimedMessage), EValidityCheckType.False)]
        public float intervalTimeOfTimedMessage = 2f;

        /// <summary>
        /// 定时消息的增量时间
        /// </summary>
        private float deltaTimeOfTimedMessage = 0;

        [Name("显示OS->Unity日志信息")]
        public bool showLogOfToUnity = false;

        [Name("显示Unity->OS日志信息")]
        public bool showLogOfToOS = false;

#endregion

        [EndGroup]
        [Name("启动时向OS发送场景加载完成")]
        [Tip("在当前场景加载完成后会向OS(Andriod的JAVA,IOS的Object-C) 发送场景加载完成的回调消息")]
        public bool sendLoadSceneFinishToOSWhenStart = true;

#region 运行时信息
#if !UNITY_5_6_OR_NEWER
        [Name("运行时信息")]
        public RuntimeInfo runtimeInfo = new RuntimeInfo();
#endif
#endregion

        public static readonly string MsgCmd = EVariableName.MsgCmd.ToString();

        public static readonly string Msg = EVariableName.Msg.ToString();

        private string activeSceneName = "";

        /// <summary>
        /// 启动
        /// </summary>
        public override void Start()
        {
            base.Start();
            //获取当前场景名称
            activeSceneName = SceneManager.GetActiveScene().name;
            if (sendUnityEngineLoadedFinishWhenStart)
            {
                SendMessageToOS(EUnityToOSMsgCmd.UnityEngineLoadedFinish);
            }
            if (sendLoadSceneFinishToOSWhenStart)
            {
                var scene = SceneManager.GetActiveScene();
                SendMessageToOS(EUnityToOSMsgCmd.LoadSceneFinish, scene.name, scene.path);
            }
            if (unloadAllSubSceneWhenStart)
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
#endif
                    SceneAssetsManager.UnloadAllSubSceneWithoutActiveScene();

#if UNITY_EDITOR
                }
#endif
            }
            if (loadNextSceneWhenStart)
            {
                LoadNextScene();
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public override void Update()
        {
            switch (sceneHandleState)
            {
                case ESceneHandleState.ImportAndLoad://有导入或加载操作
                    {
                        float p;
                        if (SceneAssetsManager.TryGetAsyncImportAndLoadSceneProgress(out p))
                        {
                            ScriptManager.CallUDF(importAndLoadScene.progressCallbackFunc, p.ToString());

                            if (importAndLoadScene.sendTimedMessage)//用户期望发送定时消息
                            {
                                SendMessageToOS(EUnityToOSMsgCmd.TimedMessageInImportAndLoad, p.ToString(), importAndLoadScene.sceneName);
                            }
                        }
                        else
                        {
                            sceneHandleState = ESceneHandleState.Free;
                        }
                        break;
                    }
                case ESceneHandleState.Import: //有导入操作
                    {
                        float p;
                        if (SceneAssetsManager.TryGetAsyncImportSceneProgress(out p))
                        {
                            ScriptManager.CallUDF(importScene.progressCallbackFunc, p.ToString());

                            if (importScene.sendTimedMessage)//用户期望发送定时消息
                            {
                                SendMessageToOS(EUnityToOSMsgCmd.TimedMessageInImport, p.ToString(), importScene.sceneName);
                            }
                        }
                        else
                        {
                            sceneHandleState = ESceneHandleState.Free;
                        }
                        break;
                    }
                case ESceneHandleState.Load://有加载操作
                    {
                        float p;
                        if (SceneAssetsManager.TryGetAsyncLoadSceneProgress(out p))
                        {
                            ScriptManager.CallUDF(loadScene.progressCallbackFunc, p.ToString());

                            if (loadScene.sendTimedMessage)//用户期望发送定时消息
                            {
                                SendMessageToOS(EUnityToOSMsgCmd.TimedMessageInLoad, p.ToString(), loadScene.sceneName);
                            }
                        }
                        else
                        {
                            sceneHandleState = ESceneHandleState.Free;
                        }
                        break;
                    }
                default://无导入或加载操作
                    {
                        if (SceneAssetsManager.IsBusy())
                        {
                            //有导入或加载场景操作在执行中...
                        }
                        else if (sendTimedMessage)//用户期望发送定时消息
                        {
                            deltaTimeOfTimedMessage += Time.deltaTime;
                            if (deltaTimeOfTimedMessage >= intervalTimeOfTimedMessage)
                            {
                                //发送定时消息
                                SendMessageToOS(EUnityToOSMsgCmd.TimedMessage, Time.realtimeSinceStartup.ToString(), deltaTimeOfTimedMessage.ToString());
                                deltaTimeOfTimedMessage = 0;
                            }
                        }
                        break;
                    }
            }
        }

        public override void Reset()
        {
            base.Reset();
#if !UNITY_5_6_OR_NEWER
            runtimeInfo.Reset();
#endif
        }

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            interact = OSInteract.FindOrCreate(this);
            interact.onOSToUnity += OnOSSendMessageToUnity;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            interact.onOSToUnity -= OnOSSendMessageToUnity;
        }

        /// <summary>
        /// 获取这个插件脚本
        /// </summary>
        /// <returns></returns>
        public override List<Script> GetScripts()
        {
            return Script.GetScriptsOfEnum<EOSInteractScriptID>(this, (int)EOSInteractScriptID._Begin + 1, (int)EOSInteractScriptID.MaxCurrent);
        }

        /// <summary>
        /// 运行对应脚本
        /// </summary>
        /// <param name="id">脚本ID</param>
        /// <param name="param">脚本传递参数</param>
        /// <returns></returns>
        public override ReturnValue RunScript(int id, ScriptParamList param)
        {
            switch ((EOSInteractScriptID)id)
            {
                case EOSInteractScriptID.SendMessageToOS:
                    {
                        SendMessageToOS((EUnityToOSMsgCmd)param[0], param[1] as string, param[2] as string, param[3] as string, param[4] as string);
                        return ReturnValue.Yes;
                    }
                case EOSInteractScriptID.BackOS:
                    {
#region 返回OS
                        //记录当前场景信息
                        RecordCurrentSceneInfo();

                        switch (param[1] as string)
                        {
                            case "直接返回": break;
                            case "返回并加载主场景":
                            default:
                                {
                                    LoadMainScene();
                                    break;
                                }
                        }

                        //最后发送返回OS消息
                        BackOS(param[2] as string, param[3] as string);
                        return ReturnValue.Yes;
#endregion
                    }
                case EOSInteractScriptID.SwitchSceneByOS:
                    {
#region  通过OS切换场景

                        //记录当前场景信息
                        RecordCurrentSceneInfo();

                        SendMessageToOS(EUnityToOSMsgCmd.SwitchSceneByOS, param[1] as string, param[2] as string, param[3] as string);
                        StaticVarHelper.TrySetOrAddSetVarValue(ScriptManager.instance, nameof(EVariableName.NextSceneRuleWhenFail), ((ESceneHandleRuleWhenFail)param[4]).ToString());
                        return ReturnValue.Yes;
#endregion
                    }
                case EOSInteractScriptID.SwitchSceneByMainScene:
                    {
                        #region SwitchSceneByMainScene
                        //设置期望切换的场景信息
                        StaticVarHelper.TrySetOrAddSetVarValue(ScriptManager.instance, nameof(EVariableName.NextScenePath), param[1] as string);
                        StaticVarHelper.TrySetOrAddSetVarValue(ScriptManager.instance, nameof(EVariableName.NextSceneName), param[2] as string);
                        StaticVarHelper.TrySetOrAddSetVarValue(ScriptManager.instance, nameof(EVariableName.NextSceneRuleWhenFail), ((ESceneHandleRuleWhenFail)param[3]).ToString());

                        //记录当前场景信息
                        RecordCurrentSceneInfo();

                        //加载主场景
                        LoadMainScene();
                        break;
#endregion
                    }
                case EOSInteractScriptID.HandleSwitchSceneFail:
                    {
#region HandleSwitchSceneFailed
                        return ReturnValue.Create(HandleLoadSceneFail());
#endregion
                    }
                case EOSInteractScriptID.GetPreviousSceneInfo:
                    {
#region GetPreviousSceneInfo
                        switch (param[1] as string)
                        {
                            case "场景全路径":
                                {                                    
                                    if (StaticVarHelper.TryGetVarValue(ScriptManager.instance, nameof(EVariableName.PreviousScenePath), out var value))
                                    {
                                        return ReturnValue.True(value);
                                    }
                                    break;
                                }
                            case "场景名称":
                                {
                                    if (StaticVarHelper.TryGetVarValue(ScriptManager.instance, nameof(EVariableName.PreviousSceneName), out var value))
                                    {
                                        return ReturnValue.True(value);
                                    }
                                    break;
                                }
                        }
                        break;
#endregion
                    }
                case EOSInteractScriptID.AnalogOSSendMessageToUnity:
                    {
#if UNITY_EDITOR
                        if (!Application.isPlaying) throw new InvalidOperationException("仅在播放模式(即运行时)可使用‘模拟OS向Unity发送消息’的脚本命令!");

#endif
                        AnalogOSSendMessageToUnity((EOSToUnityMsgCmd)param[1], param[2].ToString(), param[3].ToString(), param[4].ToString(), param[5].ToString());
                        break;
                    }
            }
            return new ReturnValue();
        }

        private void RecordCurrentSceneInfo()
        {
            var scene = SceneManager.GetActiveScene();
            StaticVarHelper.TrySetOrAddSetVarValue(ScriptManager.instance, nameof(EVariableName.PreviousScenePath), scene.path);
            StaticVarHelper.TrySetOrAddSetVarValue(ScriptManager.instance, nameof(EVariableName.PreviousSceneName), scene.name);
        }

        private bool HandleScene(string scenePath, string sceneName, SceneHandleInfo sceneHandleInfo, ESceneHandleState sceneHandleStateIfFinish, EUnityToOSMsgCmd messageIfFail, bool handleLoadSceneFail, Func<bool> autoHandleFunc)
        {
            //直接触发回调函数
            var scriptManager = ScriptManager.instance;
            if (scriptManager)
            {
                scriptManager.TrySetOrAddSetVarValue(EVariableName.scenePath.ToString(), scenePath);// u3d资源包位置
                scriptManager.TrySetOrAddSetVarValue(EVariableName.sceneName.ToString(), sceneName);// 场景名
                scriptManager.CallUserDefineFun(sceneHandleInfo.callbackFunc, sceneHandleStateIfFinish.ToString());//直接回调自定义函数
            }

            //如果需要自动处理
            if (sceneHandleInfo.autoHandle)
            {
                sceneHandleInfo.sceneName = sceneName;
                if (autoHandleFunc())
                {
                    this.sceneHandleState = sceneHandleStateIfFinish;
                }
                else
                {
                    SendMessageToOS(messageIfFail, sceneName, scenePath);
                    ScriptManager.CallUDF(loadScene.callbackFuncWhenFail, string.Format("场景[{0}]({1})自动处理启动失败!", sceneName, scenePath));
                    if (handleLoadSceneFail) HandleLoadSceneFail();
                    return false;
                }
            }

            return true;
        }

        public bool ImportAndLoadScene(string scenePath, string sceneName)
        {
            return HandleScene(scenePath, sceneName, importAndLoadScene, ESceneHandleState.ImportAndLoad, EUnityToOSMsgCmd.ImportAndLoadSceneFailed, true, () =>
            {
                return SceneAssetsManager.ImportAndLoadSceneAsync(scenePath, sceneName, LoadSceneMode.Single, (ok, error) =>
                {
                    if (ok)
                    {
                        //如果场景成功加载后，之前场景信息已被卸载，不再执行操作
                        //SendMessageToOS(EUnityToOSMsgCmd.LoadSceneFinish, sceneName, scenePath);
                        //ScriptManager.CallUDF(importAndLoadScene.callbackFuncWhenFinish, sceneName);
                    }
                    else
                    {
                        SendMessageToOS(EUnityToOSMsgCmd.ImportAndLoadSceneFailed, sceneName, scenePath);
                        ScriptManager.CallUDF(importAndLoadScene.callbackFuncWhenFail, string.Format("场景[{0}]({1})自动处理失败: {2}", sceneName, scenePath, error));
                        HandleLoadSceneFail();
                    }
                });
            });
        }

        public bool ImportScene(string scenePath, string sceneName)
        {
            return HandleScene(scenePath, sceneName, importScene, ESceneHandleState.Import, EUnityToOSMsgCmd.ImportSceneFailed, false, () =>
            {
                return SceneAssetsManager.ImportSceneAsync(scenePath, sceneName, (ok, error) =>
                {
                    if (ok)
                    {
                        SendMessageToOS(EUnityToOSMsgCmd.ImportSceneFinish, sceneName, scenePath);
                        ScriptManager.CallUDF(importScene.callbackFuncWhenFinish, sceneName);
                    }
                    else
                    {
                        SendMessageToOS(EUnityToOSMsgCmd.ImportSceneFailed, sceneName, scenePath);
                        ScriptManager.CallUDF(importScene.callbackFuncWhenFail, string.Format("场景[{0}]({1})自动处理失败: {2}", sceneName, scenePath, error));
                    }
                });
            });
        }

        public bool LoadScene(string scenePath, string sceneName)
        {
            return HandleScene(scenePath, sceneName, loadScene, ESceneHandleState.Load, EUnityToOSMsgCmd.LoadSceneFailed, true, () =>
            {
                return SceneAssetsManager.LoadSceneAsync(sceneName, LoadSceneMode.Single, (ok, error) =>
                {
                    if (ok)
                    {
                        //如果场景成功加载后，之前场景信息已被卸载，不再执行操作
                        //SendMessageToOS(EUnityToOSMsgCmd.LoadSceneFinish, sceneName, scenePath);
                        //ScriptManager.CallUDF(loadScene.callbackFuncWhenFinish, sceneName);
                    }
                    else
                    {
                        SendMessageToOS(EUnityToOSMsgCmd.LoadSceneFailed, sceneName, scenePath);
                        ScriptManager.CallUDF(loadScene.callbackFuncWhenFail, string.Format("场景[{0}]({1})自动处理失败: {2}", sceneName, scenePath, error));
                        HandleLoadSceneFail();
                    }
                });
            });
        }

        public bool LoadOrImportAndLoadScene(string scenePath, string sceneName)
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                if (SceneAssetsManager.sceneNameAssetsDictionary.ContainsKey(sceneName))
                {
                    //执行加载操作
                    return LoadScene(scenePath, sceneName);
                }
                else if (!string.IsNullOrEmpty(scenePath))
                {
                    //执行导入并加载操作
                    return ImportAndLoadScene(scenePath, sceneName);
                }
            }
            return HandleLoadSceneFail();
        }

        private bool SwitchScene(string scenePathVarName, string sceneNameVarName)
        {            
            if (StaticVarHelper.TryGetVarValue(ScriptManager.instance, scenePathVarName, out var scenePath))
            {
                StaticVarHelper.TryRemoveVar(ScriptManager.instance, scenePathVarName);
            }
            if (StaticVarHelper.TryGetVarValue(ScriptManager.instance, sceneNameVarName, out var sceneName))
            {
                StaticVarHelper.TryRemoveVar(ScriptManager.instance, sceneNameVarName);
            }
            //必须有场景名称才允许切换场景，否则认为是一个无效的处理命令！
            if (string.IsNullOrEmpty(sceneName)) return false;
            return LoadOrImportAndLoadScene(scenePath, sceneName);
        }

        private bool LoadNextScene()
        {
            return SwitchScene(EVariableName.NextScenePath.ToString(), EVariableName.NextSceneName.ToString());
        }

        private bool LoadPreviousScene()
        {
            return SwitchScene(EVariableName.PreviousScenePath.ToString(), EVariableName.PreviousSceneName.ToString());
        }

        private bool HandleLoadSceneFail()
        {
            //优先使用用户最新设定的处理逻辑
            if (StaticVarHelper.TryGetVarValue(ScriptManager.instance, nameof(EVariableName.NextSceneRuleWhenFail), out var value))
            {
                StaticVarHelper.TryRemoveVar(ScriptManager.instance, nameof(EVariableName.NextSceneRuleWhenFail));
            }
            if (string.IsNullOrEmpty(value))//使用参数设置的失败时场景处理规则
            {
                return HandleLoadSceneFail(sceneHandleRuleWhenFail);
            }
            else
            {
                return HandleLoadSceneFail(EnumHelper.StringToEnum<ESceneHandleRuleWhenFail>(value, ESceneHandleRuleWhenFail.None));
            }
        }

        private bool HandleLoadSceneFail(ESceneHandleRuleWhenFail rule)
        {
            //Debug.LogFormat("HandleLoadSceneFail: {0}", rule);
            switch (rule)
            {
                case ESceneHandleRuleWhenFail.LoadPreviousScene:
                    {
                        if (!LoadPreviousScene())
                        {
                            HandleLoadSceneFail(ESceneHandleRuleWhenFail.LoadMainSceneAndBackOS);
                        }
                        break;
                    }
                case ESceneHandleRuleWhenFail.LoadMainSceneAndBackOS:
                    {
                        LoadMainScene();
                        BackOS();
                        break;
                    }
                case ESceneHandleRuleWhenFail.BackOS:
                    {
                        BackOS();
                        break;
                    }
                case ESceneHandleRuleWhenFail.LoadMainScene:
                    {
                        LoadMainScene();
                        break;
                    }
                case ESceneHandleRuleWhenFail.ApplicationQuit:
                    {
                        Application.Quit();
                        break;
                    }
                default:
                    {
                        return false;
                    }
            }
            return true;
        }

        private void BackOS(string param1 = "", string param2 = "")
        {
            SendMessageToOS(EUnityToOSMsgCmd.BackOS, param1, param2);
        }

        private void LoadMainScene()
        {
            //加载主场景
            SceneAssetsManager.LoadMainScene();
        }
    }
}
