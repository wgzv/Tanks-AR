using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.EditorSMS.States.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.Others;
using XCSJ.PluginSMS.States.Selections;
using static XCSJ.PluginSMS.States.Others.Put;

namespace XCSJ.EditorSMS.States.Others
{
    [CustomEditor(typeof(Put))]
    public class PutInspector : StateComponentInspector<Put>
    {

    }
}
