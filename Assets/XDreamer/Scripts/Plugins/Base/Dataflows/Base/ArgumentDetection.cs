using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Dataflows.Base
{

    /// <summary>
    /// 实参检测:用于检测形参值(传入的参数值)与实参值(当前对象的存储值）是否符合检测规则
    /// </summary>
    [Serializable]
    [Name("实参检测")]
    [Tip("用于检测形参值(传入的参数值)与实参值(当前对象的存储值）是否符合检测规则")]
    public class ArgumentDetection : Argument
    {
        /// <summary>
        /// 检测规则:对形参值(传入的参数值)与实参值(当前对象的存储值）执行检测的检测规则
        /// </summary>
        [Name("检测规则")]
        [Tip("对形参值(传入的参数值)与实参值(当前对象的存储值）执行检测的检测规则")]
        [EnumPopup]
        public EDetectionRule _detectionRule = EDetectionRule.Equal;

        /// <summary>
        /// 检测顺序:明确形参值(传入的参数值)与实参值(当前对象的存储值）在检测规则中的左值与右值的对应关系
        /// </summary>
        [Name("检测顺序")]
        [Tip("明确形参值(传入的参数值)与实参值(当前对象的存储值）在检测规则中的左值与右值的对应关系")]
        [EnumPopup]
        public EDetectionOrder _detectionOrder = EDetectionOrder.Parameter_Argument;

        /// <summary>
        /// 索引:待检测的形参索引
        /// </summary>
        [Name("索引")]
        [Tip("待检测的形参索引")]
        [Range(0, 7)]
        public int _index = 0;

        /// <summary>
        /// 检查形参值(传入的参数值)与实参值(当前对象的存储值）是否符合检测规则
        /// </summary>
        /// <param name="value">形参值(传入的参数值)</param>
        /// <returns></returns>
        public virtual bool Check(object value)
        {
            switch (_detectionOrder)
            {
                case EDetectionOrder.Parameter_Argument: return _detectionRule.Check(value, this.value);
                case EDetectionOrder.Argument_Parameter: return _detectionRule.Check(this.value, value);
                default: return false;
            }
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            switch (_detectionOrder)
            {
                case EDetectionOrder.Parameter_Argument: return string.Format("{0} {1}", _detectionRule.ToAbbreviations(), base.ToFriendlyString());
                case EDetectionOrder.Argument_Parameter: return string.Format("{0} {1}", base.ToFriendlyString(), _detectionRule.ToAbbreviations());
                default: return "";
            }
        }
    }

    /// <summary>
    /// 检测顺序
    /// </summary>
    [Name("检测顺序")]
    public enum EDetectionOrder
    {
        /// <summary>
        /// 形参-实参:将形参与实参根据检测规则做检测，形参为左值,实参为右值
        /// </summary>
        [Name("形参-实参")]
        [Tip("将形参与实参根据检测规则做检测，形参为左值,实参为右值")]
        Parameter_Argument,

        /// <summary>
        /// 实参-形参:将实参与形参根据检测规则做检测，实参为左值,形参为右值
        /// </summary>
        [Name("实参-形参")]
        [Tip("将实参与形参根据检测规则做检测，实参为左值,形参为右值")]
        Argument_Parameter,
    }
}
