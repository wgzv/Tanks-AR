using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.CommonUtils.PluginCrossSectionShader;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Views.Sliders;

namespace XCSJ.CommonUtils.EditorCrossSectionShader
{
    [CustomEditor(typeof(PlaneCuttingUIController))]
    public class PlaneCuttingUIControllerInspector : BaseInspectorWithLimit<PlaneCuttingUIController>
    {
        private const float SliderMaxValue = 100000;

        private bool expendedPlaneActiveAndAngle = true;

        private bool expendedPlaneX = true;
        private bool expendedPlaneY = true;
        private bool expendedPlaneZ = true;

        private bool DisplayPlaneXInfo()
        {
            return targetObject.planeXInfo.toggle ? targetObject.planeXInfo.toggle.isOn : false;
        }

        private bool DisplayPlaneYInfo()
        {
            return targetObject.planeYInfo.toggle ? targetObject.planeYInfo.toggle.isOn : false;
        }

        private bool DisplayPlaneZInfo()
        {
            return targetObject.planeZInfo.toggle ? targetObject.planeZInfo.toggle.isOn : false;
        }

        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(PlaneCuttingUIController.planeXInfo):
                    {
                        if (expendedPlaneActiveAndAngle && DisplayPlaneXInfo())
                        {
                            CommonFun.BeginLayout();
                            DrawToggleRectTransformInfo(serializedObject, type, memberProperty, arrayElementInfo);
                            CommonFun.EndLayout();
                        }
                        return false;
                    }
                case nameof(PlaneCuttingUIController.planeYInfo):
                    {
                        if (expendedPlaneActiveAndAngle && DisplayPlaneYInfo())
                        {
                            CommonFun.BeginLayout();
                            DrawToggleRectTransformInfo(serializedObject, type, memberProperty, arrayElementInfo);
                            CommonFun.EndLayout();
                        }
                        return false;
                    }
                case nameof(PlaneCuttingUIController.planeZInfo):
                    {
                        if (expendedPlaneActiveAndAngle && DisplayPlaneZInfo())
                        {
                            CommonFun.BeginLayout();
                            DrawToggleRectTransformInfo(serializedObject, type, memberProperty, arrayElementInfo);
                            CommonFun.EndLayout();
                        }
                        return false;
                    }
                case nameof(PlaneCuttingUIController.planeX):
                    {
                        if (expendedPlaneActiveAndAngle && DisplayPlaneXInfo())
                        {
                            CommonFun.BeginLayout();
                            {
                                expendedPlaneX = DrawToggleXYZSlider(serializedObject, type, memberProperty, arrayElementInfo, expendedPlaneX, null, () =>
                                {
                                    if (targetObject.planeX.active)
                                    {
                                        if (OnToggleValueChangedEditorGUI(targetObject.planeX.active, "激活操作"))
                                        {
                                            targetObject.SetPlaneXActive(targetObject.planeX.active.isOn);
                                        }
                                    }
                                });
                            }
                            CommonFun.EndLayout();
                        }
                        return false;
                    }
                case nameof(PlaneCuttingUIController.planeY):
                    {
                        if (expendedPlaneActiveAndAngle && DisplayPlaneYInfo())
                        {
                            CommonFun.BeginLayout();
                            {
                                expendedPlaneY = DrawToggleXYZSlider(serializedObject, type, memberProperty, arrayElementInfo, expendedPlaneY, null, () =>
                                {
                                    if (targetObject.planeY.active)
                                    {
                                        if (OnToggleValueChangedEditorGUI(targetObject.planeY.active, "激活操作"))
                                        {
                                            targetObject.SetPlaneYActive(targetObject.planeY.active.isOn);
                                        }
                                    }
                                });
                            }
                            CommonFun.EndLayout();
                        }
                        return false;
                    }
                case nameof(PlaneCuttingUIController.planeZ):
                    {
                        if (expendedPlaneActiveAndAngle && DisplayPlaneZInfo())
                        {
                            CommonFun.BeginLayout();
                            {
                                expendedPlaneZ = DrawToggleXYZSlider(serializedObject, type, memberProperty, arrayElementInfo, expendedPlaneZ, null, () =>
                                {
                                    if (targetObject.planeZ.active)
                                    {
                                        if (OnToggleValueChangedEditorGUI(targetObject.planeZ.active, "激活操作"))
                                        {
                                            targetObject.SetPlaneZActive(targetObject.planeZ.active.isOn);
                                        }
                                    }
                                });
                            }
                            CommonFun.EndLayout();
                        }
                        return false;
                    }
                default:
                    break;
            }
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }

        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(PlaneCuttingUIController.position):
                    {
                        if (GUILayout.Button("关联包围盒尺寸", GUILayout.Width(100)))
                        {
                            targetObject.SetPositionRange();
                        }
                        break;
                    }
                default:
                    break;
            }
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }

        private int selectedIndex = 0;
        private GUIContent[] guiContents = null;

        public override void OnAfterPropertyFieldVertical(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(PlaneCuttingUIController.position):
                    {
                        expendedPlaneActiveAndAngle = UICommonFun.Foldout(expendedPlaneActiveAndAngle, "平面激活与角度");
                        if (expendedPlaneActiveAndAngle)
                        {
                            CommonFun.BeginLayout();
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    if (guiContents == null)
                                    {
                                        guiContents = new GUIContent[3];
                                        guiContents[0] = new GUIContent(CommonFun.Name(typeof(PlaneCuttingUIController), nameof(PlaneCuttingUIController.planeXInfo)));
                                        guiContents[1] = new GUIContent(CommonFun.Name(typeof(PlaneCuttingUIController), nameof(PlaneCuttingUIController.planeYInfo)));
                                        guiContents[2] = new GUIContent(CommonFun.Name(typeof(PlaneCuttingUIController), nameof(PlaneCuttingUIController.planeZInfo)));

                                        if (targetObject.planeYInfo.toggle && targetObject.planeYInfo.toggle.isOn)
                                        {
                                            selectedIndex = 1;
                                        }

                                        if (targetObject.planeZInfo.toggle && targetObject.planeZInfo.toggle.isOn)
                                        {
                                            selectedIndex = 2;
                                        }
                                    }
                                    selectedIndex = GUILayout.Toolbar(selectedIndex, guiContents);

                                    switch (selectedIndex)
                                    {
                                        case 0:
                                            {
                                                if (targetObject.planeXInfo.toggle && !targetObject.planeXInfo.toggle.isOn)
                                                {
                                                    targetObject.SetPlaneInfoActive(true, false, false);
                                                }
                                                break;
                                            }
                                        case 1:
                                            {
                                                if (targetObject.planeYInfo.toggle && !targetObject.planeYInfo.toggle.isOn)
                                                {
                                                    targetObject.SetPlaneInfoActive(false, true, false);
                                                }
                                                break;
                                            }
                                        case 2:
                                            {
                                                if (targetObject.planeZInfo.toggle && !targetObject.planeZInfo.toggle.isOn)
                                                {
                                                    targetObject.SetPlaneInfoActive(false, false, true);
                                                }
                                                break;
                                            }
                                        default:
                                            break;
                                    }
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                            CommonFun.EndLayout();
                        }
                        break;
                    }
                case nameof(PlaneCuttingUIController.cuttingPlane):
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.PrefixLabel("操作");
                            if (OnToggleValueChangedGUI(targetObject.aixs, CommonFun.Name(typeof(PlaneCuttingUIController), nameof(PlaneCuttingUIController.aixs))))
                            {
                                targetObject.SetAixsActive(targetObject.aixs.isOn);
                            }

                            if (OnToggleValueChangedGUI(targetObject.plane, CommonFun.Name(typeof(PlaneCuttingUIController), nameof(PlaneCuttingUIController.plane))))
                            {
                                targetObject.SetPlaneActive(targetObject.plane.isOn);
                            }

                            if (OnToggleValueChangedGUI(targetObject.cuttingPlane, CommonFun.Name(typeof(PlaneCuttingUIController), nameof(PlaneCuttingUIController.cuttingPlane))))
                            {
                                targetObject.SetCuttingPlaneActive(targetObject.cuttingPlane.isOn);
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        break;
                    }
                default:
                    break;
            }
            base.OnAfterPropertyFieldVertical(type, memberProperty, arrayElementInfo);
        }

        /// <summary>
        /// 返回值为Toggle是否切换
        /// </summary>
        public static bool OnToggleValueChangedGUI(Toggle toggle, string title="", params GUILayoutOption[] options)
        {            
            EditorGUI.BeginDisabledGroup(!toggle);
            {
                bool old = toggle ? toggle.isOn : false;
                bool newValue = GUILayout.Toggle(old, title, options);
                if(toggle)
                {
                    toggle.isOn = newValue;
                }
                if (newValue != old)
                {
                    return true;
                }
            }
            EditorGUI.EndDisabledGroup();
            return false;
        }

        /// <summary>
        /// 返回值为Toggle是否切换
        /// </summary>
        public static bool OnToggleValueChangedEditorGUI(Toggle toggle, string title = "", params GUILayoutOption[] options)
        {
            if (toggle)
            {
                bool old = toggle.isOn;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(title);
                toggle.isOn = GUILayout.Toggle(toggle.isOn, "", options);
                EditorGUILayout.EndHorizontal();
                if (toggle.isOn != old)
                {
                    return true;
                }
            }
            return false;
        }

        public static void SetSliderValueRange(Slider slider, float min, float max)
        {
            if(slider && min<=max)
            {
                slider.minValue = min;
                slider.maxValue = max;
            }
        }

        public static void DrawToggleRectTransformInfo(SerializedObject serializedObject, Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo, Action onBeforeVertical = null, Action onAfterVertical = null)
        {
            var toggle = memberProperty.FindPropertyRelative(nameof(ToggleRectTransformInfo.toggle));
            var rectTransform = memberProperty.FindPropertyRelative(nameof(ToggleRectTransformInfo.rectTransform));

            EditorGUI.BeginChangeCheck();

            onBeforeVertical?.Invoke();

            EditorGUILayout.PropertyField(toggle, CommonFun.NameTooltip(typeof(ToggleRectTransformInfo), nameof(ToggleRectTransformInfo.toggle)));
            EditorGUILayout.PropertyField(rectTransform, CommonFun.NameTooltip(typeof(ToggleRectTransformInfo), nameof(ToggleRectTransformInfo.rectTransform)));

            onAfterVertical?.Invoke();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        public static bool DrawXYZSlider(SerializedObject serializedObject, Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo, bool expanded, Action onBeforeVertical = null, Action onAfterVertical = null)
        {
            expanded = UICommonFun.Foldout(expanded, CommonFun.Name(type, memberProperty.name));
            if (expanded)
            {
                CommonFun.BeginLayout();
                {
                    var root = memberProperty.FindPropertyRelative(nameof(XYZSlider.root));
                    var x = memberProperty.FindPropertyRelative(nameof(XYZSlider.x));
                    var y = memberProperty.FindPropertyRelative(nameof(XYZSlider.y));
                    var z = memberProperty.FindPropertyRelative(nameof(XYZSlider.z));

                    EditorGUI.BeginChangeCheck();

                    onBeforeVertical?.Invoke();

                    EditorGUILayout.PropertyField(root, CommonFun.NameTooltip(typeof(XYZSlider), nameof(XYZSlider.root)));
                    EditorGUILayout.PropertyField(x, CommonFun.NameTooltip(typeof(XYZSlider), nameof(XYZSlider.x)));
                    EditorGUILayout.PropertyField(y, CommonFun.NameTooltip(typeof(XYZSlider), nameof(XYZSlider.y)));
                    EditorGUILayout.PropertyField(z, CommonFun.NameTooltip(typeof(XYZSlider), nameof(XYZSlider.z)));

                    onAfterVertical?.Invoke();

                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                    }

                }
                CommonFun.EndLayout();
            }

            return expanded;
        }

        public static bool DrawToggleXYZSlider(SerializedObject serializedObject, Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo, bool expanded, Action onBeforeVertical = null, Action onAfterVertical = null)
        {
            return DrawXYZSlider(serializedObject, type, memberProperty, arrayElementInfo, expanded, onBeforeVertical, () =>
            {
                var active = memberProperty.FindPropertyRelative(nameof(ToggleXYZSlider.active));
                EditorGUILayout.PropertyField(active, CommonFun.NameTooltip(typeof(ToggleXYZSlider), nameof(ToggleXYZSlider.active)));

                onAfterVertical?.Invoke();
            });                     
        }
    }

}
