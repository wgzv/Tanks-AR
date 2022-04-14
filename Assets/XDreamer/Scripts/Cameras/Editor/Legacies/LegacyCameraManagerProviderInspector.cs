using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCameras.Base;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Legacies;

namespace XCSJ.EditorCameras.Legacies
{
    /// <summary>
    /// 旧版相机提供者检查器
    /// </summary>
    [CustomEditor(typeof(LegacyCameraManagerProvider), true)]
    public class LegacyCameraManagerProviderInspector : MBInspector<LegacyCameraManagerProvider>
    {
        /// <summary>
        /// 当纵向绘制之前回调
        /// </summary>
        public override void OnBeforeVertical()
        {
            base.OnBeforeVertical();
            EditorGUILayout.HelpBox("产品功能升级，旧版相机不推荐用户使用！", MessageType.Warning);
            if (XDreamerBaseOption.weakInstance.obsoleteCategoryItemDisplay == EObsoleteDisplayRule.None
                && GUILayout.Button("启用旧版相机"))
            {
                XDreamerBaseOption.weakInstance.obsoleteCategoryItemDisplay = EObsoleteDisplayRule.NoError;
                XDreamerBaseOption.weakInstance.MarkDirty();
            }
        }

        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            DrawCameras();
        }

#pragma warning disable CS0618 // 类型或成员已过时

        /// <summary>
        /// 相机列表
        /// </summary>
        [Name("相机列表")]
        private static bool _display = false;

        private void DrawCameras()
        {
            _display = UICommonFun.Foldout(_display, CommonFun.NameTip(GetType(), nameof(_display)));
            if (!_display) return;
            var isPlaying = Application.isPlaying;
            CommonFun.BeginLayout();

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label("相机");
            GUILayout.Label("激活", UICommonOption.Width32);
            GUILayout.Label("当前", UICommonOption.Width32);
            GUILayout.Label("初始", UICommonOption.Width32);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var cache = ComponentCache.Get(typeof(BaseCamera), true);
            var current = targetObject.currentCamera;
            var init = targetObject._initCamera;
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as BaseCamera;

                UICommonFun.BeginHorizontal(i);

                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                EditorGUILayout.ObjectField(component, component.GetType(), true);

                EditorGUI.BeginDisabledGroup(isPlaying);
                {
                    var active = component.isActiveAndEnabled;
                    if (EditorGUILayout.Toggle(active, UICommonOption.Width32) != active)
                    {
                        if (active)
                        {
                            component.gameObject.XSetActive(false);
                        }
                        else
                        {
                            component.gameObject.XSetActive(true);
                        }
                    }

                    EditorGUILayout.Toggle(component == current, UICommonOption.Width32);

                    if (EditorGUILayout.Toggle(component == init, UICommonOption.Width32))
                    {
                        if (component != init)
                        {
                            targetObject._initCamera = component;
                            init = targetObject._initCamera;
                        }
                    }
                }
                EditorGUI.EndDisabledGroup();

                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }

#pragma warning restore CS0618 // 类型或成员已过时
    }
}
