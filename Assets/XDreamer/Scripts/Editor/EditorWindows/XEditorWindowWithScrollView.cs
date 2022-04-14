using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.EditorWindows
{
    /// <summary>
    /// 用于明文的带滚动视图的编辑器窗口基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class XEditorWindowWithScrollView<T> : EditorWindowWithScrollView<T>, IHasCustomMenu
        where T : XEditorWindowWithScrollView<T>
    {
        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            OnAnyAssetsChanged();
            XDreamerEvents.onAnyAssetsChanged += OnAnyAssetsChanged;

#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui += OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate += OnSceneGUI;
#endif
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();

            XDreamerEvents.onAnyAssetsChanged -= OnAnyAssetsChanged;
#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui -= OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
#endif
        }

        /// <summary>
        /// 当任意资产变更时回调
        /// </summary>
        protected virtual void OnAnyAssetsChanged() { }

        /// <summary>
        /// 当绘制场景GUI时
        /// </summary>
        /// <param name="sceneView"></param>
        public virtual void OnSceneGUI(SceneView sceneView) { }

        /// <summary>
        /// 编辑脚本内容
        /// </summary>
        private GUIContent content_EditScript = new GUIContent("编辑脚本");

        /// <summary>
        /// 添加项到菜单：窗口增加点击的菜单项
        /// </summary>
        /// <param name="menu"></param>
        public virtual void AddItemsToMenu(GenericMenu menu)
        {
#if XDREAMER_EDITION_XDREAMERDEVELOPER
            menu.AddItem(content_EditScript, false, () =>
            {
                EditorHelper.OpenMonoScript(this);
            });
#endif
        }
    }
}
