using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorExtension.Base.Attributes;
using XCSJ.EditorExtension.Base.Kernel;
using XCSJ.EditorExtension.Base.XUnityEditor;
using XCSJ.EditorExtension.Base.XUnityEditor.PackageManager.UI;
using XCSJ.EditorExtension.CNScripts;
using XCSJ.Extension.Base;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Menus;
using XCSJ.Scripts;

namespace XCSJ.EditorExtension.Base
{
    /// <summary>
    /// 编辑器辅助类
    /// </summary>
    public static class EditorHelper
    {
        /// <summary>
        /// UnityEditor前缀
        /// </summary>
        public const string UnityEditorPrefix = nameof(UnityEditor) + ".";

        /// <summary>
        /// UnityEditorInternal前缀
        /// </summary>
        public const string UnityEditorInternalPrefix = nameof(UnityEditorInternal) + ".";

        /// <summary>
        /// 初始化
        /// </summary>
        [InitializeOnLoadMethod]
        public static void Init()
        {
            //优先插件初始化
            PlguinsHelper.Init();
            EditorHandlerExtension.Init();
            TypeMacroHelper.Init();
            EditorInputSystemHelper.Init();
            //EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemCallback;

            XDreamerEvents.onSceneAnyAssetsChanged += EditorObjectHelper.ComponentCollectionCache.Clear;

            EditorScriptHelper.Init();

#if XDREAMER_EDITION_XDREAMERDEVELOPER
            CategoryListExtension.onBeforeDrawItem += OnBeforeDrawItem_CategoryList;
            ScriptViewerWindow.onAfterCommandTitle += OnAfterCommandTitle;
#endif
        }

        /// <summary>
        /// 编辑
        /// </summary>
        [Name("编辑")]
        [Tip("编辑脚本命令所在的脚本")]
        [XCSJ.Attributes.Icon(EIcon.Edit)]
        public static XGUIContent editGUIContent { get; } = new XGUIContent(typeof(EditorHelper), nameof(editGUIContent), true);

        private static void OnAfterCommandTitle(ScriptViewerWindow scriptViewerWindow, ScriptStringInfo scriptStringInfo, ScriptString scriptString, IVariableHandle variableHandle)
        {
            if (GUILayout.Button(editGUIContent, EditorStyles.miniButton, UICommonOption.Width32, UICommonOption.Height20))
            {
                OpenMonoScript(scriptStringInfo.script?.owner?.GetType());
            }
        }

        /// <summary>
        /// 打开首选项窗口
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static EditorWindow OpenPreferencesWindow(string name = "")
        {
            return DefaultEditorHandler.instance.OpenPreferencesWindow(name);
        }

        /// <summary>
        /// 打开项目设置窗口
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static EditorWindow OpenProjectSettingsWindow(string name = "")
        {
            return DefaultEditorHandler.instance.OpenProjectSettingsWindow(name);
        }

        #region 导入UnityPackage

        /// <summary>
        /// 如果需要导入包
        /// </summary>
        /// <param name="macro"></param>
        /// <param name="packagePath"></param>
        /// <param name="confirmImportFunc"></param>
        public static void ImportPackageIfNeed(Macro macro, string packagePath, Func<bool> confirmImportFunc = null)
        {
            if (macro == null || string.IsNullOrEmpty(packagePath)) return;
            if (confirmImportFunc == null) confirmImportFunc = () => true;

            //可以定义但是又没有定义对应的Macro,那么可能是UnityPackage未导入
            if (macro.CanDefindInSelected() && !macro.DefindInSelected() && confirmImportFunc())
            {
                var fullPath = GetValidFullPath(packagePath);
                //Debug.Log(GetValidFullPath(packagePath));
                if (FileHelper.Exists(fullPath))
                {
                    AssetDatabase.ImportPackage(fullPath, true);
                }
                else
                {
                    Log.WarningFormat("导入UnityPackage时未找到文件:{0}", fullPath);
                }
            }
        }

        /// <summary>
        /// 按钮点击后，如果需要导入包
        /// </summary>
        /// <param name="macro"></param>
        /// <param name="packagePath"></param>
        public static void ImportPackageIfNeedWithButton(Macro macro, string packagePath)
        {
            ImportPackageIfNeed(macro, packagePath, () =>
            {
                var bk = GUI.backgroundColor;
                GUI.backgroundColor = XDreamerBaseOption.weakInstance.errorColor;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(CommonFun.TempContent("库缺失", "逻辑执行需要的库缺失"));
                var ret = GUILayout.Button(CommonFun.TempContent("导入", "导入[" + packagePath + "]资产"));
                EditorGUILayout.EndHorizontal();
                GUI.backgroundColor = bk;
                return ret;
            });
        }

        /// <summary>
        /// 弹出对话框并确认后，如果需要导入包
        /// </summary>
        /// <param name="macro"></param>
        /// <param name="packagePath"></param>
        public static void ImportPackageIfNeedWithDialog(Macro macro, string packagePath)
        {
            ImportPackageIfNeed(macro, packagePath, () => EditorUtility.DisplayDialog("库缺失", "导入[" + packagePath + "]资产", "导入", "取消"));
        }

        /// <summary>
        /// 获取有效的全路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetValidFullPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return "";
            if (path.IndexOf("/Assets/") == 0)
            {
                path = Application.dataPath + path.Substring("/Assets".Length);
            }
            else if (path.IndexOf("Assets/") == 0)
            {
                path = Application.dataPath + path.Substring("Assets".Length);
            }
            return path;
        }

        #endregion

        #region 打开包管理器

        /// <summary>
        /// 如果需要打开包管理器
        /// </summary>
        /// <param name="macro"></param>
        /// <param name="packageNameOrDisplayName"></param>
        /// <param name="confirmImportFunc"></param>
        public static void OpenPackageManagerIfNeed(Macro macro, string packageNameOrDisplayName, Func<bool> confirmImportFunc = null)
        {
            if (macro == null || string.IsNullOrEmpty(packageNameOrDisplayName)) return;
            if (confirmImportFunc == null) confirmImportFunc = () => true;

            //可以定义但是又没有定义对应的Macro,那么可能是包管理器中的某些包未导入
            if (macro.CanDefindInSelected() && !macro.DefindInSelected() && confirmImportFunc())
            {
                PackageManagerWindow.OpneWindow(packageNameOrDisplayName);
            }
        }

        /// <summary>
        /// 按钮点击后，如果需要打开包管理器
        /// </summary>
        /// <param name="macro"></param>
        /// <param name="packageNameOrDisplayName"></param>
        public static void OpenPackageManagerIfNeedWithButton(Macro macro, string packageNameOrDisplayName)
        {
            OpenPackageManagerIfNeed(macro, packageNameOrDisplayName, () =>
            {
                var bk = GUI.backgroundColor;
                GUI.backgroundColor = XDreamerBaseOption.weakInstance.errorColor;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(CommonFun.TempContent("库缺失", "逻辑执行需要的库缺失"));
                var ret = GUILayout.Button(CommonFun.TempContent("打开", "打开[" + packageNameOrDisplayName + "]包管理器"));
                EditorGUILayout.EndHorizontal();
                GUI.backgroundColor = bk;
                return ret;
            });
        }

        /// <summary>
        /// 弹出对话框并确认后，如果需要打开包管理器
        /// </summary>
        /// <param name="macro"></param>
        /// <param name="packageNameOrDisplayName"></param>
        public static void OpenPackageManagerIfNeedWithDialog(Macro macro, string packageNameOrDisplayName)
        {
            OpenPackageManagerIfNeed(macro, packageNameOrDisplayName, () => EditorUtility.DisplayDialog("库缺失", "打开[" + packageNameOrDisplayName + "]包管理器", "打开", "取消"));
        }

        #endregion

        /// <summary>
        /// 设置图标
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="icon"></param>
        /// <param name="overwrite"></param>
        public static void SetIcon(UnityEngine.Object obj, Texture2D icon, bool overwrite = true)
        {
            if (!obj || !icon) return;
            var setIcon = EditorGUIUtility_LinkType.GetIconForObject(obj);
            if (setIcon != null && !overwrite) return;

            EditorGUIUtility_LinkType.SetIconForObject(obj, icon);
            EditorUtility_LinkType.ForceReloadInspectors();
            MonoImporter_LinkType.CopyMonoScriptIconToImporters(obj as MonoScript);
        }

        #region 层级窗口

        /// <summary>
        /// 当层级窗口绘制的Gizmos图标被点击时回调
        /// </summary>
        public static event Action<Component, Rect, Texture2D> onHierarchyWindowItemGizmosIconClicked;

        /// <summary>
        /// 调用层级窗口绘制的Gizmos图标被点击事件
        /// </summary>
        /// <param name="component"></param>
        /// <param name="rect"></param>
        /// <param name="texture2D"></param>
        private static void CallHierarchyWindowItemGizmosIconClicked(Component component, Rect rect, Texture2D texture2D)
        {
            onHierarchyWindowItemGizmosIconClicked?.Invoke(component, rect, texture2D);
            EditorApplicationExtension.CallHierarchyWindowItemGizmosIconClicked(component, rect, texture2D);
        }

        /// <summary>
        /// 层级窗口项的绘制回调
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="selectionRect"></param>
        private static void HierarchyWindowItemCallback(int instanceID, Rect selectionRect)
        {
            if (!(EditorUtility.InstanceIDToObject(instanceID) is GameObject go)) return;
            var rt = new Rect(selectionRect.xMax, selectionRect.y, selectionRect.height, selectionRect.height);
            //GUI.Box(selectionRect,"");
            foreach (var component in go.GetComponents<Component>())
            {
                //特别说明：
                //剔除组件无效情况，否则会导致整个HierarchyWindow渲染混乱；
                //组件失效可能原因:DLL中命名空间调整、组件代码文件移除、Meta文件中guid变更等情况引起的组件反序列化失败！
                if (component && GizmosIconLib.instance.TryGet(component.GetType(), out Texture2D texture2D))
                {
                    rt.x -= selectionRect.height;
                    if (GUI.Button(rt, texture2D, GUIStyle.none))
                    {
                        CallHierarchyWindowItemGizmosIconClicked(component, rt, texture2D);
                    }
                }
            }
            //
            HierarchyWindowItemOnEvent();
        }

        /// <summary>
        /// 层级窗口项的事件处理
        /// </summary>
        private static void HierarchyWindowItemOnEvent()
        {
            Event e = Event.current;
            switch (e.type)
            {
                case EventType.DragPerform:
                    {
                        //Log.Debug("DragPerform");
                        bool use = false;
                        //如果拖动的是特殊的manager~不允许执行
                        foreach (var obj in DragAndDrop.objectReferences)
                        {
                            if (!(obj is GameObject go)) continue;
                            if (CommonFun.GameObjectIsRootOrContainIBaseManager(go))
                            {
                                //Log.Debug(obj.GetType().Name + " , " + obj.name);
                                use = true;
                            }
                        }
                        if (use) e.Use();
                        break;
                    }
                case EventType.DragUpdated:
                    {
                        //Log.Debug("DragUpdated");
                        bool use = false;
                        //如果拖动的是特殊的manager~不允许执行
                        foreach (var obj in DragAndDrop.objectReferences)
                        {
                            if (!(obj is GameObject go)) continue;
                            if (CommonFun.GameObjectIsRootOrContainIBaseManager(go))
                            {
                                //Log.Debug(obj.GetType().Name + " , " + obj.name);
                                use = true;
                            }
                        }
                        if (use) e.Use();
                        break;
                    }
                case EventType.DragExited:
                    {
                        //Log.Debug("DragExited");
                        //e.Use();
                        break;
                    }
            }
        }

        /// <summary>
        /// 设置层级窗口选中游戏对象展开和折叠
        /// </summary>
        /// <param name="expand"></param>
        public static void SetHierarchySelectionExpanded(bool expand)
        {
            Selection.gameObjects.Foreach(go => SetHierarchyExpanded(go, expand));
        }

        /// <summary>
        /// 设置层级窗口选中T类型游戏对象展开和折叠
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="expand">展开或折叠</param>
        public static void SetHierarchySelectionExpanded<T>(bool expand) where T : Component
        {
            Selection.gameObjects.Foreach(go => SetHierarchyExpanded(go, expand));
        }

        /// <summary>
        /// 设置层级窗口所有T类型游戏对象展开和折叠
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="expand">展开或折叠</param>
        /// <param name="includeInactive">包含不活跃对象</param>
        public static void SetHierarchyExpanded<T>(bool expand, bool includeInactive = true) where T : Component
        {
            GetAllGameObject<T>(includeInactive).Foreach(go => SetHierarchyExpanded(go, expand));
        }

        /// <summary>
        /// 获取所有机构游戏对象
        /// </summary>
        /// <param name="includeInactive">包含不活跃对象</param>
        /// <param name="sortChildFirst">子节点靠前，父对象在链表尾部</param>
        /// <returns>游戏对象集</returns>
        public static IEnumerable<GameObject> GetAllGameObject<T>(bool includeInactive = true, bool sortChildFirst = true) where T : Component
        {
            var all = CommonFun.GetComponentsInChildren(typeof(T), includeInactive).ToList().ConvertAll(c => c.gameObject);

            var list = all.Distinct().ToList();
            if (sortChildFirst) list.Sort((x, y) => x.transform.IsChildOf(y.transform) ? -1 : 0);
            return list;
        }

        /// <summary>
        /// 使用反射方法展开和折叠层级窗口中的游戏对象
        /// 注意：折叠时，必须先折叠子对象，父对象才能折叠。
        /// </summary>
        /// <param name="gameObject">游戏对象</param>
        /// <param name="expand">展开或折叠</param>
        public static void SetHierarchyExpanded(GameObject gameObject, bool expand)
        {
            //var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
            //var methodInfo = type.GetMethod("SetExpandedRecursive");

            //EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
            //var window = EditorWindow.focusedWindow;
            //methodInfo.Invoke(window, new object[] { gameObject.GetInstanceID(), expand });

            var window = SceneHierarchyWindow.s_LastInteractedHierarchy;
            if (window)
            {
                window.SetExpandedRecursive(gameObject.GetInstanceID(), expand);
                window.editorWindow.Repaint();
            }
        }

        #endregion

        /// <summary>
        /// 获取对象在磁盘上的全路径
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetFullPath(this UnityEngine.Object obj)
        {
            if (!obj) return null;
            var assetPath = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(assetPath)) return null;
            return UICommonFun.ToFullPath(assetPath);
        }

        /// <summary>
        /// 获取Mono脚本
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MonoScript GetMonoScript(this Type type)
        {
            if (type == null) return default;
            while (type.DeclaringType != null)//处理嵌套类情况
            {
                type = type.DeclaringType;
            }
            return Resources.FindObjectsOfTypeAll<MonoScript>().FirstOrDefault(s => s.GetClass() == type);
        }

        /// <summary>
        /// 打开Mono脚本
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public static MonoScript OpenMonoScript(this MonoScript script)
        {
            if (script) AssetDatabase.OpenAsset(script);
            return script;
        }

        /// <summary>
        /// 打开Mono脚本
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MonoScript OpenMonoScript(this Type type) => OpenMonoScript(GetMonoScript(type));

        /// <summary>
        /// 打开Mono脚本
        /// </summary>
        /// <param name="scriptableObject"></param>
        /// <returns></returns>
        public static MonoScript OpenMonoScript(this ScriptableObject scriptableObject) => OpenMonoScript(scriptableObject ? MonoScript.FromScriptableObject(scriptableObject) : default);

        /// <summary>
        /// 打开Mono脚本
        /// </summary>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        public static MonoScript OpenMonoScript(this MonoBehaviour behaviour) => OpenMonoScript(behaviour ? MonoScript.FromMonoBehaviour(behaviour) : default);

        /// <summary>
        /// 打开Mono脚本
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static MonoScript OpenMonoScript(this MemberInfo memberInfo)
        {
            if (memberInfo is Type type)
            {
                return OpenMonoScript(type);
            }
            else if (memberInfo is MethodInfo methodInfo)
            {
                return OpenMonoScript(methodInfo.ReflectedType ?? methodInfo.DeclaringType);
            }
            return default;
        }

        /// <summary>
        /// 获取检查器Mono脚本
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static MonoScript GetInspectorMonoScript(this UnityEngine.Object obj) => obj ? MonoScript.FromScriptableObject(BaseInspector.GetEditor(obj)) : default;

        /// <summary>
        /// 获取检查器Mono脚本
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MonoScript GetInspectorMonoScript(this Type type) => GetMonoScript(BaseInspector.GetEditorType(type));

        /// <summary>
        /// 打开检查器Mono脚本
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static MonoScript OpenInspectorMonoScript(this UnityEngine.Object obj) => OpenMonoScript(GetInspectorMonoScript(obj));

        /// <summary>
        /// 打开检查器Mono脚本
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MonoScript OpenInspectorMonoScript(this Type type) => OpenMonoScript(GetInspectorMonoScript(type));

        private static void OnBeforeDrawItem_CategoryList(IItem item, Rect rect)
        {
            var btnRect = new Rect(rect.x, rect.y, rect.height, rect.height);
            if (GUI.Button(btnRect, ""))
            {
                OpenMonoScript(item.memberInfo);
            }
        }

        /// <summary>
        /// 绘制菜单
        /// </summary>
        /// <param name="selectText"></param>
        /// <param name="texts"></param>
        /// <returns></returns>
        public static void DrawMenu(string selectText, string[] texts, Action<string> onMenuItemClicked)
        {
            var tmpSelectText = selectText;
            var menu = new GenericMenu();
            foreach (var text in texts)
            {
                var tempText = text;//做换存量
                menu.AddItem(new GUIContent(tempText), tempText == selectText, () => onMenuItemClicked?.Invoke(tempText));
            }
            menu.ShowAsContext();
        }

        /// <summary>
        /// 绘制菜单
        /// </summary>
        /// <param name="selectText"></param>
        /// <param name="texts"></param>
        /// <param name="onMenuItemClicked"></param>
        public static void DrawMenu(string selectText, string[] texts, Action<int, string> onMenuItemClicked)
        {
            var tmpSelectText = selectText;
            var menu = new GenericMenu();
            int i = 0;
            foreach (var text in texts)
            {
                var tempText = text;//做换存量
                var tempIndex = i;
                menu.AddItem(new GUIContent(tempText), tempText == selectText, () => onMenuItemClicked?.Invoke(tempIndex, tempText));
                i++;
            }
            menu.ShowAsContext();
        }

        /// <summary>
        /// 变量字符串弹出式菜单
        /// </summary>
        /// <param name="varString"></param>
        /// <param name="onMenuItemClicked"></param>
        /// <param name="options"></param>
        public static void VarStringPopup(string varString, Action<string> onMenuItemClicked, params GUILayoutOption[] options)
        {
            if (GUILayout.Button(CommonFun.TempContent(varString, varString), EditorObjectHelper.MiniPopup, options))
            {
                var scriptManager = ScriptManager.instance;
                if (!scriptManager) return;

                var varStrings = new List<string>();

                foreach (var varScope in EnumCache<EVarScope>.Array)
                {
                    if (scriptManager.varContext.TryGetVarCollection(varScope, out var varCollection) && varCollection != null)
                    {
                        var start = CommonFun.Name(varCollection.varScope) + "/";

                        foreach (var kv in varCollection.varDictionary)
                        {
                            kv.Value.hierarchyVar.Foreach((parent, index, key, current) =>
                            {
                                if (parent == null)
                                {
                                    varStrings.Add(start + current.varHierarchyString.Replace("/", ""));
                                }
                                else
                                {
                                    varStrings.Add(start + current.varHierarchyString);
                                }
                            });
                        }
                    }
                }

                DrawMenu(varString, varStrings.ToArray(), newSelectText =>
                {
                    onMenuItemClicked?.Invoke(newSelectText.Substring(newSelectText.IndexOf("/") + 1));
                });
            }
        }
    }
}
