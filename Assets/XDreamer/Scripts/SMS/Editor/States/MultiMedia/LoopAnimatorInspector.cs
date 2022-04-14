using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.MultiMedia;

namespace XCSJ.EditorSMS.States.MultiMedia
{
    [CustomEditor(typeof(LoopAnimator))]
    public class LoopAnimatorInspector : UnityAnimatorInspector<LoopAnimator>
    {
    }
}
