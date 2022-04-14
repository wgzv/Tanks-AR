using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.Dataflows.FieldBinds;

namespace XCSJ.EditorSMS.States.Dataflows.FieldBinds
{
    /// <summary>
    /// 字段绑定检查器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FieldBindInspector<T> : BaseFieldBindInspector<T> where T : FieldBind<T>
    {
        /// <summary>
        /// 获取辅助信息
        /// </summary>
        /// <returns></returns>
        public override StringBuilder GetHelpInfo()
        {
            return base.GetHelpInfo().AppendFormat("实时参数信息:\t{0}\n缓冲参数信息:\t{1}", stateComponent.GetRealtimeParamInfo(), stateComponent.GetBufferParamInfo());
        }

        /// <summary>
        /// 绘制字段名称下拉框
        /// </summary>
        /// <returns></returns>
        protected string DrawFieldNamePopup()
        {
            var obj = stateComponent.obj;
            var fieldName = stateComponent.fieldName;
            if (Equals(obj, null))
            {
                fieldName = EditorGUILayout.TextField(fieldName);
                UICommonFun.Popup(fieldName, new string[0], false, GUILayout.Width(80));
                return fieldName;
            }

            var allFieldNames = UICommonFun.GetPropertyNameInInspector(stateComponent.obj as UnityEngine.Object);
            var allFieldInfos = stateComponent.GetBindFields();

            var validFieldInfos = new List<FieldInfo>();
            allFieldNames.ForEach(fn =>
            {
                var validFieldInfo = allFieldInfos.FirstOrDefault(fi => fi.Name == fn);
                if (validFieldInfo != null)
                {
                    validFieldInfos.Add(validFieldInfo);
                }
            });

            var currentFieldInfo = stateComponent.fieldInfoRealtime;
            var validFieldInfoDisplayNames = validFieldInfos.ToList(fi => CommonFun.Name(fi)).ToArray();

            EditorGUILayout.BeginHorizontal();
            {
                fieldName = EditorGUILayout.TextField(fieldName);

                var currentFieldInfoDisplayName = UICommonFun.Popup(CommonFun.Name(currentFieldInfo), validFieldInfoDisplayNames, false, GUILayout.Width(80));
                var index = Array.IndexOf(validFieldInfoDisplayNames, currentFieldInfoDisplayName);
                if (index >= 0)
                {
                    fieldName = validFieldInfos[index].Name;
                }
            }
            EditorGUILayout.EndHorizontal();

            return fieldName;
        }

        /// <summary>
        /// 默认绘制字段名
        /// </summary>
        /// <param name="memberProperty"></param>
        protected virtual void DefaultDrawFieldName(SerializedProperty memberProperty)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(CommonFun.NameTooltip(stateComponent, stateComponent.fieldNameOfFieldName));
            memberProperty.stringValue = DrawFieldNamePopup();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制字段名
        /// </summary>
        /// <param name="memberProperty"></param>
        protected virtual void DrawFieldName(SerializedProperty memberProperty) => DefaultDrawFieldName(memberProperty);

        /// <summary>
        /// 是否需要绘制
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberProperty"></param>
        /// <param name="arrayElementInfo"></param>
        /// <returns></returns>
        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            if (memberProperty.name == stateComponent.fieldNameOfFieldName)
            {
                DrawFieldName(memberProperty);
                return false;
            }
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }

        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("操作");
            if (GUILayout.Button(CommonFun.TempContent("复制实时参数信息", "将实时参数信息复制到系统剪贴板")))
            {
                CommonFun.CopyTextToClipboardForPC(stateComponent.GetRealtimeParamInfo());
            }
            if (GUILayout.Button(CommonFun.TempContent("复制缓冲参数信息", "将缓冲参数信息复制到系统剪贴板")))
            {
                CommonFun.CopyTextToClipboardForPC(stateComponent.GetBufferParamInfo());
            }
            EditorGUILayout.EndHorizontal();
            base.OnAfterVertical();
        }
    }
}
