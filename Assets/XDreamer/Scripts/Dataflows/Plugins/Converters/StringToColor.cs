using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Models;
using XCSJ.PluginDataflows.Models;

namespace XCSJ.PluginDataflows.Converters
{
    /// <summary>
    /// 字符串转颜色 ：用于将字符串转为某颜色值
    /// </summary>
    [Name("字符串转颜色")]
    public class StringToColor : ObservableMB
    {
        /// <summary>
        /// 字符串与颜色对应表
        /// </summary>
        [Serializable]
        public class StringColor
        {
            /// <summary>
            /// 字符串值
            /// </summary>
            public string value = "";

            /// <summary>
            /// 颜色
            /// </summary>
            public Color color = Color.white;
        }

        /// <summary>
        /// 字符串与颜色映射数组
        /// </summary>
        [Name("字符串与颜色映射表")]
        public StringColor[] _stringColors = new StringColor[0];

        private StringColor _currentConfig = new StringColor();

        /// <summary>
        /// 当前字符串
        /// </summary>
        public string stringValue
        {
            get
            {
                return _currentConfig.value;
            }
            set
            {
                foreach (var item in _stringColors)
                {
                    if (item.value == value)
                    {
                        this.SetProperty(ref _currentConfig.value, item.value, nameof(stringValue));
                        this.SetProperty(ref _currentConfig.color, item.color, nameof(color));
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 当前颜色
        /// </summary>
        public Color color
        {
            get
            {
                return _currentConfig.color;
            }
            set
            {
                foreach (var item in _stringColors)
                {
                    if (item.color == value)
                    {
                        this.SetProperty(ref _currentConfig.value, item.value, nameof(stringValue));
                        this.SetProperty(ref _currentConfig.color, item.color, nameof(color));
                        break;
                    }
                }
            }
        }
    }
}
