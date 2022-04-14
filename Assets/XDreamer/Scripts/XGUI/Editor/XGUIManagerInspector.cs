using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.EditorXGUI
{
    /// <summary>
    /// XGUI管理器检查器
    /// </summary>
    [CustomEditor(typeof(XGUIManager), true)]
    public class XGUIManagerInspector : BaseManagerInspector<XGUIManager>
    {
        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            DrawSubWindowList();
        }

        /// <summary>
        /// 窗口列表
        /// </summary>
        [Name("窗口列表")]
        [Tip("当前场景中所有的子窗口对象")]
        public bool _display = true;

        private void DrawSubWindowList()
        {
            _display = UICommonFun.Foldout(_display, CommonFun.NameTip(GetType(), nameof(_display)));
            if (!_display) return;

            CommonFun.BeginLayout();

            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("窗口", "窗口组件所在的游戏对象；本项只读；"));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            var cache = ComponentCache.Get(typeof(SubWindow), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as SubWindow;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //窗口组件
                EditorGUILayout.ObjectField(component.gameObject, typeof(GameObject), true);

                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }

    }
}
