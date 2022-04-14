using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.EditorWindows;
using XCSJ.EditorExtension.XAssets.Compilers;
using XCSJ.Helper;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.EditorExtension.XAssets.Manuals
{
    /// <summary>
    /// 手册生成器
    /// </summary>
#if XDREAMER_EDITION_XDREAMERDEVELOPER
    [XDreamerEditorWindow("开发者专用")]
#endif
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.Manual)]
    public class ManualCreaterWindow : XEditorWindowWithScrollView<ManualCreaterWindow>
    {
        public const string Title = Product.Name + "手册生成器";

        /// <summary>
        /// 初始化
        /// </summary>
#if XDREAMER_EDITION_XDREAMERDEVELOPER
        [MenuItem(XDreamerEditor.EditorWindowMenu + Title)]
#endif
        public static void Init() => OpenAndFocus();

        [Name("类型")]
        public static List<Type> types { get; private set; } = null;

        public static bool ExistType(Type type)
        {
            if (types == null) return false;
            InitDataIfNeed();
            return types.Contains(type);
        }
        public static Type GetType(string type, bool namePathOrFullName = true)
        {
            if (types == null || string.IsNullOrEmpty(type)) return null;
            InitDataIfNeed();
            var fullName = namePathOrFullName ? ManualHelper.ToFullName(type) : type;
            return types.FirstOrDefault(t => t.FullName == fullName);
        }
        public static void InitDataIfNeed()
        {
            if (types == null)
            {
                types = TypeHelper.FindTypeInAppWithClass(typeof(UnityEngine.Object)).Where(t => NeedOutput(t)).ToList();
                //types = TypeHelper.GetTypes(t => NeedOutput(t)).ToList();
            }
        }
        public static void InitData()
        {
            types = null;
            InitDataIfNeed();
        }

        private const string NamespacePrifix = nameof(XCSJ) + ".";
        private const string ClassTypeLimit = "+<>c";

        /// <summary>
        /// 对于<see cref="Type"/>类型要求属于<see cref="XCSJ"/>命名空间且为共有类型时才可输出；其他成员信息（字段<see cref="FieldInfo"/>、属性<see cref="PropertyInfo"/>、方法<see cref="MethodInfo"/>）类型只要不是特殊名称的均可输出
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        private static bool NeedOutput(MemberInfo memberInfo)
        {
            if (memberInfo == null) return false;
            if (memberInfo is Type type)
            {
                var typeString = type.ToString();
                return (type.IsPublic || type.IsNestedPublic)
                    && typeString.StartsWith(NamespacePrifix)
                    && !typeString.Contains(ClassTypeLimit);
            }

            if (!NeedOutput(memberInfo.DeclaringType)) return false;

            if (memberInfo is FieldInfo fieldInfo)
            {
                return !fieldInfo.IsSpecialName;
            }
            if (memberInfo is PropertyInfo propertyInfo)
            {
                return !propertyInfo.IsSpecialName;
            }
            if (memberInfo is MethodInfo methodInfo)
            {
                return !methodInfo.IsSpecialName;
            }
            return true;
        }

        private bool manualIvalid = true;

        private void CheckManual()
        {
            if (outputMode != EOutputMode.None) return;
            if (!ManualHelper.ManualDirectoryExists())
            {
                manualIvalid = true;
                return;
            }
            if (manualIvalid)//从无效变为有效时，做部分数据的初始化
            {
                RefreshParams();
                htmlTemplateFile.InitFile();
            }
            manualIvalid = false;
        }

        private void OnFocus()
        {
            CheckManual();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            InitData();

            CheckManual();

            htmlTemplate.needOutput += NeedOutput;
        }

        public override void OnDisable()
        {
            manualIvalid = true;
            htmlTemplate.needOutput -= NeedOutput;
            base.OnDisable();
        }

        public string[] filters = new string[] { "html", "html" };

        public static void DrawRevealInFinder(string path)
        {
            if (GUILayout.Button(CommonFun.NameTip(EIcon.Open), EditorStyles.miniButtonRight, UICommonOption.Width32, UICommonOption.Height16))
            {
                EditorUtility.RevealInFinder(path);
            }
        }

        public override void OnGUI()
        {
            if (manualIvalid)
            {
                UICommonFun.NotificationLayout("手册目录无效,本功能不可用！");
                return;
            }

            InitDataIfNeed();
            InitTypeNodeIfNeed();

            EditorGUILayout.LabelField(CommonFun.Name(this, nameof(types)), types.Count.ToString());

            EditorGUILayout.BeginHorizontal();
            workDirectory = EditorGUILayout.TextField(CommonFun.NameTip(this, nameof(workDirectory)), workDirectory);
            if (string.IsNullOrEmpty(workDirectory)) workDirectory = ManualHelper.Manual;
            if (GUILayout.Button("...", EditorStyles.miniButtonMid, UICommonOption.Width32, UICommonOption.Height16))
            {
                string dir = EditorUtility.SaveFolderPanel("选择工作目录", Path.GetDirectoryName(workDirectory), nameof(ManualHelper.Manual));
                if (!string.IsNullOrEmpty(dir))
                {
                    workDirectory = dir;
                }
            }
            DrawRevealInFinder(workDirectory);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            dataManualJSPath = EditorGUILayout.TextField(CommonFun.NameTip(this, nameof(dataManualJSPath)), dataManualJSPath);
            if (string.IsNullOrEmpty(dataManualJSPath)) dataManualJSPath = ManualHelper.DataManualJS;
            if (GUILayout.Button("...", EditorStyles.miniButtonMid, UICommonOption.Width32, UICommonOption.Height16))
            {
                string selectedFile = EditorUtility.SaveFilePanel("选择文件", Path.GetDirectoryName(dataManualJSPath), "data", "js");
                if (!string.IsNullOrEmpty(selectedFile))
                {
                    dataManualJSPath = selectedFile;
                }
            }
            DrawRevealInFinder(dataManualJSPath);
            EditorGUILayout.EndHorizontal();

            //模版文件
            XEditorGUI.DrawObject(htmlTemplateFile, null, (obj, f, o) =>
            {
                if (obj == null)
                {
                    if (GUILayout.Button("重置", GUILayout.Width(50)))
                    {
                        htmlTemplateFile.InitFile();
                        GUI.FocusControl("");
                    }
                }
                else
                {
                    var path = o as string;
                    EditorGUI.BeginChangeCheck();
                    UICommonFun.SelectFileButtonInAssets(ref path, CommonFun.TempContent("..."), null, null, filters);
                    if (EditorGUI.EndChangeCheck())
                    {
                        f.SetValue(obj, path);
                    }
                }
            });

            DrawOutput(); 
            DrawTypeNodeOperation();
            base.OnGUI();
            HandleOutput();
        }

        public override void OnGUIWithScrollView()
        {
            DrawTypeNode();
        }

        protected override void OnOptionModified(Option option)
        {
            base.OnOptionModified(option);
            if(option is XAssetStoreOption)
            {
                CheckManual();
                Repaint();
            }
        }

        #region 类型节点

        public static string ParentKey(string key) => Path.GetFileNameWithoutExtension(key);
        public static string IndexKey(Type type) => ManualHelper.GetFullName(type);

        private TypeNode xcsj = null;
        private Dictionary<string, TypeNode> typeNodes = new Dictionary<string, TypeNode>();
        private List<TypeNode> typeNodeList = new List<TypeNode>();

        private TypeNode GetTypeNode(string key)
        {
            if (string.IsNullOrEmpty(key)) return xcsj;
            if (typeNodes.TryGetValue(key, out TypeNode data)) return data;

            data = new TypeNode(key);
            Add(GetTypeNode(ParentKey(key)), data);
            return data;
        }

        private void Add(TypeNode parent, TypeNode son)
        {
            Add(son);
            son.parent = parent;
            parent.children.Add(son);
        }

        private void Add(TypeNode data)
        {
            typeNodes[data.key] = data;
            typeNodeList.Add(data);
        }

        public void Insert(Type type)
        {
            if (typeNodes.TryGetValue(IndexKey(type), out TypeNode data))
            {
                data.type = type;
                return;
            }
            data = new TypeNode(type);
            var parent = GetTypeNode(ParentKey(data.key));
            Add(parent, data);
        }

        private void InitTypeNodeIfNeed()
        {
            if (xcsj == null)
            {
                InitTypeNode(types);
            }
        }
        private void InitTypeNode(List<Type> types)
        {
            xcsj = new TypeNode(nameof(XCSJ));
            typeNodes.Clear();
            typeNodeList.Clear();
            Add(xcsj);
            foreach (var type in types)
            {
                Insert(type);
            }
            xcsj.Foreach(n => n.children.Sort((x, y) => string.Compare(x.key, y.key)));//排序
            UpdateOutputTypeCount();
            UpdateOutputTypeBySearchText();
        }

        [Name("显示输出类型信息")]
        public bool displayTypes = false;

        [Name("类型操作")]
        private enum ETypeOperation
        {
            [Name("全选选择")]
            All,
            [Name("全部取消")]
            None,
            [Name("反选")]
            Switch,

            [Name("展开")]
            Expand,

            [Name("折叠")]
            Unexpand,

            [Name("折叠/展开")]
            SwitchExpand,
        }

        public string searchText = "";
        [Name("输出类型数量")]
        public int outputTypeCount = 0;

        private void UpdateOutputTypeCount()
        {
            outputTypeCount = GetOutputTypes().Count;
        }
        private void UpdateOutputTypeBySearchText()
        {
            typeNodeList.ForEach(n =>
            {
                if (n.Any(node => node.name.SearchMatch(searchText)))
                {
                    n.visible = true;
                }
                else
                {
                    n.visible = false;
                }
            });
        }

        private void DrawTypeNodeOperation()
        {
            if (displayTypes = UICommonFun.Foldout(displayTypes, CommonFun.NameTip(typeof(ManualCreaterWindow), nameof(displayTypes))))
            {
                CommonFun.BeginLayout();
                EditorGUILayout.LabelField(CommonFun.NameTip(typeof(ManualCreaterWindow), nameof(outputTypeCount)), CommonFun.TempContent(outputTypeCount.ToString()));
                EditorGUI.BeginChangeCheck();
                UICommonFun.EnumButton<ETypeOperation>(to =>
                {
                    switch (to)
                    {
                        case ETypeOperation.All:
                            {
                                typeNodeList.ForEach(n => n.output = true);
                                break;
                            }
                        case ETypeOperation.None:
                            {
                                typeNodeList.ForEach(n => n.output = false);
                                break;
                            }
                        case ETypeOperation.Switch:
                            {
                                typeNodeList.ForEach(n => n.output = !n.output);
                                break;
                            }
                        case ETypeOperation.Expand:
                            {
                                typeNodeList.ForEach(n => n.expanded = true);
                                break;
                            }
                        case ETypeOperation.Unexpand:
                            {
                                typeNodeList.ForEach(n => n.expanded = false);
                                break;
                            }
                        case ETypeOperation.SwitchExpand:
                            {
                                typeNodeList.ForEach(n => n.expanded = !n.expanded);
                                break;
                            }
                    }
                });
                if (EditorGUI.EndChangeCheck())
                {
                    UpdateOutputTypeCount();
                }
                EditorGUI.BeginChangeCheck();
                searchText = UICommonFun.SearchTextField(searchText);
                if (EditorGUI.EndChangeCheck())
                {
                    UpdateOutputTypeBySearchText();
                }

                CommonFun.EndLayout();
            }
        }

        private void DrawTypeNode()
        {
            if (!displayTypes) return;

            CommonFun.BeginLayout(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            TreeView.Draw(xcsj, (node, label) =>
            {
                var typeNode = node as TypeNode;

                EditorGUI.BeginChangeCheck();
                if (typeNode.type == null)
                {
                    EditorGUI.BeginChangeCheck();
                    typeNode.output = EditorGUILayout.ToggleLeft(label, typeNode.output, GUILayout.ExpandWidth(true));
                    if (EditorGUI.EndChangeCheck())
                    {
                        typeNode.Foreach(n =>
                        {
                            if (n.visible)
                            {
                                n.output = typeNode.output;
                            }
                        });
                    }
                }
                else
                {
                    typeNode.output = EditorGUILayout.ToggleLeft(label, typeNode.output, GUILayout.ExpandWidth(true));
                }
                if (EditorGUI.EndChangeCheck())
                {
                    UpdateOutputTypeCount();
                }
            });
            CommonFun.EndLayout();
        }

        private List<Type> GetOutputTypes()
        {
            return typeNodeList.Where(n => n.output && n.type != null).ToList(n => n.type);
        }

        public class TypeNode : ITreeNodeGraph
        {
            public Type type = null;
            public TypeNode parent = null;
            public List<TypeNode> children = new List<TypeNode>();

            public bool output = true;

            public string name = "";
            public string key = "";

            public TypeNode(string key)
            {
                this.key = key;
                if (key.Contains("."))
                {
                    this.name = Path.GetExtension(key);
                }
                else
                {
                    this.name = key;
                }
            }
            public TypeNode(Type type)
            {
                this.type = type;
                this.name = string.Format("{0} ({1})", type.Name, CommonFun.Name(type));
                this.key = IndexKey(type);
            }

            public GUIContent display => CommonFun.TempContent(name);

            public bool enable { get; set; } = true;
            public bool visible { get; set; } = true;

            public int depth => parent == null ? 0 : parent.depth + 1;

            public bool expanded { get; set; } = true;
            public bool selected { get; set; } = false;

            ITreeNodeGraph ITreeNodeGraph.parent => parent;

            ITreeNodeGraph[] ITreeNodeGraph.children => children.ToArray();

            public string displayName => name;

            string IName.name { get => name; set => name = value; }

            ITreeNode ITreeNode.parent => parent;

            ITreeNode[] ITreeNode.children => children.ToArray();

            public void OnClick()
            {
            }

            public List<TypeNode> Get(Func<TypeNode, bool> func)
            {
                List<TypeNode> typeNodes = new List<TypeNode>();
                if (func(this)) typeNodes.Add(this);
                foreach (var n in children)
                {
                    typeNodes.AddRange(n.Get(func));
                }
                return typeNodes;
            }

            /// <summary>
            /// 遍历当前对象与所有子级对象
            /// </summary>
            /// <param name="action"></param>
            public void Foreach(Action<TypeNode> action)
            {
                action(this);
                foreach (var n in children)
                {
                    n.Foreach(action);
                }
            }

            /// <summary>
            /// 如果当前对象与所有子级对象中有任意一个满足，则返回True
            /// </summary>
            /// <param name="func"></param>
            /// <returns></returns>
            public bool Any(Func<TypeNode, bool> func)
            {
                if (func(this)) return true;
                foreach (var n in children)
                {
                    if (n.Any(func)) return true;
                }
                return false;
            }
        }

        #endregion

        #region 输出

        private void DrawOutput()
        {
            if (outputOption = UICommonFun.Foldout(outputOption, CommonFun.NameTip(GetType(), nameof(outputOption))))
            {
                CommonFun.BeginLayout();

                detailLog = EditorGUILayout.Toggle(CommonFun.NameTip(this, nameof(detailLog)), detailLog);

                outMissNoteLog = EditorGUILayout.Toggle(CommonFun.NameTip(this, nameof(outMissNoteLog)), outMissNoteLog);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(CommonFun.NameTip(GetType(), nameof(outputStep1)));
                if (GUILayout.Button(CommonFun.Tip(GetType(), nameof(outputStep1))))
                {
                    Compiler.ModifyCsprojs(Path.GetDirectoryName(Application.dataPath));
                    EditorUtility.OpenWithDefaultApp(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this)));
                }
                EditorGUILayout.EndHorizontal();

                //outputRule = (EOutputRule)UICommonFun.EnumPopup(CommonFun.NameTip(GetType(), nameof(outputRule)), outputRule);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(CommonFun.NameTip(GetType(), nameof(outputStep3)));
                if (GUILayout.Button(CommonFun.Tip(GetType(), nameof(outputStep3))))
                {
                    OuputAsync(GetOutputTypes());
                }
                EditorGUILayout.EndHorizontal();
                CommonFun.EndLayout();
            }
        }

        [Name("输出选项-请遵守操作步骤")]
        public bool outputOption = false;

        [Name("详细日志")]
        public bool detailLog = true;

        [Name("输出缺失注释的日志")]
        public bool outMissNoteLog = false;

        [Name("1.工程修改")]
        [Tip("修改当前工程csproj文件,并使用VS编译当前工程")]
        private string outputStep1 = "";

        //[Name("2.输出规则")]
        //private EOutputRule outputRule = EOutputRule.All;

        [Name("2.生成并输出手册")]
        [Tip("开始生成")]
        private string outputStep3 = "";

        //public enum EOutputRule
        //{
        //    [Name("完整输出")]
        //    [Tip("包含类型、命名空间、JS等文件")]
        //    All = 0,

        //    [Name("类型")]
        //    [Tip("仅包含类型文件")]
        //    Type,
        //}

        public enum EOutputMode
        {
            None,
            Begin,
            End,

            BeginType,
            InType,
            EndType,
            CompletedType,
            CancleType,

            BeginData,
            InData,
            EndData,
            CompleteData,
            CancleData,

            Refresh,
        }
        private EOutputMode outputMode = EOutputMode.None;
        private DateTime beginTime;
        private DateTime beginTypeTime;
        private DateTime begintDataTime;
        private int outputTypeIndex = 0;
        private int outputDataIndex = 0;

        /// <summary>
        /// 待输出的类型列表
        /// </summary>
        private List<Type> inTypes = new List<Type>();
        private void HandleOutput()
        {
            try
            {
                switch (outputMode)
                {
                    case EOutputMode.Begin:
                        {
                            Log.DebugFormat("手册生成开始!");
                            outputTypeIndex = 0;
                            outputDataIndex = 0;
                            beginTime = DateTime.Now;
                            outputMode = _BeginOutput() ? EOutputMode.BeginType : EOutputMode.End;
                            Repaint();
                            break;
                        }
                    case EOutputMode.End:
                        {
                            EditorUtility.ClearProgressBar();
                            _EndOutput();
                            outputMode = EOutputMode.Refresh;
                            Log.DebugFormat("手册生成结束!共计类型: {0}/{1} 个,数据: {2}/{3} 个,耗时: {4} 秒.",
                                outputTypeIndex,
                                inTypes.Count,
                                outputDataIndex,
                                ManualDataJS.dataList.Count,
                                (DateTime.Now - beginTime).TotalSeconds);
                            Repaint();
                            break;
                        }
                    case EOutputMode.Refresh:
                        {
                            AssetDatabase.Refresh();
                            outputMode = EOutputMode.None;
                            Log.DebugFormat("资产数据库刷新结束!手册生成总耗时: {0} 秒.", (DateTime.Now - beginTime).TotalSeconds);
                            break;
                        }
                    case EOutputMode.BeginType:
                        {
                            beginTypeTime = DateTime.Now;
                            outputMode = EOutputMode.InType;
                            outputTypeIndex = 0;
                            Log.DebugFormat("开始类型输出!");
                            break;
                        }
                    case EOutputMode.InType:
                        {
                            if (_InType(ref outputTypeIndex))
                            {
                                outputMode = EOutputMode.CompletedType;
                            }
                            var count = inTypes.Count;
                            if (EditorUtility.DisplayCancelableProgressBar("类型输出中...", string.Format("{0}/{1}", outputTypeIndex, count), outputTypeIndex * 1f / count))
                            {
                                outputMode = EOutputMode.CancleType;
                            }
                            Repaint();
                            break;
                        }
                    case EOutputMode.EndType:
                        {
                            EditorUtility.ClearProgressBar();
                            outputMode = EOutputMode.BeginData;
                            //switch (outputRule)
                            //{
                            //    case EOutputRule.All:
                            //        {
                            //            outputMode = EOutputMode.BeginData;
                            //            break;
                            //        }
                            //    case EOutputRule.Type:
                            //        {
                            //            outputMode = EOutputMode.EndData;
                            //            break;
                            //        }
                            //}
                            Repaint();
                            break;
                        }
                    case EOutputMode.CompletedType:
                        {
                            Log.DebugFormat("类型输出完成!共计类型: {0}/{1} 个,耗时: {2} 秒.", outputTypeIndex, inTypes.Count, (DateTime.Now - beginTypeTime).TotalSeconds);
                            outputMode = EOutputMode.EndType;
                            Repaint();
                            break;
                        }
                    case EOutputMode.CancleType:
                        {
                            Log.DebugFormat("类型输出取消!共计类型: {0}/{1} 个,耗时: {2} 秒.", outputTypeIndex, inTypes.Count, (DateTime.Now - beginTypeTime).TotalSeconds);
                            outputMode = EOutputMode.EndType;
                            Repaint();
                            break;
                        }
                    case EOutputMode.BeginData:
                        {
                            begintDataTime = DateTime.Now;
                            outputMode = EOutputMode.InData;
                            outputDataIndex = 0;

                            Log.DebugFormat("开始数据输出!");
                            Repaint();
                            break;
                        }
                    case EOutputMode.InData:
                        {
                            if (_InData(ref outputDataIndex))
                            {
                                outputMode = EOutputMode.CompleteData;
                            }
                            var count = ManualDataJS.dataList.Count;
                            if (EditorUtility.DisplayCancelableProgressBar("数据输出中...", string.Format("{0}/{1}", outputDataIndex, count), outputDataIndex * 1f / count))
                            {
                                outputMode = EOutputMode.CancleData;
                            }
                            Repaint();
                            break;
                        }
                    case EOutputMode.EndData:
                        {
                            EditorUtility.ClearProgressBar();
                            outputMode = EOutputMode.End;
                            Repaint();
                            break;
                        }
                    case EOutputMode.CompleteData:
                        {
                            Log.DebugFormat("数据输出完成!共计类型: {0}/{1} 个,耗时: {2} 秒.", outputDataIndex, ManualDataJS.dataList.Count, (DateTime.Now - begintDataTime).TotalSeconds);
                            outputMode = EOutputMode.EndData;
                            Repaint();
                            break;
                        }
                    case EOutputMode.CancleData:
                        {
                            Log.DebugFormat("数据输出取消!共计类型: {0}/{1} 个,耗时: {2} 秒.", outputDataIndex, ManualDataJS.dataList.Count, (DateTime.Now - begintDataTime).TotalSeconds);
                            outputMode = EOutputMode.EndData;
                            Repaint();
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Log.Exception("手册生成异常: " + ex);
                outputMode = EOutputMode.End;
                Repaint();
            }
        }

        private void RefreshParams()
        {
            workDirectory = ManualHelper.Manual;
            dataManualJSPath = ManualHelper.DataManualJS;
        }

        [Name("工作目录")]
        public string workDirectory = "";

        [Name("Data.Manual.js")]
        public string dataManualJSPath = "";

        public HtmlTemplateFile htmlTemplateFile = new HtmlTemplateFile();
        public HtmlTemplate htmlTemplate = new HtmlTemplate();
        public ManualDataJS manualDataJS = new ManualDataJS();

        private bool _BeginOutput()
        {
            if (string.IsNullOrEmpty(workDirectory))
            {
                Log.Error("工作目录无效:" + workDirectory);
                return false;
            }
            NoteHelper.Init();
            NoteHelper.outMissNoteLog = outMissNoteLog;
            htmlTemplate.Init(htmlTemplateFile, workDirectory);
            manualDataJS.Init();

            return true;
        }
        private void _EndOutput()
        {
            OuputDataJS(manualDataJS, htmlTemplate);
            NoteHelper.Release();
        }

        private bool _InType(ref int index, int countPerTime = 1)
        {
            var count = inTypes.Count;
            for (int i = 0; index < count && i < countPerTime; ++index, ++i)
            {
                var type = inTypes[index];
                if (detailLog)
                {
                    Log.DebugFormat("{0}: {1}", index, type);
                }
                _InType(type);
            }
            return index >= count;
        }
        private void _InType(Type type)
        {
            manualDataJS.Insert(type);
            OuputType(type, htmlTemplate);
        }

        private bool _InData(ref int index, int countPerTime = 1)
        {
            var count = ManualDataJS.dataList.Count;
            for (int i = 0; index < count && i < countPerTime; ++index, ++i)
            {
                var data = ManualDataJS.dataList[index];
                if (detailLog)
                {
                    Log.DebugFormat("{0}: {1}", index, data.key);
                }
                _InData(data);
            }
            return index >= count;
        }
        private void _InData(Data data)
        {
            OuputData(data, htmlTemplate);
        }

        /// <summary>
        /// 输出类型异步--开始
        /// </summary>
        /// <param name="types"></param>
        public void OuputAsync(IEnumerable<Type> types)
        {
            if (outputMode != EOutputMode.None || types == null) return;
            inTypes.Clear();
            inTypes.AddRange(types);
            inTypes.Sort((x, y) => string.Compare(x.FullName, y.FullName));//执行排序后再按顺序输出-保证生成的数据JS有序性；
            outputMode = EOutputMode.Begin;
        }

        public void Ouput(IEnumerable<Type> types)
        {
            if (!_BeginOutput()) return;

            foreach (var type in types)
            {
                _InType(type);
            }

            _EndOutput();
        }

        private void OuputType(Type type, HtmlTemplate htmlTemplate)
        {
            htmlTemplate.Output(type);
        }
        private void OuputData(Data data, HtmlTemplate htmlTemplate)
        {
            htmlTemplate.Output(data);
        }
        private void OuputDataJS(ManualDataJS dataJS, HtmlTemplate htmlTemplate)
        {
            FileHelper.OutputFile(dataManualJSPath, dataJS.ToDataJS(htmlTemplate));
        }

        #endregion
    }
}
