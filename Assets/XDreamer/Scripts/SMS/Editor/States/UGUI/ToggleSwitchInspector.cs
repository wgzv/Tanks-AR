using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace XCSJ.PluginSMS.States.UGUI
{
    [CustomEditor(typeof(ToggleSwitch))]
    public class ToggleSwitchInspectorr : StateComponentInspector<ToggleSwitch>
    {
        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(stateComponent.toggle):
                    {
                        EditorXGUIHelper.DrawCreateButton(stateComponent.toggle, () =>
                        {
                            ToolsMenu.CreateUIInCanvas(() =>
                            {
                                var component = ToolsMenu.CreateUIWithStyle<Toggle>();
                                memberProperty.objectReferenceValue = component;
                                return component.gameObject;
                            });
                        });
                        break;
                    }
            }
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }
    }
}
