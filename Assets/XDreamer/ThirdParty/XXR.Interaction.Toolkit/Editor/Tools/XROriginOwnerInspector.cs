using UnityEditor;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using UnityEngine;
using XCSJ.PluginCamera;
using XCSJ.PluginsCameras;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.EditorCameras;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using System.Linq;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Base;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit;
using UnityEditor.XR.Interaction.Toolkit;
#endif

namespace XCSJ.EditorXXR.Interaction.Toolkit.Tools
{
    /// <summary>
    /// XR原点拥有者检查器
    /// </summary>
    [CustomEditor(typeof(XROriginOwner))]
    public class XROriginOwnerInspector : MBInspector<XROriginOwner>
    {
        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            EditorCameraHelperExtension.DrawSelectCameraManager();
            EditorXRITHelper.DrawSelectXRITManager();
            EditorXRITHelper.DrawOpenXRInteractionDebugger();

            DrawHMDDetailInfos();
            DrawXRControllerDetailInfos();
        }

        /// <summary>
        /// HMD列表
        /// </summary>
        [Name("HMD列表")]
        [Tip("当前XR装备的所有HMD的相机控制器组件对象或相机组件对象")]
        private static bool _displayHMDs = true;

        private void DrawHMDDetailInfos()
        {
            _displayHMDs = UICommonFun.Foldout(_displayHMDs, CommonFun.NameTip(GetType(), nameof(_displayHMDs)));
            if (!_displayHMDs) return;

            CommonFun.BeginLayout();

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("HMD对象", "HMD型组件对象；本项只读；"));
            GUILayout.Label(CommonFun.TempContent("激活启用", "HMD型组件对象是否激活并启用；本项只读；"), UICommonOption.Width60);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var hasCameraController = false;
            var owner = targetObject;

            //相机控制器
            {
                var cache = ComponentCache.Get(typeof(BaseCameraMainController), true);
                for (int i = 0; i < cache.components.Length; i++)
                {
                    var component = cache.components[i] as BaseCameraMainController;
                    if (!component.GetComponentsInParent<XROriginOwner>().Any(o => o == owner)) continue;
                    hasCameraController = true;

                    UICommonFun.BeginHorizontal(i);

                    //编号
                    EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                    //相机控制器
                    EditorGUILayout.ObjectField(component, component.GetType(), true);

                    //激活启用
                    EditorGUILayout.Toggle(component.isActiveAndEnabled, UICommonOption.Width60);

                    UICommonFun.EndHorizontal();
                }
            }

            if (!hasCameraController)//相机
            {
                var cache = ComponentCache.Get(typeof(Camera), true);
                for (int i = 0; i < cache.components.Length; i++)
                {
                    var component = cache.components[i] as Camera;
                    if (!component.GetComponentsInParent<XROriginOwner>().Any(o => o == owner)) continue;

                    UICommonFun.BeginHorizontal(i);

                    //编号
                    EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                    //相机
                    EditorGUILayout.ObjectField(component, component.GetType(), true);

                    //激活启用
                    EditorGUILayout.Toggle(component.isActiveAndEnabled, UICommonOption.Width60);

                    UICommonFun.EndHorizontal();
                }
            }

            CommonFun.EndLayout();
        }

        /// <summary>
        /// XR控制器列表
        /// </summary>
        [Name("XR控制器列表")]
        [Tip("当前XR装备的所有XR控制器对象")]
        private static bool _displayXRRigs = true;

        private void DrawXRControllerDetailInfos()
        {
            _displayXRRigs = UICommonFun.Foldout(_displayXRRigs, CommonFun.NameTip(GetType(), nameof(_displayXRRigs)));
            if (!_displayXRRigs) return;

            CommonFun.BeginLayout();

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("XR控制器", "XR控制器组件对象；本项只读；"));
            GUILayout.Label(CommonFun.TempContent("激活启用", "XR控制器组件对象是否激活并启用；本项只读；"), UICommonOption.Width60);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

#if XDREAMER_XR_INTERACTION_TOOLKIT

            var owner = targetObject;
            var cache = ComponentCache.Get(typeof(XRBaseController), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as XRBaseController;
                if (!component.GetComponentsInParent<XROriginOwner>().Any(o => o == owner)) continue;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //XR装备对象
                EditorGUILayout.ObjectField(component, component.GetType(), true);

                //激活启用
                EditorGUILayout.Toggle(component.isActiveAndEnabled, UICommonOption.Width60);

                UICommonFun.EndHorizontal();
            }
#endif

            CommonFun.EndLayout();
        }
    }
}
