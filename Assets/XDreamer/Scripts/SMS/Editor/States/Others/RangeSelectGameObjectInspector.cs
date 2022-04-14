using System;
using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.States.Base;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.Others;
using XCSJ.PluginSMS.States.Selections;

namespace XCSJ.EditorSMS.States.Others
{
    [CustomEditor(typeof(RangeSelectGameObject))]
    public class RangeSelectGameObjectInspector : WorkClipInspector<RangeSelectGameObject>
    {

    }
}

