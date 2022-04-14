﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginEasyAR;
using UnityEditor.Callbacks;
using XCSJ.EditorExtension.Base;
using UnityEditor.SceneManagement;
#if XDREAMER_EASYAR_4_0_1 || XDREAMER_EASYAR_3_0_1
using easyar;
#elif XDREAMER_EASYAR_2_3_0
using EasyAR;
#endif

namespace XCSJ.EditorEasyAR
{
    [CustomEditor(typeof(EasyARManager))]
    public class EasyARManagerInspector : BaseManagerInspector<EasyARManager>
    {
        #region 编译宏

        public static readonly Macro XDREAMER_EASYAR_4_0_1 = new Macro(nameof(XDREAMER_EASYAR_4_0_1), BuildTargetGroup.Standalone, BuildTargetGroup.iOS, BuildTargetGroup.Android);

        public static readonly Macro XDREAMER_EASYAR_3_0_1 = new Macro(nameof(XDREAMER_EASYAR_3_0_1), BuildTargetGroup.Standalone, BuildTargetGroup.iOS, BuildTargetGroup.Android);

        public static readonly Macro XDREAMER_EASYAR_2_3_0 = new Macro(nameof(XDREAMER_EASYAR_2_3_0), BuildTargetGroup.Standalone, BuildTargetGroup.iOS, BuildTargetGroup.Android);

        /// <summary>
        /// 初始化宏
        /// </summary>
        [Macro]
        public static void InitMacro()
        {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID
            if (TypeHelper.ExistsAndAssemblyFileExists("easyar.EasyARController", true, false))
            {
                XDREAMER_EASYAR_4_0_1.DefineIfNoDefined();
            }
            else if (TypeHelper.ExistsAndAssemblyFileExists("easyar.Engine", true, false))
            {
                XDREAMER_EASYAR_3_0_1.DefineIfNoDefined();
            }
            else if (TypeHelper.ExistsAndAssemblyFileExists("EasyAR.Engine", true, false))
            {
                XDREAMER_EASYAR_2_3_0.DefineIfNoDefined();
            }
            else
#endif
            {
                XDREAMER_EASYAR_2_3_0.UndefineWithSelectedBuildTargetGroup();
                XDREAMER_EASYAR_3_0_1.UndefineWithSelectedBuildTargetGroup();
                XDREAMER_EASYAR_4_0_1.UndefineWithSelectedBuildTargetGroup();
            }
        }

        #endregion

        public const string UnityPackagePath =
#if XDREAMER_EASYAR_2_3_0
            "Assets/XDreamer-ThirdPartyUnityPackage/EasyAR_SDK_2.3.0_Basic.unitypackage";
#elif XDREAMER_EASYAR_3_0_1
            "Assets/XDreamer-ThirdPartyUnityPackage/EasyARSense_3.0.1-final_Basic.unitypackage";
#else
            "Assets/XDreamer-ThirdPartyUnityPackage/EasyARSenseUnityPlugin_4.1.0.811.unitypackage";
#endif

        /// <summary>
        /// 初始化
        /// </summary>
        [InitializeOnLoadMethod]
        public static void Init()
        {
            InitMacro();
            PluginCommonUtilsRootInspector.onCreatedManager += (t) =>
            {
                if (t == typeof(EasyARManager))
                {
#if XDREAMER_EASYAR_2_3_0
                    EditorHelper.ImportPackageIfNeedWithDialog(XDREAMER_EASYAR_2_3_0, UnityPackagePath);
#elif XDREAMER_EASYAR_3_0_1
                    EditorHelper.ImportPackageIfNeedWithDialog(XDREAMER_EASYAR_3_0_1, UnityPackagePath);
#else
                    EditorHelper.ImportPackageIfNeedWithDialog(XDREAMER_EASYAR_4_0_1, UnityPackagePath);
#endif
                }
            };

            EditorSceneManager.sceneOpened += (scene, mode) =>
            {
                UICommonFun.DelayCall(() =>
                {
                    if (EasyARManager.instance)
                    {
#if XDREAMER_EASYAR_2_3_0
                        EditorHelper.ImportPackageIfNeedWithDialog(XDREAMER_EASYAR_2_3_0, UnityPackagePath);
#elif XDREAMER_EASYAR_3_0_1
                        EditorHelper.ImportPackageIfNeedWithDialog(XDREAMER_EASYAR_3_0_1, UnityPackagePath);
#else
                        EditorHelper.ImportPackageIfNeedWithDialog(XDREAMER_EASYAR_4_0_1, UnityPackagePath);
#endif
                    }
                });
            };
        }

        /// <summary>
        /// 二维码信息
        /// </summary>
#if XDREAMER_EASYAR_2_3_0
        public QRCodeScannerBehaviour qrCode;
#else
        public Component qrCode;
#endif

        public ScriptEasyAREvent easyAREvent;

#if XDREAMER_EASYAR_4_0_1
        public SurfaceTrackerFrameFilter surfaceTracker;
#endif

        /// <summary>
        /// 
        /// </summary>
        public ImageTargetMB[] markers;

        public override void OnBeforeVertical()
        {
#region 检测是否需要导入UnityPackage
#if XDREAMER_EASYAR_4_0_1
            EditorHelper.ImportPackageIfNeedWithDialog(XDREAMER_EASYAR_4_0_1, UnityPackagePath);
#elif XDREAMER_EASYAR_3_0_1
            EditorHelper.ImportPackageIfNeedWithButton(XDREAMER_EASYAR_3_0_1, UnityPackagePath);
#else
            EditorHelper.ImportPackageIfNeedWithButton(XDREAMER_EASYAR_2_3_0, UnityPackagePath);
#endif
#endregion
            base.OnBeforeVertical();
        }

        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(targetObject.easyAR):
                    {
                        if (!EditorGUILayout.Toggle((targetObject.easyAR && targetObject.easyAR.gameObject.activeSelf), GUILayout.Width(20)))
                        {
                            if (targetObject.easyAR) targetObject.easyAR.gameObject.SetActive(false);
                        }
                        else
                        {
                            if (!targetObject.easyAR)
                            {
                                memberProperty.objectReferenceValue = LoadEasyAR();
                            }
                            else
                            {
                                targetObject.easyAR.gameObject.SetActive(true);
                            }
                        }
                        break;
                    }
            }
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }

        public override void OnAfterVertical()
        {
#if XDREAMER_EASYAR_2_3_0
            #region 二维码扫描器

            if (!EditorGUILayout.Toggle("二维码扫描器", (qrCode && qrCode.gameObject.activeSelf)))
            {
                if (qrCode) qrCode.gameObject.SetActive(false);
            }
            else
            {
                if (!qrCode)
                {
                    if (targetObject.easyAR) qrCode = LoadQRCodeScanner(targetObject.easyAR.transform);
                }
                else
                {
                    qrCode.gameObject.SetActive(true);
                }
            }

            #endregion
#endif

#if XDREAMER_EASYAR_4_0_1
            #region 绘制开启表面跟踪

            if (!EditorGUILayout.Toggle(new GUIContent("是否开启表面跟踪", "打开开启表面跟踪"), (surfaceTracker && surfaceTracker.gameObject.activeSelf)))
            {
                if (surfaceTracker) surfaceTracker.gameObject.SetActive(false);
            }
            else
            {
                if (!surfaceTracker)
                {
                    surfaceTracker = LoadSurfaceTracker(targetObject.easyAR.transform).GetComponent<SurfaceTrackerFrameFilter>();
                    if (surfaceTracker) surfaceTracker.transform.localScale = Vector3.one;
                }
                else
                {
                    surfaceTracker.gameObject.SetActive(true);
                }
            }

            #endregion 绘制开启表面跟踪
#endif

            #region 脚本EasyAR事件

            if (!EditorGUILayout.Toggle(CommonFun.NameTooltip(typeof(ScriptEasyAREvent)), (easyAREvent && easyAREvent.enabled)))
            {
                
                if (easyAREvent) easyAREvent.enabled = false;
            }
            else
            {
                if (!easyAREvent)
                {
                    easyAREvent = targetObject.gameObject.AddComponent<ScriptEasyAREvent>();
                }
                else
                {
                    easyAREvent.enabled = true;
                }
            }

#endregion

#region 图片(Marker)目标识别

            EditorGUILayout.Separator();
            if (GUILayout.Button("添加 图片(Marker)目标识别"))
            {
                var imageMarker = LoadImageTargetMarker(targetObject.transform);
                if (imageMarker) imageMarker.transform.localScale = Vector3.one;
            }

            #endregion

#if XDREAMER_EASYAR_4_0_1
            #region 添加WorldRoot

            if ((surfaceTracker && surfaceTracker.gameObject.activeSelf) && GUILayout.Button("添加 世界根节点"))
            {
                var worldRoot = GameObject.Find("WorldRoot");
                if (!worldRoot)
                {
                    worldRoot = LoadWorldRoot();
                    if (worldRoot) worldRoot.transform.localScale = Vector3.one;
                }
                targetObject.easyAR.GetComponent<ARSession>().WorldRootController = worldRoot.GetComponent<WorldRootController>();
            }
            
            #endregion 添加WorldRoot
#endif

            base.OnAfterVertical();
        }

        public override void OnEnable()
        {
#if XDREAMER_EASYAR_4_0_1 || XDREAMER_EASYAR_3_0_1
            if (!targetObject.easyAR)
            {
                //先在当前场景中查找组件
                targetObject.easyAR = EasyARManager.InitEasyAR(null);

                //没找到，那么从预制件加载~
                if (!targetObject.easyAR) targetObject.easyAR = LoadEasyAR();
            }
            if (!targetObject.cameraDevice)
            {
                targetObject.cameraDevice = EasyARManager.InitCameraDevice(null);
            }
#elif XDREAMER_EASYAR_2_3_0
            if (!targetObject.easyAR)
            {
                //先在当前场景中查找组件
                targetObject.easyAR = EasyARManager.InitEasyAR(null);

                //没找到，那么从预制件加载~
                if (!targetObject.easyAR) targetObject.easyAR = LoadEasyAR();
            }
            if (!targetObject.cameraDevice)
            {
                targetObject.cameraDevice = EasyARManager.InitCameraDevice(null);
            }
            if (!qrCode)
            {
                qrCode = GameObject.FindObjectOfType<QRCodeScannerBehaviour>();
            }
#endif
            if (!easyAREvent)
            {
                easyAREvent = GameObject.FindObjectOfType<ScriptEasyAREvent>();
            }
#if XDREAMER_EASYAR_4_0_1
            if (!surfaceTracker)
            {
                surfaceTracker = targetObject.easyAR.GetComponentInChildren<SurfaceTrackerFrameFilter>(true);
            }
#endif
            base.OnEnable();
        }
#if XDREAMER_EASYAR_4_0_1
        private easyar.EasyARController LoadEasyAR()
        {
            var go = UICommonFun.LoadAndInstantiateFromAssets("Assets/EasyAR/Prefabs/Composites/EasyAR_ImageTracker-1.prefab", "EasyAR_Startup", targetObject.transform);
            if (!go) return null;
            go.transform.localScale = Vector3.one;
            GameObject mainCamera = GameObject.Find("Main Camera");
            if (!mainCamera) {
                mainCamera = new GameObject("Main Camera");
                mainCamera.tag = "MainCamera";
                mainCamera.AddComponent<Camera>();
            }
            mainCamera.transform.parent = go.transform;
            Camera camera = mainCamera.GetComponent<Camera>();
            if (camera) camera.clearFlags = CameraClearFlags.SolidColor;
            return go.GetComponent<easyar.EasyARController>();
        }

        private Component LoadQRCodeScanner(Transform parent) => null;

        private GameObject LoadSurfaceTracker(Transform parent)
        {
            return UICommonFun.LoadAndInstantiateFromAssets("Assets/EasyAR/Prefabs/Primitives/SurfaceTracker.prefab", "SurfaceTracker", parent);
        }

        private GameObject LoadWorldRoot()
        {
            return UICommonFun.LoadAndInstantiateFromAssets("Assets/EasyAR/Prefabs/Primitives/WorldRoot.prefab", "WorldRoot", targetObject.transform);
        }

#elif XDREAMER_EASYAR_3_0_1
        private EasyARBehaviour LoadEasyAR()
        {
            var path = UICommonFun.GetAssetsPath(EFolder.ThirdParty);
            var go = UICommonFun.LoadAndInstantiateFromAssets(path + "/EasyAR/Prefabs/EasyAR_Startup.prefab", "EasyAR_Startup", targetObject.transform);
            if (!go) return null;
            return go.GetComponent<EasyARBehaviour>();
        }

        private Component LoadQRCodeScanner(Transform parent) => null;
#elif XDREAMER_EASYAR_2_3_0
        private EasyARBehaviour LoadEasyAR()
        {
            var go = UICommonFun.LoadAndInstantiateFromAssets("Assets/EasyAR/Prefabs/EasyAR_Startup.prefab", "EasyAR_Startup", targetObject.transform);
            if (!go) return null;
            return go.GetComponent<EasyARBehaviour>();
        }

        private QRCodeScannerBehaviour LoadQRCodeScanner(Transform parent)
        {
            var go = UICommonFun.LoadAndInstantiateFromAssets("Assets/EasyAR/Prefabs/Primitives/QRCodeScanner.prefab", "QRCodeScanner", parent);
            if (!go) return null;
            return go.GetComponent<QRCodeScannerBehaviour>();
        }

#else
        private Component LoadEasyAR() => null;

        private Component LoadQRCodeScanner(Transform parent) => null;
#endif

        private GameObject LoadImageTargetMarker(Transform parent)
        {
            var path = UICommonFun.GetAssetsPath(EFolder.ThirdParty);
            return UICommonFun.LoadAndInstantiateFromAssets(path + "/EasyAR/Prefabs/ImageTargetOfMarker.prefab", "ImageTargetOfMarker", parent);
        }
    }
}
