using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.CNScripts;
using XCSJ.EditorExtension.Base;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.EditorExtension.CNScripts.ScriptParamDrawers
{
    /// <summary>
    /// 标准化字符串 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.StandardString)]
    public class StandardString_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            EditorGUI.indentLevel = 2;
            paramObject = VariableHelper.Format(EditorGUILayout.TextField(VariableHelper.Format(paramObject as string)));
        }
    }

    /// <summary>
    /// 字符串 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.String)]
    public class String_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            EditorGUI.indentLevel = 2;
            paramObject = EditorGUILayout.TextArea(paramObject as string);
        }
    }

    /// <summary>
    /// 全局变量名类型 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.GlobalVariableName)]
    public class GlobalVariableName_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            EditorGUI.indentLevel = 2;
            paramObject = UICommonFun.Popup(true, paramObject as string, scriptManager.GetVariableNames());
        }
    }

    /// <summary>
    /// 数组 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.Array)]
    public class Array_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            EditorGUI.indentLevel = 2;
            var names = scriptManager.varDictionary.WhereCast(kv => kv.Value.hierarchyVar.GetVarType() == EVarType.Array, kv => kv.Value.name).ToArray();
            paramObject = UICommonFun.Popup(true, paramObject as string, names);
        }
    }

    /// <summary>
    /// 字典 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.Dictionary)]
    public class Dictionary_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            EditorGUI.indentLevel = 2;
            var names = scriptManager.varDictionary.WhereCast(kv => kv.Value.hierarchyVar.GetVarType() == EVarType.Dictionary, kv => kv.Value.name).ToArray();
            paramObject = UICommonFun.Popup(true, paramObject as string, names);
        }
    }

    /// <summary>
    /// 变量 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.Variable)]
    public class Variable_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            EditorGUI.indentLevel = 2;
            paramObject = VariableHelper.Format(EditorGUILayout.TextField(paramObject as string));
        }
    }

    /// <summary>
    /// unity资源对象类型 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.UnityAssetObjectType)]
    public class UnityAssetObjectType_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            EditorGUI.indentLevel = 2;
            paramObject = UICommonFun.EnumPopup((EUnityAssetObjectType)paramObject);
        }
    }

    /// <summary>
    /// unity资源对象 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.UnityAssetObject)]
    public class UnityAssetObject_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();

            EditorGUI.indentLevel = 2;
            var unityAssetObjectManager = UnityAssetObjectManager.instance;
            try
            {
                EditorGUILayout.BeginHorizontal();
                if (!(paramObject is ParamList pl)) pl = new ParamList();

                UnityEngine.Object obj = null;
                string type = pl["type"] as string;
                string name = pl["name"] as string;
                //Debug.Log("UnityAssetObject 渲染 type: [" + type + "] , name: [" + name + "]");

                Type unityAssetObjectType = null;
                if (unityAssetObjectManager.GetUnityAssetObjectType(param.limitType?.Name) != null)
                {
                    unityAssetObjectType = param.limitType;
                    type = unityAssetObjectType.Name;
                }
                else
                {
                    string[] names = unityAssetObjectManager.GetUnityAssetObjectTypeNames();
                    int selectedIndex = unityAssetObjectManager.IndexOfType(type);
                    if (selectedIndex < 0)
                    {
                        type = names[0];
                    }
                    EUnityAssetObjectType euaot = (EUnityAssetObjectType)UICommonFun.EnumPopup((EUnityAssetObjectType)Enum.Parse(typeof(EUnityAssetObjectType), type));
                    type = euaot.ToString();
                    unityAssetObjectType = unityAssetObjectManager.GetUnityAssetObjectType(type);
                }

                //对象控件
                obj = unityAssetObjectManager.GetUnityAssetObject(type, name);

                if (unityAssetObjectType != null)
                {
                    //Debug.Log("t: [" + t.Name + "]");
                    obj = EditorGUILayout.ObjectField(obj, unityAssetObjectType, true);
                }

                if (obj)
                {
                    //Debug.Log("obj: [" + obj.name + "] , type: [" + obj.GetType().Name + "]");
                    name = obj.name;
                    ScriptViewerWindow.CallHasObjectUsed(obj);
                }

                //从缓冲区直接选择
                name = UICommonFun.Popup(name, unityAssetObjectManager.GetAssetNames(unityAssetObjectType), false);

                //名称控件
                name = EditorGUILayout.TextField(name);

                pl["type"] = type;
                pl["name"] = name;

                paramObject = pl;
            }
            catch// (Exception ex)
            {
                //throw ex;
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    /// <summary>
    /// 文件路径 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.File)]
    [ScriptParamDrawer(EParamType.SaveFile)]
    [ScriptParamDrawer(EParamType.OpenFolder)]
    [ScriptParamDrawer(EParamType.SaveFolder)]
    public class FilePath_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            EditorGUI.indentLevel = 2;
            string path = paramObject as string;
            if (string.IsNullOrEmpty(path))
            {
                path = Application.streamingAssetsPath;
            }
            EditorGUILayout.BeginHorizontal();
            paramObject = EditorGUILayout.TextField(path);
            if (GUILayout.Button("路径", GUILayout.Width(60)))
            {
                string retpath = "";
                switch (paramType)
                {
                    case EParamType.File:
                        {
                            retpath = EditorUtility.OpenFilePanelWithFilters("打开文件", System.IO.Path.GetDirectoryName(path.Trim()), UICommonFun.GetFileFilters());
                            break;
                        }
                    case EParamType.SaveFile:
                        {
                            retpath = EditorUtility.SaveFilePanel("保存文件", System.IO.Path.GetDirectoryName(path.Trim()), "", "*");
                            break;
                        }
                    case EParamType.OpenFolder:
                        {
                            retpath = EditorUtility.OpenFolderPanel("打开文件夹", System.IO.Path.GetDirectoryName(path.Trim()), "");
                            break;
                        }
                    case EParamType.SaveFolder:
                        {
                            retpath = EditorUtility.SaveFolderPanel("保存文件夹", System.IO.Path.GetDirectoryName(path.Trim()), "");
                            break;
                        }
                }
                paramObject = retpath;
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 游戏对象 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.GameObject)]
    public class GameObject_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            try
            {
                EditorGUI.indentLevel = 2;
                GameObject go = paramObject as GameObject;
                if (go && param.limitType != null && ComponentManager.ContainsType(param.limitType))
                {
                    if (!go.GetComponent(param.limitType)) go = null;
                }
                paramObject = EditorGUILayout.ObjectField(go, typeof(GameObject), true) as GameObject;
            }
            catch (ExitGUIException) { }
        }
    }

    /// <summary>
    /// 组件 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.Component)]
    public class Component_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            try
            {
                EditorGUI.indentLevel = 2;
                int index = (int)paramObject;
                string[] ns = ComponentManager.GetNames();
                if (param.limitType != null && ComponentManager.ContainsType(param.limitType))
                {
                    index = ComponentManager.GetIndex(param.limitType);
                }
                paramObject = EditorGUILayout.Popup(index, ns);
            }
            catch (ExitGUIException) { }
        }
    }

    /// <summary>
    /// 游戏对象组件 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.GameObjectComponent)]
    public class GameObjectComponent_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            try
            {
                EditorGUI.indentLevel = 2;
                EditorGUILayout.BeginHorizontal();
                if (!(paramObject is ParamList pl)) pl = new ParamList();

                var component = pl[nameof(Component)] as Component;
                if (param.limitType != null && ComponentManager.ContainsType(param.limitType))
                {
                    pl[nameof(Component)] = EditorGUILayout.ObjectField(component, param.limitType, true) as Component;
                }
                else
                {
                    var go = pl[nameof(GameObject)] as GameObject;
                    var typeString = "";
                    if (component)
                    {
                        typeString = ComponentManager.TypeToKey(component.GetType());
                        go = component.gameObject;
                    }
                    pl[nameof(GameObject)] = go = EditorGUILayout.ObjectField(go, typeof(GameObject), true) as GameObject;
                    if (!go)
                    {
                        pl[nameof(Component)] = component = null;
                    }
                    else if (component && go != component.gameObject)
                    {
                        pl[nameof(Component)] = component = go.GetComponent(component.GetType());
                    }

                    if (go)
                    {
                        var array = go.GetComponents(typeof(Component)).ToList(c => ComponentManager.TypeToKey(c.GetType())).Distinct().ToArray();
                        var index = Array.IndexOf(array, typeString);
                        index = EditorGUILayout.Popup(index, array);
                        if (index >= 0 && index < array.Length)
                        {
                            pl[nameof(Component)] = go.GetComponent(ComponentManager.GetType(array[index]));
                        }
                    }
                    else
                    {
                        EditorGUILayout.Popup(-1, new string[] { });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    /// <summary>
    /// 组件游戏对象 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.ComponentGameObject)]
    public class ComponentGameObject_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            try
            {
                EditorGUI.indentLevel = 2;
                EditorGUILayout.BeginHorizontal();
                if (!(paramObject is ParamList pl)) pl = new ParamList();
                var component = pl[nameof(Component)] as Component;
                Type t = null;
                string[] ns = ComponentManager.GetNames();
                if (param.limitType != null && ComponentManager.ContainsType(param.limitType))
                {
                    pl["selectedIndex"] = ComponentManager.GetIndex(param.limitType);
                    t = param.limitType;
                    if (component != null && component.gameObject != null)
                    {
                        component = component.gameObject.GetComponent(t);
                    }
                    else
                    {
                        component = null;
                    }
                }
                else
                {
                    int selectedIndex = (int)(pl["selectedIndex"] ?? 0);
                    t = ComponentManager.GetType(ns[selectedIndex]);
                    int newSelectedIndex = EditorGUILayout.Popup(selectedIndex, ns);
                    if (newSelectedIndex != selectedIndex)
                    {
                        pl["selectedIndex"] = newSelectedIndex;
                        if (component != null && component.gameObject != null)
                        {
                            component = component.gameObject.GetComponent(ComponentManager.GetType(ns[newSelectedIndex]));
                        }
                        else
                        {
                            component = null;
                        }
                    }
                }
                pl[nameof(Component)] = EditorGUILayout.ObjectField(component, t, true);
            }
            catch (ExitGUIException) { }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    /// <summary>
    /// 游戏对象脚本事件 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.GameObjectSciptEvent)]
    public class GameObjectSciptEvent_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            try
            {
                EditorGUI.indentLevel = 2;
                EditorGUILayout.BeginHorizontal();
                if (!(paramObject is ParamList pl)) pl = new ParamList();

                MonoBehaviour mono = pl["MonoBehaviour"] as MonoBehaviour;
                string[] ns = ScriptEventManager.GetEventNames();
                Type t = null;
                if (param.limitType != null && ScriptEventManager.ContainsType(param.limitType))
                {
                    pl["scriptEventIndex"] = ScriptEventManager.GetEventIndex(param.limitType);
                    t = param.limitType;
                    if (mono != null && mono.gameObject != null)
                    {
                        mono = mono.gameObject.GetComponent(t) as MonoBehaviour;
                    }
                    else
                    {
                        mono = null;
                    }
                }
                else
                {
                    IScriptEvent ise = mono as IScriptEvent;
                    int scriptEventIndex = (int)(pl["scriptEventIndex"] ?? 0);
                    t = ScriptEventManager.GetScriptEventType(ns[scriptEventIndex]);
                    int newScriptEventSelectedIndex = EditorGUILayout.Popup(scriptEventIndex, ns);
                    if (newScriptEventSelectedIndex != scriptEventIndex)
                    {
                        pl["scriptEventIndex"] = newScriptEventSelectedIndex;
                        if (mono != null && mono.gameObject != null)
                        {
                            mono = mono.gameObject.GetComponent(ScriptEventManager.GetScriptEventType(ns[newScriptEventSelectedIndex])) as MonoBehaviour;
                        }
                        else
                        {
                            mono = null;
                        }
                    }
                }
                pl["MonoBehaviour"] = EditorGUILayout.ObjectField(mono, t, true);
            }
            catch (ExitGUIException) { }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    /// <summary>
    /// 游戏对象脚本事件函数 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.GameObjectSciptEventFunction)]
    public class GameObjectSciptEventFunction_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            try
            {
                EditorGUI.indentLevel = 2;
                EditorGUILayout.BeginHorizontal();
                if (!(paramObject is ParamList pl)) pl = new ParamList();

                MonoBehaviour mono = pl["MonoBehaviour"] as MonoBehaviour;
                IScriptEvent ise = mono as IScriptEvent;

                string[] ns = ScriptEventManager.GetEventNames();
                Type t = null;
                int scriptListIndex = (int)(pl["scriptListIndex"] ?? 0);
                string scriptListName = pl["scriptListName"] as string;

                if (param.limitType != null && ScriptEventManager.ContainsType(param.limitType))
                {
                    pl["scriptEventIndex"] = ScriptEventManager.GetEventIndex(param.limitType);
                    t = param.limitType;
                    if (mono != null && mono.gameObject != null)
                    {
                        mono = mono.gameObject.GetComponent(t) as MonoBehaviour;
                    }
                    else
                    {
                        mono = null;
                    }
                }
                else
                {
                    int scriptEventIndex = (int)(pl["scriptEventIndex"] ?? 0);
                    t = ScriptEventManager.GetScriptEventType(ns[scriptEventIndex]);
                    int newScriptEventSelectedIndex = EditorGUILayout.Popup(scriptEventIndex, ns);
                    if (newScriptEventSelectedIndex != scriptEventIndex)
                    {
                        pl["scriptEventIndex"] = newScriptEventSelectedIndex;
                        if (mono != null && mono.gameObject != null)
                        {
                            mono = mono.gameObject.GetComponent(ScriptEventManager.GetScriptEventType(ns[newScriptEventSelectedIndex])) as MonoBehaviour;
                        }
                        else
                        {
                            mono = null;
                        }
                        scriptListIndex = 0;
                    }
                }

                pl["MonoBehaviour"] = EditorGUILayout.ObjectField(mono, t, true);
                if (ise != null)
                {
                    ise.UpdateFunctionIfNeed();

                    if (string.IsNullOrEmpty(scriptListName))
                    {
                        //
                    }
                    else
                    {
                        scriptListIndex = ise.IndexOfFunctionName(scriptListName);
                    }
                    scriptListIndex = EditorGUILayout.Popup(scriptListIndex, ise.GetFunctionDisplayNames());
                    pl["scriptListIndex"] = scriptListIndex;
                    var function = ise.GetFunction(scriptListIndex);
                    if (function != null)
                    {
                        pl["scriptListName"] = function.name;
                    }
                    else
                    {
                        pl["scriptListName"] = "";
                    }
                }
                else
                {
                    EditorGUILayout.Popup(0, new string[] { });
                    pl["scriptListIndex"] = 0;
                    pl["scriptListName"] = "";
                }
            }
            catch (ExitGUIException) { }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    /// <summary>
    /// 游戏对象脚本事件变量 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.GameObjectSciptEventVariable)]
    public class GameObjectSciptEventVariableValue_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            try
            {
                EditorGUI.indentLevel = 2;
                EditorGUILayout.BeginHorizontal();
                if (!(paramObject is ParamList pl)) pl = new ParamList();

                MonoBehaviour mono = pl["MonoBehaviour"] as MonoBehaviour;
                IScriptEvent ise = mono as IScriptEvent;

                string[] ns = ScriptEventManager.GetEventNames();
                Type t = null;

                if (param.limitType != null && ScriptEventManager.ContainsType(param.limitType))
                {
                    pl["scriptEventIndex"] = ScriptEventManager.GetEventIndex(param.limitType);
                    t = param.limitType;
                    if (mono != null && mono.gameObject != null)
                    {
                        mono = mono.gameObject.GetComponent(t) as MonoBehaviour;
                    }
                    else
                    {
                        mono = null;
                    }
                }
                else
                {
                    int scriptEventIndex = (int)(pl["scriptEventIndex"] ?? 0);
                    t = ScriptEventManager.GetScriptEventType(ns[scriptEventIndex]);
                    int newScriptEventSelectedIndex = EditorGUILayout.Popup(scriptEventIndex, ns);
                    if (newScriptEventSelectedIndex != scriptEventIndex)
                    {
                        pl["scriptEventIndex"] = newScriptEventSelectedIndex;
                        if (mono != null && mono.gameObject != null)
                        {
                            mono = mono.gameObject.GetComponent(ScriptEventManager.GetScriptEventType(ns[newScriptEventSelectedIndex])) as MonoBehaviour;
                        }
                        else
                        {
                            mono = null;
                        }
                    }
                }
                pl["MonoBehaviour"] = EditorGUILayout.ObjectField(mono, t, true);

                string[] varNS = new string[] { };
                int varIndex = -1;
                if (ise != null)
                {
                    varNS = ise.GetVariableNames();
                    varIndex = varNS.IndexOf(pl["varName"] as string);
                }
                //已有变量的选择框
                int newIndex = EditorGUILayout.Popup(varIndex, varNS, GUILayout.ExpandWidth(true));
                //Debug.Log("index: " + varIndex.ToString() + ", newIndex: " + newIndex);
                if (newIndex != varIndex) pl["varName"] = varNS[newIndex];
                //Debug.Log("varNS.Length: " + varNS.Length.ToString() + ", varIndex: " + varIndex);
                //局部变量名
                pl["varName"] = VariableHelper.Format(EditorGUILayout.TextField(VariableHelper.Format(pl["varName"] as string), GUILayout.MinWidth(80)));
            }
            catch (ExitGUIException) { }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    /// <summary>
    /// 颜色 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.Color)]
    public class Color_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            EditorGUI.indentLevel = 2;
            try
            {
                paramObject = EditorGUILayout.ColorField((Color)(paramObject));
            }
            //catch (ExitGUIException) { }
            catch// (Exception colorEx)
            {
                //EditorGUILayout.ColorField 始终抛出 UnityEngine.ExitGUIException 异常~
                //查询得知，这个异常是无害的~所以将异常不处理了~
                //网址：http://answers.unity3d.com/questions/385235/editorguilayoutcolorfield-inside-guilayoutwindow-c.html
                //Log.Debug("脚本编辑器窗口 Color Exception: " + colorEx.ToString());
                //throw colorEx;
            }
        }
    }

    /// <summary>
    /// 包围盒 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.Bounds)]
    public class Bounds_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            //EditorGUI.indentLevel = 2;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(10));
            paramObject = EditorGUILayout.BoundsField((Bounds)(paramObject));
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 整型包围盒 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.BoundsInt)]
    public class BoundsInt_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            //EditorGUI.indentLevel = 2;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(10));
            paramObject = EditorGUILayout.BoundsIntField((BoundsInt)(paramObject));
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 脚本事件 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.ScriptEvent)]
    public class ScriptEvent_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            EditorGUI.indentLevel = 2;
            paramObject = EditorGUILayout.Popup((int)paramObject, ScriptEventManager.GetEventNames());
        }
    }

    /// <summary>
    /// 用户自定义函数 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.UserDefineFun)]
    public class UserDefineFun_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            EditorGUI.indentLevel = 2;
            string[] userDefineFunNamesArray = null;
            if (scriptManager) userDefineFunNamesArray = scriptManager.GetFunctionNames();
            if (userDefineFunNamesArray == null || userDefineFunNamesArray.Length == 0)
            {
                EditorGUILayout.SelectableLabel("无自定义函数");
                paramObject = 0;
            }
            else
            {
                int index = (int)paramObject;
                if (index < 0 || index >= userDefineFunNamesArray.Length) index = 0;
                paramObject = EditorGUILayout.Popup(index, userDefineFunNamesArray);
            }
        }
    }

    /// <summary>
    /// 矩形 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.Rect)]
    public class Rect_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            //EditorGUI.indentLevel = 2;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(10));
            paramObject = EditorGUILayout.RectField((Rect)paramObject);
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 整型矩形 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.RectInt)]
    public class RectInt_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            EditorGUI.indentLevel = 2;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(25));
            paramObject = EditorGUILayout.RectIntField((RectInt)paramObject);
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 二维向量 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.Vector2)]
    public class Vector2_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            //EditorGUI.indentLevel = 2;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(15));
            paramObject = EditorGUILayout.Vector2Field("", (Vector2)paramObject, GUILayout.Height(16));
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 整型二维向量 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.Vector2Int)]
    public class Vector2Int_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            //EditorGUI.indentLevel = 2;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(15));
            paramObject = EditorGUILayout.Vector2IntField("", (Vector2Int)paramObject, GUILayout.Height(16));
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 最大最小滑动条 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.MinMaxSlider)]
    public class MinMaxSlider_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            EditorGUI.indentLevel = 2;
            float[] comboInfo = param.GetLimitArray<float>();
            float leftValue = ScriptOption.scriptParamFloatLeft;
            float rightValue = ScriptOption.scriptParamFloatRight;
            if (comboInfo != null && comboInfo.Length >= 2)
            {
                leftValue = comboInfo[0];
                rightValue = comboInfo[1];
            }
            Vector2 v2 = (Vector2)paramObject;
            UICommonFun.MinMaxSliderLayout(ref v2.x, ref v2.y, leftValue, rightValue, 100);
            paramObject = v2;
        }
    }

    /// <summary>
    /// 三维向量 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.Vector3)]
    public class Vector3_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            //EditorGUI.indentLevel = 2;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(15));
            paramObject = EditorGUILayout.Vector3Field("", (Vector3)paramObject, GUILayout.Height(0));
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 整型三维向量 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.Vector3Int)]
    public class Vector3Int_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            //EditorGUI.indentLevel = 2;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(15));
            paramObject = EditorGUILayout.Vector3IntField("", (Vector3Int)paramObject, GUILayout.Height(0));
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 四维向量 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.Vector4)]
    public class Vector4_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            //EditorGUI.indentLevel = 2;
            string[] comboInfo = param.GetLimitArray<string>();//param.tag as string[];
            string remark = CommonFun.ArrayToString<string>(comboInfo);
            if (string.IsNullOrEmpty(remark)) remark = "****";
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(25));
            paramObject = EditorGUILayout.Vector4Field(remark, (Vector4)paramObject, GUILayout.Height(2));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator();
        }
    }

    /// <summary>
    /// 整型 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.Int)]
    public class Int_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            EditorGUI.indentLevel = 2;
            paramObject = EditorGUILayout.IntField((int)paramObject);
        }
    }

    /// <summary>
    /// 整型滑动条 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.IntSlider)]
    public class IntSlider_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            EditorGUI.indentLevel = 2;
            int[] comboInfo = param.GetLimitArray<int>();//param.tag as int[];
            int leftValue = ScriptOption.scriptParamIntLeft;
            int rightValue = ScriptOption.scriptParamIntRight;
            if (comboInfo != null && comboInfo.Length >= 2)
            {
                leftValue = comboInfo[0];
                rightValue = comboInfo[1];
            }
            paramObject = EditorGUILayout.IntSlider((int)paramObject, leftValue, rightValue);
        }
    }

    /// <summary>
    /// 整型弹框 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.IntPopup)]
    public class IntPopup_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            EditorGUI.indentLevel = 2;
            string[] nameArray = param.GetLimitArray<string>();
            int[] valueArray = param.GetLimitArray<int>();
            if (nameArray.Length < valueArray.Length)
            {
                List<string> nameList = new List<string>(nameArray);
                for (int i = nameArray.Length; i < valueArray.Length; ++i)
                {
                    nameList.Add(valueArray[i].ToString());
                }
                paramObject = EditorGUILayout.IntPopup((int)paramObject, nameList.ToArray(), valueArray);
            }
            else if (nameArray.Length > valueArray.Length)
            {
                List<string> nameList = new List<string>(nameArray);
                for (int i = 0; i < valueArray.Length; ++i)
                {
                    nameList.Add(nameArray[i]);
                }
                paramObject = EditorGUILayout.IntPopup((int)paramObject, nameArray, valueArray);
            }
            else
            {
                paramObject = EditorGUILayout.IntPopup((int)paramObject, nameArray, valueArray);
            }
        }
    }

    /// <summary>
    /// 长整型 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.Long)]
    public class Long_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            EditorGUI.indentLevel = 2;
            paramObject = EditorGUILayout.LongField((long)paramObject);
        }
    }

    /// <summary>
    /// 双精度浮点数 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.Double)]
    public class Double_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            EditorGUI.indentLevel = 2;
            paramObject = EditorGUILayout.DoubleField((double)paramObject);
        }
    }

    /// <summary>
    /// 单精度浮点数 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.Float)]
    public class Float_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            EditorGUI.indentLevel = 2;
            paramObject = EditorGUILayout.FloatField((float)paramObject);
        }
    }

    /// <summary>
    /// 浮点数滑动条 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.FloatSlider)]
    public class FloatSlider_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            EditorGUI.indentLevel = 2;
            float[] comboInfo = param.GetLimitArray<float>();//param.tag as float[];
            float leftValue = ScriptOption.scriptParamFloatLeft;
            float rightValue = ScriptOption.scriptParamFloatRight;
            if (comboInfo != null && comboInfo.Length >= 2)
            {
                leftValue = comboInfo[0];
                rightValue = comboInfo[1];
            }
            paramObject = EditorGUILayout.Slider((float)paramObject, leftValue, rightValue);
        }
    }

    /// <summary>
    /// 组合 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.Combo)]
    public class Combo_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            EditorGUI.indentLevel = 2;
            string[] comboInfo = param.GetLimitArray<string>();//param.tag as string[];
            if (comboInfo == null)
            {
                throw new InvalidOperationException("脚本(Combo):[" + scriptStringInfo.script.name[Language.languageType] + "]的第[" + paramIndex + "]参数:[" + param.name[Language.languageType] + "]无效！");
            }
            char[] chArray = new char[] { ScriptOption.ScriptParamDelimiterChar, ScriptOption.ScriptParamDelimiterCharCN, ScriptOption.WrapLineChar };
            foreach (var spv in comboInfo)
            {
                //检测非法字符
                if (spv.IndexOfAny(chArray) >= 0)
                {
                    throw new InvalidOperationException("脚本(Combo):[" + scriptStringInfo.script.name[Language.languageType] + "]参数: [" + paramName + "] 的每个Combo字符串中不可以出现 " + CommonFun.ArrayToString<char>(chArray) + " 等字符！");
                }
            }
            int index = Array.IndexOf(comboInfo, paramObject as string);
            index = EditorGUILayout.Popup(index, comboInfo);
            if (index < 0 || index >= comboInfo.Length)
            {
                paramObject = param.defaultObject ?? comboInfo.FirstOrDefault() ?? "";
                GUI.changed = true;
            }
            else paramObject = comboInfo[index];
        }
    }

    /// <summary>
    /// 键码 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.KeyCode)]
    public class KeyCode_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            EditorGUI.indentLevel = 2;
            paramObject = UICommonFun.EnumPopup((KeyCode)paramObject);
        }
    }

    /// <summary>
    /// 布尔 脚本参数绘制器
    /// </summary>    
    [ScriptParamDrawer(EParamType.Bool)]
    public class Bool_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            EditorGUI.indentLevel = 2;
            paramObject = UICommonFun.EnumPopup((EBool)paramObject);
        }
    }

    /// <summary>
    /// 布尔2 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.Bool2)]
    public class Bool2_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            EditorGUI.indentLevel = 2;
            paramObject = UICommonFun.EnumPopup((EBool2)paramObject);
        }
    }

    /// <summary>
    /// 布尔2 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.B2)]
    public class B2_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            EditorGUI.indentLevel = 2;
            paramObject = UICommonFun.EnumPopup((EB2)paramObject);
        }
    }

    /// <summary>
    /// 坐标轴类型 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.CoordinateType)]
    public class CoordinateType_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            EditorGUI.indentLevel = 2;
            paramObject = UICommonFun.EnumPopup((ECoordinateType)paramObject);
        }
    }

    /// <summary>
    /// 文本锚点 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.TextAnchor)]
    public class TextAnchor_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            EditorGUI.indentLevel = 2;
            paramObject = UICommonFun.EnumPopup((TextAnchor)paramObject);
        }
    }

    /// <summary>
    /// 运行时平台 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.RuntimePlatform)]
    public class RuntimePlatform_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            EditorGUI.indentLevel = 2;
            paramObject = UICommonFun.EnumPopup((RuntimePlatform)paramObject);
        }
    }

    /// <summary>
    /// 鼠标按钮 脚本参数绘制器
    /// </summary>
    [ScriptParamDrawer(EParamType.MouseButton)]
    public class MouseButton_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            //base.OnDrawContent();
            EditorGUI.indentLevel = 2;
            paramObject = UICommonFun.EnumPopup((EMouseButtonType)paramObject);
        }
    }
}
