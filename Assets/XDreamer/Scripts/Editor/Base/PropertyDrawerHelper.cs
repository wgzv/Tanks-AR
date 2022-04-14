using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.Base
{
    /// <summary>
    /// 属性绘制器辅助类
    /// </summary>
    public static class PropertyDrawerHelper
    {
        /// <summary>
        /// 间隔宽度
        /// </summary>
        public const int SpaceWidth = ComponentPopupAttributeDrawer.SpaceWidth;

        /// <summary>
        /// 绘制字符串弹出式菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <param name="values"></param>
        /// <param name="hasText"></param>
        /// <param name="popupWidth"></param>
        /// <returns>成功弹出菜单返回True，否则返回False</returns>
        public static bool DrawStringPopup(Rect position, SerializedProperty property, GUIContent label, string[] values, bool hasText, float popupWidth)
        {
            string[] stringArray = values;
            int index = stringArray.IndexOf(property.stringValue);

            if (stringArray != null && index < stringArray.Length)
            {
                if (hasText)
                {
                    var rect = new Rect(position.x, position.y, position.width - popupWidth - 2, position.height);
                    property.stringValue = EditorGUI.TextField(rect, label, property.stringValue);

                    rect.x = rect.x + rect.width + 2;
                    rect.width = popupWidth;
                    var newIndex = EditorGUI.Popup(rect, index, stringArray);
                    if (newIndex != index) property.stringValue = stringArray[newIndex];
                }
                else
                {
                    var newIndex = EditorGUI.Popup(position, label, index, CommonFun.TempContent(stringArray));
                    if (newIndex != index) property.stringValue = stringArray[newIndex];
                }
                return true;
            }
            return false;
        }
    }
}
