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
    /// 距离尺寸标注检查器
    /// </summary>
    [CustomEditor(typeof(DistanceDimensioning))]
    public class DistanceDimensioningInspector: DimensioningInspector<DistanceDimensioning>
    {
    }
}
