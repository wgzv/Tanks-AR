﻿using System;
using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorTools.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Windows.Weathers;

namespace XCSJ.EditorXGUI.Windows.Weathers
{
    /// <summary>
    /// 天气检查器
    /// </summary>
    [CustomEditor(typeof(Weather))]
    public class WeatherInspector : ToolMBInspector<Weather>
    {
        /// <summary>
        /// 显示辅助信息
        /// </summary>
        protected override bool displayHelpInfo => true;

        /// <summary>
        /// 获取辅助信息
        /// </summary>
        /// <returns></returns>
        public override StringBuilder GetHelpInfo()
        {
            var info = base.GetHelpInfo();
            info.AppendFormat("URI:\t{0}\n", targetObject.uri);
            info.Append(@"天气数据由URI指向的网络数据运营商提供!
对天气数据的准确性、安全性、完整性等XDreamer官方均不做担保!
XDreamer仅提供天气数据获取的方法与途径!
因网络数据运营商数据接口调整导致的组件不可用,XDreamer官方将会尽快提出解决方法!
使用者在使用天气数据过程中请遵守网络数据运营商的规范或守则!
如使用者与网络数据运营商产生任何纠纷时，XDreamer官方不参与任何纠纷也不承担责任!");
            return info;
        }

        /// <summary>
        /// 当横向绘制对象的成员属性之后回调
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberProperty"></param>
        /// <param name="arrayElementInfo"></param>
        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(mb.cityCode):
                    {
                        if (GUILayout.Button(CommonFun.TempContent("查询城市代码", "跳转网页查询城市代码"), UICommonOption.Width100))
                        {
                            Application.OpenURL("https://github.com/baichengzhou/weather.api/blob/master/src/main/resources/citycode-2019-08-23.json");
                        }
                        break;
                    }
            }
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }

        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            EditorGUILayout.Separator();
            var uri = targetObject.uri;
            if (GUILayout.Button(CommonFun.TempContent("查询天气数据", "跳转网页请求天气数据,打开网址：" + uri)))
            {
                Application.OpenURL(uri);
            }
            if (GUILayout.Button(CommonFun.TempContent("网络数据运营商技术博客", "跳转网页打开网络数据运营商技术博客网址")))
            {
                Application.OpenURL("https://www.sojson.com/blog/305.html");
            }
        }
    }
}
