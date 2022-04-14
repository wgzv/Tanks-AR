using System.IO;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.EditorWindows;
using XCSJ.Helper;
using XCSJ.Interfaces;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.EditorExtension.XAssets.Manuals
{
    /// <summary>
    /// 手册目录生成器
    /// </summary>
#if XDREAMER_EDITION_XDREAMERDEVELOPER
    [XDreamerEditorWindow("开发者专用")]
#endif
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.Manual)]
    public class ManualCategoryCreaterWindow : XEditorWindowWithScrollView<ManualCategoryCreaterWindow>
    {
        /// <summary>
        /// 手册目录生成器
        /// </summary>
        public const string Title = Product.Name + "手册目录生成器";

        /// <summary>
        /// 初始化
        /// </summary>
#if XDREAMER_EDITION_XDREAMERDEVELOPER
        [MenuItem(XDreamerEditor.EditorWindowMenu + Title)]
#endif
        public static void Init() => OpenAndFocus();

        private bool manualIvalid = false;

        private void CheckManual()
        {
            if (!ManualHelper.ManualDirectoryExists())
            {
                manualIvalid = true;
                return;
            }
            if (manualIvalid)//从无效变为有效时，做部分数据的初始化
            {
                RefreshParams();
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
            CheckManual();
        }

        public override void OnDisable()
        {
            manualIvalid = true;
            if (!manualIvalid)
            {
                CheckModifyFlag();
            }
            base.OnDisable();
        }

        protected override void OnOptionModified(Option option)
        {
            base.OnOptionModified(option);
            if (option is XAssetStoreOption)
            {
                CheckManual();
                Repaint();
            }
        }

        private void RefreshParams()
        {
            dataJSPath = ManualHelper.DataJS;;
        }

        [Name("Data.js")]
        public string dataJSPath = "";

        public DataJSFile<DataNode> data = null;

        public Vector2 leftArea = new Vector2(300, 0);

        [Name("显示成员")]
        public bool displayChildren = false;

        private static DataJSFile<DataNode> New()
        {
            var file = new DataJSFile<DataNode>() { name = nameof(data) };
            file.listdata.Add(new DataNode());
            file.Init();
            return file;
        }

        private void NewFile()
        {
            data = New();
        }

        private void ImportFile()
        {
            if (!FileHelper.Exists(dataJSPath))
            {
                Debug.LogErrorFormat("文件[{0}]不存在！", dataJSPath);
                NewFile();
            }
            else if (!DataJSFile<DataNode>.TryLoad(FileHelper.InputFile(dataJSPath), out data))
            {
                Debug.LogError("导入文件失败!");
                NewFile();
            }
        }

        private void ExportFile()
        {
            FileHelper.OutputFile(dataJSPath, data.ToString());
            Debug.Log("导出文件完成:" + dataJSPath);
        }

        public bool modifyFlag = false;

        private void CheckModifyFlag()
        {
            if (modifyFlag)
            {
                modifyFlag = false;
                if (EditorUtility.DisplayDialog("提示", "内容已经发生修改,是否保存?", "保存", "取消"))
                {
                    ExportFile();
                }
            }
        }

        public override void OnGUI()
        {
            if (manualIvalid)
            {
                UICommonFun.NotificationLayout("手册目录无效,本功能不可用！");
                return;
            }

            EditorGUILayout.BeginHorizontal();
            dataJSPath = EditorGUILayout.TextField(CommonFun.NameTip(this, nameof(dataJSPath)), dataJSPath);
            if (string.IsNullOrEmpty(dataJSPath)) dataJSPath = ManualHelper.DataJS;
            if (GUILayout.Button("...", EditorStyles.miniButtonMid, UICommonOption.Width32, UICommonOption.Height16))
            {
                string selectedFile = EditorUtility.SaveFilePanel("选择文件", Path.GetDirectoryName(dataJSPath), "data", "js");
                if (!string.IsNullOrEmpty(selectedFile))
                {
                    dataJSPath = selectedFile;
                }
            }
            ManualCreaterWindow.DrawRevealInFinder(dataJSPath);
            EditorGUILayout.EndHorizontal();

            displayChildren = EditorGUILayout.Toggle(CommonFun.NameTip(this, nameof(displayChildren)), displayChildren);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(CommonFun.NameTip(EIcon.New, false), EditorStyles.miniButtonLeft, UICommonOption.Height32))
            {
                CheckModifyFlag();
                NewFile();
            }
            if (GUILayout.Button(CommonFun.NameTip(EIcon.Import, false), EditorStyles.miniButtonMid, UICommonOption.Height32) || data == null)
            {
                CheckModifyFlag();
                ImportFile();
            }
            if (GUILayout.Button(CommonFun.NameTip(EIcon.Export, false), EditorStyles.miniButtonRight, UICommonOption.Height32))
            {
                modifyFlag = false;
                ExportFile();
            }
            EditorGUILayout.EndHorizontal();

            try
            {
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(leftArea.x), GUILayout.ExpandHeight(true));
                DrawLeftArea();
                EditorGUILayout.EndVertical();

                leftArea = UICommonFun.ResizeSeparatorLayout(leftArea, true, true, GUILayout.Width(3), GUILayout.ExpandHeight(true));

                EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                base.OnGUI();
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }
            finally
            {
                if (EditorGUI.EndChangeCheck())
                {
                    modifyFlag = true;
                }
            }
        }

        public override void OnGUIWithScrollView()
        {
            DrawRigthArea();
        }

        public Vector2 leftScrollValue = new Vector2();

        private void DrawLeftArea()
        {
            leftScrollValue = EditorGUILayout.BeginScrollView(leftScrollValue, false, false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            TreeView.Draw(data.listdata.ToArray(), null, null, (node, c) =>
            {
                var dataNode = node as DataNode;
                TreeView.DefaultDrawAction(node, c);

                #region 操作按钮

                EditorGUI.BeginChangeCheck();
                if (dataNode.parent == null)
                {
                    var index = data.listdata.IndexOf(dataNode);
                    XEditorGUI.DrawListOperation(data.listdata, index, XEditorGUI.EListOperationFlags.All, (f, l, i) =>
                    {
                        switch (f)
                        {
                            case XEditorGUI.EListOperationFlags.New:
                                {
                                    data.listdata.Add(new DataNode());
                                    break;
                                }
                            case XEditorGUI.EListOperationFlags.InsertChild:
                                {
                                    dataNode.children.Add(new DataNode());
                                    break;
                                }
                            case XEditorGUI.EListOperationFlags.Insert:
                                {
                                    var newDataNode = JsonHelper.ToObject<DataNode>(JsonHelper.ToJson(dataNode));
                                    data.listdata.Insert(index, newDataNode);
                                    break;
                                }
                            default:
                                {
                                    XEditorGUI.DefautListOperation(f, l, i);
                                    break;
                                }
                        }
                    }, UICommonOption.WH24x16);

                    EditorGUILayout.LabelField((index + 1).ToString(), UICommonOption.WH24x16);
                }
                else
                {
                    var index = dataNode.parent.children.IndexOf(dataNode);
                    XEditorGUI.DrawListOperation(dataNode.parent.children, index, XEditorGUI.EListOperationFlags.All, (f, l, i) =>
                    {
                        switch (f)
                        {
                            case XEditorGUI.EListOperationFlags.New:
                                {
                                    dataNode.parent.children.Add(new DataNode());
                                    break;
                                }
                            case XEditorGUI.EListOperationFlags.InsertChild:
                                {
                                    dataNode.children.Add(new DataNode());
                                    break;
                                }
                            case XEditorGUI.EListOperationFlags.Insert:
                                {
                                    var newDataNode = JsonHelper.ToObject<DataNode>(JsonHelper.ToJson(dataNode));
                                    dataNode.parent.children.Insert(index, newDataNode);
                                    break;
                                }
                            default:
                                {
                                    XEditorGUI.DefautListOperation(f, l, i);
                                    break;
                                }
                        }
                    }, UICommonOption.WH24x16);

                    EditorGUILayout.LabelField((index + 1).ToString(), UICommonOption.WH24x16);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    data.Init();
                }

                #endregion
            });

            EditorGUILayout.EndScrollView();
        }

        private void DrawRigthArea()
        {
            if (DataNode.current == null) return;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(CommonFun.NameTip(DataNode.current, nameof(DataNode.current.parent)));
            if(GUILayout.Button(DataNode.current.parent != null ? DataNode.current.parent.display : CommonFun.TempContent("")))
            {
                if(DataNode.current.parent != null)
                {
                    DataNode.current = DataNode.current.parent;
                }
            }
            EditorGUILayout.EndHorizontal();

            XEditorGUI.DrawObject(DataNode.current, (o, f) =>
            {
                switch (f.Name)
                {
                    case nameof(DataNode.children):
                        {
                            return !displayChildren;
                        }
                }
                return false;
            });
        }
    }

    public class DataNode : Data<DataNode>, ITreeNodeGraph
    {
        [Json(false)]
        public GUIContent display => new GUIContent(text, string.IsNullOrEmpty(a_attr.text_en) ? text : a_attr.text_en);

        bool ITreeNodeGraph.enable { get => true; set { } }
        bool ITreeNodeGraph.visible { get => true; set { } }

        int ITreeNodeGraph.depth
        {
            get
            {
                var parent = (this as ITreeNodeGraph).parent;
                return parent == null ? 0 : parent.depth + 1;
            }
        }

        [Json(false)]
        public bool expanded { get => state.opened; set => state.opened = value; }
        [Json(false)]
        public bool selected { get; set; } = false;

        [Name("父级节点")]
        internal DataNode parent = null;

        ITreeNodeGraph ITreeNodeGraph.parent => parent;

        ITreeNode ITreeNode.parent => parent;

        ITreeNodeGraph[] ITreeNodeGraph.children => children.ToArray();

        ITreeNode[] ITreeNode.children => children.ToArray();

        string ITreeNode.displayName => text;

        string IName.name { get => text; set => text = value; }

        private static DataNode _current = null;
        internal static DataNode current
        {
            get => _current;
            set
            {
                if (_current != null) _current.selected = false;
                _current = value;
                if (_current != null) _current.selected = true;
            }
        }

        public void OnClick()
        {
            current = this;
            if (children.Count > 0)
            {
                state.opened = !state.opened;
            }
            GUI.FocusControl("");
        }

        public override bool Init(DataNode data)
        {
            parent = data;
            return base.Init(data);
        }
    }
}
