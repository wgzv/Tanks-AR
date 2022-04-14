﻿using System.Linq;
//using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorTools;
using XCSJ.EditorXGUI;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginsCameras;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Controllers;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.EditorCameras.Tools
{
    /// <summary>
    /// 工具库相机菜单
    /// </summary>
    public class ToolsMenu
    {
        #region 相机

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static CameraController CreateCameraController(string name, bool multiGameObjectMode = true) => CameraHelperExtension.CreateCameraController(name, multiGameObjectMode);

        /// <summary>
        /// 飞行相机
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Name("飞行相机")]
        [XCSJ.Attributes.Icon(nameof(FlyCamera))]
        [Tool(CameraHelperExtension.CategoryName, nameof(CameraManager), rootType = typeof(CameraManager), groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(CameraManager))]
        public static CameraController FlyCamera(ToolContext toolContext) => CreateFlyCamera(toolContext);

        /// <summary>
        /// 创建飞行相机
        /// </summary>
        /// <param name="toolContext"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static CameraController CreateFlyCamera(ToolContext toolContext, string name = "飞行相机")
        {
            var cameraController = CameraHelperExtension.CreateFlyCamera(name);
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, cameraController.gameObject);
            return cameraController;
        }

        /// <summary>
        /// 定点相机
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Name("定点相机")]
        [XCSJ.Attributes.Icon(nameof(FixedCamera))]
        [Tool(CameraHelperExtension.CategoryName, nameof(CameraManager), rootType = typeof(CameraManager), groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(CameraManager))]
        public static void FixedCamera(ToolContext toolContext)
        {
            var cameraController = CameraHelperExtension.CreateFixedCamera("定点相机");
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, cameraController.gameObject);
        }

        /// <summary>
        /// 定点注视相机
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Name("定点注视相机")]
        [XCSJ.Attributes.Icon(nameof(FixedLookAtCamera))]
        [Tool(CameraHelperExtension.CategoryName, nameof(CameraManager), rootType = typeof(CameraManager), groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(CameraManager))]
        public static void FixedLookAtCamera(ToolContext toolContext)
        {
            var cameraController = CameraHelperExtension.CreateFixedLookAtCamera("定点注视相机");
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, cameraController.gameObject);
        }

        /// <summary>
        /// 跟随相机
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Name("跟随相机")]
        [XCSJ.Attributes.Icon(nameof(FollowCamera))]
        [Tool(CameraHelperExtension.CategoryName, nameof(CameraManager), rootType = typeof(CameraManager), groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(CameraManager))]
        public static void FollowCamera(ToolContext toolContext)
        {
            var cameraController = CameraHelperExtension.CreateFollowCamera("跟随相机");
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, cameraController.gameObject);
        }

        /// <summary>
        /// 绕物相机
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Name("绕物相机")]
        [XCSJ.Attributes.Icon(nameof(AroundCamera))]
        [Tool(CameraHelperExtension.CategoryName, nameof(CameraManager), rootType = typeof(CameraManager), groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(CameraManager))]
        public static void AroundCamera(ToolContext toolContext)
        {
            var cameraController = CameraHelperExtension.CreateAroundCamera("绕物相机");
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, cameraController.gameObject);
        }

        /// <summary>
        /// 平移绕物相机
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Name("平移绕物相机")]
        [XCSJ.Attributes.Icon(nameof(MoveAroundCamera))]
        [Tool(CameraHelperExtension.CategoryName, nameof(CameraManager), rootType = typeof(CameraManager), groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(CameraManager))]
        public static void MoveAroundCamera(ToolContext toolContext)
        {
            var cameraController = CameraHelperExtension.CreateMoveAroundCamera("平移绕物相机");
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, cameraController.gameObject);
        }

        /// <summary>
        /// 角色相机
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Name("角色相机")]
        [XCSJ.Attributes.Icon(nameof(WalkCamera))]
        [Tool(CameraHelperExtension.CategoryName, nameof(CameraManager), rootType = typeof(CameraManager), groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(CameraManager))]
        public static void CharacterCamera(ToolContext toolContext)
        {
            var (characterController, cameraController) = CameraHelperExtension.CreateCharacterCamera("角色相机");
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, characterController.gameObject);
        }

        /// <summary>
        /// 行走相机
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Name("行走相机")]
        [Tip("特殊的角色相机，交互控制方式与旧版行走相机相似；")]
        [XCSJ.Attributes.Icon(nameof(WalkCamera))]
        [Tool(CameraHelperExtension.CategoryName, nameof(CameraManager), rootType = typeof(CameraManager), groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(CameraManager))]
        public static void WalkCamera(ToolContext toolContext)
        {
            var (characterController, cameraController) = CameraHelperExtension.CreateWalkCamera("行走相机");
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, characterController.gameObject);
        }

        /// <summary>
        /// 平移飞行相机
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Name("平移飞行相机")]
        [Tip("特殊的飞行相机，交互控制方式与Dota、魔兽争霸、红警等游戏的控制方式相似；")]
        [XCSJ.Attributes.Icon(nameof(FlyCamera))]
        [Tool(CameraHelperExtension.CategoryName, nameof(CameraManager), rootType = typeof(CameraManager), groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(CameraManager))]
        public static void MoveFlyCamera(ToolContext toolContext)
        {
            var cameraController = CameraHelperExtension.CreateMoveFlyCamera("平移飞行相机");
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, cameraController.gameObject);
        }

        #endregion

        #region 相机相关界面

        /// <summary>
        /// 通用相机摇杆控制器
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Name("通用相机摇杆控制器")]
        [Tip("可创建控制通用相机控制器（非角色控制型）的摇杆型UGUI界面;")]
        [XCSJ.Attributes.Icon(EIcon.Camera)]
        [Tool(XGUICategory.View, nameof(CameraManager), rootType = typeof(Canvas), groupRule = EToolGroupRule.None, needRootParentIsNull = true)]
        [Tool(CameraHelperExtension.ControllersCategoryName, nameof(CameraManager), rootType = typeof(Canvas), groupRule = EToolGroupRule.None, needRootParentIsNull = true)]
        [RequireManager(typeof(CameraManager))]
        public static void CommonCameraJoystickController(ToolContext toolContext)
        {
            EditorXGUI.ToolsMenu.CreateUIToolAndStretchHV(toolContext, () => EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath("常用/通用相机摇杆控制器.prefab"));
        }

        /// <summary>
        /// 角色相机摇杆控制器
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Name("角色相机摇杆控制器")]
        [Tip("可创建控制专用于角色相机控制器的摇杆型UGUI界面;")]
        [XCSJ.Attributes.Icon(EIcon.Camera)]
        [Tool(XGUICategory.View, nameof(CameraManager), rootType = typeof(Canvas), groupRule = EToolGroupRule.None, needRootParentIsNull = true)]
        [Tool(CameraHelperExtension.ControllersCategoryName, nameof(CameraManager), rootType = typeof(Canvas), groupRule = EToolGroupRule.None, needRootParentIsNull = true)]
        [RequireManager(typeof(CameraManager))]
        public static void CharacterCameraJoystickController(ToolContext toolContext)
        {
            EditorXGUI.ToolsMenu.CreateUIToolAndStretchHV(toolContext, () => EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath("常用/角色相机摇杆控制器.prefab"));
        }

        /// <summary>
        /// 相机列表窗口
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Name("相机列表")]
        [Tip("可创建控制专用于角色相机控制器的摇杆型UGUI界面;")]
        [XCSJ.Attributes.Icon(EIcon.Camera)]
        [Tool(XGUICategory.Window, nameof(CameraManager), rootType = typeof(Canvas), groupRule = EToolGroupRule.None, needRootParentIsNull = true)]
        [Tool("常用", rootType = typeof(Canvas), groupRule = EToolGroupRule.None, needRootParentIsNull = true)]
        [Tool(CameraHelperExtension.ControllersCategoryName, rootType = typeof(Canvas), groupRule = EToolGroupRule.None, needRootParentIsNull = true)]
        [RequireManager(typeof(CameraManager))]
        public static void CameraControllerListWindow(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext,  EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath("常用/相机列表窗口.prefab"));
        }

        #endregion
    }
}
