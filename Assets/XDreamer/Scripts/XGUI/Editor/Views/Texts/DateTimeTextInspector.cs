﻿using System;
using System.Text;
using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorTools.Base;
using XCSJ.Helper;
using XCSJ.PluginXGUI.Views.Texts;

namespace XCSJ.EditorXGUI.Views.Texts
{
    /// <summary>
    /// 日期时间文本检查器
    /// </summary>
    [CustomEditor(typeof(DateTimeText))]
    public class DateTimeTextInspector : ToolMBInspector<DateTimeText>
    {
        /// <summary>
        /// 显示帮助信息
        /// </summary>
        protected override bool displayHelpInfo => true;

        /// <summary>
        /// 当纵向绘制对象的成员属性之后回调
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberProperty"></param>
        /// <param name="arrayElementInfo"></param>
        public override void OnAfterPropertyFieldVertical(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(targetObject.timeSpanTicks):
                    {
                        if (targetObject._dataTimeRule != DateTimeText.EDataTimeRule.CurrentSystem) break;
                        var timeSpan = new TimeSpan(memberProperty.longValue);
                        try
                        {
                            EditorGUILayout.BeginHorizontal();

                            EditorGUILayout.PrefixLabel("时间跨度");

                            EditorGUI.BeginChangeCheck();
                            var days = EditorGUILayout.IntField(timeSpan.Days);
                            EditorGUILayout.LabelField("天", UICommonOption.Width16);

                            var hours = EditorGUILayout.IntField(timeSpan.Hours);
                            EditorGUILayout.LabelField("时", UICommonOption.Width16);

                            var minutes = EditorGUILayout.IntField(timeSpan.Minutes);
                            EditorGUILayout.LabelField("分", UICommonOption.Width16);

                            var seconds = EditorGUILayout.IntField(timeSpan.Seconds);
                            EditorGUILayout.LabelField("秒", UICommonOption.Width16);

                            var milliseconds = EditorGUILayout.IntField(timeSpan.Milliseconds);
                            EditorGUILayout.LabelField("毫秒", UICommonOption.Width32);

                            if (EditorGUI.EndChangeCheck())
                            {
                                memberProperty.longValue = new TimeSpan(days, hours, minutes, seconds, milliseconds).Ticks;
                            }
                        }
                        catch { }
                        finally
                        {
                            EditorGUILayout.EndHorizontal();
                        }
                        break;
                    }
            }
            base.OnAfterPropertyFieldVertical(type, memberProperty, arrayElementInfo);
        }

        /// <summary>
        /// 获取辅助信息
        /// </summary>
        /// <returns></returns>
        public override StringBuilder GetHelpInfo()
        {
            var sb = base.GetHelpInfo();
            var timeSpan = targetObject.timeSpan;
            sb.AppendFormat("当前时间:\t{0}", DateTime.Now.ToDefaultFormat());
            sb.AppendFormat("\n时间跨度:\t{0}天{1}时{2}分{3}秒{4}毫秒", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
            sb.AppendFormat("\n显示时间:\t{0}", targetObject.dateTime.ToDefaultFormat());
            sb.AppendFormat("\n文本:\t{0}", targetObject.Text());
            return sb;
        }
    }
}
