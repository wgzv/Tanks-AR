using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension;
using XCSJ.EditorExtension.Base.XUnityEditor;
using XCSJ.EditorExtension.Base.XUnityEditorInternal;
using XCSJ.EditorExtension.EditorWindows;
using XCSJ.EditorTools.Windows.Packages;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.EditorTools.Windows
{
    /// <summary>
    /// 包导出工具
    /// </summary>
#if XDREAMER_EDITION_XDREAMERDEVELOPER
    [XDreamerEditorWindow("开发者专用")]
#endif
    [Name(Title)]
    public class PackageExportWindow : XEditorWindowWithScrollView<PackageExportWindow>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = Product.Name + "包导出工具";

        /// <summary>
        /// 初始化
        /// </summary>
#if XDREAMER_EDITION_XDREAMERDEVELOPER
        [MenuItem(XDreamerEditor.EditorWindowMenu + Title)]
#endif
        public static void Init() => OpenAndFocus();

        /// <summary>
        /// 对应Assets文件夹
        /// </summary>
        private AssetItem assetsItem = new AssetItem();

        /// <summary>
        /// 导出的配置文件
        /// </summary>
        private List<ExportFile> exportFiles = new List<ExportFile>();

        private ExportFile exportFile = new ExportFile();

        private XGUIStyle toggleMixed = new XGUIStyle(x => new GUIStyle(EditorStyles_LinkType.toggleMixed ?? EditorStyles.toggle));

        /// <summary>
        /// 导出文件夹路径
        /// </summary>
        public string exportFolderPath = "";

        /// <summary>
        /// 显示资产
        /// </summary>
        public bool displayAssets = false;

        /// <summary>
        /// 前缀
        /// </summary>
        [Name("前缀")]
        public string prefix = "";

        /// <summary>
        /// 后缀
        /// </summary>
        [Name("后缀")]
        public string suffix = "";

        private enum EState
        {
            Init = 0,

            AssetsBeginLoad,

            AssetsLoading,

            AssetsEndLoad,

            AssetsLoaded,
        }

        private EState state = EState.Init;

        /// <summary>
        /// 定时刷星
        /// </summary>
        public override bool timedRepaint => true;

        /// <summary>
        /// 启动
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            prefix = Product.Name + "-";
            suffix = "-V" + Product.Version;
            state = EState.Init;
            ReloadFile();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
        }

        /// <summary>
        /// 绘制GUI
        /// </summary>
        public override void OnGUI()
        {
            switch (state)
            {
                case EState.Init:
                    {
                        if (Event.current.type == EventType.Repaint)
                        {
                            state = EState.AssetsBeginLoad;
                        }
                        break;
                    }
                case EState.AssetsBeginLoad:
                    {
                        UICommonFun.NotificationLayout("资产开始导入.");
                        if(Event.current.type == EventType.Repaint)
                        {
                            state = EState.AssetsLoading;
                            UICommonFun.DelayCall(ReloadAssets, 0.5f);
                        }
                        break;
                    }
                case EState.AssetsLoading:
                    {
                        UICommonFun.NotificationLayout("资产导入中...");
                        break;
                    }
                case EState.AssetsEndLoad:
                    {
                        UICommonFun.NotificationLayout("资产导入完成.");
                        if (Event.current.type == EventType.Repaint)
                        {
                            state = EState.AssetsLoaded;
                        }
                        break;
                    }
                case EState.AssetsLoaded:
                    {
                        EditorGUILayout.Separator();

                        //绘制导出选项
                        DrawExportOption();

                        EditorGUILayout.Separator();

                        //绘制导出文件列表
                        DrawExportFiles();

                        EditorGUILayout.Separator();

                        //绘制资产信息
                        if (!(displayAssets = UICommonFun.Foldout(displayAssets, CommonFun.TempContent("资产"), () =>
                        {
                            if (GUILayout.Button(UICommonOption.Reset, UICommonOption.WH20x16))
                            {
                                state = EState.AssetsBeginLoad;
                            }

                        }))) return;
                        base.OnGUI();
                        break;
                    }
            }

        }

        /// <summary>
        /// 绘制带滚动视图的GUI
        /// </summary>
        public override void OnGUIWithScrollView()
        {
            CommonFun.BeginLayout();
            TreeView.Draw(assetsItem, (node, label) =>
            {
                var item = node as AssetItem;

                EditorGUI.BeginChangeCheck();

                GUIStyle style = EditorStyles.toggle;
                if (item.isFolder && item.enabledState == EEnableState.Mixed)
                {
                    style = toggleMixed;
                }
                var enable = EditorGUILayout.Toggle(GUIContent.none, item.enable, style, GUILayout.Width(18));
                EditorGUILayout.LabelField(label, GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck())
                {
                    item.SetEnable(enable);
                }
            });
            CommonFun.EndLayout();
        }

        /// <summary>
        /// 配置目录
        /// </summary>
        private GUIContent content_ConfigDir = new GUIContent("配置目录");

        /// <summary>
        /// 添加项到菜单：窗口增加点击的菜单项
        /// </summary>
        /// <param name="menu"></param>
        public override void AddItemsToMenu(GenericMenu menu)
        {
            base.AddItemsToMenu(menu);
#if XDREAMER_EDITION_XDREAMERDEVELOPER
            menu.AddItem(content_ConfigDir, false, () =>
            {
                Application.OpenURL(UICommonFun.ToFullPath(ExportFile.Dir));
            });
#endif
        }

        private void DrawExportOption()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("操作");
            if (GUILayout.Button("导出", EditorStyles.miniButtonLeft, UICommonOption.Height16))
            {
                var fs = exportFiles.Where(f => f.export);
                if (fs.Count() > 0)
                {
                    if (string.IsNullOrEmpty(exportFolderPath)) exportFolderPath = Application.dataPath;
                    var dir = Path.GetDirectoryName(exportFolderPath);
                    var name = Path.GetFileName(exportFolderPath);
                    var path = EditorUtility.SaveFolderPanel("导出到...", dir, name);
                    if (!string.IsNullOrEmpty(path))
                    {
                        exportFolderPath = path;
                        if (path.StartsWith(Application.dataPath))
                        {
                            Log.Error("不允许将包导出到当前工程的数据路径（或子级路径）:" + Application.dataPath);
                        }
                        else
                        {
                            Export(fs, exportFolderPath);
                            GUIUtility.ExitGUI();
                        }
                    }
                }
            }

            if (GUILayout.Button(UICommonOption.Reset, EditorStyles.miniButtonRight, UICommonOption.WH20x16))
            {
                Refresh();
            }

            EditorGUILayout.EndHorizontal();

            prefix = EditorGUILayout.TextField(CommonFun.NameTip(GetType(), nameof(prefix)), prefix);
            suffix = EditorGUILayout.TextField(CommonFun.NameTip(GetType(), nameof(suffix)), suffix);
        }

        private const int toggleWidth = 18;
        private const int indexWidth = 60 - toggleWidth;
        private const int operationWidth = 60;
        private const int nameWidth = 320;

        private void DrawExportFiles()
        {
            #region 文件列表

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            EditorGUI.BeginChangeCheck();

            var export = EditorGUILayout.Toggle(GUIContent.none, exportFiles.All(f => f.export), GUILayout.Width(toggleWidth));
            if (EditorGUI.EndChangeCheck())
            {
                exportFiles.ForEach(f => f.export = export);
            }

            GUILayout.Label("NO.", GUILayout.Width(indexWidth));
            GUILayout.Label("名称", GUILayout.Width(nameWidth));
            GUILayout.Label("导出后文件名");
            GUILayout.Label("操作", GUILayout.Width(operationWidth - 16));

            if (GUILayout.Button(UICommonOption.Reset, UICommonOption.WH20x16))
            {
                ReloadFile();
            }

            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < exportFiles.Count; i++)
            {
                var file = exportFiles[i];
                UICommonFun.BeginHorizontal(i);

                file.export = EditorGUILayout.Toggle(GUIContent.none, file.export, GUILayout.Width(toggleWidth));

                GUILayout.Label((i + 1).ToString(), GUILayout.Width(indexWidth));

                if (GUILayout.Button(file.name, GUI.skin.label, GUILayout.Width(nameWidth)))
                {
                    PackageHelper.SyncFileToAssetItem(file, assetsItem);
                }

                if (GUILayout.Button(PackageHelper.GetFileName(file, prefix, suffix), GUI.skin.label))
                {
                    PackageHelper.SyncFileToAssetItem(file, assetsItem);
                }

                if (GUILayout.Button("编辑", GUILayout.Width(operationWidth)))
                {
                    exportFile.name = file.name;
                }

                UICommonFun.EndHorizontal();
            }

            #endregion

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("新建", GUILayout.Width(indexWidth));
            exportFile.name = EditorGUILayout.TextField(exportFile.name, GUILayout.Width(nameWidth));
            GUILayout.Label(PackageHelper.GetFileName(exportFile, prefix, suffix));

            if (GUILayout.Button("保存", GUILayout.Width(operationWidth)) && !string.IsNullOrEmpty(exportFile.name))
            {
                //根据当前资产生成导出文件
                var newFile = PackageHelper.Create(assetsItem);
                newFile.name = exportFile.name;

                bool save = true;
                if (FileHelper.Exists(newFile.path))
                {
                    if (!EditorUtility.DisplayDialog("冲突", string.Format("文件{0}已经存在，是否覆盖?", newFile.name), "覆盖", "取消"))
                    {
                        save = false;
                    }
                }

                if (save)
                {
                    //保存
                    newFile.Save();

                    //重新加载文件
                    UICommonFun.DelayCall(ReloadFile);

                    //刷新
                    AssetDatabase.Refresh();
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void Refresh()
        {
            ReloadFile();
            state = EState.AssetsBeginLoad; 
        }

        private void ReloadFile()
        {
            exportFiles.Clear();
            exportFiles.AddRange(ExportFile.LoadAll());
            exportFiles.Sort((x, y) => string.Compare(x.name, y.name));
        }

        private void ReloadAssets()
        {
            state = EState.AssetsLoading;

            var begin = DateTime.Now;
            assetsItem = PackageHelper.GetAssets();
            var span = DateTime.Now - begin;
            Debug.LogFormat("资产导入耗时: {0}秒(即{1}分{2}秒)", span.TotalSeconds, span.Minutes, span.Seconds);

            state = EState.AssetsEndLoad;
        }

        private void Export(IEnumerable<ExportFile> files, string dir)
        {
            var begin = DateTime.Now;
            foreach (var file in files)
            {
                PackageHelper.Export(file, assetsItem, dir, prefix, suffix);
            }
            var span = DateTime.Now - begin;
            Debug.LogFormat("包导出耗时: {0}秒(即{1}分{2}秒),目录:{3}", span.TotalSeconds, span.Minutes, span.Seconds, dir);
        }
    }
}
