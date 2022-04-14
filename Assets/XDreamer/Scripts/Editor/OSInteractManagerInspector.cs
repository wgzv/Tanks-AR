using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using XCSJ.Extension;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.Helper;
using XCSJ.EditorExtension.XGUI;
using XCSJ.EditorXGUI;

namespace XCSJ.EditorExtension
{
    /// <summary>
    /// OS与unity交互管理类的编辑器类
    /// </summary>
    [CustomEditor(typeof(OSInteractManager))]
    public class OSInteractManagerInspector : BaseManagerInspector<OSInteractManager>
    {
        public bool varToggle = true;

        public override bool OnModifiedPropertyField(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case "sendUnityEngineLoadedFinishWhenStart":
                    {
                        if (memberProperty.boolValue)
                        {
                            var dialogInfo = string.Format("{0}\r\n\r\n是否确定发送该消息?", CommonFun.Tooltip(type, memberProperty.name));
                            if (!EditorUtility.DisplayDialog("特别提示", dialogInfo, "确定发送", "不发送(推荐)")) return false;
                        }
                        break;
                    }
                case "unloadAllSubSceneWhenStart":
                    {
                        if (memberProperty.boolValue)
                        {
                            var dialogInfo = string.Format("{0}\r\n\r\n是否确定卸载?", CommonFun.Tooltip(type, memberProperty.name));
                            if (!EditorUtility.DisplayDialog("特别提示", dialogInfo, "确定卸载", "不卸载(推荐)")) return false;
                        }
                        break;
                    }
                case "loadNextSceneWhenStart":
                    {
                        if (memberProperty.boolValue)
                        {
                            var dialogInfo = string.Format("{0}\r\n\r\n是否确定启用加载后续场景功能?", CommonFun.Tooltip(type, memberProperty.name));
                            if (!EditorUtility.DisplayDialog("特别提示", dialogInfo, "确定启用", "不启用(推荐)")) return false;
                        }
                        break;
                    }
            }
            return base.OnModifiedPropertyField(type, memberProperty, arrayElementInfo);
        }

        private const string backOSPrefabPath = "Assets/XDreamer-Assets/基础/Prefabs/常用/返回OS按钮.prefab";

        public override void OnAfterVertical()
        {
            //varToggle = UICommonFun.Foldout(varToggle, CommonFun.NameTooltip(GetType(), "varToggle"));
            //if (varToggle)
            //{
            //    CommonFun.BeginLayout();

            //    foreach(var e in EnumHelper.EnumAll<EVariableName>())
            //    {
            //        var exist = ScriptManager.instance.variableMap.ContainsKey(e.ToString());
            //        if (EditorGUILayout.Toggle(CommonFun.NameTooltip(e), exist) && !exist)
            //        {
            //            ScriptManager.instance.AddVariable(e.ToString());
            //        }
            //    }

            //    CommonFun.EndLayout();
            //}
            //base.OnAfterVertical();

            if (GUILayout.Button("添加[返回OS]按钮"))
            {
                var go = UICommonFun.LoadAndInstantiateFromAssets<GameObject>(backOSPrefabPath);
                if (go)
                {
                    EditorXGUIHelper.SetObjectToCanvas(go);
                    EditorGUIUtility.PingObject(go);
                }
            }
        }
    }
}
