using System;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.DataBase;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Others
{
    /// <summary>
    /// 功能测试
    /// </summary>
    [Name(Title, nameof(FunctionTest))]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Function)]
    public class FunctionTest : LifecycleExecutor<FunctionTest>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "功能测试";

        /// <summary>
        /// Field JSON字符串
        /// </summary>
        [Name("Field JSON字符串")]
        [TextArea]
        public string fieldJsonString = "";

        /// <summary>
        /// FieldSet JSON字符串
        /// </summary>
        [Name("FieldSet JSON字符串")]
        [TextArea]
        public string fieldSetJsonString = "";

        private T JsonTest<T>(string jsonString) where T : class
        {
            try
            {
                var obj = JsonHelper.ToObject<T>(jsonString);
                if (obj == null)
                {
                    Log.WarningFormat(nameof(FunctionTest) + "将[{0}]转为[{1}]类型失败！", jsonString, typeof(T).Name);
                }
                else
                {
                    Log.DebugFormat(nameof(FunctionTest) + "将[{0}]转为[{1}]类型成功:[{2}]", jsonString, typeof(T).Name, JsonHelper.ToJson(obj));
                }
                return obj;
            }
            catch (Exception ex)
            {
                Log.ExceptionFormat(nameof(FunctionTest) + "将[{0}]转为[{1}]类型时异常:{2}", jsonString, typeof(T).Name, ex);
                return default;
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            var field = JsonTest<Field>(fieldJsonString);
            if (field != null)
            {
                var newField = new DataBase.Field();
                newField.index = field.index;
                newField.name = field.name;
                Debug.LogFormat("索引:{0}, 名称:{1}", newField.index, newField.name);
            }
        }
    }
}
