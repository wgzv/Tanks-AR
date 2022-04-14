using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginRepairman.Devices;
using XCSJ.PluginSMS.Kernel;
using Tool = XCSJ.PluginRepairman.Devices.Tool;

namespace XCSJ.EditorRepairman.Inspectors
{
    /// <summary>
    /// 零件属性
    /// </summary>
    [CustomEditor(typeof(Part), true)]
    public class PartInspector : ItemInspector
    {

    }

    /// <summary>
    /// 模块属性
    /// </summary>
    [CustomEditor(typeof(Module), true)]
    public class ModuleInspector : PartInspector
    {
        private Module _module => target as Module;

        private bool expanded = true;

        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            expanded = UICommonFun.Foldout(expanded, "零件列表");

            if (expanded)
            {
                CommonFun.BeginLayout();
                {
                    TreeView.Draw((targetObject as Module).childrenParts, TreeView.DefaultDrawExpandedFunc, TreeView.DefaultPrefixFunc, (node, content) =>
                    {
                        var part = node as Part;

                        if (GUILayout.Button(content, GUI.skin.label))
                        {
                            node.OnClick();

                            Selection.activeObject = part;
                        }
                    });
                }
                CommonFun.EndLayout();
            }

            // 批量添加选中游戏对象为零件
            if (GUILayout.Button(new GUIContent("添加选中游戏对象为[零件]", EditorIconHelper.GetIconInLib(EIcon.Add)), UICommonOption.Height18))
            {
                CreateSelectionToComponent<Part>(() => Part.CreatePart(_module.parent));
            }

            // 批量添加选中游戏对象为零件
            if (GUILayout.Button(new GUIContent("添加选中游戏对象为[模块]", EditorIconHelper.GetIconInLib(EIcon.Add)), UICommonOption.Height18))
            {
                CreateSelectionToComponent<Module>(()=> Module.CreateModule(_module.parent));
            }
        }

        private void CreateSelectionToComponent<T>(Func<State> createFun) where T : Part
        {
            if (Selection.gameObjects.Length == 0)
            {
                Debug.LogWarning("请选择场景中的游戏对象!");
                return;
            }

            Selection.gameObjects.Foreach(go =>
            {
                var state = createFun.Invoke();
                if (state)
                {
                    var part = state.GetComponent<T>();
                    if (part)
                    {
                        part.go = go;
                        state.XSetName(go.name);
                    }
                }
            });
        }
    }

    [CustomEditor(typeof(Tool), true)]
    public class ToolInspector : ItemInspector
    {

    }
}
