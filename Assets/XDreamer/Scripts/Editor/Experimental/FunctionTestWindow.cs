using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.EditorCommonUtils;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.Extension.Base.InputSystems;
using XCSJ.Collections;
using XCSJ.EditorExtension.EditorWindows;
using XCSJ.PluginSMS.Kernel;
using XCSJ.EditorTools.Windows.CodeCreaters;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils.Safety;
using XCSJ.Kernel;
using System.IO;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace XCSJ.EditorExtension.Experimental
{
    /// <summary>
    /// 功能测试窗口类！开发测试使用！
    /// </summary>
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.Debug)]
    [XDreamerEditorWindow("开发者专用")]
    public class FunctionTestWindow : XEditorWindowWithScrollView<FunctionTestWindow>
    {
        public const string Title = "功能测试窗口";

        /// <summary>
        /// 初始化
        /// </summary>
#if XDREAMER_EDITION_XDREAMERDEVELOPER
        [MenuItem(XDreamerMenu.NamePath + Title)]
#endif
        public static void Init() => OpenAndFocus();

        public Vector2 v1 = new Vector2(200, 0);
        public Vector2 v2 = new Vector2(200, 0);

        public GameObject gameObject;

        /// <summary>
        /// 渲染界面UI时
        /// </summary>
        public override void OnGUIWithScrollView()
        {
            EditorGUILayout.Separator();
            try
            {
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("材质查找"))
                    {
                        Material[] mats = Resources.FindObjectsOfTypeAll<Material>();
                        foreach (var mat in mats)
                        {
                            Log.Debug(mat);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    //GUILayout.Label("All " + Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object)).Length);
                    //GUILayout.Label("Textures " + Resources.FindObjectsOfTypeAll(typeof(Texture)).Length);
                    //GUILayout.Label("AudioClips " + Resources.FindObjectsOfTypeAll(typeof(AudioClip)).Length);
                    //GUILayout.Label("Meshes " + Resources.FindObjectsOfTypeAll(typeof(Mesh)).Length);
                    //GUILayout.Label("Materials " + Resources.FindObjectsOfTypeAll(typeof(Material)).Length);
                    //GUILayout.Label("GameObjects " + Resources.FindObjectsOfTypeAll(typeof(GameObject)).Length);
                    //GUILayout.Label("Components " + Resources.FindObjectsOfTypeAll(typeof(Component)).Length);


                    if (GUILayout.Button("查找UnityEngine命名空间的Component所有子类并输出到文件"))
                    {
                        FindSubclassOfComponentAndOutputToFile();
                    }

                    if (GUILayout.Button("查找UnityEngine命名空间的Object所有子类并输出到文件"))
                    {
                        FindSubclassOfObjectAndOutputToFile();
                    }
                    
                    if (GUILayout.Button("查找UnityEditor.EditorWindow子类"))
                    {
                        foreach (var t in TypeHelper.FindTypeInAppWithClass(typeof(EditorWindow), false))
                        {
                            Debug.LogFormat("{0}=>{1}", t.Assembly.Location, t.Name);
                        }
                    }
                    if (GUILayout.Button("查找UnityEditor.EditorWindow子类并且窗口打开的"))
                    {
                        foreach (var t in TypeHelper.FindTypeInAppWithClass(typeof(EditorWindow), false))
                        {
                            var array = Resources.FindObjectsOfTypeAll(t);
                            if (array != null && array.Length > 0)
                            {
                                Debug.LogFormat("{0}=>{1}", t.Assembly.Location, t.Name);
                            }
                        }
                    }
#if ENABLE_INPUT_SYSTEM
                    if (GUILayout.Button("检查EInputDevice枚举与InputDevice类匹配情况"))
                    {
                        var types = TypeHelper.FindTypeInAppWithClass(typeof(InputDevice), true, true);
                        var array = EnumCache<EInputDevice>.Array.Where(id => id != EInputDevice.None);
                        Debug.Log("InputDevice类数目：" + types.Count);
                        Debug.Log("EInputDevice枚举有效字段数目：" + array.Count());
                        types.Where(t => !array.Any(id => id.ToString() == t.Name)).Foreach(t => Debug.LogWarning("InputDevice类：[" + t + "]在EInputDevice枚举中缺失！"));
                        array.Where(id => !types.Any(t => id.ToString() == t.Name)).Foreach(t => Debug.LogWarning("EInputDevice枚举：[" + t + "]无InputDevice类与之对应！"));
                        array.Where(id =>
                        {
                            var type = InputDeviceTypeAttribute.GetInputDeviceType(id);
                            return !types.Contains(type) || type?.Name != id.ToString();
                        }).Foreach(t => Debug.LogWarning("EInputDevice枚举：[" + t + "]无有效InputDeviceTypeAttribute特性修饰的InputDevice类与之对应！"));

                        var map = new Dictionary<int, EInputDevice>();
                        array.Foreach(id =>
                        {
                            if (map.ContainsKey((int)id))
                            {
                                Debug.LogWarning("EInputDevice枚举中：[" + id.ToString() + "]与[" + map[(int)id].ToString() + "]的数值[" + ((int)id).ToString() + "]重复！");
                            }
                            else
                            {
                                map.Add((int)id, id);
                                //Debug.Log("EInputDevice枚举字符串：[" + id.ToString() + "]与值[" + ((int)id).ToString() + "]");
                            }
                        });
                    }
#endif                    

                    gameObject = EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true) as GameObject;

                    //if (Event.current.type == EventType.MouseDown)
                    //{
                    //    GameObject go = null;
                    //    foreach (var p in AssetDatabase.GetAllAssetPaths())
                    //    {
                    //        go = AssetDatabase.LoadMainAssetAtPath(p) as GameObject;
                    //        if (go)
                    //        {
                    //            Debug.Log(p);
                    //            break;
                    //        }
                    //    }
                    //    if (go)
                    //    {
                    //        Debug.Log(go);
                    //        DragAndDrop.objectReferences = new UnityEngine.Object[] { go };
                    //        DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                    //        DragAndDrop.StartDrag("开始拖拽");
                    //    }
                    //}                    

                    {
                        var rect = EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                        //Debug.Log(rect);

                        GUILayout.Box("left", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));

                        v1 = UICommonFun.ResizeSeparatorLayout(v1, true, false, 100, rect.width * 0.4f, GUILayout.Width(10), GUILayout.ExpandHeight(true));

                        GUILayout.Box("middle", GUILayout.ExpandHeight(true), GUILayout.Width(v1.x));


                        v2 = UICommonFun.ResizeSeparatorLayout(v2, true, false, 100, rect.width * 0.4f, GUILayout.Width(10), GUILayout.ExpandHeight(true));

                        GUILayout.Box("middle", GUILayout.ExpandHeight(true), GUILayout.Width(v2.x));

                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception("函数测试窗口 Exception: " + ex.ToString());
            }
            finally
            {
                EditorGUILayout.EndVertical();
            }
        }

        #region 查找所有Unity Component

        public List<Type> FindAllSubclassOfComponentInUnityEngineNameSpace()
        {
            return TypeHelper.FindTypeInAppWithClass(typeof(Component)).Where(t => t.FullName.StartsWith("UnityEngine.") && !ObsoleteAttributeCache.Exist(t)).ToList();
        }

        private List<Type> ignoreSubclassOfComponent = new List<Type>
        {
            //typeof(UnityEngine.NetworkView),
        };

        private void FindSubclassOfComponentAndOutputToFile()
        {
            var filePath = EditorUtility.SaveFilePanel("保存", Application.streamingAssetsPath, "compontents", "txt");
            FindSubclassOfComponentAndOutputToFile(filePath);
        }

        private void FindSubclassOfComponentAndOutputToFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;
            StringBuilder sb = new StringBuilder();

            var types = FindAllSubclassOfComponentInUnityEngineNameSpace().Where(t => !ignoreSubclassOfComponent.Contains(t)
            && !t.FullName.StartsWith("UnityEngine.Networking.")).ToList();

            types.Sort((x, y) => string.Compare(x.FullName, y.FullName));

            foreach (var type in types)
            {
                sb.AppendFormat("            typeof({0}),\r\n", type.FullName);
            }
            FileHelper.OutputFile(filePath, sb.ToString());
        }

        #endregion

        #region 查找所有UnityEngine.Object

        public List<Type> FindAllSubclassOfObjectInUnityEngineNameSpace()
        {
            return TypeHelper.FindTypeInAppWithClass(typeof(UnityEngine.Object)).Where(t => t.FullName.StartsWith("UnityEngine.") && !ObsoleteAttributeCache.Exist(t)).ToList();
        }

        private List<Type> ignoreSubclassOfObject = new List<Type>
        {
            //typeof(UnityEngine.NetworkView),
        };

        private void FindSubclassOfObjectAndOutputToFile()
        {
            var filePath = EditorUtility.SaveFilePanel("保存", Application.streamingAssetsPath, "objects", "txt");
            FindSubclassOfObjectAndOutputToFile(filePath);
        }

        private void FindSubclassOfObjectAndOutputToFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;
            StringBuilder sb = new StringBuilder();

            var types = FindAllSubclassOfObjectInUnityEngineNameSpace().Where(t => !ignoreSubclassOfObject.Contains(t)
            && !t.FullName.StartsWith("UnityEngine.Networking.")).ToList();

            types.Sort((x, y) => string.Compare(x.FullName, y.FullName));

            foreach (var type in types)
            {
                sb.AppendFormat("            typeof({0}),\r\n", type.FullName);
            }
            FileHelper.OutputFile(filePath, sb.ToString());
        }

        #endregion
    }
}
