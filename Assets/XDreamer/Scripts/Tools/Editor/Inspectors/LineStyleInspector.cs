using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginTools.LineNotes;

namespace XCSJ.EditorTools.Inspectors
{
    [CustomEditor(typeof(LineStyle))]
    public class LineStyleInspector : BaseInspectorWithLimit<LineStyle>
    {
        public override void OnEnable()
        {
            if (targetObject.mat == null)
            {
                targetObject.mat = AssetDatabase.LoadAssetAtPath(UICommonFun.Assets + "/XDreamer-Assets/基础/Materials/常用/ColoredBlended.mat", typeof(Material)) as Material;
            }
        }
    }
}
