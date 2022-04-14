using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorTools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginSMS;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.EditorSMS.States.TimeLine
{
    public class CreateTimeUITool 
    {
        #region 时间轴播放器界面

        /// <summary>
        /// 创建时间轴播放器
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("时间轴播放器")]
        [XCSJ.Attributes.Icon(EIcon.Play)]
        [Tool(XGUICategory.Window, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager), typeof(SMSManager))]
        public static GameObject CreateTimeLinePlayer(ToolContext toolContext)
        {
            var go = EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath("状态机系统/时间轴播放器界面.prefab");
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
            return go;
        }

        #endregion
    }
}
