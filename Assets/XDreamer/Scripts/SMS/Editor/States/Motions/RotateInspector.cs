using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    [CustomEditor(typeof(Rotate))]
    public class RotateInspector : TransformMotionInspector<Rotate>
    {
    }
}
