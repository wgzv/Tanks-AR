using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.Base
{
    /// <summary>
    /// 值类型
    /// </summary>
    [Name("值类型")]
    public enum EValueType
    {
        [Name("值")]
        Value = 0,

        [Name("变量")]
        Variable,

        [Name("游戏对象名称")]
        GameOjbectName,

        [Name("游戏对象路径名")]
        GameOjbectNamePath,

        [Name("游戏对象别名")]
        [Tip("游戏对象无别名时，返回游戏对象路径名")]
        GameOjbectAlias,
    }

    /// <summary>
    /// 值类型扩展
    /// </summary>
    public static class ValueTypeExtension
    {
        /// <summary>
        /// 根据值类型获取值
        /// </summary>
        /// <param name="valueType"></param>
        /// <param name="value"></param>
        /// <param name="go"></param>
        /// <returns></returns>
        public static string GetValue(this EValueType valueType, string value, GameObject go)
        {
            switch (valueType)
            {
                case EValueType.Value: return value;
                case EValueType.Variable: return ScriptManager.TryGetGlobalVariableValue(value, out string tmpValue) ? tmpValue : "";
                case EValueType.GameOjbectName: return go ? go.name : "";
                case EValueType.GameOjbectNamePath: return CommonFun.GameObjectToStringWithoutAlias(go);
                case EValueType.GameOjbectAlias: return CommonFun.GameObjectToString(go);
                default: return "";
            }
        }

        /// <summary>
        /// 根据值类型获取值字符串
        /// </summary>
        /// <param name="valueType"></param>
        /// <param name="value"></param>
        /// <param name="go"></param>
        /// <returns></returns>
        public static string GetValueString(this EValueType valueType, string value, GameObject go)
        {
            switch (valueType)
            {
                case EValueType.Value: return value;
                case EValueType.Variable: return ScriptOption.VarFlag + value;
                case EValueType.GameOjbectName: return go ? go.name : "";
                case EValueType.GameOjbectNamePath: return CommonFun.GameObjectToStringWithoutAlias(go);
                case EValueType.GameOjbectAlias: return CommonFun.GameObjectToString(go);
                default: return "";
            }
        }
    }
}
