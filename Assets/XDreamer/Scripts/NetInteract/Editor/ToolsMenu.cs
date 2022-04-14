﻿using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorTools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginNetInteract;

namespace XCSJ.EditorNetInteract
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    public static class ToolsMenu
    {
        /// <summary>
        /// 通用相机摇杆网络控制器
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Name("通用相机摇杆网络控制器")]
        [Tip("可创建控制通用相机网络控制器（非角色控制型）的摇杆型UGUI界面;")]
        [XCSJ.Attributes.Icon(EIcon.JoyStick)]
        [Tool(NetInteractHelper.Title, nameof(NetInteractManager), rootType = typeof(Canvas), groupRule = EToolGroupRule.None, needRootParentIsNull = true)]
        [RequireManager(typeof(NetInteractManager))]
        public static void CommonCameraJoystickNetController(ToolContext toolContext)
        {
            EditorXGUI.ToolsMenu.CreateUIToolAndStretchHV(toolContext, () => EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath(NetInteractHelper.Title + "/通用相机摇杆网络控制器.prefab"));
        }

        /// <summary>
        /// 服务器配置面板
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Name("服务器配置面板")]
        [Tip("查询服务器端IP；设置端口；启动和停止服务器")]
        [XCSJ.Attributes.Icon(EIcon.Config)]
        [Tool(NetInteractHelper.Title, nameof(NetInteractManager), rootType = typeof(Canvas), groupRule = EToolGroupRule.None, needRootParentIsNull = true)]
        [RequireManager(typeof(NetInteractManager))]
        public static void CreateServerConfigPanel(ToolContext toolContext)
        {
            EditorXGUI.ToolsMenu.CreateUIInCanvas(() => EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath(NetInteractHelper.Title + "/[网络交互]服务器配置面板.prefab"));
        }

        /// <summary>
        /// 客户端配置面板
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Name("客户端配置面板")]
        [Tip("配置服务器IP和端口；连接和断开服务器")]
        [XCSJ.Attributes.Icon(EIcon.Config)]
        [Tool(NetInteractHelper.Title, nameof(NetInteractManager), rootType = typeof(Canvas), groupRule = EToolGroupRule.None, needRootParentIsNull = true)]
        [RequireManager(typeof(NetInteractManager))]
        public static void CreateClientConfigPanel(ToolContext toolContext)
        {
            EditorXGUI.ToolsMenu.CreateUIInCanvas(() => EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath(NetInteractHelper.Title + "/[网络交互]客户端配置面板.prefab"));
        }
    }
}