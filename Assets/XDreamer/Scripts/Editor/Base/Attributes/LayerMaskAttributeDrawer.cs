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
    /// 图层蒙版特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(LayerMaskAttribute))]
    public class LayerMaskAttributeDrawer : PropertyDrawer<LayerMaskAttribute>
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
                        var names = LayerMaskAttribute.GetLayerNames(-1);
                        var selectedNames = LayerMaskAttribute.GetLayerNames(property.intValue);
                        var mask = 0;
                        for (int i = 0; i < selectedNames.Length; i++)
                        {
                            var index = Array.IndexOf(names, selectedNames[i]);
                            if (index >= 0)
                            {
                                mask |= (1 << index);
                            }
                        }
                        var newMask = EditorGUI.MaskField(position, label, mask, names);
                        if (newMask == 0 || newMask == -1)
                        {
                            property.intValue = newMask;
                        }
                        else if (newMask != mask)
                        {
                            var newNames = newMask.ToMaskIndexes().Where(i => i >= 0 && i < names.Length).Cast(i => names[i]).ToArray();
                            property.intValue = LayerMask.GetMask(newNames);
                        }
                        return;
                    }
            }
            base.OnGUI(position, property, label);
        }
    }
}
