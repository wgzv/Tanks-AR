using System;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Styles.Base;
using XCSJ.PluginXGUI.Styles.Tools;

namespace XCSJ.EditorXGUI.Styles.Tools
{
    /// <summary>
    /// 样式配置器属性面板
    /// </summary>
    [CustomEditor(typeof(StyleConfiger))]
    [CanEditMultipleObjects]
    public class StyleConfigerInspector : BaseInspectorWithLimit<StyleConfiger>
    {
        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(StyleConfiger._styleName):
                    {
                        var name = UICommonFun.Popup(CommonFun.NameTip(targetObject.GetType(), nameof(StyleConfiger._styleName)), targetObject._styleName, XStyleCache.GetNames(), false, GUILayout.ExpandWidth(true));
                        if (!string.IsNullOrEmpty(name) && name != targetObject._styleName)
                        {
                            XStyleCache.SetDefaultStyle(name);
                        }
                        return false;
                    }
            }
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }
    }
}
