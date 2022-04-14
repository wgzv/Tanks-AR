using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorExtension.EditorWindows;
using XCSJ.EditorTools.Windows.CodeCreaters.Base;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorTools.Windows.CodeCreaters
{
    /// <summary>
    /// 代码生成器
    /// </summary>
#if XDREAMER_EDITION_XDREAMERDEVELOPER
    [XDreamerEditorWindow("开发者专用")]
#endif
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.Script)]
    public class CodeCreaterWindow : XEditorWindowWithScrollView<CodeCreaterWindow>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = Product.Name + "代码生成器";

        /// <summary>
        /// 初始化
        /// </summary>
#if XDREAMER_EDITION_XDREAMERDEVELOPER
        [MenuItem(XDreamerEditor.EditorWindowMenu + Title)]
#endif
        public static void Init() => OpenAndFocus();

        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="type"></param>
        public static void Open(Type type = null)
        {
            OpenAndFocus();
            if (type != null) UICommonFun.DelayCall(() => instance.typeFullName = type.FullNameToHierarchyString());
        }

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            UpdateCodeCreater();
        }

        #region 类型

        private string[] typeFullNames => cacheValue?.typeFullNames ?? Empty<string>.Array;

        /// <summary>
        /// 类型全名称
        /// </summary>
        public string typeFullName
        {
            get => cacheValue?.typeFullName ?? "";
            set
            {
                if (typeFullName != value)
                {
                    UpdateCodeCreater(cacheValue?.GetType(value));
                }
            }
        }

        /// <summary>
        /// 类型
        /// </summary>
        public Type type => cacheValue?.type;

        #endregion

        #region 输出路径

        /// <summary>
        /// 输出路径
        /// </summary>
        [Name("输出路径")]
        public string outputPath
        {
            get => cacheValue?.outputPath ?? "";
            set
            {
                if (cacheValue != null && !string.IsNullOrEmpty(value)) cacheValue.outputPath = value;
            }
        }

        private void RestOutputPath() => cacheValue?.RestOutputPath();

        #endregion

        #region 代码生成器

        /// <summary>
        /// 代码生成器
        /// </summary>
        public BaseCodeCreater codeCreater => cacheValue?.codeCreater;

        private CodeCreaterCacheValue cacheValue;

        private Dictionary<ECodeCreaterType, CodeCreaterCacheValue> cacheValues = new Dictionary<ECodeCreaterType, CodeCreaterCacheValue>();

        private ECodeCreaterType _codeCreaterType = ECodeCreaterType.PropertySet;

        /// <summary>
        /// 代码生成器类型
        /// </summary>
        [Name("代码生成器类型")]
        public ECodeCreaterType codeCreaterType
        {
            get => _codeCreaterType;
            set
            {
                if (value == _codeCreaterType) return;
                _codeCreaterType = value;
                UpdateCodeCreater(syncType);
            }
        }

        /// <summary>
        /// 同步类型：在切换代码生成器类型时是否尝试同步当前正在处理的目标类型
        /// </summary>
        [Name("同步类型")]
        [Tip("在切换代码生成器类型时是否尝试同步当前正在处理的目标类型")]
        public bool syncType = false;

        private void UpdateCodeCreater(bool syncType = true) => UpdateCodeCreater(type, syncType);

        private void UpdateCodeCreater(Type type, bool syncType = true)
        {
            if (!syncType && cacheValues.TryGetValue(codeCreaterType, out CodeCreaterCacheValue cacheValue) && cacheValue != null)
            {
                this.cacheValue = cacheValue;
            }
            else
            {
                cacheValues[codeCreaterType] = this.cacheValue = CodeCreaterCache.Get(codeCreaterType, DefaultCodeCreaterCache.ValidType(codeCreaterType, type) ? type : default);
            }    
        }

        private class CodeCreaterCache : TIVCache<CodeCreaterCache, ECodeCreaterType, Type, CodeCreaterCacheValue>
        {
            public static CodeCreaterCacheValue Get(ECodeCreaterType codeCreaterType, Type type) => type != null ? GetCacheValue(codeCreaterType, type) : DefaultCodeCreaterCache.GetCacheValue(codeCreaterType);
        }

        private class DefaultCodeCreaterCache : TICache<DefaultCodeCreaterCache, ECodeCreaterType, CodeCreaterCacheValue>
        {
            protected override KeyValuePair<bool, CodeCreaterCacheValue> CreateValue(ECodeCreaterType key1)
            {
                var value = new CodeCreaterCacheValue();
                value.key1 = key1;
                value.Init();
                return new KeyValuePair<bool, CodeCreaterCacheValue>(true, value);
            }

            public static bool ValidType(ECodeCreaterType codeCreaterType, string typeFullName) => GetCacheValue(codeCreaterType) is CodeCreaterCacheValue cacheValue && cacheValue.ValidType(typeFullName);

            public static bool ValidType(ECodeCreaterType codeCreaterType, Type type) => GetCacheValue(codeCreaterType) is CodeCreaterCacheValue cacheValue && cacheValue.ValidType(type);
        }

        private class CodeCreaterCacheValue : TIVCacheValue<CodeCreaterCacheValue, ECodeCreaterType, Type>
        {
            public Dictionary<string, Type> types = new Dictionary<string, Type>();

            public string[] typeFullNames = Empty<string>.Array;

            /// <summary>
            /// 类型全名称
            /// </summary>
            public string typeFullName = "";

            /// <summary>
            /// 类型
            /// </summary>
            public Type type => key2;

            public string outputPath = "";

            public BaseCodeCreater codeCreater = default;

            public override bool Init()
            {
                codeCreater = key1.CreateCodeCreater();
                typeFullName = key2?.FullNameToHierarchyString() ?? "";
                codeCreater?.ResetTargetObjectTypeInfo(key2);
                RefindTypes();
                RestOutputPath();
                return true;
            }

            private void RefindTypes()
            {
                if (codeCreater == null) return;
                types.Clear();
                try
                {
                    var list = TypeHelper.GetTypes(codeCreater.ValidTargetObjectType);
                    list.Sort((x, y) => string.Compare(x.FullNameToHierarchyString(), y.FullNameToHierarchyString()));
                    list.ForEach(t =>
                    {
                        var key = t.FullNameToHierarchyString();
                        if (!types.ContainsKey(key)) types.Add(t.FullNameToHierarchyString(), t);
                    });
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
                typeFullNames = types.Keys.ToArray();
            }

            public void RestOutputPath()
            {
                var dir = "";
                if (string.IsNullOrEmpty(outputPath) || !Directory.Exists(dir = Path.GetDirectoryName(outputPath)))
                {
                    outputPath = Application.dataPath + "/" + (codeCreater?.name ?? "") + ".cs";
                    return;
                }
                outputPath = PathHelper.Format(dir + "/" + (codeCreater?.name ?? "") + ".cs");
            }

            public Type GetType(string typeFullName) => types.TryGetValue(typeFullName, out var t) ? t : default;

            public bool ValidType(string typeFullName) => types.ContainsKey(typeFullName);

            public bool ValidType(Type type) => ValidType(type?.FullNameToHierarchyString() ?? "");
        }

        /// <summary>
        /// 重置代码生成器的目标类型信息
        /// </summary>
        private void ResetCodeCreaterTargetObjectTypeInfo()
        {
            var type = this.type;
            if (codeCreater != null && codeCreater.targetObjectType == type)
            {
                codeCreater.ResetTargetObjectTypeInfo(type);
            }
            else
            {
                UpdateCodeCreater();
            }
        }

        #endregion

        private void InitByCreateType()
        {
            if (type is Type targetType && codeCreater?.GetCreateType() is Type createType)
            {
                var monoScript = createType.GetMonoScript();
                var fullPath = monoScript.GetFullPath();
                if (!string.IsNullOrEmpty(fullPath))
                {
                    outputPath = fullPath;
                }
                codeCreater.InitByCreateType(createType);
            }
        }

        private void OpenCreateType()
        {
            if (type is Type targetType && codeCreater?.GetCreateType() is Type createType)
            {
                EditorHelper.OpenMonoScript(createType);
            }
        }

        /// <summary>
        /// 绘制GUI
        /// </summary>
        public override void OnGUI()
        {
            codeCreaterType = UICommonFun.Toolbar(codeCreaterType, ENameTip.None, GUILayout.Height(28));

            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            var typeFullName = this.typeFullName;
            var fullName = UICommonFun.Popup(CommonFun.TempContent("目标类型"), typeFullName, typeFullNames);
            if (typeFullName != fullName)
            {
                this.typeFullName = fullName.Replace(".", "/");
                InitByCreateType();
            }
            syncType = UICommonFun.ButtonToggle(CommonFun.NameTip(GetType(), nameof(syncType)), syncType, UICommonOption.Width120);
            EditorGUILayout.EndHorizontal();

            outputPath = UICommonFun.DrawSaveFile(CommonFun.NameTip(GetType(), nameof(outputPath)), CommonFun.TempContent("文件路径"), outputPath, UICommonOption.Width120);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("功能");
            if (GUILayout.Button("重置类型信息"))
            {
                ResetCodeCreaterTargetObjectTypeInfo();
            }
            if (GUILayout.Button("预览代码"))
            {
                CodePreviewWindow.Open(codeCreater?.OutputCode() ?? "");
            }
            if (GUILayout.Button("保存代码"))
            {
                FileHelper.OutputFile(outputPath, codeCreater?.OutputCode() ?? "");
                AssetDatabase.Refresh();
            }
            if (GUILayout.Button("另存为", UICommonOption.Width120))
            {
                var path = EditorUtility.SaveFilePanel("另存为", Path.GetDirectoryName(outputPath), Path.GetFileName(outputPath), Path.GetExtension(outputPath));
                if(!string.IsNullOrEmpty(path))
                {
                    FileHelper.OutputFile(path, codeCreater?.OutputCode() ?? "");
                }
            }
            if (GUILayout.Button("通过创建类型初始化", UICommonOption.Width120))
            {
                InitByCreateType();
            }
            if (GUILayout.Button("打开创建类型文件", UICommonOption.Width120))
            {
                OpenCreateType();
            }
            if (GUILayout.Button("重置输出路径", UICommonOption.Width120))
            {
                RestOutputPath();
            }
            EditorGUILayout.EndHorizontal();

            codeCreater?.OnGUI();
            base.OnGUI();
        }

        /// <summary>
        /// 绘制带滚动视图的GUI
        /// </summary>
        public override void OnGUIWithScrollView()
        {
            codeCreater?.OnGUIWithScrollView();
        }

        private GUIContent content_EditCodeCreaterScript = new GUIContent("编辑[代码生成器]脚本");

        /// <summary>
        /// 添加项到菜单：窗口增加点击的菜单项
        /// </summary>
        /// <param name="menu"></param>
        public override void AddItemsToMenu(GenericMenu menu)
        {
            base.AddItemsToMenu(menu);
            menu.AddItem(content_EditCodeCreaterScript, false, () =>
            {
                EditorHelper.OpenMonoScript(codeCreater?.GetType());
            });
        }
    }

}
