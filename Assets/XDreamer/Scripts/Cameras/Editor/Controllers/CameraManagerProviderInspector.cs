using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCameras.Base;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Controllers;

namespace XCSJ.EditorCameras.Controllers
{
    /// <summary>
    /// 相机管理器提供者检查器
    /// </summary>
    [CustomEditor(typeof(CameraManagerProvider), true)]
    public class CameraManagerProviderInspector : BaseCameraManagerProviderInspector<CameraManagerProvider>
    {
        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();
            DrawCameraControllers();
        }

        /// <summary>
        /// 相机控制器列表
        /// </summary>
        [Name("相机控制器列表")]
        [Tip("当前场景中所有的相机控制器对象")]
        private static bool _display = true;

        private void DrawCameraControllers()
        {
            _display = UICommonFun.Foldout(_display, CommonFun.NameTip(GetType(), nameof(_display)));
            if (!_display) return;
            CommonFun.BeginLayout();

            DrawCameraControllersInternal(targetObject);

            CommonFun.EndLayout();
        }

        internal static void DrawCameraControllersInternal(CameraManagerProvider provider)
        {
            DrawCameraControllersInternal(provider, ComponentCache.Get(typeof(BaseCameraMainController), true).components);
        }
        internal static void DrawCameraControllersInternal(CameraManagerProvider provider, IList<Component> components)
        {
            if (!provider || components == null) return;

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("相机控制器", "相机控制器所在的游戏对象；本项只读；"));
            GUILayout.Label(CommonFun.TempContent("相机拥有者", "相机控制器拥有者所在的游戏对象；本项只读；"));
            GUILayout.Label(CommonFun.TempContent("激活", "相机控制器组件与所在游戏对象（包含父级游戏对象）是否激活并启用；如父层级不激活，当前相机控制器也不激活；本项只读；"), UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("启用", "相机控制器组件与当前所在游戏对象（不考虑父级游戏对象）是否激活并启用；编辑态可修改，运行态只读；"), UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("当前", "表示是否是当前相机控制器；本项只读；"), UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("初始", "设置为初始相机控制器；同一时间至多仅有一个相机控制器可被设置为初始相机控制器；编辑态可修改，运行态只读；"), UICommonOption.Width32);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var isPlaying = Application.isPlaying;
            var current = provider.currentCameraController;
            var init = provider.initCameraController;
            var count = components.Count;
            for (int i = 0; i < count; i++)
            {
                var component = components[i] as BaseCameraMainController;
                if (!init && component.isActiveAndEnabled)
                {
                    //如果初始相机控制器无效，则使用列表中第一个激活并启用的相机控制器作为初始相机控制器
                    provider.initCameraController = init = component;
                }

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //相机控制器
                var gameObject = component.gameObject;
                EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

                //相机拥有者
                var cameraOwner = component.cameraOwner.ownerGameObject;
                EditorGUILayout.ObjectField(cameraOwner, typeof(GameObject), true);

                EditorGUI.BeginDisabledGroup(isPlaying);
                {
                    //激活
                    EditorGUILayout.Toggle(component.isActiveAndEnabled, UICommonOption.Width32);

                    //启用
                    var enable = component.enabled && component.gameObject.activeSelf;
                    if (EditorGUILayout.Toggle(enable, UICommonOption.Width32) != enable)
                    {
                        if (enable)
                        {
                            //相机控制器所在游戏对象取消激活
                            component.gameObject.XSetActive(false);
                            component.XSetEnable(false);
                        }
                        else
                        {
                            //相机控制器所在游戏对象激活
                            component.gameObject.XSetActive(true);
                            component.XSetEnable(true);
                        }
                    }

                    //当前
                    EditorGUILayout.Toggle(component == current, UICommonOption.Width32);

                    //初始
                    if (EditorGUILayout.Toggle(component == init, UICommonOption.Width32))
                    {
                        if (component != init)
                        {
                            provider.initCameraController = component;
                            init = provider.initCameraController;
                        }
                    }
                }
                EditorGUI.EndDisabledGroup();

                UICommonFun.EndHorizontal();
            }
        }
    }
}
