using System;
using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginSMS.Transitions.Base;
using XCSJ.PluginSMS.Transitions.UGUI;

namespace XCSJ.EditorSMS.Transitions.CNScripts
{
    [CustomEditor(typeof(VariableCompareSkip))]
    public class VariableCompareSkipInspector : TransitionComponentInspector
    {
        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(ButtonClickSkip.needWaitInStateFinished):
                    {
                        return false;
                    }
            }
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }
    }
}