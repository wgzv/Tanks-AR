using System;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.CNScripts;
using XCSJ.PluginsCameras.Legacies.CNScripts;

namespace XCSJ.EditorCameras.Legacies.CNScripts
{
    /// <summary>
    /// 锁定相机(UGUI专用)脚本事件检查器
    /// </summary>
    [CustomEditor(typeof(LockCameraForUGUIScriptEvent))]
    [Obsolete("产品功能升级，不推荐用户再使用本功能组件，请使用新的功能组件替代！", false)]
    public class LockCameraForUGUIScriptEventInspector : BaseScriptEventInspector<LockCameraForUGUIScriptEvent, ELockCameraForUGUIScriptEventType, LockCameraForUGUIScriptEventSet>
    {
        /// <summary>
        /// 创建脚本事件
        /// </summary>
        [MenuItem(EditorScriptHelper.UGUIMenu + LockCameraForUGUIScriptEvent.Title, false)]
        public static void CreateScriptEvent()
        {
            CreateComponentWithRequireInternal<RectTransform>();
        }

        /// <summary>
        /// 验证创建脚本事件
        /// </summary>
        /// <returns></returns>
        [MenuItem(EditorScriptHelper.UGUIMenu + LockCameraForUGUIScriptEvent.Title, true)]
        public static bool ValidateCreateScriptEvent()
        {
            return ValidateCreateComponentWithRequireInternal<RectTransform>();
        }
    }
}
