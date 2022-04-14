using System.Collections;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.EditorWindows;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.EditorExtension.CNScripts.Windows
{
    /// <summary>
    /// 层级变量编辑窗口
    /// </summary>
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.Variable)]
    public class HierarchyVarEditorWindow : XEditorWindowWithScrollView<HierarchyVarEditorWindow>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "层级变量编辑窗口";

        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="variable"></param>
        /// <param name="variableSerializedProperty"></param>
        public static void Open(UnityEngine.Object obj, Variable variable, SerializedProperty variableSerializedProperty)
        {
            OpenAndFocus();
            instance.Set(obj, variable, variableSerializedProperty);
        }

        private void Set(UnityEngine.Object obj, Variable variable, SerializedProperty variableSerializedProperty)
        {
            variableHandle = obj as IVariableHandle;
            if (variableHandle == null)
            {
                Close();
                return;
            }

            this.obj = obj;
            this.variable = variable;
            this.variableSerializedProperty = variableSerializedProperty;

            varValueJsonString = hierarchyVar.ToJson(true);
        }

        private IVariableHandle variableHandle;
        private UnityEngine.Object obj;
        private Variable variable;
        private SerializedProperty variableSerializedProperty;

        private HierarchyVar hierarchyVar=> variable.hierarchyVar;
        private string varValueJsonString;
        private bool hasChanged = false;
        private EScriptWindowViewerMode viewerMode = EScriptWindowViewerMode.List;

        private static string addKeyText = "";
        private static string addValueText = "";
        private bool formatKey = true;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            Undo.undoRedoPerformed += OnUndo;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            Undo.undoRedoPerformed -= OnUndo;
        }

        private void OnUndo()
        {
            variable.MarkDirty();
            varValueJsonString = hierarchyVar.ToJson(true);
            Repaint();
        }

        /// <summary>
        /// 绘制GUI
        /// </summary>
        public override void OnGUI()
        {
            if (variableHandle == null)
            {
                Close();
                return;
            }
            var hierarchyVar = this.hierarchyVar;
            if (hierarchyVar == null) return;

            EditorGUILayout.BeginHorizontal(GUI.skin.box, UICommonOption.Height16, GUILayout.ExpandWidth(true));

            GUILayout.Label("变量所属对象:");
            EditorGUILayout.ObjectField(obj, obj.GetType(), true, GUILayout.ExpandWidth(true));

            GUILayout.Label("变量作用域:");
            EditorGUILayout.LabelField(CommonFun.NameTip(hierarchyVar.varScope), UICommonOption.Width60, UICommonOption.Height16);

            //GUILayout.FlexibleSpace();

            GUILayout.Label("变量字符串:");
            EditorGUILayout.SelectableLabel(hierarchyVar.varHierarchyString, UICommonOption.Height16);

            GUILayout.Label("格式化键:");
            formatKey = UICommonFun.ButtonToggle(UICommonOption.Script, formatKey, EditorStyles.miniButton, UICommonOption.WH24x16);

            EditorGUI.BeginChangeCheck();
            var selectList = UICommonFun.ButtonToggle(CommonFun.NameTip(EScriptWindowViewerMode.List, ENameTip.EmptyTextWhenHasImage), viewerMode == EScriptWindowViewerMode.List, EditorStyles.miniButtonLeft, UICommonOption.WH24x16);
            var selectText = UICommonFun.ButtonToggle(CommonFun.NameTip(EScriptWindowViewerMode.Text, ENameTip.EmptyTextWhenHasImage), viewerMode == EScriptWindowViewerMode.Text, EditorStyles.miniButtonRight, UICommonOption.WH24x16);
            if (EditorGUI.EndChangeCheck())
            {
                switch (viewerMode)
                {
                    case EScriptWindowViewerMode.List:
                        {
                            if (selectText)
                            {
                                viewerMode = EScriptWindowViewerMode.Text;
                                varValueJsonString = hierarchyVar.ToJson(true);
                            }
                            break;
                        }
                    case EScriptWindowViewerMode.Text:
                        {
                            if (selectList)
                            {
                                viewerMode = EScriptWindowViewerMode.List;
                                hierarchyVar.TrySetValue(varValueJsonString, hierarchyVar.GetJsonType().ToVarType());
                            }
                            break;
                        }
                }
            }

            //更新
            if (update = UICommonFun.ButtonToggle(updateGUIContent, update, EditorStyles.miniButtonLeft, UICommonOption.WH24x16))
            {
                ApplyModify();
            }

            //应用
            if (GUILayout.Button(applyGUIContent, EditorStyles.miniButtonRight, UICommonOption.WH24x16))
            {
                hasChanged = true;
                ApplyModify();
            }

            //打开
            if (GUILayout.Button(keyGUIContent, UICommonOption.WH24x16))
            {
                HierarchyKeyExtensionViewerEditorWindow.OpenAndFocus();
            }

            EditorGUILayout.EndHorizontal();

            base.OnGUI();
        }

        /// <summary>
        /// 应用
        /// </summary>
        [Name("应用")]
        [Tip("将修改信息应用到当前正在编辑的变量对象")]
        [XCSJ.Attributes.Icon(EIcon.Submit)]
        private static XGUIContent applyGUIContent { get; } = new XGUIContent(typeof(HierarchyVarEditorWindow), nameof(applyGUIContent), true);

        /// <summary>
        /// 更新
        /// </summary>
        [Name("更新")]
        [Tip("将修改信息实时更新到当前正在编辑的变量对象")]
        [XCSJ.Attributes.Icon(EIcon.Update)]
        private static XGUIContent updateGUIContent { get; } = new XGUIContent(typeof(HierarchyVarEditorWindow), nameof(updateGUIContent), true);


        [Name("层级键扩展")]
        [Tip("打开层级键扩展查看器,同时关闭当前编辑窗口")]
        [XCSJ.Attributes.Icon(EIcon.Variable)]
        private static XGUIContent keyGUIContent { get; } = new XGUIContent(typeof(HierarchyVarEditorWindow), nameof(keyGUIContent), true);

        private bool update = true;

        /// <summary>
        /// 带滚动视图的GUI
        /// </summary>
        public override void OnGUIWithScrollView()
        {
            EditorGUI.BeginChangeCheck();
            switch (viewerMode)
            {
                case EScriptWindowViewerMode.List:
                    {

                        Draw(hierarchyVar, 0);
                        break;
                    }
                case EScriptWindowViewerMode.Text:
                    {
                        varValueJsonString = EditorGUILayout.TextArea(varValueJsonString, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                        break;
                    }
            }
            hasChanged = EditorGUI.EndChangeCheck() || hasChanged;
        }

        private void Draw(HierarchyVar hierarchyVar, int index)
        {
            UICommonFun.BeginHorizontal(index);

            //变量名
            EditorGUILayout.PrefixLabel(hierarchyVar.name);

            //类型
            var jsonType = hierarchyVar.GetJsonType();
            EditorGUILayout.LabelField(jsonType.ToVarType().ToGUIContent(), UICommonOption.Width20);

            //根据不同类型绘制不同的信息
            switch (jsonType)
            {
                case JsonType.Array:
                    {
                        EditorGUILayout.LabelField("新增成员:", UICommonOption.Width60);
                        addValueText = EditorGUILayout.TextField(addValueText);
                        if (GUILayout.Button(UICommonOption.Insert, UICommonOption.WH20x16))
                        {
                            CommonFun.FocusControl();
                            hierarchyVar.Add(addValueText);
                        }

                        break;
                    }
                case JsonType.Object:
                    {
                        EditorGUILayout.LabelField("新增键值:", UICommonOption.Width60);
                        addKeyText = EditorGUILayout.TextField(addKeyText);
                        if (formatKey) addKeyText = VariableHelper.Format(addKeyText);
                        if (GUILayout.Button(UICommonOption.Insert, EditorStyles.miniButtonRight, UICommonOption.WH20x16))
                        {
                            CommonFun.FocusControl();
                            if (!string.IsNullOrEmpty(addKeyText))
                            {
                                hierarchyVar.TryGetOrAddSetChild(addKeyText, "", EVarType.String, out _);
                            }
                        }

                        break;
                    }
                default:
                    {
                        EditorGUI.BeginChangeCheck();
                        var newText = EditorGUILayout.TextField(hierarchyVar.stringValue);
                        if (EditorGUI.EndChangeCheck())
                        {
                            hierarchyVar.TrySetValue(newText, EVarType.String);
                        }
                        break;
                    }
            }

            //类型切换
            EditorGUI.BeginChangeCheck();
            var newVarTye = (EVarType)UICommonFun.EnumPopup(jsonType.ToVarType(), UICommonOption.Width60);
            if (EditorGUI.EndChangeCheck())
            {
                hierarchyVar.TrySetVarType(newVarTye, hierarchyVar.stringValue);
                jsonType = hierarchyVar.GetJsonType();
            }

            //复制
            if (GUILayout.Button(UICommonOption.Copy, EditorStyles.miniButtonMid, UICommonOption.WH20x16))
            {
                CommonFun.CopyTextToClipboardForPC(hierarchyVar.varHierarchyString);
            }

            //删除
            var parent = hierarchyVar.parent;
            if (parent != null && GUILayout.Button(UICommonOption.Delete, EditorStyles.miniButtonRight, UICommonOption.WH20x16))
            {
                CommonFun.FocusControl();

                var tmpParent = parent;
                var tmpIndex = index;
                UICommonFun.DelayCall(() =>
                {
                    tmpParent.TryRemoveChild(tmpIndex, false, out _);
                    Repaint();
                });
            }

            UICommonFun.EndHorizontal();

            switch (jsonType)
            {
                case JsonType.Array:
                    {
                        int i = 0;
                        CommonFun.BeginLayout();
                        foreach (HierarchyVar hv in (IList)hierarchyVar)
                        {
                            Draw(hv, i++);
                        }
                        CommonFun.EndLayout();

                        break;
                    }
                case JsonType.Object:
                    {
                        int i = 0;
                        CommonFun.BeginLayout();
                        foreach (DictionaryEntry kv in (IDictionary)hierarchyVar)
                        {
                            Draw(kv.Value as HierarchyVar, i++);
                        }
                        CommonFun.EndLayout();
                        break;
                    }
            }
        }

        private void ApplyModify()
        {
            if (!hasChanged) return;
            hasChanged = false;

            var hierarchyVar = this.hierarchyVar;
            if (hierarchyVar == null || variableSerializedProperty == null || variableHandle == null) return;

            switch (viewerMode)
            {
                case EScriptWindowViewerMode.Text:
                    {
                        hierarchyVar.TrySetValue(varValueJsonString, hierarchyVar.GetJsonType().ToVarType());
                        break;
                    }
            }

            variableSerializedProperty.FindPropertyRelative(nameof(Variable._varType)).intValue = (int)hierarchyVar.GetVarType();
            variableSerializedProperty.FindPropertyRelative(nameof(Variable.value)).stringValue = hierarchyVar.stringValue;
            variableSerializedProperty.serializedObject.ApplyModifiedProperties();

            UICommonFun.DelayCall(variable.MarkDirty);
        }

        private void OnLostFocus()
        {
            ApplyModify();
            Close();
        }
    }
}
