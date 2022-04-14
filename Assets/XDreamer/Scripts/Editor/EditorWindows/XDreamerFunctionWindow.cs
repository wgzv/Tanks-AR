using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension;
using XCSJ.EditorExtension.Base;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.EditorExtension.EditorWindows
{
    /// <summary>
    /// XDreamer功能串口类
    /// </summary>
    [Name(Title)]
    [Tip("场景相关操作编辑工具")]
    [XCSJ.Attributes.Icon(EIcon.Tool)]
    [XDreamerEditorWindow("开发者专用")]
    public class XDreamerFunctionWindow : XEditorWindowWithScrollView<XDreamerFunctionWindow>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = Product.Name + "功能窗口";

        /// <summary>
        /// 初始化
        /// </summary>
        [MenuItem(XDreamerEditor.EditorWindowMenu + Title)]
        public static void Init() => OpenAndFocus();    

        /// <summary>
        /// 启用时
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            PluginCommonUtilsRootInspector.FindAndDeleteRedundantManagerComponent();
        }

        /// <summary>
        /// 绘制GUI时
        /// </summary>
        public override void OnGUI()
        {
            selectToolbar = UICommonFun.Toolbar(selectToolbar, ENameTip.Image, UICommonOption.Height24);
            base.OnGUI();
        }

        /// <summary>
        /// 绘制带滚动视图的GUI时
        /// </summary>
        public override void OnGUIWithScrollView()
        {
            switch (selectToolbar)
            {
                case EToolbar.CommonFunction:
                    {
                        DrawCommonFunction();
                        break;
                    }
                case EToolbar.XDreamerInfo:
                    {
                        DrawSceneXDreamerInfo();
                        break;
                    }
                case EToolbar.NameCheck:
                    {
                        DrawNameCheck();
                        break;
                    }
            }
        }

        /// <summary>
        /// 被选择的工具条
        /// </summary>
        public EToolbar selectToolbar = EToolbar.CommonFunction;

        /// <summary>
        /// 工具条枚举
        /// </summary>
        public enum EToolbar
        {
            /// <summary>
            /// 常用功能
            /// </summary>
            [Name("常用功能")]
            CommonFunction,

            /// <summary>
            /// 名称检查
            /// </summary>
            [Name("名称检查")]
            NameCheck,

            /// <summary>
            /// XDreamer信息
            /// </summary>
            [Name("XDreamer信息")]
            XDreamerInfo,
        }

        #region 常用功能

        private void DrawCommonFunction()
        {
            if (GUILayout.Button("检测编译宏")) MacroAttribute.InvokeMacroMethod();

            if (GUILayout.Button("移除所有XDreamer前缀编译宏")) XDreamerEditor.RemoveAllCompileMacroOfXDreamerPrefix();

            if (GUILayout.Button("清除当前场景游戏对象的无效脚本组件")) XDreamerEditor.ClearMissingScriptsInCurrentScene();

#if XDREAMER_EDITION_DEVELOPER //开发者模式

            if (GUILayout.Button("Unity开发者模式-开启")) EditorPrefs.SetBool("DeveloperMode", true);
            if (GUILayout.Button("Unity开发者模式-关闭")) EditorPrefs.SetBool("DeveloperMode", false);

#endif
        }

        #endregion

        #region XDreamer信息

        private const string DefauleFileName = "XDreamer说明";

        private const string DefaultFileExt = ".txt";

        private static string defalutSavePath => Application.dataPath + "/" + DefauleFileName + DefaultFileExt;

        /// <summary>
        /// 保存信息的路径
        /// </summary>
        public string saveInfoPath = "";

        /// <summary>
        /// 是否输出当前场景的组件使用信息
        /// </summary>
        [Name("场景组件信息")]
        [Tip("勾选,输出当前场景的组件使用信息")]
        public bool saveSceneComponentInfo = true;

        /// <summary>
        /// 是否输出当前目标平台下定义的XDreamer相关的编译宏
        /// </summary>
        [Name("编译宏")]
        [Tip("勾选,输出当前目标平台下定义的XDreamer相关的编译宏;")]
        public bool saveCompileMacro = true;

        private void DrawSceneXDreamerInfo()
        {
            try
            {
                CommonFun.BeginLayout(false);

                saveSceneComponentInfo = EditorGUILayout.Toggle(CommonFun.NameTooltip(this, nameof(saveSceneComponentInfo)), saveSceneComponentInfo);

                saveCompileMacro = EditorGUILayout.Toggle(CommonFun.NameTooltip(this, nameof(saveCompileMacro)), saveCompileMacro);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("操作");
                if (GUILayout.Button(CommonFun.TempContent("导出", "导出当前场景使用XDreamer的相关信息")))
                {
                    if (string.IsNullOrEmpty(saveInfoPath)) saveInfoPath = defalutSavePath;
                    saveInfoPath = EditorUtility.SaveFilePanel("保存文件", Path.GetDirectoryName(saveInfoPath), DefauleFileName, DefaultFileExt);

                    SaveXDreamerInfo(saveInfoPath);
                }

                EditorGUILayout.EndHorizontal();
            }
            finally
            {
                CommonFun.EndLayout();
            }

            CommonFun.BeginLayout(false);
            EditorGUILayout.TextArea(GetXDreamerInfo(), GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            CommonFun.EndLayout();
        }

        private string GetXDreamerInfo()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("{0}版本:\t{1}", Product.Name, Product.Version);
            sb.AppendFormat("\r\n核心版本:\t\t{0}", Product.coreVersion);
            sb.AppendFormat("\r\nUnity版本:\t{0}", Product.UnityVersion);
            sb.AppendFormat("\r\nUnity当前版本:\t{0}", Application.unityVersion);
            sb.AppendFormat("\r\nUnity编译目标:\t{0}", EditorUserBuildSettings.activeBuildTarget);
            sb.AppendFormat("\r\nUnity脱机目标:\t{0}", EditorUserBuildSettings.selectedStandaloneTarget);

            if (saveSceneComponentInfo && PluginCommonUtilsRoot.Root)
            {
                var root = PluginCommonUtilsRoot.Root;

                sb.Append("\r\n组件信息:");
                for (int i = 0; i < root.managerTypeInfoList.Count; ++i)
                {
                    var info = root.managerTypeInfoList[i];
                    var name = CommonFun.Name(info.type);
                    if (name.Length < 8) name += new string(' ', 8 - name.Length);
                    sb.AppendFormat("\r\n\t{0}\t{1}\t{2}\t{3}", i + 1, name, VersionAttribute.GetVersion(info.type), info.mono ? "启用" : "");
                }
            }

            if (saveCompileMacro)
            {
                sb.Append("\r\n编译宏:");
                var macros = Macro.GetScriptingDefineSymbols(EditorUserBuildSettings.selectedBuildTargetGroup);
                macros.Sort();
                foreach (var df in macros)
                {
                    if (df.StartsWith(XDreamerEditor.CompileMacroPrefix))
                    {
                        sb.AppendFormat("\r\n\t{0}", df);
                    }
                }
            }

            return sb.ToString();
        }

        private void SaveXDreamerInfo(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            FileHelper.OutputFile(path, GetXDreamerInfo());
        }

        #endregion

        #region 名称检查

        /// <summary>
        /// 刷新插件GUI内容
        /// </summary>
        [Name("刷新插件")]
        [Tip("重新获取当前场景的插件授权信息")]
        [XCSJ.Attributes.Icon(EIcon.Reset)]
        private static XGUIContent refreshGUIContent { get; } = new XGUIContent(typeof(PluginCommonUtilsRootInspector), nameof(refreshGUIContent), true);

        private int noWidth = 25;
        private int resetButtonWidth = 80;

        private void DrawNameCheck()
        {
            var root = PluginCommonUtilsRoot.Root;
            if (!root)
            {
                UICommonFun.NotificationLayout(Product.Name + "未启用");
                return;
            }

            try
            {
                CommonFun.BeginLayout(false);

                if (GUILayout.Button("全部重置"))
                {
                    CommonFun.GetOrAddComponent<XDreamer>(root.gameObject);
                    root.name = XDreamer.Name;
                    foreach (var info in root.managerTypeInfoList)
                    {
                        if (info.mono) info.mono.name = PluginCommonUtilsRoot.DefaultName(info.mono.GetType());
                    }
                    UICommonFun.MarkSceneDirty();
                }

                #region 标题

                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                //GUILayout.Label("NO.", GUILayout.Width(noWidth));
                if (GUILayout.Button(refreshGUIContent, GUIStyle.none, GUILayout.Width(noWidth), UICommonOption.Height16))
                {
                    PluginCommonUtilsRootInspector.UpdateAllManagerComponent();
                }

                GUILayout.Label("游戏对象");
                GUILayout.Label("标准名称");
                GUILayout.Label("功能", GUILayout.Width(resetButtonWidth));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();

                #endregion

                UICommonFun.BeginHorizontal(true);
                {
                    GUILayout.Label("0", GUILayout.Width(noWidth));
                    EditorGUILayout.ObjectField(root, typeof(PluginCommonUtilsRoot), true);
                    EditorGUILayout.TextField(XDreamer.Name);

                    EditorGUI.BeginDisabledGroup(root.name == XDreamer.Name || !root.GetComponent<XDreamer>());
                    if (GUILayout.Button("重置", GUILayout.Width(resetButtonWidth)))
                    {
                        root.name = XDreamer.Name;
                        CommonFun.GetOrAddComponent<XDreamer>(root.gameObject);
                    }
                    EditorGUI.EndDisabledGroup();
                }
                UICommonFun.EndHorizontal();

                for (int i = 0; i < root.managerTypeInfoList.Count; ++i)
                {
                    var info = root.managerTypeInfoList[i];

                    UICommonFun.BeginHorizontal(i % 2 == 1);

                    //"NO."
                    GUILayout.Label((i + 1).ToString(), GUILayout.Width(noWidth));

                    EditorGUILayout.ObjectField(info.mono, typeof(MonoBehaviour), true);

                    var defaultName = PluginCommonUtilsRoot.DefaultName(info.mono ? info.mono.GetType() : info.type);

                    EditorGUILayout.TextField(defaultName);

                    EditorGUI.BeginDisabledGroup(!info.mono || info.mono.name == defaultName);
                    if (GUILayout.Button("重置", GUILayout.Width(resetButtonWidth)) && info.mono && defaultName != info.mono.name)
                    {
                        info.mono.name = defaultName;
                        UICommonFun.MarkSceneDirty();
                    }
                    EditorGUI.EndDisabledGroup();

                    UICommonFun.EndHorizontal();
                }

            }
            finally
            {
                CommonFun.EndLayout();
            }
        }

        #endregion
    }
}
