using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.EditorSMS.States.Base
{
    [CustomEditor(typeof(EmptyWorkClip))]
    public class EmptyWorkClipInspector : WorkClipInspector<EmptyWorkClip>
    {
    }
}
