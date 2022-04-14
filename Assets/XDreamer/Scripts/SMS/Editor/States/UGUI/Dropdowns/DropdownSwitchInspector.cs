using System;
using System.Collections.Generic;
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
using XCSJ.PluginSMS.States.UGUI.Dropdowns;

namespace XCSJ.EditorSMS.States.UGUI.Dropdowns
{
    /// <summary>
    /// 下拉框切换检查器
    /// </summary>
    [CustomEditor(typeof(DropdownSwitch))]
    public class DropdownSwitchInspcetor : StateComponentInspector<DropdownSwitch>
    {
        /// <summary>
        /// 当横向绘制对象的成员属性之后回调
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberProperty"></param>
        /// <param name="arrayElementInfo"></param>
        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(stateComponent._dropdown):
                    {
                        EditorXGUIHelper.DrawCreateButton(stateComponent.dropdown, () =>
                        {
                            ToolsMenu.CreateUIInCanvas(() =>
                            {
                                var dropdown = ToolsMenu.CreateUIWithStyle<Dropdown>();
                                memberProperty.objectReferenceValue = dropdown;
                                return dropdown.gameObject;
                            });
                        });
                        break;
                    }
                case nameof(stateComponent.triggerValue):
                    {
                        if (stateComponent.dropdown)
                        {
                            List<Dropdown.OptionData> options = stateComponent.dropdown.options;
                            string[] displayOptions = new string[options.Count];
                            int[] optionValues = new int[options.Count];
                            for (int i = 0; i < options.Count; i++)
                            {
                                displayOptions[i] = options[i].text;
                                optionValues[i] = i;
                            }
                            stateComponent.triggerValue = EditorGUILayout.IntPopup(stateComponent.triggerValue, displayOptions, optionValues, GUILayout.Width(100));
                        }
                        break;
                    }
            }

            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }
    }
}
