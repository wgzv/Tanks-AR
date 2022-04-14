using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Styles.Base;

namespace XCSJ.EditorXGUI.Styles.Base
{
    /// <summary>
    /// 基础样式元素属性绘制器
    /// </summary>
    [CustomEditor(typeof(BaseStyleElement), true)]
    public class BaseStyleElementInspector : SOInspector<BaseStyleElement>
    {
        /// <summary>
        /// 样式元素父类
        /// </summary>
        public static StyleElementCollection parent { get; set; }

        /// <summary>
        /// 开始启用渲染
        /// </summary>
        public override void OnBeforeVertical()
        {
            base.OnBeforeVertical();

            // 改名称
            var newName = EditorGUILayout.DelayedTextField("名称", targetObject.name, UICommonOption.Height16);
            if (newName != targetObject.name && parent && parent.ValidName(newName))
            {
                targetObject.XSetName(newName);
            }
        }

        /// <summary>
        /// 属性绘制
        /// </summary>
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                targetObject.SendPropertyChangedEvent();
            }
        }
    }
}
