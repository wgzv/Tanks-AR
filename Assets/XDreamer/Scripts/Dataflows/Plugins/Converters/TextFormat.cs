using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Models;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginDataflows.Models;

namespace XCSJ.PluginDataflows.Converters
{
    /// <summary>
    /// 文本格式对象
    /// </summary>
    [Name("文本格式对象")]
    public class TextFormat : ObservableMB
    {
        [Name("文本格式")]
        [EnumPopup]
        public ETextFormat _textFormat = ETextFormat.None;

        /// <summary>
        /// 小数点位数
        /// </summary>
        [Name("小数点位数")]
        [Range(0,10)]
        [HideInSuperInspector(nameof(_textFormat), EValidityCheckType.NotEqual, ETextFormat.DecimalNumber)]
        public int _decimalPointDigit;

        /// <summary>
        /// 字符串格式
        /// </summary>
        [Name("字符串格式")]
        [HideInSuperInspector(nameof(_textFormat), EValidityCheckType.NotEqual, ETextFormat.Custom)]
        public string _formatString;

        /// <summary>
        /// 附加单位
        /// </summary>
        [Name("附加单位")]
        public string _unit;

        private string _text;

        /// <summary>
        /// 可绑定的属性
        /// </summary>
        public string text
        {
            get => _text; 
            set
            {
                var tmp = value;

                switch (_textFormat)
                {
                    case ETextFormat.IntegerNumber:
                        {
                            if (float.TryParse(value, out float rs))
                            {
                                tmp = Convert.ToInt64(rs).ToString();
                            }
                            break;
                        }
                    case ETextFormat.DecimalNumber:
                        {
                            if (float.TryParse(value, out float rs))
                            {
                                tmp = rs.ToString("F" + _decimalPointDigit.ToString());
                            }
                            break;
                        }
                    case ETextFormat.Custom:
                        {
                            tmp = string.IsNullOrEmpty(_formatString) ? value : string.Format(_formatString, value);
                            break;
                        }
                }

                tmp += _unit;
                this.SetProperty(ref _text, tmp, nameof(text));
            }
        }
    }

    [Name("文本格式")]
    public enum ETextFormat
    {
        [Name("无")]
        None,

        [Name("整数")]
        IntegerNumber,

        [Name("十进制小数")]
        DecimalNumber,

        [Name("自定义")]
        Custom,
    }
}
