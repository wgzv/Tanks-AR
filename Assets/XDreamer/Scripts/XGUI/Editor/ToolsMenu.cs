using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.EditorTools;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginTools.GameObjects;
using XCSJ.PluginTools.Notes;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.EditorXGUI
{
    public static class ToolsMenu
    {
        /// <summary>
        /// 缩放UI类型工具
        /// </summary>
        /// <param name="toolContext"></param>
        /// <param name="createTool"></param>
        public static void CreateUIToolAndStretchHV(ToolContext toolContext, Func<GameObject> createTool)
        {
            if (toolContext==null || createTool==null) return;

            var go = createTool.Invoke();
            if (go)
            {
                EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
                // 纠正坐标
                var rectTransform = go.GetComponent<RectTransform>();
                if (rectTransform)
                {
                    rectTransform.XStretchHV();
                }
            }
        }

        /// <summary>
        /// 创建UI元素：
        /// 在父节点为空的画布下创建组和UI元素，并将UI元素挂接在组下
        /// 当组名为空时，不创建组，直接将UI元素挂在画布下
        /// </summary>
        /// <param name="createUI"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static RectTransform CreateUIInCanvas(Func<GameObject> createUI, string groupName = "")
        {
            if (createUI == null) return null;

            var canvas = EditorXGUIHelper.FindOrCreateRootCanvas();
            if (!canvas) return null;

            var go = createUI.Invoke();
            if (go)
            {
                var parent = canvas;
                if (!string.IsNullOrEmpty(groupName))
                {
                    var group = GameObject.Find(groupName);
                    if (!group)
                    {
                        group = EditorHandler.Create(groupName, canvas.transform);
                    }
                    if (group)
                    {
                        parent = group;
                    }
                }
                // 如果场景中选中的游戏对象在根画布下，则设置为父级
                else if (Selection.activeGameObject && Selection.activeGameObject.transform.IsChildOf(canvas.transform))
                {
                    parent = Selection.activeGameObject;
                }
                go.XSetParent(parent);
                go.XModifyProperty(() => GameObjectUtility.EnsureUniqueNameForSibling(go));
                var rect = go.GetComponent<RectTransform>();
                rect.XCenterHV();

                EditorGUIUtility.PingObject(go);
                return rect;
            }
            return null;
        }

        /// <summary>
        /// 加载XGUI预制体
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static GameObject LoadPrefab_DefaultXGUIPath(string path) => EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath("XGUI/" + path);

        #region 画布

        /// <summary>
        /// 加载场景画布
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("加载场景画布")]
        [XCSJ.Attributes.Icon(EIcon.UI)]
        [Tool(XGUICategory.Window, nameof(XGUIManager), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager))]
        public static void CreateLoadSceneCanvas(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, LoadPrefab_DefaultXGUIPath("加载场景画布.prefab"));
        }

        #endregion

        #region Window

        /// <summary>
        /// 创建全屏窗口
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("全屏窗口")]
        [Tip("在根窗口下，依赖画布的对象")]
        [XCSJ.Attributes.Icon(EIcon.Window)]
        //[Tool(XGUICategory.Window, nameof(XGUIManager), rootType = typeof(XRootWindow), index = 0)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        public static void CreateWindow(ToolContext toolContext)
        {
            var xrw = FindOrCreateXRootWindow();
            if (xrw)
            {
                var go = UnityObjectHelper.CreateGameObject("全屏窗口");
                var rectTransform = go.XAddComponent<RectTransform>();
                go.XAddComponent<Window>();
                go.XAddComponent<Image>();

                EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
                // 纠正坐标
                rectTransform.XStretchHV();
            }
        }

        /// <summary>
        /// 查找和创建根窗口对象
        /// </summary>
        /// <returns></returns>
        public static RootWindow FindOrCreateXRootWindow()
        {
            var xrw = UnityEngine.Object.FindObjectOfType<RootWindow>() as RootWindow;
            if (!xrw)
            {
                var go = EditorXGUIHelper.FindOrCreateRootCanvas();
                Undo.AddComponent<RootWindow>(go);
            }
            return xrw;
        }

        #endregion

        #region SubWindow

        /// <summary>
        /// 垂直收缩窗口
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("窗口模版")]
        [Tip("窗口展开和收缩方向为垂直方向")]
        [XCSJ.Attributes.Icon(EIcon.Window)]
        [Tool(XGUICategory.Window, nameof(XGUIManager), rootType = typeof(Canvas), index = 1, needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Tool("常用", nameof(XGUIManager), rootType = typeof(Canvas), index = 1, needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager))]
        public static void CreateVerticalWindow(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, LoadPrefab_DefaultXGUIPath("窗口模版.prefab"));
        }

        /// <summary>
        /// 游戏对象列表
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("游戏对象列表")]
        [Tip("游戏对象列表")]
        [XCSJ.Attributes.Icon(EIcon.Model)]
        [Tool(XGUICategory.Window, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Tool("游戏对象", nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager))]
        public static GameObject CreateGameObjectListWindow(ToolContext toolContext)
        {
            var gameObjectList = LoadPrefab_DefaultXGUIPath("游戏对象列表.prefab");
            if (gameObjectList)
            {
                var vd = gameObjectList.GetComponentInChildren<GameObjectViewItemDataList>();
                if (vd)
                {
                    vd._renderTextureCamera = XCSJ.EditorTools.ToolsMenu.CreateRenderTextureCamera(toolContext);
                }
            }
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, gameObjectList);
            return gameObjectList;
        }

        /// <summary>
        /// 全息影像UI
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("全息影像UI")]
        [Tip("将视图划分为斜45度的四个区域，并将4个相机图像投影到对应的4个区域中。")]
        [XCSJ.Attributes.Icon(EIcon.AR)]
        [Tool(XGUICategory.Window, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        public static void CreateHolographicImageUGUI(ToolContext toolContext)
        {
            CreateUIToolAndStretchHV(toolContext, () => EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("全息图像/全息影像UI.prefab"));
        }

        /// <summary>
        /// 调色板
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("调色板")]
        [Tip("通过拾取面板颜色改变模型材质颜色")]
        [XCSJ.Attributes.Icon(EIcon.ColorPicker)]
        [Tool(XGUICategory.Window, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Tool("常用", nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        public static void CreateColorPicker(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, LoadPrefab_DefaultXGUIPath("调色板.prefab"));
        }

        /// <summary>
        /// 导航图
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("导航图")]
        [Tip("导航图")]
        [XCSJ.Attributes.Icon(EIcon.MiniMap)]
        [Tool(XGUICategory.Window, nameof(XGUIManager), rootType = typeof(XGUIManager), groupRule = EToolGroupRule.None)]
        [Tool("常用", nameof(XGUIManager), rootType = typeof(XGUIManager), groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        public static void CreateMiniMap(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, LoadPrefab_DefaultXGUIPath("导航图.prefab"));
        }

        /// <summary>
        /// 图片浏览器
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("图片浏览器")]
        [XCSJ.Attributes.Icon(EIcon.Image)]
        [Tip("基于UGUI的用于图片浏览的控件")]
        [Tool(XGUICategory.Window, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Tool("常用", nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        public static void CreatePictureBrowser(ToolContext toolContext)
        {
            ToolsMenu.CreateUIToolAndStretchHV(toolContext, () => LoadPrefab_DefaultXGUIPath("图片浏览器.prefab"));
        }

        /// <summary>
        /// 天气面板
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("天气面板")]
        [Tip("显示指定城市的天气状况")]
        [XCSJ.Attributes.Icon(EIcon.Weather)]
        [Tool(XGUICategory.Window, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Tool("常用", nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        public static void CreateWeatherPanel(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, LoadPrefab_DefaultXGUIPath("天气面板.prefab"));
        }

        #endregion

        #region View

        /// <summary>
        /// 时间文本
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("时间文本")]
        [Tip("显示当前时间的文本")]
        [XCSJ.Attributes.Icon(EIcon.Timer)]
        [Tool(XGUICategory.View, nameof(XGUIManager), rootType = typeof(Canvas), index = 2, needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Tool("常用", nameof(XGUIManager), rootType = typeof(Canvas), index = 2, needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        public static void CreateDataTimeText(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, LoadPrefab_DefaultXGUIPath("时间文本.prefab"));
        }

        /// <summary>
        /// 相机摇杆控制器
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("相机摇杆控制器")]
        [Tip("使用两个摇杆UI控制相机的移动与旋转")]
        [XCSJ.Attributes.Icon(EIcon.Camera)]
        [Tool(XGUICategory.View, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Obsolete("产品功能升级，不推荐用户再使用本功能组件，请使用新的功能组件替代！", false)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        public static void CreateCameraUI(ToolContext toolContext)
        {
            CreateUIToolAndStretchHV(toolContext, () => EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath("常用/相机摇杆控制器.prefab"));
        }

        #endregion

        #region UnityUGUI

        private const string UnityUGUI = "UGUI";

        /// <summary>
        /// 文本
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("文本")]
        [XCSJ.Attributes.Icon(EIcon.Text)]
        [Tool(UnityUGUI, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, index = 1, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        public static void CreateText(ToolContext toolContext)
        {
            CreateUIInCanvas(() => CreateUIWithStyle<Text>().gameObject);
        }

        /// <summary>
        /// 按钮
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("按钮")]
        [XCSJ.Attributes.Icon(EIcon.Button)]
        [Tool(UnityUGUI, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, index = 2, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        public static void CreateButton(ToolContext toolContext)
        {
            CreateUIInCanvas(() => CreateUIWithStyle<Button>().gameObject);
        }

        /// <summary>
        /// 切换
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("切换")]
        [XCSJ.Attributes.Icon(EIcon.Toggle)]
        [Tool(UnityUGUI, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, index = 3, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        public static void CreateToggle(ToolContext toolContext)
        {
            CreateUIInCanvas(() => CreateUIWithStyle<Toggle>().gameObject);
        }

        /// <summary>
        /// 精灵图像
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("精灵图像")]
        [XCSJ.Attributes.Icon(EIcon.RawImage)]
        [Tool(UnityUGUI, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, index = 4, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        public static void CreateImage(ToolContext toolContext)
        {
            CreateUIInCanvas(() => EditorXGUIHelper.CreateUGUI<Image>().gameObject);
        }

        /// <summary>
        /// 贴图图像
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("贴图图像")]
        [XCSJ.Attributes.Icon(EIcon.RawImage)]
        [Tool(UnityUGUI, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, index = 5, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        public static void CreateRawImage(ToolContext toolContext)
        {
            CreateUIInCanvas(() => EditorXGUIHelper.CreateUGUI<RawImage>().gameObject);
        }

        /// <summary>
        /// 下拉列表
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("下拉列表")]
        [XCSJ.Attributes.Icon(EIcon.Dropdown)]
        [Tool(UnityUGUI, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        public static void CreateDropdown(ToolContext toolContext)
        {
            CreateUIInCanvas(() => CreateUIWithStyle<Dropdown>().gameObject);
        }

        /// <summary>
        /// 输入框
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("输入框")]
        [XCSJ.Attributes.Icon(EIcon.InputField)]
        [Tool(UnityUGUI, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        public static void CreateInputField(ToolContext toolContext)
        {
            CreateUIInCanvas(() => CreateUIWithStyle<InputField>().gameObject);
        }

        /// <summary>
        /// 面板
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("面板")]
        [XCSJ.Attributes.Icon(EIcon.RawImage)]
        [Tool(UnityUGUI, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        public static void CreatePanel(ToolContext toolContext)
        {
            var rect = CreateUIInCanvas(() => LoadPrefab_DefaultXGUIPath(GetUnityUGUIPath("Panel.prefab")));
            if (rect)
            {
                rect.XStretchHV();
            }
        }

        /// <summary>
        /// 滚动条
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("滚动条")]
        [XCSJ.Attributes.Icon(EIcon.ScrollBar)]
        [Tool(UnityUGUI, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        public static void CreateScrollbar(ToolContext toolContext)
        {
            CreateUIInCanvas(() => EditorXGUIHelper.CreateUGUI<Scrollbar>().gameObject);
        }

        /// <summary>
        /// 滚动视图
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("滚动视图")]
        [XCSJ.Attributes.Icon(EIcon.ScrollBar)]
        [Tool(UnityUGUI, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        public static void CreateScrollView(ToolContext toolContext)
        {
            CreateUIInCanvas(() => CreateUIWithStyle<ScrollRect>().gameObject);
        }

        /// <summary>
        /// 滑动条
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("滑动条")]
        [XCSJ.Attributes.Icon(EIcon.Slider)]
        [Tool(UnityUGUI, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        public static void CreateSlider(ToolContext toolContext)
        {
            CreateUIInCanvas(() => CreateUIWithStyle<Slider>().gameObject);
        }

        private static string GetUnityUGUIPath(string fileName) => "UnityDefaultUI/" + fileName;

        /// <summary>
        /// 创建带风格的UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateUIWithStyle<T>() where T : UIBehaviour
        {
            GameObject go = null;
            if (typeof(T) == typeof(Button))
            {
                go = LoadPrefab_DefaultXGUIPath(GetUnityUGUIPath("Button.prefab"));
            }
            else if (typeof(T) == typeof(Dropdown))
            {
                go = LoadPrefab_DefaultXGUIPath(GetUnityUGUIPath("Dropdown.prefab"));
            }
            else if (typeof(T) == typeof(InputField))
            {
                go = LoadPrefab_DefaultXGUIPath(GetUnityUGUIPath("InputField.prefab"));
            }
            else if (typeof(T) == typeof(ScrollRect))
            {
                go = LoadPrefab_DefaultXGUIPath(GetUnityUGUIPath("ScrollView.prefab"));
            }
            else if (typeof(T) == typeof(Slider))
            {
                go = LoadPrefab_DefaultXGUIPath(GetUnityUGUIPath("Slider.prefab"));
            }
            else if (typeof(T) == typeof(Text))
            {
                go = LoadPrefab_DefaultXGUIPath(GetUnityUGUIPath("Text.prefab"));
            }
            else if (typeof(T) == typeof(Toggle))
            {
                go = LoadPrefab_DefaultXGUIPath(GetUnityUGUIPath("Toggle.prefab"));
            }

            var ui = default(T);
            if (go)
            {
                UndoHelper.RegisterCreatedObjectUndo(go);
                ui = go.GetComponent<T>();
            }

            return ui;
        }

        #endregion
    }
}
