using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginXBox.Tools;

namespace XCSJ.EditorXBox.Tools
{
    /// <summary>
    /// 变换通过XBox检查器
    /// </summary>
    [CustomEditor(typeof(TransformByXBox), true)]
    public class TransformByXBoxInspector : MBInspector<TransformByXBox>
    {
        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            EditorXBoxHelper.DrawSelectXBoxManager();

            if (GUILayout.Button("设置为默认 移动"))
            {
                targetObject.SetDefaultMove();
            }
            if (GUILayout.Button("设置为默认 世界Y旋转"))
            {
                targetObject.SetDefaultRotateWorldY();
            }
            if (GUILayout.Button("设置为默认 本地X旋转"))
            {
                targetObject.SetDefaultRotateLocalX();
            }
        }
    }
}
