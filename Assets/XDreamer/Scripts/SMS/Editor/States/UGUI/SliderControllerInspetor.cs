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
    [CustomEditor(typeof(SliderController))]
    public class SliderControllerInspetor : StateComponentInspector<SliderController>
    {
        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(stateComponent.numberValueTrigger):
                    {
                        if (stateComponent.slider)
                        {
                            stateComponent.numberValueTrigger.compareRule = (ENumberValueCompareRule)UICommonFun.EnumPopup(CommonFun.NameTip(typeof(FloatValueTrigger), nameof(FloatValueTrigger.compareRule)), stateComponent.numberValueTrigger.compareRule);

                            if (stateComponent.numberValueTrigger.compareRule!= ENumberValueCompareRule.Changed)
                            {
                                stateComponent.numberValueTrigger.triggerValue = EditorGUILayout.Slider(CommonFun.NameTooltip(typeof(FloatValueTrigger), nameof(FloatValueTrigger.triggerValue)),
    stateComponent.numberValueTrigger.triggerValue, stateComponent.slider.minValue, stateComponent.slider.maxValue);
                            }
                            return false;
                        }
                        break;
                    }
            }
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }
        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(stateComponent.slider):
                    {
                        EditorXGUIHelper.DrawCreateButton(stateComponent.slider, () =>
                        {
                            ToolsMenu.CreateUIInCanvas(() =>
                            {
                                var slider = ToolsMenu.CreateUIWithStyle<Slider>();
                                memberProperty.objectReferenceValue = slider;
                                return slider.gameObject;
                            });
                        });
                        break;
                    }
            }
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }
    }
}
