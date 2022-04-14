using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.CNScripts.NGUI;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.CNScripts.NGUI
{
    /// <summary>
    /// NGUI触发脚本事件编辑Inspector窗口，为ScriptEvent组件的编辑窗口；
    /// </summary>
    [CanEditMultipleObjects, CustomEditor(typeof(NGUITriggerScriptEvent))]
    [Obsolete("因NGUI不再提供更新,本组件不推荐用户使用")]
    public class NGUITriggerScriptEventInspector : BaseScriptEventInspector<NGUITriggerScriptEvent, ENGUITriggerScriptEventType, NGUITriggerScriptEventSet>
    {
        //[MenuItem(XDreamerMenu.ScriptEvent + "脚本NGUI触发事件", false)]
        public static void CreateScriptEvent()
        {
            CreateComponentInternal();
        }

        //[MenuItem(XDreamerMenu.ScriptEvent + "脚本NGUI触发事件", true)]
        public static bool ValidateCreateScriptEvent()
        {
            return ValidateCreateComponentInternal();
        }
    }
}
