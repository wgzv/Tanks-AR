using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Maths;

namespace XCSJ.EditorExtension.Base.Attributes
{
    /// <summary>
    /// 图层特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(LayerAttribute))]
    public class LayerAttributeDrawer : PropertyDrawer<LayerAttribute>
    {
        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    {
                        property.intValue = EditorGUI.LayerField(position, label, property.intValue);
                        return;
                    }
            }
            base.OnGUI(position, property, label);
        }
    }
}
