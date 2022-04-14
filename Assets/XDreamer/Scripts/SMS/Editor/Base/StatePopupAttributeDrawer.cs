using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.States;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;

namespace XCSJ.EditorSMS.Base
{
    /// <summary>
    /// 状态弹出特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(StatePopupAttribute))]
    public class StatePopupAttributeDrawer : PropertyDrawer<StatePopupAttribute>
    {
        private State selectedState = null;

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
                        if (selectedState)
                        {
                            property.objectReferenceValue = selectedState;
                            selectedState = null;
                        }
                        var state = property.objectReferenceValue as State;
                        if (state)
                        {
                            label.tooltip += string.Format("\n名称路径:" + state.GetNamePath());
                        }

                        var popupWidth = propertyAttribute.width;
                        var rect = new Rect(position.x, position.y, position.width - popupWidth - SpaceWidth, EditorGUIUtility.singleLineHeight);
                        EditorGUI.ObjectField(rect, property, label); 
                        state = property.objectReferenceValue as State;

                        var stateCollection = SMSHelper.GetStateCollection(property.serializedObject.targetObject, propertyAttribute.stateCollectionType);

                        rect.x = rect.x + rect.width + SpaceWidth;
                        rect.width = popupWidth;
                        if (propertyAttribute.componentType != null)
                        {
                            //property.objectReferenceValue = EditorSMSHelper.Popup(rect, propertyAttribute.componentType, state, stateCollection, propertyAttribute.searchFlags);
                            EditorSMSHelperExtension.StatePopup(rect, propertyAttribute.componentType, property, stateCollection, propertyAttribute.searchFlags);
                        }
                        else
                        {
                            if (typeof(StateMachine).IsAssignableFrom(fieldInfo.FieldType))
                            {
                                //property.objectReferenceValue = EditorSMSHelper.StateMachinePopup(rect, state as StateMachine);
                                EditorSMSHelperExtension.StateMachinePopup(rect, property);
                            }
                            else if (typeof(SubStateMachine).IsAssignableFrom(fieldInfo.FieldType))
                            {
                                //property.objectReferenceValue = EditorSMSHelper.SubStateMachinePopup(rect, state as SubStateMachine, stateCollection as SubStateMachine, propertyAttribute.searchFlags);
                                EditorSMSHelperExtension.SubStateMachinePopup(rect, property, stateCollection as SubStateMachine, propertyAttribute.searchFlags);
                            }
                            else if (typeof(State).IsAssignableFrom(fieldInfo.FieldType))
                            {
                                //property.objectReferenceValue = EditorSMSHelper.StatePopup(rect, state, stateCollection, propertyAttribute.searchFlags);
                                EditorSMSHelperExtension.StatePopup(rect, property, stateCollection, propertyAttribute.searchFlags);
                            }
                        }

                        // 使用窗口设置状态值
                        SMTreeEditorWindow.SelectStateWithButton((win, s) => selectedState = s, state);
                        return;
                    }
            }
            base.OnGUI(position, property, label);
        }
    }

    /// <summary>
    /// 跳转弹出特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(TransitionPopupAttribute))]
    public class TransitionPopupAttributeDrawer : PropertyDrawer<TransitionPopupAttribute>
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
                        var transition = property.objectReferenceValue as Transition;
                        if (transition)
                        {
                            label.tooltip += string.Format("\n名称路径:" + transition.GetNamePath());
                        }

                        var popupWidth = propertyAttribute.width;
                        var rect = new Rect(position.x, position.y, position.width - popupWidth - SpaceWidth, EditorGUIUtility.singleLineHeight);
                        EditorGUI.ObjectField(rect, property, label);
                        //transition = property.objectReferenceValue as Transition;

                        var stateCollection = SMSHelper.GetStateCollection(property.serializedObject.targetObject, propertyAttribute.stateCollectionType);

                        rect.x = rect.x + rect.width + SpaceWidth;
                        rect.width = popupWidth;
                        if (propertyAttribute.componentType != null)
                        {
                            //property.objectReferenceValue = EditorSMSHelper.Popup(rect, propertyAttribute.componentType, transition, stateCollection, propertyAttribute.searchFlags);
                            EditorSMSHelperExtension.TransitionPopup(rect, propertyAttribute.componentType, property, stateCollection, propertyAttribute.searchFlags);
                        }
                        else
                        {
                            //property.objectReferenceValue = EditorSMSHelper.TransitionPopup(rect, transition, stateCollection, propertyAttribute.searchFlags);
                            EditorSMSHelperExtension.TransitionPopup(rect,
property, stateCollection, propertyAttribute.searchFlags);
                        }
                        return;
                    }
            }
            base.OnGUI(position, property, label);
        }
    }

    /// <summary>
    /// 状态组件弹出特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(StateComponentPopupAttribute))]
    public class StateComponentPopupAttributeDrawer : PropertyDrawer<StateComponentPopupAttribute>
    {
        private StateComponent selectedStateComponent = null;

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
                        if (selectedStateComponent)
                        {
                            property.objectReferenceValue = selectedStateComponent;
                            selectedStateComponent = null;
                        }
                        var stateComponent = property.objectReferenceValue as StateComponent;
                        if (stateComponent)
                        {
                            label.tooltip += string.Format("\n名称路径:" + stateComponent.GetNamePath());
                        }

                        var popupWidth = propertyAttribute.width;
                        var stateCollection = SMSHelper.GetStateCollection(property.serializedObject.targetObject, propertyAttribute.stateCollectionType);

                        var components = stateComponent ? stateComponent.GetComponents(componentType) : Empty<StateComponent>.Array;
                        if (components.Length > 1)
                        {
                            var rect = new Rect(position.x, position.y, position.width - 2 * popupWidth - 2 * SpaceWidth, EditorGUIUtility.singleLineHeight);

                            EditorGUI.ObjectField(rect, property, label);

                            rect.x = rect.x + rect.width + SpaceWidth;
                            rect.width = popupWidth;

                            EditorSMSHelperExtension.StateComponentPopup(rect, componentType, property, stateCollection, propertyAttribute.searchFlags);

                            rect.x = rect.x + rect.width + SpaceWidth;
                            var componentNew = property.objectReferenceValue as StateComponent;
                            if (componentNew != stateComponent) components = componentNew ? componentNew.GetComponents(componentType) : Empty<StateComponent>.Array;

                            var index = components.IndexOf(componentNew);
                            var names = components.Cast(c => c.name).ToArray();

                            EditorGUI.BeginChangeCheck();
                            var newIndex = EditorGUI.Popup(rect, index, names);
                            if (EditorGUI.EndChangeCheck())
                            {
                                property.objectReferenceValue = newIndex >= 0 ? components[newIndex] as StateComponent : default;
                            }
                        }
                        else
                        {
                            var rect = new Rect(position.x, position.y, position.width - popupWidth - SpaceWidth, EditorGUIUtility.singleLineHeight);
                            EditorGUI.ObjectField(rect, property, label);                           

                            rect.x = rect.x + rect.width + SpaceWidth;
                            rect.width = popupWidth;
                            EditorSMSHelperExtension.StateComponentPopup(rect, componentType, property, stateCollection, propertyAttribute.searchFlags);
                        }                       

                        // 使用异步窗口设置状态值
                        SMTreeEditorWindow.SelectStateComponentWithButton(componentType, (win, sc) => selectedStateComponent = sc, stateComponent);

                        return;
                    }
            }
            base.OnGUI(position, property, label);
        }
    }

    /// <summary>
    /// 跳转组件弹出特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(TransitionComponentPopupAttribute))]
    public class TransitionComponentPopupAttributeDrawer : PropertyDrawer<TransitionComponentPopupAttribute>
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
                        var transitionComponent = property.objectReferenceValue as TransitionComponent;
                        if (transitionComponent)
                        {
                            label.tooltip += string.Format("\n名称路径:" + transitionComponent.GetNamePath());
                        }

                        var popupWidth = propertyAttribute.width;
                        var stateCollection = SMSHelper.GetStateCollection(property.serializedObject.targetObject, propertyAttribute.stateCollectionType);

                        var components = transitionComponent ? transitionComponent.GetComponents(componentType) : Empty<TransitionComponent>.Array;
                        if (components.Length > 1)
                        {
                            var rect = new Rect(position.x, position.y, position.width - 2*popupWidth - 2 * SpaceWidth, EditorGUIUtility.singleLineHeight);
                            EditorGUI.ObjectField(rect, property, label);
                            //transitionComponent = property.objectReferenceValue as TransitionComponent;                       

                            rect.x = rect.x + rect.width + SpaceWidth;
                            rect.width = popupWidth;

                            EditorSMSHelperExtension.TransitionComponentPopup(rect, componentType, property, stateCollection, propertyAttribute.searchFlags);

                            rect.x = rect.x + rect.width + SpaceWidth;
                            var componentNew = property.objectReferenceValue as TransitionComponent;
                            if (componentNew != transitionComponent) components = componentNew ? componentNew.GetComponents(componentType) : Empty<TransitionComponent>.Array;

                            var index = components.IndexOf(componentNew);
                            var names = components.Cast(c => c.name).ToArray();

                            EditorGUI.BeginChangeCheck();
                            var newIndex = EditorGUI.Popup(rect, index, names);
                            if (EditorGUI.EndChangeCheck())
                            {
                                property.objectReferenceValue = newIndex >= 0 ? components[newIndex] as TransitionComponent : default;
                            }

                        }
                        else
                        {
                            var rect = new Rect(position.x, position.y, position.width - popupWidth - SpaceWidth, EditorGUIUtility.singleLineHeight);
                            EditorGUI.ObjectField(rect, property, label);
                            //transitionComponent = property.objectReferenceValue as TransitionComponent;                       

                            rect.x = rect.x + rect.width + SpaceWidth;
                            rect.width = popupWidth;

                            EditorSMSHelperExtension.TransitionComponentPopup(rect, componentType, property, stateCollection, propertyAttribute.searchFlags);
                        }
                                     
                        return;
                    }
            }
            base.OnGUI(position, property, label);
        }
    }

    /// <summary>
    /// 状态组弹出特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(StateGroupPopupAttribute))]
    public class StateGroupPopupAttributeDrawer : PropertyDrawer<StateGroupPopupAttribute>
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
                        var stateGroup = property.objectReferenceValue as StateGroup;
                        if (stateGroup)
                        {
                            label.tooltip += string.Format("\n名称路径:" + stateGroup.GetNamePath());
                        }

                        var popupWidth = propertyAttribute.width;
                        var rect = new Rect(position.x, position.y, position.width - popupWidth - SpaceWidth, EditorGUIUtility.singleLineHeight);
                        EditorGUI.ObjectField(rect, property, label);
                        //stateGroup = property.objectReferenceValue as StateGroup;

                        var stateCollection = SMSHelper.GetStateCollection(property.serializedObject.targetObject, propertyAttribute.stateCollectionType);

                        rect.x = rect.x + rect.width + SpaceWidth;
                        rect.width = popupWidth;

                        if (propertyAttribute.componentType != null)
                        {
                            //property.objectReferenceValue = EditorSMSHelper.Popup(rect, propertyAttribute.componentType, stateGroup, stateCollection, propertyAttribute.searchFlags);
                            EditorSMSHelperExtension.StateGroupPopup(rect, propertyAttribute.componentType, property, stateCollection, propertyAttribute.searchFlags);
                        }
                        else
                        {
                            //property.objectReferenceValue = EditorSMSHelper.StateGroupPopup(rect, stateGroup, stateCollection, propertyAttribute.searchFlags);
                            EditorSMSHelperExtension.StateGroupPopup(rect, property, stateCollection, propertyAttribute.searchFlags);
                        }

                        return;
                    }
            }
            base.OnGUI(position, property, label);
        }
    }

    /// <summary>
    /// 状态组组件弹出特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(StateGroupComponentPopupAttribute))]
    public class StateGroupComponentPopupAttributeDrawer : PropertyDrawer<StateGroupComponentPopupAttribute>
    {
        private StateGroupComponent selectedStateGroupComponent = null;

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
                        if (selectedStateGroupComponent)
                        {
                            property.objectReferenceValue = selectedStateGroupComponent;
                            selectedStateGroupComponent = null;
                        }
                        var stateGroupComponent = property.objectReferenceValue as StateGroupComponent;
                        if (stateGroupComponent)
                        {
                            label.tooltip += string.Format("\n名称路径:" + stateGroupComponent.GetNamePath());
                        }

                        var popupWidth = propertyAttribute.width;

                        var stateCollection = SMSHelper.GetStateCollection(property.serializedObject.targetObject, propertyAttribute.stateCollectionType);

                        var components = stateGroupComponent ? stateGroupComponent.GetComponents(componentType) : Empty<StateGroupComponent>.Array;
                        if (components.Length > 1)
                        {
                            var rect = new Rect(position.x, position.y, position.width - 2*popupWidth - 2 * SpaceWidth, EditorGUIUtility.singleLineHeight);
                            EditorGUI.ObjectField(rect, property, label);

                            rect.x = rect.x + rect.width + SpaceWidth;
                            rect.width = popupWidth;

                            EditorSMSHelperExtension.StateGroupComponentPopup(rect, componentType, property, stateCollection, propertyAttribute.searchFlags);

                            rect.x = rect.x + rect.width + SpaceWidth;
                            var componentNew = property.objectReferenceValue as StateGroupComponent;
                            if (componentNew != stateGroupComponent) components = componentNew ? componentNew.GetComponents(componentType) : Empty<StateGroupComponent>.Array;

                            var index = components.IndexOf(componentNew);
                            var names = components.Cast(c => c.name).ToArray();

                            EditorGUI.BeginChangeCheck();
                            var newIndex = EditorGUI.Popup(rect, index, names);
                            if (EditorGUI.EndChangeCheck())
                            {
                                property.objectReferenceValue = newIndex >= 0 ? components[newIndex] as StateGroupComponent : default;
                            }
                        }
                        else
                        {
                            var rect = new Rect(position.x, position.y, position.width - popupWidth - SpaceWidth, EditorGUIUtility.singleLineHeight);
                            EditorGUI.ObjectField(rect, property, label);
                            //stateGroupComponent = property.objectReferenceValue as StateGroupComponent;

                            rect.x = rect.x + rect.width + SpaceWidth;
                            rect.width = popupWidth;

                            EditorSMSHelperExtension.StateGroupComponentPopup(rect, componentType, property, stateCollection, propertyAttribute.searchFlags);
                        }

                        return;
                    }
            }
            base.OnGUI(position, property, label);
        }
    }
}
