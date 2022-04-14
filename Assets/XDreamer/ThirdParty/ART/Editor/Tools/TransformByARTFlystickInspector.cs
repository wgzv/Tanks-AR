using UnityEditor;
using UnityEngine;
using XCSJ.EditorCameras.Base;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginART;
using XCSJ.PluginART.Tools;

namespace XCSJ.EditorART.Tools
{
    /// <summary>
    /// 变换通过ART Flystick检查器
    /// </summary>
    [CustomEditor(typeof(TransformByARTFlystick), true)]
    public class TransformByARTFlystickInspector : BaseTransformByARTInspector<TransformByARTFlystick>
    {
        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

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
