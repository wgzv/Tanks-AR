using System;
using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginPhysicses.States;

namespace XCSJ.EditorPhysicses.States
{
    /// <summary>
    /// 限定计算器属性检查器
    /// </summary>
    [CustomEditor(typeof(LimitCalculatorTriggerCompare), true)]
    public class LimitCalculatorTriggerCompareInspector : StateComponentInspector<LimitCalculatorTriggerCompare>
    {
        private LimitCalculatorTriggerCompare currentObj => targetObject as LimitCalculatorTriggerCompare;

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberProperty"></param>
        /// <param name="arrayElementInfo"></param>
        /// <returns></returns>
        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(LimitCalculatorTriggerCompare._switchState):
                    {
                        if (currentObj._limitCalculatorTrigger)
                        {
                            return currentObj._limitCalculatorTrigger._limitCalculatorType == PluginPhysicses.Base.Limits.ELimitCalculatorType.Switch;
                        }
                        break;
                    }
                case nameof(LimitCalculatorTriggerCompare._minMiddleMaxState):
                    {
                        if (currentObj._limitCalculatorTrigger)
                        {
                            return currentObj._limitCalculatorTrigger._limitCalculatorType == PluginPhysicses.Base.Limits.ELimitCalculatorType.MinMiddleMax;
                        }
                        break;
                    }
            }
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }
    }
}
