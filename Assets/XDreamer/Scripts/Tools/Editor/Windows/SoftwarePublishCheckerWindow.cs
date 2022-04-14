using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorExtension.Base.XUnityEditor;
using XCSJ.EditorExtension.Base.XUnityEditorInternal;
using XCSJ.EditorExtension.EditorWindows;
using XCSJ.EditorTools.Windows.CodeCreaters;
using XCSJ.EditorTools.Windows.Packages;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Safety;
using XCSJ.PluginSMS.Kernel;
using XCSJ.Tools;

namespace XCSJ.EditorTools.Windows
{
#if XDREAMER_EDITION_XDREAMERDEVELOPER
    [XDreamerEditorWindow("开发者专用")]
#endif
    [Name(Title)]
    public class SoftwarePublishCheckerWindow : XEditorWindowWithScrollView<SoftwarePublishCheckerWindow>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = Product.Name + "软件发布检查器";

        /// <summary>
        /// 初始化
        /// </summary>
#if XDREAMER_EDITION_XDREAMERDEVELOPER
        [MenuItem(XDreamerMenu.NamePath + Title)]
        [MenuItem(XDreamerEditor.EditorWindowMenu + Title)]
#endif
        public static void Init() => OpenAndFocus();

        private List<Type> noMatchVersionTypes = new List<Type>();
        private string expectedVersion = "";

        enum ECheckState
        {
            None,

            Fail,

            Success,
        }

        private ECheckState checkState_TypeVersion = ECheckState.None;
        private ECheckState checkState_AccessData = ECheckState.None;

        static XGUIStyle none { get; } = new XGUIStyle("sv_label_4");
        static XGUIStyle fail { get; } = new XGUIStyle("sv_label_6");
        static XGUIStyle success { get; } = new XGUIStyle("sv_label_3");

        GUIStyle GetGUIStyle(params ECheckState[] checkStates)
        {
            if (checkStates.All(s => s == ECheckState.Success)) return success;
            if (checkStates.Any(s => s == ECheckState.Fail)) return fail;
            return none;
        }

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            checkState_TypeVersion = ECheckState.None;
            checkState_AccessData = ECheckState.None;
        }

        /// <summary>
        /// 绘制带滚动视图的GUI
        /// </summary>
        public override void OnGUIWithScrollView()
        {
            DrawCheckVersions();

            DrawCreateAccessData();

            DrawOpenExportWindow();


            if (GUILayout.Button("一键检查", GetGUIStyle(checkState_AccessData, checkState_TypeVersion), UICommonOption.Height24))
            {
                CheckVersions();
                CreateAccessData();
                OpenExportWindow();
            }
            if (GUILayout.Button("输出 权限数据文件-旧版（DLL中）"))
            {
                AccessData.TryGetAccessDataJson(out var json, out _);                
                CodePreviewWindow.Open(JsonHelper.ToJson(JsonHelper.ToObject<AccessData>(Base64.FromBase64(json)), true));
            }
            if (GUILayout.Button("输出 权限数据文件-新版"))
            {
                CodePreviewWindow.Open(JsonHelper.ToJson(AccessData.Create(), true));
            }
        }

        private void DrawCheckVersions()
        {
            if (GUILayout.Button("1、点击检查所有插件版本号及核心版本号", GetGUIStyle(checkState_TypeVersion), UICommonOption.Height24))
            {
                CheckVersions();
            }

            //绘制不匹配版本号的项
            var count = noMatchVersionTypes.Count;
            if (count > 0)
            {
                CommonFun.BeginLayout();
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                EditorGUILayout.LabelField("NO.", UICommonOption.Width32);
                EditorGUILayout.LabelField("插件名称");
                EditorGUILayout.LabelField("当前版本号", UICommonOption.Width120);
                EditorGUILayout.LabelField("期望版本号", UICommonOption.Width120);
                if (GUILayout.Button(CommonFun.TempContent("打开", "打开所有版本号不一致的插件文件"), UICommonOption.buttonToLableStyle, UICommonOption.Width32))
                {
                    noMatchVersionTypes.ForEach(t => EditorHelper.OpenMonoScript(t));
                }
                EditorGUILayout.EndHorizontal();
                for (int i = 0; i < count; i++)
                {
                    UICommonFun.BeginHorizontal(i);

                    var type = noMatchVersionTypes[i];
                    EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);
                    EditorGUILayout.LabelField(CommonFun.Name(type));
                    EditorGUILayout.LabelField(VersionAttribute.GetVersion(type), UICommonOption.Width120);
                    EditorGUILayout.LabelField(expectedVersion, UICommonOption.Width120);
                    if (GUILayout.Button(CommonFun.TempContent("打开", "打开对应版本号不一致的插件文件"), UICommonOption.Width32))
                    {
                        EditorHelper.OpenMonoScript(type);
                    }
                    UICommonFun.EndHorizontal();
                }
                CommonFun.EndLayout();
            }
        }
        
        /// <summary>
        /// 点击检查所有插件版本号及核心版本号
        /// </summary>
        private void CheckVersions()
        {
            checkState_TypeVersion = ECheckState.Success;
            checkState_AccessData = ECheckState.None;

            noMatchVersionTypes.Clear();
            var today = DateTime.Today;
            expectedVersion = today.ToVersion().ToString();
            foreach (var m in PluginCommonUtilsRoot.GetManagerTypeInfoInAppWithSort())
            {
                var mVersion = VersionAttribute.GetVersion(m.type);
                if (mVersion != expectedVersion)
                {
                    noMatchVersionTypes.Add(m.type);
                    Debug.LogErrorFormat("管理类[{0}]({1})版本号[{2}]与期望版本号[{3}]不一致！请修复...", CommonFun.Name(m.type), m.typeFullName, mVersion, expectedVersion);
                    checkState_TypeVersion = ECheckState.Fail;
                }
            }
            var coreVersion = today.ToCoreVersion().ToString();
            var coreVersion3 = Product.coreVersion.ToString(3);
            if (!coreVersion3.StartsWith(coreVersion))
            {
                Debug.LogErrorFormat("核心版本号[{0}]与期望版本号[{1}]不一致！请修复...", coreVersion3, coreVersion);
                checkState_TypeVersion = ECheckState.Fail;
            }
            if (checkState_TypeVersion == ECheckState.Success)
            {
                Debug.Log("所有版本号检查有效!");
            }
        }
        private void DrawCreateAccessData()
        {
            if (GUILayout.Button("2、点击生成权限数据文件", GetGUIStyle(checkState_AccessData), UICommonOption.Height24))
            {
                CreateAccessData();
            }
        }

        /// <summary>
        /// 点击生成权限数据文件
        /// </summary>
        private void CreateAccessData()
        {
            AccessData.CreateToFile(out bool isNew);
            if (isNew)
            {
                checkState_AccessData = ECheckState.Fail;
                Debug.LogError("权限数据文件变更，需要重新编译主工程代码！");
            }
            else
            {
                checkState_AccessData = ECheckState.Success;
            }
        }

        private void DrawOpenExportWindow()
        {
            if (GUILayout.Button("3、点击打开导出软件包窗口", GetGUIStyle(checkState_AccessData, checkState_TypeVersion), UICommonOption.Height24))
            {
                OpenExportWindow();
            }
        }

        /// <summary>
        /// 点击打开导出软件包窗口
        /// </summary>
        private void OpenExportWindow()
        {
            if (checkState_TypeVersion != ECheckState.Success)
            {
                Debug.LogWarning("版本检查未通过！");
            }
            else if (checkState_AccessData != ECheckState.Success)
            {
                Debug.LogWarning("权限数据文件检查未通过！");
            }
            else
            {
                PackageExportWindow.Init();
            }
        }

        /// <summary>
        /// 权限数据文件目录
        /// </summary>
        private GUIContent content_AccessDataFileDir = new GUIContent("权限数据文件目录");

        /// <summary>
        /// 添加项到菜单：窗口增加点击的菜单项
        /// </summary>
        /// <param name="menu"></param>
        public override void AddItemsToMenu(GenericMenu menu)
        {
            base.AddItemsToMenu(menu);
#if XDREAMER_EDITION_XDREAMERDEVELOPER
            menu.AddItem(content_AccessDataFileDir, false, () =>
            {
                AccessData.RevealInFinder();
            });
#endif
        }

        #region 组件与管理器关系

        /// <summary>
        /// 权限数据：组件与管理器关系
        /// </summary>
        public class AccessData
        {
            /// <summary>
            /// 获取权限数据文件路径
            /// </summary>
            /// <returns></returns>
            internal static string GetFilePath() => Application.dataPath + "/../../commonUtils/XCSJ.PluginCommonUtils/XDreamer.cs";

            internal  const string BeginFlags = "private static string accessDataJson_Base64 { get; } = \"";
            internal  const string EndFlags = "\";//End Access Data JSON";

            internal static bool TryGetAccessDataJson(out string json,out string fullText)
            {
                fullText = FileHelper.InputFile(GetFilePath(), false, false);
                var bi = fullText.IndexOf(BeginFlags);
                var ei = fullText.IndexOf(EndFlags);
                json = fullText.Substring(bi + BeginFlags.Length, ei - bi - BeginFlags.Length);
                return true;
            }

            /// <summary>
            /// 在查找器中显示
            /// </summary>
            internal static void RevealInFinder()=> EditorUtility.RevealInFinder(GetFilePath());

            public static AccessData Create()
            {
                var accessData = new AccessData();

                //项列表:组件与管理器类关系
                {
                    foreach (var t in TypeHelper.FindTypeInAppWithClass(typeof(MB)))
                    {
                        accessData.Add(t);
                    }
                    foreach (var t in TypeHelper.FindTypeInAppWithClass(typeof(StateComponent)))
                    {
                        accessData.Add(t);
                    }
                    foreach (var t in TypeHelper.FindTypeInAppWithClass(typeof(TransitionComponent)))
                    {
                        accessData.Add(t);
                    }
                    foreach (var t in TypeHelper.FindTypeInAppWithClass(typeof(StateGroupComponent)))
                    {
                        accessData.Add(t);
                    }
                }

                //XR类型列表
                {
                    foreach (var t in TypeHelper.FindTypeInApp(typeof(Manager), typeof(IXRAccess)))
                    {
                        accessData.xrTypes.Add(t.FullName);
                    }
                }

                return accessData;
            }

            /// <summary>
            /// 生成关系图并输出到文件
            /// </summary>
            public static AccessData CreateToFile(out bool isNew)
            {
                var accessData = Create();

                Debug.Log("权限数据项共计:" + accessData.items.Count);
                Debug.Log("XR权限数据项共计:" + accessData.xrTypes.Count);
                if(TryGetAccessDataJson(out string oldData, out string fullText))
                {
                    var newData = Base64.ToBase64(JsonHelper.ToJson(accessData));
                    if (oldData != newData)
                    {
                        var path = GetFilePath();
                        Debug.Log("新旧权限数据内容不同！输出文件到：" + path);
                        FileHelper.OutputFile(path, fullText.Replace(oldData, newData), false, false);
                        isNew = true;
                    }
                    else
                    {
                        Debug.Log("新旧权限数据内容相同！跳过文件输出阶段！");
                        isNew = false;
                    }
                }
                else
                {
                    isNew = true;
                    Debug.LogErrorFormat("获取旧权限数据内容失败！");
                }
                
                AccessData.RevealInFinder();
                return accessData;
            }

            /// <summary>
            /// 添加类型
            /// </summary>
            /// <param name="componentType"></param>
            public void Add(Type componentType)
            {
                if (componentType == null) return;
                if (typeof(Manager).IsAssignableFrom(componentType)) return;
                if (typeof(XDreamer).IsAssignableFrom(componentType)) return;
                if (typeof(PluginCommonUtilsRoot).IsAssignableFrom(componentType)) return;
                if (typeof(GlobalMB).IsAssignableFrom(componentType)) return;
                if (!componentType.FullName.StartsWith("XCSJ.")) return;
                var item = new Item();
                items.Add(item);
                item.componentTypeFullName = componentType.FullName;
                item.managerTypeFullNames.AddRangeWithDistinct(ManagerTypeLimitAttribute.GetManagerTypes(componentType)?.Cast(t => t.FullName));
                if (item.managerTypeFullNames.Count == 0)
                {
                    Debug.LogWarningFormat("[{0}]类没有被任何管理器管理器类型限定特性修饰！", item.componentTypeFullName);
                }
            }

            /// <summary>
            /// XR类型列表
            /// </summary>
            public List<string> xrTypes = new List<string>();

            /// <summary>
            /// 项列表:组件与管理器类关系
            /// </summary>
            public List<Item> items = new List<Item>();

            /// <summary>
            /// 项
            /// </summary>
            public class Item
            {
                /// <summary>
                /// 组件类型全名称
                /// </summary>
                [Json("c")]
                public string componentTypeFullName { get; set; } = "";

                /// <summary>
                /// 管理器类型全名称列表
                /// </summary>
                [Json("m")]
                public List<string> managerTypeFullNames { get; set; } = new List<string>();
            }
        }

        #endregion
    }
}
