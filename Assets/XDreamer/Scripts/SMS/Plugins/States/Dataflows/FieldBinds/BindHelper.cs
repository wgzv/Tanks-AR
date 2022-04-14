using System;
using System.Collections;
using System.Collections.Generic;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Helper;
using XCSJ.PluginSMS.States.Dataflows.FieldBinds.Base;

namespace XCSJ.PluginSMS.States.Dataflows.FieldBinds
{
    /// <summary>
    /// 绑定助手
    /// </summary>
    public class BindHelper
    {
        /// <summary>
        /// 绑定规则枚举转为简写字符串
        /// </summary>
        /// <param name="bindRule">绑定规则</param>
        /// <returns>绑定规则的简化字符串</returns>
        public static string ToAbbreviations(EBindRule bindRule)
        {
            switch (bindRule)
            {
                case EBindRule.None: return "×";
                case EBindRule.VariableToObject: return "→";
                case EBindRule.ObjectToVariable: return "←";
                case EBindRule.Both: return "↔";
                default: return "?";
            }
        }

        /// <summary>
        /// 字符串转化为对象
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="liteMode">简易模式</param>
        /// <param name="liteSeparator">简易模式的分隔符</param>
        /// <param name="type">类型</param>
        /// <returns>指定类型的对象实例</returns>
        public static object StringToObject(string value, bool liteMode, string liteSeparator, Type type)
        {
            object o;
            if (type.IsArray)//Array
            {
                var elementType = TypeHelper.GetElementType(type);
                if (liteMode)
                {
                    var array = value.Split(new string[] { liteSeparator }, StringSplitOptions.None);
                    int count = array.Length;
                    var oArray = Array.CreateInstance(elementType, count);
                    for (int i = 0; i < count; i++)
                    {
                        oArray.SetValue(Converter.instance.ConvertTo(array[i], elementType), i);
                    }
                    o = oArray;
                }
                else
                {
                    var list = JsonHelper.ToObject<List<string>>(value);
                    int count = list.Count;
                    var oArray = Array.CreateInstance(elementType, count);
                    for (int i = 0; i < count; i++)
                    {
                        oArray.SetValue(Converter.instance.ConvertTo(list[i], elementType), i);
                    }
                    o = oArray;
                }
            }
            else if (typeof(IList).IsAssignableFrom(type))//List
            {
                var elementType = TypeHelper.GetElementType(type);
                if (liteMode)
                {
                    var array = value.Split(new string[] { liteSeparator }, StringSplitOptions.None);
                    var oList = (IList)TypeHelper.CreateInstance(type);
                    foreach (var v in array)
                    {
                        oList.Add(Converter.instance.ConvertTo(v, elementType));
                    }
                    o = oList;
                }
                else
                {
                    var list = JsonHelper.ToObject<List<string>>(value);
                    var oList = (IList)TypeHelper.CreateInstance(type);
                    foreach (var v in list)
                    {
                        oList.Add(Converter.instance.ConvertTo(v, elementType));
                    }
                    o = oList;
                }
            }
            else if (Converter.instance.TryConvertTo(value, type, out o))
            {
                //
            }
            return o;
        }

        /// <summary>
        /// 对象转化为字符串
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="liteMode">简易模式</param>
        /// <param name="liteSeparator">简易模式的分隔符</param>
        /// <returns>转化后的字符串</returns>
        public static string ObjectToString(object o, bool liteMode, string liteSeparator)
        {
            string value = "";
            if (o is Array)//Array
            {
                var list = new List<string>();
                foreach (object current in ((Array)o))
                {
                    list.Add(Converter.instance.ConvertTo<string>(current));
                }
                if (liteMode)
                {
                    value = list.ToStringDirect(liteSeparator);
                }
                else
                {
                    value = JsonHelper.ToJson(list);
                }
            }
            else if (o is IList)//List
            {
                var list = new List<string>();
                foreach (object current in ((IList)o))
                {
                    list.Add(Converter.instance.ConvertTo<string>(current));
                }
                if (liteMode)
                {
                    value = list.ToStringDirect(liteSeparator);
                }
                else
                {
                    value = JsonHelper.ToJson(list);
                }
            }
            else if (Converter.instance.TryConvertTo(o, out value))//最后尝试进行转换器转换
            {
                //
            }
            return value;
        }

        /// <summary>
        /// 更新对象
        /// </summary>
        /// <param name="fieldBind">字段绑定的信息</param>
        /// <param name="objFieldValue">待更新对象的字段值</param>
        /// <param name="args">额外参数信息</param>
        public static void UpdateObject(IFieldBind fieldBind, object objFieldValue, params object[] args)
        {
            if (fieldBind == null || fieldBind.obj == null) return;
            if (fieldBind.obj is IObjectUpdate objectUpdate)
            {
                objectUpdate.ObjectUpdate(fieldBind, objFieldValue, args);
            }
            else
            {
                ObjectUpdaterCache.GetObjectUpdater(fieldBind.obj.GetType())?.ObjectUpdate(fieldBind, objFieldValue, args);
            }
        }
    }

    /// <summary>
    /// 绑定规则
    /// </summary>
    public enum EBindRule
    {
        /// <summary>
        /// 无处理，不做任何绑定；
        /// </summary>
        [Name("无")]
        [Tip("不处理")]
        None,

        /// <summary>
        /// 绑定变量到对象，即变量发生修改时,将数据同步更新到对应的对象上
        /// </summary>
        [Name("变量到对象")]
        [Tip("变量发生修改时,将数据同步更新到对应的对象上")]
        VariableToObject,

        /// <summary>
        /// 绑定对象到变量，即对象发生修改时,将数据同步更新到对应的变量上
        /// </summary>
        [Name("对象到变量")]
        [Tip("对象发生修改时,将数据同步更新到对应的变量上")]
        ObjectToVariable,

        /// <summary>
        /// 双向绑定，即变量或对象发生修改时,将数据同步更新到对应的对象或变量上
        /// </summary>
        [Name("双向")]
        [Tip("变量或对象发生修改时,将数据同步更新到对应的对象或变量上")]
        Both,
    }
}
