using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

#if XDREAMER_STEAMVR
using Valve.VR;
#endif

using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.Helper;
using XCSJ.PluginHTCVive;

namespace XCSJ.EditorHTCVive
{
    [CustomEditor(typeof(HTCViveManager))]
    public class HTCViveManagerInspector : BaseManagerInspector<HTCViveManager>
    {
        #region 编译宏

        public static readonly Macro XDREAMER_STEAMVR = new Macro(nameof(XDREAMER_STEAMVR), BuildTargetGroup.Standalone);
        public static readonly Macro XDREAMER_STEAMVR_INPUT = new Macro(nameof(XDREAMER_STEAMVR_INPUT), BuildTargetGroup.Standalone);

        [InitializeOnLoadMethod]
        [Macro]
        public static void InitMacro()
        {
#if UNITY_EDITOR || UNITY_STANDALONE 
            var type = TypeHelper.GetType("Valve.VR.SteamVR_Action");
            if (type != null && FileHelper.Exists(Application.dataPath + @"/SteamVR/Input/SteamVR_Action.cs"))
            {
                try
                {
                    XDREAMER_STEAMVR.DefineIfNoDefined();

                    if (TypeHelper.GetType("Valve.VR.SteamVR_Actions") != null)
                    {
                        XDREAMER_STEAMVR_INPUT.DefineIfNoDefined();
                    }
                }
                catch
                {
                    XDREAMER_STEAMVR.UndefineWithSelectedBuildTargetGroup();
                    XDREAMER_STEAMVR_INPUT.UndefineWithSelectedBuildTargetGroup();
                }
            }
            else
#endif
            {
                XDREAMER_STEAMVR.UndefineWithSelectedBuildTargetGroup();
                XDREAMER_STEAMVR_INPUT.UndefineWithSelectedBuildTargetGroup();
            }
        }

        #endregion

        public const string UnityPackagePath = "Assets/XDreamer-ThirdPartyUnityPackage/SteamVR Plugin.unitypackage";

        [InitializeOnLoadMethod]
        public static void Init()
        {
            PluginCommonUtilsRootInspector.onCreatedManager += (t) =>
            {
                if (t == typeof(HTCViveManager))
                {
                    EditorHelper.ImportPackageIfNeedWithDialog(XDREAMER_STEAMVR, UnityPackagePath);
                }
            };

            EditorSceneManager.sceneOpened += (scene, mode) =>
            {
                UICommonFun.DelayCall(() =>
                {
                    if (HTCViveManager.instance)
                    {
                        EditorHelper.ImportPackageIfNeedWithDialog(XDREAMER_STEAMVR, UnityPackagePath);
                    }
                });
            };
        }

        public override void OnBeforeVertical()
        {
            #region 检测是否需要导入UnityPackage

            EditorHelper.ImportPackageIfNeedWithButton(XDREAMER_STEAMVR, UnityPackagePath);

            #endregion

#if XDREAMER_STEAMVR && !XDREAMER_STEAMVR_INPUT
            UICommonFun.RichHelpBox("未找到有效的StreamVR输入系统，请确认执行菜单[Window/StreamVR Input]命令并同意使用示例文件！", MessageType.Error);
#endif

            base.OnBeforeVertical();
        }
    }
}
