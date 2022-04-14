using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using XCSJ.PluginSMS.States.MultiMedia;

namespace XCSJ.EditorSMS.States.MultiMedia
{
    [CustomEditor(typeof(SingleAnimator))]
    public class SingleAnimatorInspector : UnityAnimatorInspector<SingleAnimator>
    {
    }
}
