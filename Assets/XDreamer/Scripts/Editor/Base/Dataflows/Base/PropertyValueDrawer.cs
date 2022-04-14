using UnityEditor;
using UnityEngine;
using XCSJ.Caches;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;

namespace XCSJ.EditorExtension.Base.Dataflows.Base
{
    /// <summary>
    /// 基础属性值绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(BasePropertyValue), true)]
    public class PropertyValueDrawer : PropertyDrawer
    {
        internal const float PropertyValueTypeWidth = 60;

        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);
            EditorGUI.BeginProperty(position, label, property);

            var propertyValueTypeSP = property.FindPropertyRelative(nameof(BasePropertyValue._propertyValueType));
            var propertyValueType = (EPropertyValueType)propertyValueTypeSP.intValue;

            var variableNameSP = property.FindPropertyRelative(nameof(BasePropertyValue._variableName));
            var valueSP = property.FindPropertyRelative(PropertyValueFieldNameAttribute.GetFieldName(fieldInfo.FieldType));

            if (valueSP.propertyType == SerializedPropertyType.Enum && propertyValueType == (int)EPropertyValueType.Value)//枚举值
            {
                var valueFI = FieldInfoCache.Get(fieldInfo.FieldType, valueSP.name);
                if (valueFI != null)
                {
                    var enumType = valueFI.FieldType;
                    var e = EnumValueCache.Get(enumType, valueSP.intValue.ToString(), EEnumStringType.UnderlyingType);
                    var nameTip = CommonFun.NameTip(e);
                    label.tooltip += string.Format("\n{0}:\n{1}", nameTip.text, nameTip.tooltip);
                }
            }

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var position1 = new Rect(position.x, position.y, PropertyValueTypeWidth, position.height);
            var newPropertyValueType = (EPropertyValueType)UICommonFun.EnumPopup(position1, propertyValueType, EEnumStringType.NameAttributeCN);
            if (propertyValueType != newPropertyValueType)
            {
                propertyValueTypeSP.intValue = (int)newPropertyValueType;
            }

            var position2 = new Rect(position.x + PropertyValueTypeWidth, position.y, position.width - PropertyValueTypeWidth, position.height);
            switch (newPropertyValueType)
            {
                case EPropertyValueType.Value:
                    {
                        EditorGUI.PropertyField(position2, valueSP, GUIContent.none);
                        break;
                    }
                case EPropertyValueType.Variable:
                    {
                        EditorGUI.PropertyField(position2, variableNameSP, GUIContent.none);
                        break;
                    }
            }
            EditorGUI.EndProperty();
        }
    }

    /// <summary>
    /// 字符串属性值绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(StringPropertyValue_TextArea), true)]
    public class StringPropertyValue_TextAreaDrawer : PropertyValueDrawer
    {
        private float singleHeight = -1;

        /// <summary>
        /// 获取属性高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            singleHeight = base.GetPropertyHeight(property, label);//单行高度
            var propertyValueTypeSP = property.FindPropertyRelative(nameof(BasePropertyValue._propertyValueType));
            switch ((EPropertyValueType)propertyValueTypeSP.intValue)
            {
                case EPropertyValueType.Value:
                    {
                        return singleHeight * 3;
                    }
            }
            return singleHeight;
        }

        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);
            EditorGUI.BeginProperty(position, label, property);

            var propertyValueTypeSP = property.FindPropertyRelative(nameof(BasePropertyValue._propertyValueType));
            var variableNameSP = property.FindPropertyRelative(nameof(BasePropertyValue._variableName));
            var valueSP = property.FindPropertyRelative(PropertyValueFieldNameAttribute.GetFieldName(fieldInfo.FieldType));

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var position1 = new Rect(position.x, position.y, PropertyValueTypeWidth, singleHeight);
            var propertyValueType = (EPropertyValueType)propertyValueTypeSP.intValue;
            var newPropertyValueType = (EPropertyValueType)UICommonFun.EnumPopup(position1, propertyValueType, EEnumStringType.NameAttributeCN);
            if (propertyValueType != newPropertyValueType)
            {
                propertyValueTypeSP.intValue = (int)newPropertyValueType;
            }

            var position2 = new Rect(position.x + PropertyValueTypeWidth, position.y, position.width - PropertyValueTypeWidth, position.height);
            switch (newPropertyValueType)
            {
                case EPropertyValueType.Value:
                    {
                        valueSP.stringValue = EditorGUI.TextArea(position2, valueSP.stringValue);
                        break;
                    }
                case EPropertyValueType.Variable:
                    {
                        EditorGUI.PropertyField(position2, variableNameSP, GUIContent.none);
                        break;
                    }
            }
            EditorGUI.EndProperty();
        }
    }

    /// <summary>
    /// 脚本集合属性值绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(ScriptSetPropertyValue), true)]
    public class ScriptSetPropertyValueDrawer : PropertyValueDrawer
    {
        /// <summary>
        /// 全局可记录链表管理器
        /// </summary>
        private static ReorderableListManager reorderableListManager { get; set; } = new ReorderableListManager();

        private float singleHeight = -1;

        /// <summary>
        /// 获取属性高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            singleHeight = base.GetPropertyHeight(property, label);//单行高度
            var propertyValueTypeSP = property.FindPropertyRelative(nameof(BasePropertyValue._propertyValueType));
            switch ((EPropertyValueType)propertyValueTypeSP.intValue)
            {
                case EPropertyValueType.Value:
                    {
                        return singleHeight/* * 10*/;
                    }
            }
            return singleHeight;
        }

        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);
            EditorGUI.BeginProperty(position, label, property);

            var propertyValueTypeSP = property.FindPropertyRelative(nameof(BasePropertyValue._propertyValueType));
            var variableNameSP = property.FindPropertyRelative(nameof(BasePropertyValue._variableName));
            var valueSP = property.FindPropertyRelative(PropertyValueFieldNameAttribute.GetFieldName(fieldInfo.FieldType));

            // Draw label
            var position0 = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var position1 = new Rect(position0.x, position0.y, PropertyValueTypeWidth, singleHeight);
            var propertyValueType = (EPropertyValueType)propertyValueTypeSP.intValue;
            var newPropertyValueType = (EPropertyValueType)UICommonFun.EnumPopup(position1, propertyValueType, EEnumStringType.NameAttributeCN);
            if (propertyValueType != newPropertyValueType)
            {
                propertyValueTypeSP.intValue = (int)newPropertyValueType;
            }

            var position2 = new Rect(position0.x + PropertyValueTypeWidth, position0.y, position0.width - PropertyValueTypeWidth, singleHeight);
            switch (newPropertyValueType)
            {
                case EPropertyValueType.Value:
                    {
                        var so = property.serializedObject;
                        var targetObject = so.targetObject;
                        if (targetObject is IGetScriptSet getScriptSet && getScriptSet.GetScriptSet(valueSP.propertyPath) is ScriptSet scriptSet)
                        {
                            if (scriptSet.ScriptStringList.Count == 0)
                            {
                                scriptSet.ScriptStringList.Add(new XCSJ.Scripts.ScriptString());
                                //valueSP.FindPropertyRelative(nameof(ScriptSet.ScriptStringList)).AddArrayElement();
                            }
                            UICommonFun.FunctionOnGUI(targetObject, so, scriptSet, valueSP, CommonFun.TempContent(scriptSet.name, "全局自定义函数"), out _, reorderableListManager);
                        }
                        else
                        {
                            var type = targetObject.GetType();
                            Debug.LogErrorFormat("{0}（类型：{1}）需继承接口{2}才可使用属性值类{3}",
                                CommonFun.Name(type),
                                type.FullName,
                                nameof(IGetScriptSet),
                                nameof(ScriptSetPropertyValue));
                        }
                        break;
                    }
                case EPropertyValueType.Variable:
                    {
                        EditorGUI.PropertyField(position2, variableNameSP, GUIContent.none);
                        break;
                    }
            }
            EditorGUI.EndProperty();
        }
    }
}
