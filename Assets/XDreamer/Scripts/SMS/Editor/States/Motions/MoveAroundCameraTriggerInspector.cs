using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.States.Base;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    [CustomEditor(typeof(MoveAroundCameraTrigger))]
    [Obsolete("产品功能升级，不推荐用户再使用本功能组件，请使用新的功能组件替代！", false)]
    public class MoveAroundCameraTriggerInspector : WorkClipInspector<MoveAroundCameraTrigger>
    {
        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch(memberProperty.name)
            {
                case nameof(WorkClip.useInitData):
                case nameof(WorkClip.setPercentOnEntry):
                case nameof(WorkClip.percentOnEntry):
                case nameof(WorkClip.setPercentOnExit):
                case nameof(WorkClip.percentOnExit):
                    {
                        return false;
                    }
            }
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }
    }
}
