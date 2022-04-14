using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.Dataflows.FieldBinds;

namespace XCSJ.EditorSMS.States.Dataflows.FieldBinds
{
    /// <summary>
    /// 批量字段绑定检查器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TBindInfo"></typeparam>
    public class FieldBindBatchInspector<T, TBindInfo> : BaseFieldBindInspector<T>
        where T : FieldBindBatch<T, TBindInfo>
        where TBindInfo : BindInfo
    {
        /// <summary>
        /// 绘制字段名称下拉框
        /// </summary>
        /// <param name="bindInfo"></param>
        /// <returns></returns>
        protected string DrawFieldNamePopup(BindInfo bindInfo)
        {
            var obj = bindInfo.obj;
            var fieldName = bindInfo.fieldName;
            if (Equals(obj, null))
            {
                fieldName = EditorGUILayout.TextField(fieldName);
                UICommonFun.Popup(fieldName, new string[0], false, GUILayout.Width(80));
                return fieldName;
            }

            var allFieldNames = UICommonFun.GetPropertyNameInInspector(bindInfo.obj as UnityEngine.Object);
            var allFieldInfos = bindInfo.GetBindFields();

            var validFieldInfos = new List<FieldInfo>();
            allFieldNames.ForEach(fn =>
            {
                var validFieldInfo = allFieldInfos.FirstOrDefault(fi => fi.Name == fn);
                if (validFieldInfo != null)
                {
                    validFieldInfos.Add(validFieldInfo);
                }
            });

            var currentFieldInfo = bindInfo.fieldInfoRealtime;
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
        /// <param name="arrayElementInfo"></param>
        /// <returns></returns>
        protected virtual bool DefaultDrawFieldName(SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            if (arrayElementInfo.index < 0 || arrayElementInfo.index >= stateComponent.bindInfos.Count) return false;
            var bindInfo = stateComponent.bindInfos[arrayElementInfo.index];
            if (memberProperty.name != bindInfo.fieldNameOfFieldName) return false;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(CommonFun.NameTooltip(bindInfo, bindInfo.fieldNameOfFieldName));
            memberProperty.stringValue = DrawFieldNamePopup(bindInfo);
            EditorGUILayout.EndHorizontal();

            return true;
        }

        /// <summary>
        /// 绘制字段名
        /// </summary>
        /// <param name="memberProperty"></param>
        /// <param name="arrayElementInfo"></param>
        /// <returns></returns>
        protected virtual bool DrawFieldName(SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo) => DefaultDrawFieldName(memberProperty, arrayElementInfo);

        /// <summary>
        /// 是否需要绘制
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberProperty"></param>
        /// <param name="arrayElementInfo"></param>
        /// <returns></returns>
        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            if(type == typeof(TBindInfo))
            {
                if (DrawFieldName(memberProperty, arrayElementInfo)) return false;
            }            
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }
    }
}
