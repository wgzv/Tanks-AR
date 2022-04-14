using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.CommonUtils.PluginHighlightingSystem;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.ProjectSettings;
using XCSJ.EditorExtension.Tools;
using XCSJ.EditorTools.Inspectors;
using XCSJ.Helper;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginsCameras.UI;
using XCSJ.PluginTools;
using XCSJ.PluginTools.LineNotes;
using XCSJ.PluginTools.Notes;
using XCSJ.PluginTools.Puts;
using XCSJ.PluginTools.SelectionUtils;
using XCSJ.PluginXGUI.Views.Toggles;

namespace XCSJ.EditorTools
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    public static class ToolsMenu
    {
        #region 标注
      
        /// <summary>
        /// 缩放式热点
        /// </summary>
        /// <param name="menuCommand"></param>
        /// <returns></returns>
        [Tool("标注", rootType = typeof(ToolsManager), index = 1, groupRule = EToolGroupRule.Create)]
        [Tool("常用", categoryIndex = 0, rootType = typeof(ToolsManager), groupRule = EToolGroupRule.Create)]
        [XCSJ.Attributes.Icon(EIcon.Hotspot)]
        [Name("热点")]
        [Tip("包含移入热点和点击提示热点")]
        [RequireManager(typeof(ToolsManager))]
        public static void CreateHotspot(ToolContext toolContext)
        {
            MenuHelper.DrawMenu("热点菜单", m =>
            {
                m.AddMenuItem("移入提示热点", () =>
                {
                    var go = EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("标注/移入提示热点.prefab");
                    if (go)
                    {
                        CreateTip(go.GetComponent<Tip>());
                        EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
                    }
                });

                m.AddMenuItem("点击提示热点", () =>
                {
                    var go = EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("标注/点击提示热点.prefab");
                    if (go)
                    {
                        CreateTip(go.GetComponent<Tip>());
                        EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
                    }
                });
            });
        }

        /// <summary>
        /// 创建3D标注
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool("标注", rootType = typeof(ToolsManager), groupRule = EToolGroupRule.Create)]
        [Tool("常用", rootType = typeof(ToolsManager), groupRule = EToolGroupRule.Create)]
        [XCSJ.Attributes.Icon(EIcon.Note)]
        [Name("批注-3D")]
        [RequireManager(typeof(ToolsManager))]
        public static void Create3DNote(ToolContext toolContext)
        {
            MenuHelper.DrawMenu("热点菜单", m =>
            {
                m.AddMenuItem("静态批注", () =>
                {
                    var go = EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("标注/批注-3D.prefab");
                    if (go)
                    {
                        RectTransform button = null;
                        if (go.GetComponent<UGUILineNote3D>() is UGUILineNote3D line)
                        {
                            button = UGUILineHelper.CreateButtonNote();
                            button.name = "批注文字";
                            button.GetComponentInChildren<Text>().text = "批注文字";
                            button.sizeDelta = new Vector2(80, button.sizeDelta.y);
                            line.rectTransform = button;
                        }

                        EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
                        if (button) GameObjectUtility.EnsureUniqueNameForSibling(button.gameObject);
                    }
                });

                m.AddMenuItem("动画批注", () =>
                {
                    var go = EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("标注/动画批注.prefab");
                    if (go)
                    {
                        EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
                    }
                });
            });

            
        }

        /// <summary>
        /// 创建提示
        /// </summary>
        /// <param name="tip"></param>
        public static void CreateTip(Tip tip)
        {
            if (tip)
            {
                tip.ugui = EditorXGUI.ToolsMenu.CreateUIInCanvas(() => EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("标注/提示.prefab"), "提示组");
            }
        }

        [Tool("标注", rootType = typeof(ToolsManager), groupRule = EToolGroupRule.Create)]
        [Tip("点击两点确定一个距离尺寸标注")]
        [Name("距离测量标注")]
        [XCSJ.Attributes.Icon(EIcon.Note)]
        [RequireManager(typeof(ToolsManager), typeof(ToolsExtensionManager))]
        public static void CreateDistanceDimensioningClickPointPicker(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("标注/距离测量.prefab"));
        }

        [Tool("标注", rootType = typeof(ToolsManager), groupRule = EToolGroupRule.Create)]
        [Tip("点击三个点确定一个角度尺寸标注")]
        [Name("角度测量标注")]
        [XCSJ.Attributes.Icon(EIcon.Note)]
        [RequireManager(typeof(ToolsManager), typeof(ToolsExtensionManager))]
        public static void CreateAngleDimensioningClickPointPicker(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("标注/角度测量.prefab"));
        }

        #endregion

        #region 全息影像

        public const string HolographicImageCameraPath = "全息图像/全息影像相机.prefab";

        public const string HolographicImageCameraName = "全息影像相机";

        [Tool("相机", rootType = typeof(CameraManager))]
        [Name(HolographicImageCameraName)]
        [Tip("将视图划分为斜45度的四个区域，并将4个相机图像投影到对应的4个区域中。")]
        [XCSJ.Attributes.Icon(EIcon.AR)]
        [RequireManager(typeof(CameraManager))]
        public static void CreateHolographicImageCamera(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, EditorToolsHelperExtension.LoadPrefab_DefaultToolPath(HolographicImageCameraPath));
        }

        #endregion

        #region 拖拽功能

        /// <summary>
        /// 创建拖拽工具(通过相机视图平面)
        /// </summary>
        public const string CreateDragToolName = "相机视图平面拖拽工具";

        /// <summary>
        /// 创建拖拽工具(通过相机视图平面)
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool("选择集", disallowMultiple = true, rootType = typeof(ToolsManager))]
        [Name(CreateDragToolName)]
        [Tip("一键创建[选择集修改器]、[选择集拖拽工具]和[选择集边界渲染器]三个组件,实现拖拽功能。")]
        [XCSJ.Attributes.Icon(EIcon.Drag)]
        [RequireManager(typeof(ToolsManager))]
        public static void CreateSelectionDragToolByOnce(ToolContext toolContext)
        {
            if (EditorToolsHelperExtension.CanCreateTool(toolContext, typeof(ISelectionRender), typeof(DragByCameraView), typeof(SelectionModify)))
            {
                PopupSelectionRenderMenu(CreateDragToolName, parent =>
                {
                    // 创建选择集修改器
                    FindAndCreateGameObject(typeof(SelectionModify), parent);

                    // 创建选择集拖拽工具通过相机视图
                    CreateDragByCameraViewIcon(FindAndCreateGameObject(typeof(DragByCameraView), parent));
                });
            }
        }

        private static void CreateDragByCameraViewIcon(GameObject go)
        {
            if (go)
            {
                var dragTool = go.GetComponentInChildren<DragByCameraView>(true);
                if (dragTool && !dragTool.moveIcon)
                {
                    dragTool.XModifyProperty(() =>
                    {
                        dragTool.moveIcon = SelectionDragToolInspector.LoadDefaultDragIcon();
                        if (dragTool.moveIcon)
                        {
                            dragTool.moveIcon.gameObject.SetActive(false);
                        }
                    });
                }
            }
        }

        private static GameObject FindAndCreateGameObject(Type type, GameObject parent, Func<GameObject> createFun = null)
        {
            var cs = CommonFun.GetComponentsInChildren(type, true);
            GameObject go = null;
            if (cs != null && cs.Length > 0)
            {
                go = cs[0].gameObject;
            }
            else
            {
                if(createFun!=null)
                {
                    go = createFun.Invoke();
                }
                else
                {
                    go = new GameObject(CommonFun.Name(type), type);
                    UndoHelper.RegisterCreatedObjectUndo(go);
                }
            }

            UndoHelper.RecordSetTransformParent(go.transform, parent.transform);
            return go;
        }

        /// <summary>
        /// 选择集渲染器
        /// </summary>
        public const string SelectionRenderName = "选择集渲染器";

        /// <summary>
        /// 选择集渲染器
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool("选择集", disallowMultiple = true, rootType = typeof(ToolsManager))]
        [Name(SelectionRenderName)]
        [Tip("用于表现选择游戏对象的效果")]
        [XCSJ.Attributes.Icon(EIcon.Select)]
        [RequireManager(typeof(ToolsManager))]
        public static void CreateSelectionRender(ToolContext toolContext)
        {
            if (EditorToolsHelperExtension.CanCreateTool(toolContext, typeof(ISelectionRender)))
            {
                PopupSelectionRenderMenu(SelectionRenderName);
            }
        }

        private static void PopupSelectionRenderMenu(string parentName, Action<GameObject> onParentCreated = null)
        {
            var types = TypeHelper.FindTypeInAppWithInterface(typeof(ISelectionRender));

            MenuHelper.DrawMenu(parentName + "菜单", m =>
            {
                foreach (var t in types)
                {
                    m.AddMenuItem(parentName + "_" + CommonFun.Name(t), (obj) =>
                    {
                        var parent = CreateToolGroup(parentName);
                        onParentCreated?.Invoke(parent);

                        // 设置当前对象活跃，其他同类对象非活跃
                        var activeObj = FindAndCreateGameObject(obj as Type, parent);
                        foreach (var child in parent.GetComponentsInChildren<ISelectionRender>(true))
                        {
                            if (child is MB mb && mb)
                            {
                                mb.gameObject.SetActive(mb.gameObject == activeObj);
                            }
                        }

                        EditorGUIUtility.PingObject(parent);
                    }, t);
                }
            }, types);
        }

        /// <summary>
        /// 创建拖拽工具(平移旋转缩放)
        /// </summary>
        public const string CreateTRSToolName = "平移旋转缩放拖拽工具";

        private static GameObject CreateToolGroup(string name)
        {
            var toolManager = ToolsManager.instance;
            if (!toolManager) return null;

            var transform = toolManager.transform.Find(name);
            var parent = transform ? transform.gameObject : new GameObject(name);
            if (parent.transform.parent != toolManager.transform)
            {
                parent.transform.SetParent(toolManager.transform);
            }

            return parent;
        }

        /// <summary>
        /// 创建拖拽工具(相机看视图平面平移、三轴平移、旋转和缩放)
        /// </summary>
        /// <param name="toolContext"></param>
        [Name(CreateTRSToolName)]
        [Tool("选择集", disallowMultiple = true, rootType = typeof(ToolsManager))]
        [Tool("常用", disallowMultiple = true, rootType = typeof(ToolsManager))]
        [Tip("相机看视图平面平移、三轴平移、旋转和缩放")]
        [XCSJ.Attributes.Icon(EIcon.Drag)]
        [RequireManager(typeof(ToolsManager))]
        public static void CreateDragTool(ToolContext toolContext)
        {
            var go = EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("拖拽工具.prefab");
            if (go)
            {
                EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);

                CreateDragByCameraViewIcon(go);
            }

            // 创建窗口
            XCSJ.EditorXGUI.ToolsMenu.CreateUIInCanvas(() =>
            {
                var wind = XCSJ.EditorXGUI.ToolsMenu.LoadPrefab_DefaultXGUIPath("拖拽工具窗口.prefab");
                if (wind)
                {
                    foreach (var item in wind.transform.GetComponentsInChildren<ToggleSwitchGameObjectActive>())
                    {
                        if (item.name.Contains("视图平移"))
                        {
                            var dragByCamView = go.GetComponentInChildren<DragByCameraView>(true);
                            if (dragByCamView)
                            {
                                item._gameObjects.Add(dragByCamView.gameObject);
                            }
                        }
                        else if (item.name.Contains("射线摆放"))
                        {
                            var dragByRay = go.GetComponentInChildren<DragByRay>(true);
                            if (dragByRay)
                            {
                                item._gameObjects.Add(dragByRay.gameObject);
                            }
                        }
                    }
                }
                return wind;
            });
        }

        #endregion

        [Tool("渲染器", disallowMultiple = true, rootType = typeof(ToolsManager))]
        [Name(RenderTextureCamera.Title)]
        [Tip("用于表现选择游戏对象的效果")]
        [XCSJ.Attributes.Icon(EIcon.Camera)]
        [RequireManager(typeof(ToolsManager))]
        public static RenderTextureCamera CreateRenderTextureCamera(ToolContext toolContext)
        {
            var rt = GameObject.FindObjectOfType<RenderTextureCamera>();
            if (!rt)
            {
                var go = new GameObject(RenderTextureCamera.Title);
                if (go)
                {
                    // 相机专用渲染图层
                    XTagManager.AddLayer(RenderTextureCamera.Layer);
                    rt = go.AddComponent<RenderTextureCamera>();
                    rt.renderCamera.cullingMask = 1 << LayerMask.NameToLayer(RenderTextureCamera.Layer);

                    UndoHelper.RegisterCreatedObjectUndo(go);
                    go.XSetParent(ToolsManager.instance.gameObject);
                }
            }
            return rt;
        }
    }
}
