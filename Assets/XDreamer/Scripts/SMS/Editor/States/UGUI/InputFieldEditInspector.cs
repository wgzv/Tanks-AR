using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.XGUI;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.EditorSMS.NodeKit;
using XCSJ.EditorXGUI;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.UGUI;

namespace XCSJ.EditorSMS.States.UGUI
{
    [CustomEditor(typeof(InputFieldEdit))]
    public class InputFieldEditInspector : StateComponentInspector<InputFieldEdit>
    {
        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(stateComponent.inputField):
                    {
                        EditorXGUIHelper.DrawCreateButton(stateComponent.inputField, () =>
                        {
                            ToolsMenu.CreateUIInCanvas(() =>
                            {
                                var inputField = ToolsMenu.CreateUIWithStyle<InputField>();
                                memberProperty.objectReferenceValue = inputField;
                                return inputField.gameObject;
                            });
                        });
                        break;
                    }
            }
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }
    }
}
