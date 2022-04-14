using System;
using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.EditorXGUI.Base
{
    /// <summary>
    /// 根窗口属性
    /// </summary>
    [CustomEditor(typeof(RootWindow), true)]
    public class RootWindowInspector: SubWindowInspector
    {
        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberProperty"></param>
        /// <param name="arrayElementInfo"></param>
        /// <returns></returns>
        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(SubWindow._title):
                case nameof(SubWindow._content):
                    {
                        return false;
                    }
            }
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }
    }
}
