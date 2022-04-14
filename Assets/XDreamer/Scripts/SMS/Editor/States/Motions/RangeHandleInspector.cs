using System;
using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    public class RangeHandleInspector<T> : MotionInspector<T> where T : Motion<T>, IRangeHandle
    {
        public override bool OnNeedHandleGroup(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(WorkClip.loopType):
                case nameof(WorkClip.workCurve):
                    {
                        return false;
                    }
            }
            return base.OnNeedHandleGroup(type, memberProperty, arrayElementInfo);
        }

        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch(memberProperty.name)
            {
                case nameof(WorkClip.loopType):
                case nameof(WorkClip.workCurve):
                    {
                        return false;
                    }
            }
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }
    }
}
