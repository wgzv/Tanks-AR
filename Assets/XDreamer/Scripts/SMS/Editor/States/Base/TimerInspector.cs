using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.EditorSMS.States.Base
{
    [CustomEditor(typeof(Timer))]
    public class TimerInspector : WorkClipInspector<Timer>
    {
        public override bool OnNeedHandleGroup(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(WorkClip.useInitData):
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
                case nameof(WorkClip.useInitData):
                case nameof(WorkClip.setPercentOnEntry):
                case nameof(WorkClip.percentOnEntry):
                case nameof(WorkClip.setPercentOnExit):
                case nameof(WorkClip.percentOnExit):
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
