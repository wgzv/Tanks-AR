using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    [CustomEditor(typeof(RendererRange))]
    [Obsolete(nameof(RendererRange) + "状态组件不再推荐使用")]
    public class RendererRangeInspector : RangeHandleInspector<RendererRange>
    {
        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(stateComponent.leftRange):
                case nameof(stateComponent.inRange):
                case nameof(stateComponent.rightRange):
                    {
                        return false;
                    }
            }
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }
    }
}
