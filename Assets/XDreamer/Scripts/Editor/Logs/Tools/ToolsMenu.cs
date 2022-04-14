using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.EditorTools;
using XCSJ.Extension.Logs.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.Windows;

namespace XCSJ.EditorExtension.Logs.Tools
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    public class ToolsMenu
    {
        /// <summary>
        /// 状态统计窗口
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("状态统计窗口")]
        [Tip("状态统计窗口")]
        [XCSJ.Attributes.Icon(EIcon.Window)]
        [Tool(XGUICategory.Window, nameof(XGUIManager), rootType = typeof(Canvas), index = 1, needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        //[Tool("常用", nameof(XGUIManager), rootType = typeof(Canvas), index = 1, needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        public static void CreateVerticalWindow(ToolContext toolContext)
        {
            var windowGameObject = XCSJ.EditorXGUI.ToolsMenu.LoadPrefab_DefaultXGUIPath("窗口模版.prefab");
            if(windowGameObject)
            {
                windowGameObject.XSetName("状态统计");
                var window = windowGameObject.GetComponent<UGUIWindow>();
                if (window)
                {
                    var stateStatistics = window.XGetOrAddComponent<StateStatistics>();
                    if (window.title)
                    {
                        var titleBar = window.title.GetComponent<TitleBar>();
                        if (titleBar && titleBar._title)
                        {
                            var text = titleBar._title.GetComponent<Text>();
                            if (text)
                            {
                                text.text = "状态统计";
                            }
                        }
                    }
                    if (window.content)
                    {
                        var textTransform = window.content.Find("Text");
                        if (textTransform)
                        {
                            var text = textTransform.GetComponent<Text>();
                            if (text)
                            {
                                stateStatistics.text = text;
                            }
                        }
                    }
                }
            }
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, windowGameObject);
        }
    }
}
