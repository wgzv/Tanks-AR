using System;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginDataBase.Tools;

namespace XCSJ.EditorDataBase.Tools
{
    /// <summary>
    /// 网络数据库检查器
    /// </summary>
    [CustomEditor(typeof(NetDBMB))]
    public class NetDBMBInspector : DBMBInspector<NetDBMB>
    {
        /// <summary>
        /// 当横向绘制对象的成员属性之后回调：空方法；
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="memberProperty">成员属性对象</param>
        /// <param name="arrayElementInfo">数组元素信息对象</param>
        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            //switch (memberProperty.name)
            //{
            //    case nameof(targetObject.serverPort):
            //        {
            //            if (GUILayout.Button("默认", EditorStyles.miniButtonRight, GUILayout.Width(40)))
            //            {
            //                memberProperty.intValue = DataBase.DBHelper.DefaultListenPort;
            //            }
            //            break;
            //        }
            //}
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }

        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();
            //var client = targetObject.client;
            //if (client != null)
            //{
            //    EditorGUILayout.Toggle("网络已连接:", targetObject.client.IsConnected());
            //}
        }
    }
}
