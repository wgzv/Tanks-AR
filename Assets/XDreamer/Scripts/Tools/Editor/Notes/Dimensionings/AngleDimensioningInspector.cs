using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using XCSJ.PluginTools.Notes.Dimensionings;

namespace XCSJ.EditorTools.Notes.Dimensionings
{
    /// <summary>
    /// 角度尺寸标注检查器
    /// </summary>
    [CustomEditor(typeof(AngleDimensioning))]
    public class AngleDimensioningInspector : DimensioningInspector<AngleDimensioning>
    {
    }
}
