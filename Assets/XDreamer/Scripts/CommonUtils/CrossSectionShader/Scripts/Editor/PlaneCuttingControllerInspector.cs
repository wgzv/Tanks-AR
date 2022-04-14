using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.CommonUtils.PluginCrossSectionShader;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorTools;
using XCSJ.EditorXGUI;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;

namespace XCSJ.CommonUtils.EditorCrossSectionShader
{
    [CustomEditor(typeof(PlaneCuttingController))]
    public class PlaneCuttingControllerInspector : BaseInspectorWithLimit<PlaneCuttingController>
    {
        /// <summary>
        /// 创建剖面控制器
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool("模型", rootType = typeof(ToolsManager), groupRule = EToolGroupRule.None)]
        [Name("剖面控制器")]
        [XCSJ.Attributes.Icon(EIcon.CrossSection)]
        [RequireManager(typeof(ToolsManager), typeof(ToolsExtensionManager))]
        public static void CreatePlaneCuttingController(ToolContext toolContext)
        {
            EditorXGUIHelper.CreateEventSystem();

            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("CrossSection/剖面控制器.prefab"));
        }

        public override void OnEnable()
        {
            base.OnEnable();

            if (!targetObject.cuttingMaterail)
            {
                targetObject.cuttingMaterail = GetCuttingMaterialPath();
            }

            if (!targetObject.unionCuttingMaterail)
            {
                targetObject.unionCuttingMaterail = GetUnionCuttingMaterialPath();
            }
        }

        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(PlaneCuttingController.cuttingMaterail):
                    {
                        EditorGUI.BeginDisabledGroup(targetObject.cuttingMaterail);
                        if (GUILayout.Button(CommonFun.NameTip(EIcon.Add), EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                        {
                            memberProperty.objectReferenceValue = GetCuttingMaterialPath();
                        }
                        EditorGUI.EndDisabledGroup();
                        break;
                    }
                case nameof(PlaneCuttingController.unionCuttingMaterail):
                    {
                        EditorGUI.BeginDisabledGroup(targetObject.unionCuttingMaterail);
                        if (GUILayout.Button(CommonFun.NameTip(EIcon.Add), EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                        {
                            memberProperty.objectReferenceValue = GetUnionCuttingMaterialPath();
                        }
                        EditorGUI.EndDisabledGroup();
                        break;
                    }
                default:
                    break;
            }
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }

        public override void OnAfterGroupHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(PlaneCuttingController.includeChildren):
                    {
                        if (GUILayout.Button(new GUIContent("移动到=>包围盒中心","将剖面控制器移动到剖切对象包围盒中心处"),GUILayout.Width(140)))
                        {
                            if (CommonFun.GetBounds(out Bounds bounds, targetObject.cuttedObjects, targetObject.includeChildren, targetObject.includeInactiveGameObject, targetObject.includeDisableRenderer))
                            {
                                targetObject.transform.position = bounds.center;
                            }
                        }
                        break;
                    }
                default:
                    break;
            }
            base.OnAfterGroupHorizontal(type, memberProperty, arrayElementInfo);
        }

        private Material GetCuttingMaterialPath()
        {
            return UICommonFun.LoadFromAssets<Material>("Assets/XDreamer-Assets/基础/Materials/CrossSection/GenericThreePlanesBSP.mat");
        }

        private Material GetUnionCuttingMaterialPath()
        {
            return UICommonFun.LoadFromAssets<Material>("Assets/XDreamer-Assets/基础/Materials/CrossSection/UnionGenericThreePlanesBSP.mat");
        }
    }
}
