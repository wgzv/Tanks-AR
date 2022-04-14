using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.CNScripts;
using XCSJ.EditorExtension.CNScripts.Windows;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.EditorExtension.CNScripts
{
    /// <summary>
    /// 编辑器脚本组手
    /// </summary>
    public class EditorScriptHelper
    {
        /// <summary>
        /// UGUI菜单
        /// </summary>
        public const string UGUIMenu = XDreamerMenu.ScriptEvent + "UGUI/";

        /// <summary>
        /// 输入菜单
        /// </summary>
        public const string InputMenu = XDreamerMenu.ScriptEvent + "输入/";

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            UICommonFun.onEditVariableValue += OnEditVariableValue;
        }

        private static void OnEditVariableValue(UnityEngine.Object obj, Variable variable, SerializedProperty variableSerializedProperty)
        {
            HierarchyVarEditorWindow.Open(obj, variable, variableSerializedProperty);
        }
    }
}
