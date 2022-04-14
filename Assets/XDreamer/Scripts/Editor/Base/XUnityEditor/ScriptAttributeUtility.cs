using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    [LinkType("UnityEditor.ScriptAttributeUtility")]
    public class ScriptAttributeUtility : LinkType<ScriptAttributeUtility>
    {
        #region s_SharedNullHandler

        public static XFieldInfo s_SharedNullHandler_FieldInfo { get; } = GetXFieldInfo(nameof(s_SharedNullHandler));

        private static PropertyHandler s_SharedNullHandler
        {
            get
            {
                return new PropertyHandler(s_SharedNullHandler_FieldInfo.GetValue(null));
            }
        }

        #endregion

        #region s_NextHandler

        public static XFieldInfo s_NextHandler_FieldInfo { get; } = GetXFieldInfo(nameof(s_NextHandler));

        private static PropertyHandler s_NextHandler
        {
            get
            {
                return new PropertyHandler(s_NextHandler_FieldInfo.GetValue(null));
            }
        }

        #endregion

        #region GetHandler

        public static XMethodInfo GetHandler_MethodInfo { get; } = GetXMethodInfo(nameof(GetHandler));

        public static PropertyHandler GetHandler(SerializedProperty property)
        {
            return new PropertyHandler(GetHandler_MethodInfo.Invoke(null, new object[] { property }));
        }

        #endregion

        /// <summary>
        /// 有自定义属性绘制器
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool HasCustomPropetyDrawer(SerializedProperty property)
        {
            return GetHandler(property).obj != s_SharedNullHandler.obj;
        }

        #region GetFieldInfoFromProperty

        public static XMethodInfo GetFieldInfoFromProperty_MethodInfo { get; } = GetXMethodInfo(nameof(GetFieldInfoFromProperty));

        /// <summary>
        /// 从属性获取字段信息
        /// </summary>
        /// <param name="property"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static FieldInfo GetFieldInfoFromProperty(SerializedProperty property, out Type type)
        {
            var ps = new object[] { property, default(Type) };
            var fieldInfo = GetFieldInfoFromProperty_MethodInfo.InvokeStatic(ps) as FieldInfo;
            type = ps[1] as Type;
            return fieldInfo;
        }

        #endregion

        #region GetFieldInfoFromPropertyPath

        public static XMethodInfo GetFieldInfoFromPropertyPath_MethodInfo { get; } = GetXMethodInfo(nameof(GetFieldInfoFromPropertyPath));

        /// <summary>
        /// 从属性路径获取字段信息
        /// </summary>
        /// <param name="property"></param>
        /// <param name="type"></param>
        /// <returns></returns>	
        public static FieldInfo GetFieldInfoFromPropertyPath(Type host, string path, out Type type)
        {
            var ps = new object[] { host, path, default(Type) };
            var fieldInfo = GetFieldInfoFromPropertyPath_MethodInfo.InvokeStatic(ps) as FieldInfo;
            type = ps[2] as Type;
            return fieldInfo;
        }

        #endregion
    }
}
