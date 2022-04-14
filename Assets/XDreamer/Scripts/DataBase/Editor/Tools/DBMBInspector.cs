using System;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.PluginDataBase;
using XCSJ.PluginDataBase.Tools;

namespace XCSJ.EditorDataBase.Tools
{
    /// <summary>
    /// 数据库组件检查器
    /// </summary>
    [CustomEditor(typeof(DBMB), true)]
    public class DBMBInspector : DBMBInspector<DBMB> { }

    /// <summary>
    /// 数据库组件检查器泛型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DBMBInspector<T> : BaseInspector<T> where T : DBMB
    {
        private CategoryList categoryList;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            categoryList = EditorToolsHelper.GetWithPurposes(typeof(T).Name, nameof(DBMB));
        }

        /// <summary>
        /// 当脚本对象纵向绘制之后回调
        /// </summary>
        public override void OnAfterScript()
        {
            base.OnAfterScript();
            EditorGUILayout.TextField("数据库名称(只读)", targetObject.dbName);
            EditorGUILayout.TextField("数据库显示名称(只读)", targetObject.dbDisplayName);
        }

        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();
            CategoryListExtension.DrawVertical(categoryList);
            //var client = targetObject.client;
            //if (client != null)
            //{
            //    EditorGUILayout.Toggle("网络已连接:", targetObject.client.IsConnected());
            //}
        }
    }
}
