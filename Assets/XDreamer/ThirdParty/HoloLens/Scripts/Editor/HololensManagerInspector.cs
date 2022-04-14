using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension;
using XCSJ.Helper;
using XCSJ.EditorExtension.Base;
using XCSJ.PluginCommonUtils;
using UnityEditor.SceneManagement;

#if XDREAMER_HOLOLENS
using HoloToolkit.Unity;
#endif

namespace XCSJ.PluginHoloLens
{
    [CustomEditor(typeof(HoloLensManager))]
    public class HoloLensManagerInspector : BaseManagerInspector<HoloLensManager>
    {
        #region 编译宏

        public static readonly Macro XDREAMER_HOLOLENS = new Macro(nameof(XDREAMER_HOLOLENS), BuildTargetGroup.Standalone, BuildTargetGroup.WSA, BuildTargetGroup.iOS);

        /// <summary>
        /// HoloLens一代SDK
        /// </summary>
        public static readonly Macro XDREAMER_HOLOTOOLKIT_2017_4_3_0_REFRESH = new Macro(nameof(XDREAMER_HOLOTOOLKIT_2017_4_3_0_REFRESH), BuildTargetGroup.Standalone, BuildTargetGroup.WSA, BuildTargetGroup.iOS);

        [InitializeOnLoadMethod]
        [Macro]
        public static void InitMacro()
        {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WSA || UNITY_IOS 
            var type = TypeHelper.GetType("HoloToolkit.Unity.InputModule.MixedRealityTeleport");
            if (type != null && FileHelper.Exists(Application.dataPath + @"/HoloToolkit/Input/Scripts/Utilities/Managers/MixedRealityTeleport.cs"))
            {
                try
                {
                    XDREAMER_HOLOLENS.DefineIfNoDefined();
                    if (TypeHelper.GetType("HoloToolkit.Unity.InputModule.InputManager") !=null)
                    {
                        XDREAMER_HOLOTOOLKIT_2017_4_3_0_REFRESH.DefineIfNoDefined();
                    }
                }
                catch
                {
                    XDREAMER_HOLOLENS.UndefineWithSelectedBuildTargetGroup();
                    XDREAMER_HOLOTOOLKIT_2017_4_3_0_REFRESH.UndefineWithSelectedBuildTargetGroup();
                }
            }
            else
#endif
            {
                XDREAMER_HOLOLENS.UndefineWithSelectedBuildTargetGroup();
                XDREAMER_HOLOTOOLKIT_2017_4_3_0_REFRESH.UndefineWithSelectedBuildTargetGroup();
            }
        }

        #endregion

        public const string UnityPackagePath = "Assets/XDreamer-ThirdPartyUnityPackage/HoloToolkit-Unity-2017.4.3.0-Refresh.unitypackage";

        [InitializeOnLoadMethod]
        public static void Init()
        {
            PluginCommonUtilsRootInspector.onCreatedManager += (t) =>
            {
                if (t == typeof(HoloLensManager))
                {
                    ////启用 允许非安全代码
                    //if (!PlayerSettings.allowUnsafeCode)
                    //{
                    //    if (EditorUtility.DisplayDialog("库缺失", "若期望使用[" + CommonFun.Name(typeof(HoloLensManager)) + "]功能\n需将PlayerSettings中allowUnsafeCode启用.\n\n是否启用'允许非安全代码'(allowUnsafeCode)?", "确认启用", "取消"))
                    //    {
                    //        PlayerSettings.allowUnsafeCode = true;
                    //    }
                    //}

                    EditorHelper.ImportPackageIfNeedWithDialog(XDREAMER_HOLOLENS, UnityPackagePath);                  
                }
            };

            EditorSceneManager.sceneOpened += (scene, mode) =>
            {
                UICommonFun.DelayCall(() =>
                {
                    if (HoloLensManager.instance)
                    {
                        EditorHelper.ImportPackageIfNeedWithDialog(XDREAMER_HOLOLENS, UnityPackagePath);
                    }
                });
            };
        }

        public override void OnBeforeVertical()
        {
            #region 检测是否需要导入UnityPackage

            if (!PlayerSettings.allowUnsafeCode)
            {
                var bk = GUI.backgroundColor;
                GUI.backgroundColor = XDreamerBaseOption.weakInstance.errorColor;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(CommonFun.TempContent("允许非安全代码", "若期望使用[" + CommonFun.Name(typeof(HoloLensManager)) + "]功能,需将PlayerSettings中allowUnsafeCode启用"));
                if (GUILayout.Button(CommonFun.TempContent("启用", "启用'允许非安全代码'(allowUnsafeCode)")))
                {
                    PlayerSettings.allowUnsafeCode = true;
                }
                EditorGUILayout.EndHorizontal();
                GUI.backgroundColor = bk;
            }                

            EditorHelper.ImportPackageIfNeedWithButton(XDREAMER_HOLOLENS, UnityPackagePath);

            #endregion
            base.OnBeforeVertical();
        }

        private const string spatialPutManagerPrefabPath = "Assets/" + Product.Name + "ThirdParty/HoloLens/Prefabs/SpatialPutManager.prefab";

        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

#if XDREAMER_HOLOLENS 
            if (GUILayout.Button("工程配置"))
            {
                AutoConfigureMenu.ShowProjectSettingsWindow();
            }

            if (GUILayout.Button("场景配置"))
            {
                AutoConfigureMenu.ShowSceneSettingsWindow();
            }

            if (GUILayout.Button("UWP权限配置"))
            {
                AutoConfigureMenu.ShowCapabilitySettingsWindow();
            }

            if (GUILayout.Button("创建语音识别模块"))
            {
                var obj = CommonOperation.FindOrCreateSpeechInputSource();
                if(obj) EditorGUIUtility.PingObject(obj);
            }

            if (GUILayout.Button("添加空间映射模块"))
            {
                var hm = CommonOperation.GetHoloLensManager();
                if (hm)
                {
                    var go = UICommonFun.LoadAndInstantiateFromAssets<GameObject>(spatialPutManagerPrefabPath);
                    if (go)
                    {
                        go.transform.SetParent(hm.transform);

                        EditorGUIUtility.PingObject(go);
                    }
                }
            }
#endif
        }
    }
}

