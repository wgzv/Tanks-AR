using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.Base.Attributes
{
    /// <summary>
    /// 组件集弹出特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(ObjectPopupAttribute))]
    public class ObjectPopupAttributeDrawer : PropertyDrawer<ObjectPopupAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                base.OnGUI(position, property, label);
                return;
            }
            var attr = propertyAttribute;

            var componentCollectionWidth = attr.componentCollectionWidth;
            var componentWidth = attr.componentWidth;
            var rect = new Rect(position.x, position.y, position.width - componentCollectionWidth - componentWidth - PropertyDrawerHelper.SpaceWidth * 2, EditorGUIUtility.singleLineHeight);
            base.OnGUI(rect, property, label);

            rect.x = rect.x + rect.width + PropertyDrawerHelper.SpaceWidth;
            rect.width = componentCollectionWidth;
            EditorObjectHelper.ComponentCollectionPopup(rect, property);

            rect.x = rect.x + rect.width + PropertyDrawerHelper.SpaceWidth;
            rect.width = componentWidth;
            EditorObjectHelper.ObjectComponentPopup(rect, property);
        }
    }

    /// <summary>
    /// 日期时间特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(DateTimeAttribute))]
    public class DateTimeAttributeDrawer : PropertyDrawer<DateTimeAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                base.OnGUI(position, property, label);
                return;
            }

            var buttonWidth = propertyAttribute.buttonWidth;

            var rect = new Rect(position.x, position.y, position.width - buttonWidth - PropertyDrawerHelper.SpaceWidth, EditorGUIUtility.singleLineHeight);
            base.OnGUI(rect, property, label);

            rect.x = rect.x + rect.width + PropertyDrawerHelper.SpaceWidth;
            rect.width = buttonWidth;
            if(GUI.Button(rect, "格式化"))
            {
                if(DateTime.TryParse(property.stringValue, out var dateTime))
                {
                    property.stringValue = dateTime.ToString(propertyAttribute.format);
                }
                else
                {
                    property.stringValue = DateTime.Now.ToString(propertyAttribute.format);
                }
                CommonFun.FocusControl();
            }
        }
    }

    /// <summary>
    /// 组件弹出特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(ComponentPopupAttribute))]
    public class ComponentPopupAttributeDrawer : PropertyDrawer<ComponentPopupAttribute>
    {
        /// <summary>
        /// 组件类型
        /// </summary>
        public Type componentType => propertyAttribute.componentType ?? fieldInfo.FieldType;

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
                case SerializedPropertyType.ObjectReference:
                    {
                        var propertyAttribute = this.propertyAttribute;
                        if (Application.isPlaying && !propertyAttribute.displayOnRuntime) break;
                        var componentType = this.componentType;
                        if (propertyAttribute.overrideLable)
                        {
                            label = CommonFun.NameTip(fieldInfo);
                        }

                        var component = property.objectReferenceValue as Component;
                        if (component)
                        {
                            label.tooltip += string.Format("\n名称路径:" + CommonFun.GameObjectComponentToStringWithoutAlias(component));
                        }

                        var popupWidth = propertyAttribute.width;
                        var components = component ? component.GetComponents(componentType) : Empty<Component>.Array;
                        if (components.Length > 1)
                        {
                            var rect = new Rect(position.x, position.y, position.width - 2 * popupWidth - 2 * SpaceWidth, EditorGUIUtility.singleLineHeight);
                            EditorGUI.ObjectField(rect, property, label);

                            rect.x = rect.x + rect.width + SpaceWidth;
                            rect.width = popupWidth;

                            EditorObjectHelper.GameObjectComponentPopup(rect, componentType, propertyAttribute.searchFlags, property);

                            //组件选择
                            rect.x = rect.x + rect.width + SpaceWidth;
                            //rect.width = popupWidth;

                            var componentNew = property.objectReferenceValue as Component;
                            if (componentNew != component) components = componentNew ? componentNew.GetComponents(componentType) : Empty<Component>.Array;
                            var index = components.IndexOf(componentNew);
                            var names = components.Cast((i, c) => (i + 1).ToString() + "." + c.GetType().Name).ToArray();
                            EditorGUI.BeginChangeCheck();
                            var newIndex = EditorGUI.Popup(rect, index, names);
                            if (EditorGUI.EndChangeCheck())
                            {
                                property.objectReferenceValue = newIndex >= 0 ? components[newIndex] : default;
                            }
                        }
                        else
                        {
                            var rect = new Rect(position.x, position.y, position.width - popupWidth - SpaceWidth, EditorGUIUtility.singleLineHeight);
                            EditorGUI.ObjectField(rect, property, label);

                            rect.x = rect.x + rect.width + SpaceWidth;
                            rect.width = popupWidth;

                            EditorObjectHelper.GameObjectComponentPopup(rect, componentType, propertyAttribute.searchFlags, property);
                        }

                        return;
                    }
            }
            base.OnGUI(position, property, label);
        }
    }

    /// <summary>
    /// 游戏对象弹出特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(GameObjectPopupAttribute))]
    public class GameObjectPopupAttributeDrawer : PropertyDrawer<GameObjectPopupAttribute>
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
                case SerializedPropertyType.ObjectReference:
                    {
                        var gameObject = property.objectReferenceValue as GameObject;
                        if (gameObject)
                        {
                            label.tooltip += string.Format("\n名称路径:" + CommonFun.GameObjectToStringWithoutAlias(gameObject));
                        }

                        var popupWidth = propertyAttribute.width;
                        var rect = new Rect(position.x, position.y, position.width - popupWidth - SpaceWidth, EditorGUIUtility.singleLineHeight);
                        EditorGUI.ObjectField(rect, property, label);
                        gameObject = property.objectReferenceValue as GameObject;

                        rect.x = rect.x + rect.width + SpaceWidth;
                        rect.width = popupWidth;
                        EditorObjectHelper.GameObjectPopup(rect, propertyAttribute.componentType, propertyAttribute.includeInactive, property);
                        return;
                    }
            }
            base.OnGUI(position, property, label);
        }
    }

    /// <summary>
    /// 组件类型弹出特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(ComponentTypePopupAttribute))]
    public class ComponentTypePopupAttributeDrawer: PropertyDrawer<ComponentTypePopupAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                base.OnGUI(position, property, label);
                return;
            }
            var attr = propertyAttribute;

            var buttonWidth = attr.buttonWidth;
            var rect = new Rect(position.x, position.y, position.width - buttonWidth - PropertyDrawerHelper.SpaceWidth , EditorGUIUtility.singleLineHeight);
            base.OnGUI(rect, property, label);

            rect.x = rect.x + rect.width + PropertyDrawerHelper.SpaceWidth;
            rect.width = buttonWidth;
            EditorObjectHelper.GameObjectComponentTypePopup(rect, property);
        }
    }
}
