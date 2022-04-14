using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.EditorExtension.Base.Kernel;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Menus;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.States;
using XCSJ.PluginTools;
using XCSJ.PluginTools.Kernel;

namespace XCSJ.EditorTools
{
    /// <summary>
    /// 编辑器脚本辅助扩展
    /// </summary>
    public static class EditorToolsHelperExtension
    {
        /// <summary>
        /// 初始化
        /// </summary>
        [InitializeOnLoadMethod]
        public static void Init()
        {
            DefaultToolsHandler.Init();
        }

        /// <summary>
        /// 创建编辑器-工具库
        /// </summary>
        [MenuItem(XDreamerMenu.NamePath + ToolEditorWindow.Title, false, 340)]
        public static void CreateEditor()
        {
            ToolEditorWindow.OpenAndFocus();
        }

        #region 创建工具

        /// <summary>
        /// 能否创建工具
        /// 第二个参数不为空，则优先检测该类型，
        /// </summary>
        /// <param name="toolItem">工具项</param>
        /// <param name="checkType">优先检测类型</param>
        /// <returns></returns>
        public static bool CanCreateTool(ToolItem toolItem, params Type[] firstCheckType)
        {
            if (toolItem.toolAttribute.disallowMultiple)
            {
                var typeList = new List<Type>();
                if (firstCheckType != null)
                {
                    foreach (var item in firstCheckType)
                    {
                        if (item != null)
                        {
                            typeList.Add(item);
                        }
                    }
                }
                var type = toolItem.memberInfo as Type;
                if (type != null)
                {
                    typeList.Add(type);
                }

                foreach (var t in typeList)
                {
                    var obj = ComponentCache.Get(t, true);
                    if (obj != null && obj.components.Length > 0)
                    {
                        Debug.LogWarningFormat("带[{0}]({1})类型组件的游戏对象[{2}]已经存在，不允许重复创建！",
                            CommonFun.Name(t),
                            t.FullName,
                            CommonFun.GameObjectToStringWithoutAlias(obj.components[0].gameObject));
                        EditorGUIUtility.PingObject(obj.components[0]);
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 工具类
        /// </summary>
        public static void CreateTool(ToolItem toolItem)
        {
            if (toolItem?.memberInfo is Type type)
            {
                var requestComponents = CommonFun.GetTypesOfRequireComponent(type, false);
                PopupAddComponentMenu(toolItem.name, type, requestComponents, toolItem, FindOrCreateRootAndGroup);
            }
        }

        /// <summary>
        /// 查找或创建根和组
        /// </summary>
        /// <param name="toolItem"></param>
        /// <param name="newGo"></param>
        /// <param name="root"></param>
        /// <param name="group"></param>
        public static void FindOrCreateRootAndGroup(ToolItem toolItem, GameObject newGo, out GameObject root, out GameObject group)
        {
            if (toolItem == null || !newGo)
            {
                root = default;
                group = default;
                return;
            }

            // 获取根
            root = FindRoot(toolItem.toolAttribute);

            // 查找和创建组
            var groupName = toolItem.name + CategoryOption.weakInstance.toolParentExtensionName;
            group = FindOrCreateGroup(toolItem.toolAttribute, root, groupName);

            // 父对象
            var parent = group ? group : root;
            if (parent)
            {
                var anchoredPosition = Vector2.zero;
                var rectTransform = newGo.GetComponent<RectTransform>();
                if (rectTransform)
                {
                    anchoredPosition = rectTransform.anchoredPosition;
                }

                var localScale = newGo.transform.localScale;
                {
                    newGo.transform.XSetTransformParent(parent.transform);
                }
                newGo.transform.XSetLocalScale(localScale);

                // 还原
                if (rectTransform)
                {
                    rectTransform.XSetAnchoredPosition(anchoredPosition);
                }
            }
            newGo.XModifyProperty(() => GameObjectUtility.EnsureUniqueNameForSibling(newGo));

            EditorGUIUtility.PingObject(newGo);
        }

        /// <summary>
        /// 查找或创建根和组
        /// </summary>
        /// <param name="toolItem"></param>
        /// <param name="newGo"></param>
        public static void FindOrCreateRootAndGroup(ToolItem toolItem, GameObject newGo) => FindOrCreateRootAndGroup(toolItem, newGo, out _, out _);

        /// <summary>
        /// 查找根节点
        /// </summary>
        /// <returns></returns>
        private static GameObject FindRoot(ToolAttribute toolAttribute)
        {
            var rootType = toolAttribute.rootType;
            if (rootType != null)
            {
                // 全场景查找
                List<UnityEngine.Component> components = (GameObject.FindObjectsOfType(rootType) as UnityEngine.Component[]).ToList();
                if (components.Count > 0)
                {
                    if (toolAttribute.needRootParentIsNull)
                    {
                        var component = components.Find(c => c.transform.parent == null);
                        if (component)
                        {
                            return component.gameObject;
                        }
                    }
                    else
                    {
                        return components[0].gameObject;
                    }
                }

                // 使用默认创建函数
                var root = DefaultViewHandler.instance.Create(rootType.Name, null, rootType);
                if (root)
                {
                    if (!toolAttribute.needRootParentIsNull || root.transform.parent == null)
                    {
                        return root.gameObject;
                    }
                }
            }
            return ToolsManager.instance.gameObject;
        }

        /// <summary>
        /// 查找和创建父节点
        /// 如果tool不允许重复创建，父节点就是根类型，如果未指定根类型，默认是ToolManager
        /// </summary>
        /// <param name="root"></param>
        /// <param name="groupName">工具项</param>
        /// <param name="groupName">工具组名称</param>
        /// <returns>父节点</returns>
        private static GameObject FindOrCreateGroup(ToolAttribute toolAttribute, GameObject root, string groupName)
        {
            if (toolAttribute.groupRule != EToolGroupRule.Create) return null;

            GameObject group = null;

            // 全局唯一对象，直接使用根节点作为父节点
            if (toolAttribute.disallowMultiple)
            {
                group = root;
            }
            else // 根节点下查找对应的组名称
            {
                group = root ? CommonFun.GetChildGameObject(root.transform, groupName) : GameObject.Find(groupName);
            }

            if (toolAttribute.groupRule == EToolGroupRule.Create && !group && root)
            {
                group = EditorHandler.Create(groupName, root.transform);
            }

            return group;
        }

        /// <summary>
        /// 工具库菜单
        /// </summary>
        public enum EDefaultToolsMenu
        {
            在当前选中游戏对象上添加组件,
            在当前选中游戏对象下创建游戏对象,
            创建游戏对象,
        }

        /// <summary>
        /// 弹出添加游戏对象组件菜单：因为菜单是异步回调，因此需要使用回调函数
        /// </summary>
        /// <param name="click"></param>
        /// <param name="valid"></param>
        /// <param name="extendAction"></param>
        /// <param name="menuName"></param>
        public static void PopupAddComponentMenu(Action<EDefaultToolsMenu> click, Func<EDefaultToolsMenu, bool> valid, Action<MenuInfo> extendAction = null, string menuName = nameof(PopupAddComponentMenu))
        {
            try
            {
                if (click == null) click = e => { };
                if (valid == null) valid = e => true;
                MenuHelper.DrawMenu(menuName, m =>
                {
                    EnumCache<EDefaultToolsMenu>.Array.Foreach(e =>
                    {
                        m.AddMenuItem(e.ToString(), () => click(e), () => valid(e));
                    });

                    extendAction?.Invoke(m);
                });
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// 弹出添加游戏对象组件菜单 ：因为菜单是异步回调，因此需要使用回调函数
        /// </summary>
        /// <param name="toolName"></param>
        /// <param name="addComponent"></param>
        /// <param name="needComponent"></param>
        /// <param name="parent"></param>
        /// <param name="setParent">创建回调函数</param>
        /// <returns>第一个添加的组件</returns>
        private static void PopupAddComponentMenu(string toolName, Type addComponent, List<Type> needComponents, ToolItem toolItem, Action<ToolItem, GameObject> createAndSetParent)
        {
            PopupAddComponentMenu(e =>
            {
                switch (e)
                {
                    case EDefaultToolsMenu.在当前选中游戏对象上添加组件:
                        {
                            foreach (var item in Selection.gameObjects)
                            {
                                if (needComponents.Count == 0 || needComponents.All(t => item.GetComponent(t)))
                                {
                                    if (ValidAddComponent(addComponent, item))
                                    {
                                        AddComponent(item, addComponent);
                                    }
                                }
                            }
                            break;
                        }
                    case EDefaultToolsMenu.在当前选中游戏对象下创建游戏对象:
                        {
                            var go = UnityObjectHelper.CreateGameObject(toolName);
                            go.XSetParent(Selection.activeGameObject);
                            go.XAddComponent(addComponent);
                            break;
                        }
                    case EDefaultToolsMenu.创建游戏对象:
                        {
                            var go = UnityObjectHelper.CreateGameObject(toolName);
                            createAndSetParent(toolItem, go);
                            go.XAddComponent(addComponent);
                            break;
                        }
                }
            }, e =>
            {

                switch (e)
                {
                    case EDefaultToolsMenu.在当前选中游戏对象上添加组件:
                        {
                            if (Selection.gameObjects.Length == 0) return false;

                            //如果要求管理器类型，不做处理
                            var needManager = needComponents.Any(t => typeof(Manager).IsAssignableFrom(t));
                            var hasManager = Selection.activeGameObject.GetComponent<Manager>();
                            return needManager ? hasManager:!hasManager;
                        }
                    case EDefaultToolsMenu.在当前选中游戏对象下创建游戏对象:
                        {
                            //如果要求管理器类型，不做处理
                            var needManager = needComponents.Any(t => typeof(Manager).IsAssignableFrom(t));
                            if (needManager) return false;

                            return Selection.gameObjects.Length > 0;
                        }
                    case EDefaultToolsMenu.创建游戏对象:
                        {
                            //如果要求管理器类型，不做处理
                            var needManager = needComponents.Any(t => typeof(Manager).IsAssignableFrom(t));
                            return !needManager;
                        }
                    default:return false;
                }
            }, m =>
            {
                if (needComponents.Count > 0)
                {
                    //如果要求管理器类型，不做处理
                    if (needComponents.Any(t => typeof(Manager).IsAssignableFrom(t))) return;

                    var components = CommonFun.GetComponentsInChildren(needComponents[0], true).Where(c => needComponents.All(t => c.GetComponent(t)));

                    // 场景中选择对象进行添加
                    HashSet<string> gameObjPathSet = new HashSet<string>();
                    foreach (var c in components)
                    {
                        var goPath = "游戏对象" + CommonFun.GameObjectToStringWithoutAlias(c.gameObject);
                        if (gameObjPathSet.Add(goPath))
                        {
                            m.AddMenuItem(goPath, (obj) =>
                            {
                                AddComponent((obj as Component).gameObject, addComponent);
                            }, c);
                        }
                    }

                    m.AddMenuItem("全部添加", () => components.Foreach(c => AddComponent(c.gameObject, addComponent)),
                        null, int.MaxValue, ESeparatorType.TopUp);

                    m.AddMenuItem("全部添加(仅激活对象)", () =>
                    {
                        foreach (var c in components)
                        {
                            if (c.gameObject.activeInHierarchy)
                            {
                                AddComponent(c.gameObject, addComponent);
                            }
                        }
                    });

                    m.AddMenuItem("全部移除", () => CommonFun.GetComponentsInChildren(addComponent, true).Foreach(c => c.XDestoryObject()));

                    m.AddMenuItem("全部移除(仅激活对象)", () =>
                    {
                        foreach (var c in CommonFun.GetComponentsInChildren(addComponent, false))
                        {
                            if (c.gameObject.activeInHierarchy)
                            {
                                c.XDestoryObject();
                            }
                        }
                    });
                }
            }, toolName);
        }

        private static void AddComponent(GameObject go, Type componentType)
        {
            if (go.XAddComponent(componentType))
            {
                EditorGUIUtility.PingObject(go);
            }
        }

        public static bool ValidAddComponent(Type addComponentType, GameObject go)
        {
            if (Application.isPlaying)
            {
                Log.WarningFormat("为游戏对象[{0}]添加组件[{1}]({2})失败，因运行时禁止添加任何此类型组件!",
                    CommonFun.GameObjectToStringWithoutAlias(Selection.activeGameObject),
                    CommonFun.Name(addComponentType),
                    addComponentType.FullName);
                return false;
            }

            if (go.GetComponent<XDreamer>())
            {
                Log.WarningFormat("为游戏对象[{0}]添加组件[{1}]({2})失败，因该游戏对象为{3}根游戏对象，禁止添加任何此类型组件!",
                    CommonFun.GameObjectToStringWithoutAlias(Selection.activeGameObject),
                    CommonFun.Name(addComponentType),
                    addComponentType.FullName,
                    Product.Name);
                return false;
            }

            var manager = go.GetComponent<Manager>();
            if (manager && !CommonFun.GetTypesOfRequireComponent(addComponentType, false).Any(t => t.IsAssignableFrom(manager.GetType())))
            {
                Log.WarningFormat("为游戏对象[{0}]添加组件[{1}]({2})失败，因该游戏对象包含管理器插件[{3}]({4})，禁止添加任何此类型组件!",
                    CommonFun.GameObjectToStringWithoutAlias(Selection.activeGameObject),
                    CommonFun.Name(addComponentType),
                    addComponentType.FullName,
                    CommonFun.Name(manager.GetType()),
                    manager.GetType().FullName);
                return false;
            }
            return true;
        }

        #endregion

        #region 预置物体处理

        /// <summary>
        /// 预置物体路径
        /// </summary>
        public static string prefabsPath => UICommonFun.Assets + "/XDreamer-Assets/基础/Prefabs/";

        /// <summary>
        /// 预置物体路径
        /// </summary>
        public static string toolPrefabsPath => ToolEditorWindowOption.weakInstance.prefabsRootPath;

        /// <summary>
        /// 加载XDreamer缺省预置物体路径对象
        /// </summary>
        /// <param name="path">路径，基于<see cref="prefabsPath"/>的相对路径</param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static GameObject LoadPrefab_DefaultXDreamerPath(string path) => ClonePrefab(prefabsPath + path);

        /// <summary>
        /// 加载工具库缺省预置物体路径对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static GameObject LoadPrefab_DefaultToolPath(string path) => ClonePrefab(toolPrefabsPath + path);

        /// <summary>
        /// 克隆预制体
        /// </summary>
        /// <param name="path">预制体路径</param>
        /// <returns></returns>
        public static GameObject ClonePrefab(string path)
        {
            GameObject cloneGo = null;
            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    var prefabGO = UICommonFun.LoadFromAssets<GameObject>(path);
                    if (prefabGO)
                    {
                        cloneGo = UnityObjectHelper.Instantiate(prefabGO);
                        cloneGo.XSetUniqueName(prefabGO.name);
                        cloneGo.transform.XSetLocalScale(prefabGO.transform.localScale);

                        // 对具有状态机控制器的游戏对象，进行深度克隆
                        foreach (var item in cloneGo.GetComponentsInChildren<Transform>(true))
                        {
                            if (item.GetComponent<SMController>() is SMController smc && smc && smc.stateMachine)
                            {
                                smc.stateMachine = smc.stateMachine.Clone() as StateMachine;
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarningFormat("路径:[{0}]对应资产不存在!", path);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
            return cloneGo;
        }

        #endregion
    }
}
