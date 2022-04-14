using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginDataBase;

namespace XCSJ.EditorDataBase
{
    /// <summary>
    /// 数据库管理器检查器
    /// </summary>
    [CustomEditor(typeof(DBManager))]
    public class DBManagerInspector : BaseManagerInspector<DBManager>
    {
        private static CategoryList categoryList = null;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            if (categoryList == null) categoryList = EditorToolsHelper.GetWithPurposes(nameof(DBManager));
        }

        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            categoryList.DrawVertical();
            EditorGUILayout.Separator();
            base.OnAfterVertical();
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
                case "dbs":
                    {
                        if (arrayElementInfo.serializedProperty == null && GUILayout.Button("清除无效数据库", GUILayout.Width(150)))
                        {
                            targetObject.GetDBMonoBehaviours();
                        }
                        break;
                    }
            }
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }
    }
}
