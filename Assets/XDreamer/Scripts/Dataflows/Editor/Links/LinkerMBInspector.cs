using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginDataflows.Links;

namespace XCSJ.EditorDataflows.Links
{
    /// <summary>
    /// 数据连接器检查器
    /// </summary>
    [CustomEditor(typeof(LinkerMB))]
    public class LinkerMBInspector : MBInspector<LinkerMB>
    {
        private XGUIContent spnContent { get; } = new XGUIContent(typeof(DataLinker), nameof(DataLinker._sourceBinderAlias));

        private XGUIContent targetContent { get; } = new XGUIContent(typeof(DataLinker), nameof(DataLinker._targetBinderAlias));

        private XGUIContent linkModeContent { get; } = new XGUIContent(typeof(DataLinker), nameof(DataLinker._dataLinkMode));

        private static XGUIStyle boxStyle { get; } = new XGUIStyle("box");

        private SerializedProperty _linkerSP = null;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            _linkerSP = serializedObject.FindProperty(nameof(LinkerMB._linkerList)).FindPropertyRelative(nameof(DataLinkerList._linkers));
        }

        /// <summary>
        /// 绘制属性
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberProperty"></param>
        /// <param name="arrayElementInfo"></param>
        /// <returns></returns>
        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(LinkerMB._linkerList):
                    {
                        DrawDataTitle();
                        DrawDataBody();
                        return false;
                    }
                default: break;
            }
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }

        private void DrawDataTitle()
        {
            GUILayout.BeginHorizontal(boxStyle);
            {
                EditorGUILayout.LabelField("NO.", UICommonOption.Width24);
                GUILayout.Label(spnContent);
                GUILayout.Label(linkModeContent, UICommonOption.Width48);
                GUILayout.Label(targetContent);

                DrawOperationButton(_linkerSP.arraySize);
            }
            GUILayout.EndHorizontal();
        }

        private void DrawDataBody()
        {
            if (_linkerSP.arraySize == 0) return;

            // 获取别名列表
            var aliasArray = AliasCache.instance.GetAliases(targetObject._aliasDataSource, targetObject.gameObject);
            var aliasDisable = aliasArray.Length == 0;

            GUILayout.BeginVertical(boxStyle, GUILayout.ExpandWidth(true));
            {
                for (int i = 0; i < _linkerSP.arraySize; i++)
                {
                    var sp = _linkerSP.GetArrayElementAtIndex(i);
                    UICommonFun.BeginHorizontal(i);
                    {
                        var orgColor = GUI.color;
                        if (Application.isPlaying)
                        {
                            GUI.color = targetObject._linkerList._linkers[i].isBind ? orgColor : Color.red;
                        }
                        EditorGUILayout.LabelField((i+1).ToString(), UICommonOption.Width24);

                        // 源别名
                        var sourceAliasSp = sp.FindPropertyRelative(nameof(DataLinker._sourceBinderAlias));
                        EditorGUI.BeginDisabledGroup(aliasDisable);
                        sourceAliasSp.stringValue = UICommonFun.Popup(sourceAliasSp.stringValue, aliasArray, false);
                        EditorGUI.EndDisabledGroup();

                        // 连接模式
                        EditorGUILayout.PropertyField(sp.FindPropertyRelative(nameof(DataLinker._dataLinkMode)), GUIContent.none, UICommonOption.Width48);

                        // 目标别名
                        var targetAliasSp = sp.FindPropertyRelative(nameof(DataLinker._targetBinderAlias));
                        EditorGUI.BeginDisabledGroup(aliasDisable);
                        targetAliasSp.stringValue = UICommonFun.Popup(targetAliasSp.stringValue, aliasArray, false);
                        EditorGUI.EndDisabledGroup();

                        // 添加与移除
                        DrawOperationButton(i);

                        GUI.color = orgColor;
                    }
                    UICommonFun.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
        }

        private void DrawOperationButton(int index)
        {
            if (GUILayout.Button(new GUIContent("", EditorIconHelper.GetIconInLib(EIcon.Add)), EditorStyles.miniButtonLeft, UICommonOption.WH24x16))
            {
                _linkerSP.InsertArrayElementAtIndex(index);
                _linkerSP.serializedObject.ApplyModifiedProperties();
            }
            if (GUILayout.Button(new GUIContent("", EditorIconHelper.GetIconInLib(EIcon.Delete)), EditorStyles.miniButtonRight, UICommonOption.WH24x16))
            {
                _linkerSP.DeleteArrayElementAtIndex(index);
                _linkerSP.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
