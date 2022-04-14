using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;

namespace XCSJ.PluginTools.SelectionUtils
{
    /// <summary>
    /// 选择集窗口通过IMGUI
    /// </summary>
    [Name("选择集窗口通过IMGUI")]
    public class SelectionWindowByIMGUI : ToolMB
    {
        /// <summary>
        /// 选择集窗口
        /// </summary>
        [Name("选择集窗口")]
        public SelectionWindow _selectionWindow = new SelectionWindow();

        public override void OnEnable()
        {
            base.OnEnable();
            Selection.selectionChanged += OnSelectionChanged;
            OnSelectionChanged(null, false);
            _selectionWindow.SetWindowAligin();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            Selection.selectionChanged -= OnSelectionChanged;
        }

        protected void OnSelectionChanged(GameObject[] oldSelections, bool flag)
        {
            _selectionWindow.gameObjects = Selection.selections.Where(go => go);
        }

        /// <summary>
        /// 绘制GUI
        /// </summary>
        public void OnGUI()
        {
            _selectionWindow.OnGUI();
        }
    }

    /// <summary>
    /// 选择集窗口
    /// </summary>
    [Serializable]
    public class SelectionWindow : BaseGUIWindow
    {
        public override bool autoLayout => true;

        /// <summary>
        /// 游戏对象列表
        /// </summary>
        public IEnumerable<GameObject> gameObjects { get; internal set; }

        protected override void OnDrawContentLayout()
        {
            if (gameObjects == null) return;
            foreach(var go in gameObjects)
            {
                if(go && GUILayout.Button(go.name))
                {
                    Debug.Log(CommonFun.GameObjectToStringWithoutAlias(go));
#if UNITY_EDITOR
                    UnityEditor.EditorGUIUtility.PingObject(go);
#endif
                }
            }
        }
    }
}
