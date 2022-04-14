using System;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace XCSJ.EditorExtension.Base.ProjectSettings
{
    /// <summary>
    /// 标签管理器
    /// </summary>
    public static class XProjectSetting
    {
        /// <summary>
        /// 工程设置资产路径
        /// </summary>
        public const string AssetPath = "ProjectSettings/ProjectSettings.asset";

        /// <summary>
        /// 工程设置资产对象
        /// </summary>
        public static UnityEngine.Object asset => AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(AssetPath);

        /// <summary>
        /// 工程设置序列化对象
        /// </summary>
        public static SerializedObject serializedObject => new SerializedObject(asset);

        /// <summary>
        /// 输入系统版本类型
        /// </summary>
        public enum EInputHander
        {
            /// <summary>
            /// 旧版输入系统
            /// </summary>
            InputManager = 0,

            /// <summary>
            /// 新版输入系统
            /// </summary>
            InputSystem,

            /// <summary>
            /// 同时使用新版和旧版输入系统
            /// </summary>
            Both,
        }

        /// <summary>
        /// 获取输入系统类型
        /// </summary>
        /// <returns></returns>
        public static EInputHander GetActiveInputHandler()
        {
            var so = serializedObject;

#if UNITY_2020_2_OR_NEWER

            return (EInputHander)(so.FindProperty("activeInputHandler").intValue);

#else

            var m_EnableInputSystem = so.FindProperty("enableNativePlatformBackendsForNewInputSystem");
            var m_DisableInputManager = so.FindProperty("disableOldInputManagerSupport");

            if (!m_DisableInputManager.boolValue && !m_EnableInputSystem.boolValue)
            {
                return EInputHander.InputManager;
            }
            else if (m_DisableInputManager.boolValue && m_EnableInputSystem.boolValue)
            {
                return EInputHander.InputSystem;
            }
            else
            {
                return EInputHander.Both;
            }

#endif
        }

        /// <summary>
        /// 设置输入系统类型
        /// </summary>
        /// <param name="inputHander"></param>
        /// <param name="displayApplyDialog"></param>
        public static void SetActiveInputHandler(EInputHander inputHander, bool displayApplyDialog, string introduction = "")
        {
            var so = serializedObject;

#if UNITY_2020_2_OR_NEWER

            var activeInputHandler = so.FindProperty("activeInputHandler");
            
            switch (inputHander)
            {
                case EInputHander.InputManager:
                    {
                        if (activeInputHandler.intValue == 0) return;
                        activeInputHandler.intValue = 0;
                        break;
                    }
                case EInputHander.InputSystem:
                    {
                        if (activeInputHandler.intValue == 1) return;
                        activeInputHandler.intValue = 1;
                        break;
                    }
                case EInputHander.Both:
                    {
                        if (activeInputHandler.intValue == 2) return;
                        activeInputHandler.intValue = 2;
                        break;
                    }
            }

#else

            var m_EnableInputSystem = so.FindProperty("enableNativePlatformBackendsForNewInputSystem");
            var m_DisableInputManager = so.FindProperty("disableOldInputManagerSupport");

            switch (inputHander)
            {
                case EInputHander.InputManager:
                    {
                        if (!m_DisableInputManager.boolValue && !m_EnableInputSystem.boolValue) return;

                        m_DisableInputManager.boolValue = false;
                        m_EnableInputSystem.boolValue = false;
                        break;
                    }
                case EInputHander.InputSystem:
                    {
                        if (m_DisableInputManager.boolValue && m_EnableInputSystem.boolValue) return;

                        m_DisableInputManager.boolValue = true;
                        m_EnableInputSystem.boolValue = true;
                        break;
                    }
                case EInputHander.Both:
                    {
                        if (!m_DisableInputManager.boolValue && m_EnableInputSystem.boolValue) return;

                        m_DisableInputManager.boolValue = false;
                        m_EnableInputSystem.boolValue = true;
                        break;
                    }
                default: return;
            }

#endif
            if (displayApplyDialog)
            {
                if (!EditorUtility.DisplayDialog("需要重启Unity编辑器", introduction + ", Unity编辑器重启, 输入系统设置才能生效。", "应用", "取消")) return;
            }

            so.ApplyModifiedProperties();
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorApplication.OpenProject(Environment.CurrentDirectory);
            }           
        }
    }
}
