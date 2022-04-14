using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Windows.Weathers
{
    /// <summary>
    /// 天气数据文本:将天气数据同步到UI中
    /// </summary>
    [Name("天气数据文本")]
    [Tip("将天气数据同步到UI中")]
    public abstract class WeatherDataUI : View
    {
        /// <summary>
        /// 天气:期望显示的天气数据的来源对象；如不设置，会从当前组件所在的游戏对象上查找本参数对应类型的的组件；
        /// </summary>
        [Name("天气")]
        [Tip("期望显示的天气数据的来源对象；如不设置，会从当前组件所在的游戏对象上查找本参数对应类型的的组件；")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public Weather weather;

        /// <summary>
        /// 天气数据类型:期望显示的天气数据类型
        /// </summary>
        [Name("天气数据类型")]
        [Tip("期望显示的天气数据类型")]
        [EnumPopup]
        public EWeatherDataType weatherDataType = EWeatherDataType.cityInfo_city;

        /// <summary>
        /// 预报索引:天气数据中包含未来15天的天气预报，使用索引值获取某一天的数据信息
        /// </summary>
        [Name("预报索引")]
        [Tip("天气数据中包含未来15天的天气预报，使用索引值获取某一天的数据信息")]
        [Range(0, 14)]
        public int forcastIndex = 0;

        /// <summary>
        /// 启用时
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            if (!weather)
            {
                weather = GetComponent<Weather>();
            }
            if (weather)
            {
                weather.onWeatherUpdated += OnWeatherUpdated;
                weather.UpdateWeatherIfNeed(true);
            }
        }

        /// <summary>
        /// 禁用时
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            if (weather)
            {
                weather.onWeatherUpdated -= OnWeatherUpdated;
            }
        }

        private void OnWeatherUpdated(Weather weather)
        {
            UpdateUI();
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public bool TryGetData(out string data)
        {
            if (weather)
            {
                return weather.TryGetData(weatherDataType, forcastIndex, out data);
            }
            data = null;
            return false;
        }

        /// <summary>
        /// 更新UI
        /// </summary>
        protected abstract void UpdateUI();
    }
}
