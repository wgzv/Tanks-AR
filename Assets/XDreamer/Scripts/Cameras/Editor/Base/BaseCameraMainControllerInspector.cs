using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.Controllers;
using XCSJ.EditorExtension.Base.XUnityEditor;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Tools.Controllers;

namespace XCSJ.EditorCameras.Base
{
    /// <summary>
    /// 基础相机主控制器检查器
    /// </summary>
    [CustomEditor(typeof(BaseCameraMainController), true)]
    public class BaseCameraMainControllerInspector : BaseCameraMainControllerInspector<BaseCameraMainController>
    {
    }

    /// <summary>
    /// 基础相机主控制器检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseCameraMainControllerInspector<T> : BaseMainControllerInspector<T>
       where T : BaseCameraMainController
    {
        private static GUIContent mainCameraGUIContent = new GUIContent("主相机(只读)", "本参数对应相机实体控制器中主相机；");
        private static GUIContent speedCoefficientGUIContent = new GUIContent("速度系数", "速度系数会影响所有使用速度的组件；运算时与将本系数乘到对应组件的速度上；本参数对应相机（移动/旋转）控制器中速度系数；");
        private static GUIContent dampingCoefficientGUIContent = new GUIContent("阻尼系数", "阻尼系数会影响所有使用阻尼的组件；运算时与将本系数乘到对应组件的阻尼系数上；本参数对应相机（移动/旋转）控制器中阻尼系数；");
        private static GUIContent useDampingGUIContent = new GUIContent("使用阻尼", "所有控制组件的使用阻尼属性统一启用或禁用；所有有效控制组件均使用阻尼时，本参数才为True;如无有效控制组件或是任意一个组件不使用阻尼，则本参数为False；本参数对应相机（移动/旋转）控制器上所有具有使用阻尼属性的组件；");
        private static GUIContent mainTargetGUIContent = new GUIContent("主目标", "本参数对应相机目标控制器中主目标；");
        private static GUIContent mainTargetDistanceGUIContent = new GUIContent("主目标距离", "相机目标控制器与其主目标之间的距离；");
        private static GUIContent rangeLimitGUIContent = new GUIContent("距离限定", "本参数对应相机移动控制器游戏对象上相机位置限定器组件；");
        private static Vector2 rangeLimit = new Vector2(0.001f, 10000);

        /// <summary>
        /// 当纵向绘制对象的成员属性之后回调
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberProperty"></param>
        /// <param name="arrayElementInfo"></param>
        public override void OnAfterPropertyFieldVertical(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(BaseCameraMainController._cameraEntityController):
                    {
                        var controller = targetObject;
                        {
                            EditorGUILayout.ObjectField(mainCameraGUIContent, controller.cameraEntityController.mainCamera, typeof(Camera), true);
                        }
                        break;
                    }
                case nameof(BaseCameraMainController._cameraMover):
                    {
                        var controller = targetObject;
                        {
                            EditorGUI.BeginChangeCheck();
                            var speedCoefficient = EditorGUILayout.Vector3Field(speedCoefficientGUIContent, controller.cameraMover.speedCoefficient);
                            if (EditorGUI.EndChangeCheck())
                            {
                                controller.cameraMover.speedCoefficient = speedCoefficient;
                            }
                        }
                        {
                            var cameras = controller.cameraMover.GetComponentsInChildren<ICameraDamping>();
                            EditorGUI.BeginChangeCheck();
                            var useDamping = EditorGUILayout.Toggle(useDampingGUIContent, cameras.Length > 0 && cameras.All(c => c.useDamping));
                            if (EditorGUI.EndChangeCheck())
                            {
                                cameras.Foreach(c => c.useDamping = useDamping);
                            }

                            EditorGUI.BeginChangeCheck();
                            var dampingCoefficient = EditorGUILayout.Slider(dampingCoefficientGUIContent, controller.cameraMover.dampingCoefficient, 0, CameraHelperExtension.MaxDampingCoefficient);
                            if (EditorGUI.EndChangeCheck())
                            {
                                controller.cameraMover.dampingCoefficient = dampingCoefficient;
                            }
                        }
                        {
                            if (controller.TryGetPositionLimiter(out CameraPositionLimiter min, out CameraPositionLimiter max))
                            {
                                Vector2 range = new Vector2(min._sphereLimiter._radius, max._sphereLimiter._radius);
                                EditorGUI.BeginChangeCheck();
                                range = UICommonFun.MinMaxSliderLayout(rangeLimitGUIContent, range, rangeLimit);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    min.XModifyProperty(ref min._sphereLimiter._radius, range.x);
                                    max.XModifyProperty(ref max._sphereLimiter._radius, range.y);
                                }
                            }
                        }
                        break;
                    }
                case nameof(BaseCameraMainController._cameraRotator):
                    {
                        var controller = targetObject;
                        {
                            EditorGUI.BeginChangeCheck();
                            var speedCoefficient = EditorGUILayout.Vector3Field(speedCoefficientGUIContent, controller.cameraRotator.speedCoefficient);
                            if (EditorGUI.EndChangeCheck())
                            {
                                controller.cameraRotator.speedCoefficient = speedCoefficient;
                            }
                        }
                        {
                            var cameras = controller.cameraRotator.GetComponentsInChildren<ICameraDamping>();
                            EditorGUI.BeginChangeCheck();
                            var useDamping = EditorGUILayout.Toggle(useDampingGUIContent, cameras.Length > 0 && cameras.All(c => c.useDamping));
                            if (EditorGUI.EndChangeCheck())
                            {
                                cameras.Foreach(c => c.useDamping = useDamping);
                            }

                            EditorGUI.BeginChangeCheck();
                            var dampingCoefficient = EditorGUILayout.Slider(dampingCoefficientGUIContent, controller.cameraRotator.dampingCoefficient, 0, CameraHelperExtension.MaxDampingCoefficient);
                            if (EditorGUI.EndChangeCheck())
                            {
                                controller.cameraRotator.dampingCoefficient = dampingCoefficient;
                            }
                        }
                        break;
                    }
                case nameof(BaseCameraMainController._cameraTargetController):
                    {
                        var controller = targetObject;
                        var targetController = controller.cameraTargetController;
                        var mainTargetTransform = targetController.mainTarget;
                        var bak = GUI.backgroundColor;
                        if (!mainTargetTransform) GUI.backgroundColor = Color.red;
                        EditorGUI.BeginChangeCheck();
                        var mainTarget = EditorGUILayout.ObjectField(mainTargetGUIContent, mainTargetTransform, typeof(Transform), true);
                        if (EditorGUI.EndChangeCheck())
                        {
                            targetController.mainTarget = mainTarget as Transform;
                        }
                        GUI.backgroundColor = bak;
                        EditorGUI.BeginChangeCheck();
                        var targetPosition = targetController.targetPosition;
                        var dir = controller.cameraTransformer.position - targetPosition;
                        var distance = dir.magnitude;
                        if (controller.TryGetPositionLimiter(out CameraPositionLimiter min, out CameraPositionLimiter max))
                        {
                            distance = EditorGUILayout.Slider(mainTargetDistanceGUIContent, distance, min._sphereLimiter._radius, max._sphereLimiter._radius);
                        }
                        else
                        {
                            distance = EditorGUILayout.FloatField(mainTargetDistanceGUIContent, distance);
                        }
                        if (EditorGUI.EndChangeCheck())
                        {
                            controller.cameraTransformer.position = dir.normalized * distance + targetPosition;
                        }
                        break;
                    }
            }
            base.OnAfterPropertyFieldVertical(type, memberProperty, arrayElementInfo);
        }

#if UNITY_2018_1_OR_NEWER //相机预览

        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestroy()
        {
            if (_previewCamera != null)
            {
                DestroyImmediate(this._previewCamera.gameObject, true);
                _previewCamera = null;
            }
        }

        /// <summary>
        /// 绘制场景GUI
        /// </summary>
        public void OnSceneGUI()
        {
            Camera camera = targetObject.cameraEntityController.mainCamera;
            if (!camera) return;
            if (CameraEditorUtils.IsViewportRectValidToRender(camera.rect))
            {
                if (_delegate == null)
                {
                    _delegate = GetType().GetMethod(nameof(OnOverlayGUI)).CreateDelegate(SceneViewOverlay_LinkType.WindowFunction_Type, this);
                }

#if UNITY_2019_1_OR_NEWER
                Vector2 mainPlayModeViewTargetSize = PlayModeView_LinkType.GetMainPlayModeViewTargetSize();
                if (s_PreviousMainPlayModeViewTargetSize != mainPlayModeViewTargetSize)
                {
                    base.Repaint();
                    s_PreviousMainPlayModeViewTargetSize = mainPlayModeViewTargetSize;
                }
                SceneViewOverlay_LinkType.Window(EditorGUIUtility.TrTextContent("Camera Preview"), _delegate, -100, camera, 1, null);
#else
                SceneViewOverlay_LinkType.Window(EditorGUIUtility.TrTextContent("Camera Preview"), _delegate, -100, camera, 1);
#endif
                CameraEditorUtils.HandleFrustum(camera, this.referenceTargetIndex);
            }
        }

        private Camera _previewCamera;

        private Camera previewCamera
        {
            get
            {
                if (_previewCamera == null)
                {
                    this._previewCamera = EditorUtility.CreateGameObjectWithHideFlags("Preview Camera", HideFlags.HideAndDontSave, typeof(Camera), typeof(Skybox)).GetComponent<Camera>();
                }
                this._previewCamera.enabled = false;
                return this._previewCamera;
            }
        }

        private int _referenceTargetIndex = 0;

        private int referenceTargetIndex
        {
            get
            {
                return Mathf.Clamp(this._referenceTargetIndex, 0, targets.Length - 1);
            }
            set
            {
                this._referenceTargetIndex = (Math.Abs(value * targets.Length) + value) % targets.Length;
            }
        }

        private RenderTexture previewTexture;

        private Delegate _delegate;

        SceneView_LinkType sceneView_LinkType = new SceneView_LinkType();

        /// <summary>
        /// 绘制覆盖的GUI
        /// </summary>
        /// <param name="target"></param>
        /// <param name="sceneView"></param>
        public void OnOverlayGUI(UnityEngine.Object target, SceneView sceneView)
        {
            var camera = target as Camera;
            if (!camera) return;
            {
                sceneView_LinkType.SetObject(sceneView);
                var customScene = sceneView_LinkType.customScene;
                StageHandle stageHandle = StageUtility.GetStageHandle(camera.gameObject);
                StageHandle stageHandle2 = StageUtility.GetStageHandle(customScene);
                if (!(stageHandle != stageHandle2))
                {
#if UNITY_2019_1_OR_NEWER
                    Vector2 vector = camera.targetTexture ? new Vector2(camera.targetTexture.width, camera.targetTexture.height) : PlayModeView_LinkType.GetMainPlayModeViewTargetSize();
#else
                    Vector2 vector = camera.targetTexture ? new Vector2(camera.targetTexture.width, camera.targetTexture.height) : GameView_LinkType.GetMainGameViewTargetSize();
#endif
                    var position = sceneView.position;
                    if (vector.x < 0f)
                    {
                        vector.x = position.width;
                        vector.y = position.height;
                    }
                    Rect rect = camera.rect;
                    rect.xMin = Math.Max(rect.xMin, 0f);
                    rect.yMin = Math.Max(rect.yMin, 0f);
                    rect.xMax = Math.Min(rect.xMax, 1f);
                    rect.yMax = Math.Min(rect.yMax, 1f);
                    vector.x *= Mathf.Max(rect.width, 0f);
                    vector.y *= Mathf.Max(rect.height, 0f);
                    if (!(vector.x < 1f) && !(vector.y < 1f))
                    {
                        float num = vector.x / vector.y;
                        vector.y = 0.2f * position.height;
                        vector.x = vector.y * num;
                        if (vector.y > position.height * 0.5f)
                        {
                            vector.y = position.height * 0.5f;
                            vector.x = vector.y * num;
                        }
                        if (vector.x > position.width * 0.5f)
                        {
                            vector.x = position.width * 0.5f;
                            vector.y = vector.x / num;
                        }
                        Rect rect2 = GUILayoutUtility.GetRect(vector.x, vector.y);
                        if (Event.current.type == EventType.Repaint)
                        {
                            this.previewCamera.CopyFrom(camera);
                            this.previewCamera.scene = customScene;
                            Skybox component = ((Component)this.previewCamera).GetComponent<Skybox>();
                            if ((bool)component)
                            {
                                Skybox component2 = ((Component)camera).GetComponent<Skybox>();
                                if ((bool)component2 && component2.enabled)
                                {
                                    component.enabled = true;
                                    component.material = component2.material;
                                }
                                else
                                {
                                    component.enabled = false;
                                }
                            }

#if UNITY_2019_1_OR_NEWER
                            RenderTexture previewTextureWithSize = this.GetPreviewTextureWithSizeAndAA((int)rect2.width, (int)rect2.height);
#else
                            RenderTexture previewTextureWithSize = this.GetPreviewTextureWithSize((int)rect2.width, (int)rect2.height);
                            previewTextureWithSize.antiAliasing = Mathf.Max(1, QualitySettings.antiAliasing);
#endif
                            this.previewCamera.targetTexture = previewTextureWithSize;
                            this.previewCamera.pixelRect = new Rect(0f, 0f, rect2.width, rect2.height);
                            Handles_LinkType.EmitGUIGeometryForCamera(camera, this.previewCamera);
                            if (camera.usePhysicalProperties)
                            {
                                RenderTexture active = RenderTexture.active;
                                RenderTexture.active = previewTextureWithSize;
                                GL.Clear(false, true, Color.clear);
                                RenderTexture.active = active;
                            }
                            this.previewCamera.Render();
                            Graphics.DrawTexture(rect2, previewTextureWithSize, new Rect(0f, 0f, 1f, 1f), 0, 0, 0, 0, GUI.color, EditorGUIUtility_LinkType.GUITextureBlit2SRGBMaterial);
                        }
                    }
                }
            }
        }


#if UNITY_2019_1_OR_NEWER

        private static Vector2 s_PreviousMainPlayModeViewTargetSize;

        private RenderTexture GetPreviewTextureWithSizeAndAA(int width, int height)
        {
            int num = Mathf.Max(1, QualitySettings.antiAliasing);
            if (previewTexture == null || this.previewTexture.width != width || this.previewTexture.height != height || this.previewTexture.antiAliasing != num)
            {
                this.previewTexture = new RenderTexture(width, height, 24, SystemInfo.GetGraphicsFormat(UnityEngine.Experimental.Rendering.DefaultFormat.LDR));
                this.previewTexture.antiAliasing = num;
            }
            return this.previewTexture;
        }
#else
        private RenderTexture GetPreviewTextureWithSize(int width, int height)
        {
            if (this.previewTexture == null || this.previewTexture.width != width || this.previewTexture.height != height)
            {
                this.previewTexture = new RenderTexture(width, height, 24, RenderTextureFormat.Default, RenderTextureReadWrite.sRGB);
            }
            return this.previewTexture;
        }
#endif



#endif
    }
}
