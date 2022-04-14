using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.CNScripts;
using System;

namespace XCSJ.PluginSMS.States.Base
{
    /// <summary>
    /// 日志：日志组件是输出日志的执行体，用于调试状态机执行流。
    /// </summary>
    [ComponentMenu(SMSHelperExtension.CommonUseCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(Log))]
    [XCSJ.Attributes.Icon(EIcon.Log)]
    [Tip("日志组件是输出日志的执行体，用于调试状态机执行流。")]
    public class Log : LifecycleExecutor<Log>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "日志";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.CommonUseCategoryName, typeof(SMSManager), categoryIndex = -1)]
        [StateComponentMenu(SMSHelperExtension.CommonUseCategoryName + "/" + Title, typeof(SMSManager), categoryIndex = -1)]
        [Name(Title, nameof(Log))]
        [Tip("日志组件是输出日志的执行体，用于调试状态机执行流。")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("日志级别")]
        [EnumPopup] 
        public ELogLevel logLevel = ELogLevel.Debug;

        [Name("日志标签")]
        public string logTag = "";

        [Name("日志")]
        [TextArea]
        [FormerlySerializedAs(nameof(_log))]
        public string _log = "";

        /// <summary>
        /// 日志
        /// </summary>
        public string log => _log;

        /// <summary>
        /// 日志列表
        /// </summary>
        [Name("日志列表")]

        public List<StringPropertyValue_TextArea> _logs = new List<StringPropertyValue_TextArea>();

        /// <summary>
        /// 日志列表的数据
        /// </summary>
        public string logs => _logs.ToString(s => s.GetValue(), "");

        /// <summary>
        /// 完整日志
        /// </summary>
        public string fullLog => _log + logs;

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            try
            {
                XCSJ.Log.Output(fullLog, logLevel, logTag);
            }
            catch (Exception ex)
            {
                XCSJ.Log.Exception(nameof(Log) + "执行输出时异常:" + ex);
            }
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => log + _logs.ToString(s => s.ToFriendlyString(), "");
    }
}
