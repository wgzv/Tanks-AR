using System;
using System.Collections.Generic;
using XCSJ.LitJson;

namespace XCSJ.PluginXGUI.Windows.Weathers
{
    [Serializable]
    [Import]
    public class WeatherData
    {
        public string message;
        public int status;
        public string date;
        public string time;
        public CityInfo cityInfo;
        public Data data;
    }

    [Serializable]
    [Import]
    public class CityInfo
    {
        public string city;
        public string citykey;
        public string parent;
        public string updateTime;
    }
    [Serializable]
    [Import]
    public class Data
    {
        public string shidu;
        public float pm25;
        public float pm10;
        public string quality;
        public string wendu;
        public string ganmao;
        public List<Forecast> forecast;
        public Forecast yesterday;
    }

    [Serializable]
    [Import]
    public class Forecast
    {
        public string date;
        public string high;
        public string low;
        public string ymd;
        public string week;
        public string sunrise;
        public string sunset;
        public int aqi;
        public string fx;
        public string fl;
        public string type;
        public string notice;
    }
}
