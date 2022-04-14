using System;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorDataBase.Tools;
using XCSJ.EditorExtension;
using XCSJ.EditorExtension.Base;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSqlite;

namespace XCSJ.EditorSqlite
{
    /// <summary>
    /// SQLite数据库类的编辑器类
    /// </summary>
#if UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID

#elif UNITY_EDITOR
    [Obsolete("SQLite数据库不支持当前的运行时环境(但可在编辑器环境中使用)，请使用其他类型的数据库替代")]
#else
    [Obsolete("SQLite数据库不支持当前的运行时环境，请使用其他类型的数据库替代", true)]
#endif
    [CustomEditor(typeof(SqliteMB))]
    public class SqliteMBInspector : DBMBInspector<SqliteMB>
    {
        /// <summary>
        /// 宏
        /// </summary>
        public static readonly Macro XDREAMER_SQLITE = new Macro(nameof(XDREAMER_SQLITE), BuildTargetGroup.Standalone, BuildTargetGroup.iOS, BuildTargetGroup.Android);

        /// <summary>
        /// 初始化宏
        /// </summary>
        [InitializeOnLoadMethod]
        [Macro]
        public static void InitMacro()
        {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID
            if (TypeHelper.ExistsAndAssemblyFileExists("Mono.Data.Sqlite.SqliteConnection") && Type.GetType("Mono.Data.Sqlite.SqliteConnection", false, true) != null)
            {
                XDREAMER_SQLITE.DefineIfNoDefined();
            }
            else
#endif
            {
                XDREAMER_SQLITE.UndefineWithSelectedBuildTargetGroup();
            }

#if UNITY_2018_3_OR_NEWER
            //需要移除 System.Data.dll文件
            FileHelper.Delete(Application.dataPath + "/Plugins/System.Data.dll");
            FileHelper.Delete(Application.dataPath + "/Plugins/System.Data.dll.meta");
#endif
        }

        private const string UnityPackagePath = "Assets/XDreamer-ThirdPartyUnityPackage/XDreamer_SQLite.unitypackage";

        /// <summary>
        /// 当横向绘制对象的成员属性之后回调
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberProperty"></param>
        /// <param name="arrayElementInfo"></param>
        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch(memberProperty.name)
            {
                case nameof(ResourceFileInfo.uniqueName):
                    {
                        if (GUILayout.Button(UICommonOption.Update, EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                        {
                            memberProperty.stringValue = GuidHelper.GetNewGuid();
                        }                        
                        break;
                    }
            }
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }

        /// <summary>
        /// 当脚本对象纵向绘制之后回调
        /// </summary>
        public override void OnAfterScript()
        {
#if UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID
            #region 检测是否需要导入UnityPackage

            EditorHelper.ImportPackageIfNeedWithButton(XDREAMER_SQLITE, UnityPackagePath);

            #endregion
#else
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(new GUIContent("不支持当前选择的运行时平台!", "不支持当前选择的运行时平台,但可在编辑器环境中使用!^_^"), UICommonOption.lableRedBG, GUILayout.ExpandWidth(true));

#endif
            base.OnAfterScript();
        }
    }
}
