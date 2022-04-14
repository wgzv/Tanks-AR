using System;
using UnityEditor;
using UnityEngine.UI;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.EditorXGUI;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.UGUI;

namespace XCSJ.EditorSMS.States.UGUI
{
    [CustomEditor(typeof(ScrollBarController))]
    public class ScrollBarControllerInspetor : StateComponentInspector<ScrollBarController>
    {
        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(stateComponent.numberValueTrigger):
                    {
                        if (stateComponent.scrollbar)
                        {
                            stateComponent.numberValueTrigger.compareRule = (ENumberValueCompareRule)UICommonFun.EnumPopup(CommonFun.NameTip(typeof(FloatValueTrigger), nameof(FloatValueTrigger.compareRule)), stateComponent.numberValueTrigger.compareRule);

                            if (stateComponent.numberValueTrigger.compareRule != ENumberValueCompareRule.Changed)
                            {
                                stateComponent.numberValueTrigger.triggerValue = EditorGUILayout.Slider(CommonFun.NameTooltip(typeof(FloatValueTrigger), nameof(FloatValueTrigger.triggerValue)),
stateComponent.numberValueTrigger.triggerValue, 0, 1);
                            }

                        }
                        return false;
                    }
            }
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }

        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(stateComponent.scrollbar):
                    {
                        EditorXGUIHelper.DrawCreateButton(stateComponent.scrollbar, () =>
                        {
                            ToolsMenu.CreateUIInCanvas(() =>
                            {
                                var scrollbar = EditorXGUIHelper.CreateUGUI<Scrollbar>();
                                memberProperty.objectReferenceValue = scrollbar;
                                return scrollbar.gameObject;
                            });
                        });
                        break;
                    }
            }
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }
    }
}
