using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using XCSJ.EditorTools.Base;
using XCSJ.PluginTools.Notes.Dimensionings;

namespace XCSJ.EditorTools.Notes.Dimensionings
{
    /// <summary>
    /// 尺寸标注检查器
    /// </summary>
    [CustomEditor(typeof(Dimensioning))]
    public class DimensioningInspector : DimensioningInspector<Dimensioning> { }

    /// <summary>
    /// 尺寸标注检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DimensioningInspector<T> : ToolMBInspector<T> where T : Dimensioning
    {
    }
}
