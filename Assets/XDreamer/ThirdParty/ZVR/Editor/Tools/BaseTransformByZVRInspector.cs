using UnityEditor;
using UnityEngine;
using XCSJ.EditorCameras.Base;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginZVR;
using XCSJ.PluginZVR.Tools;

namespace XCSJ.EditorZVR.Tools
{
    /// <summary>
    /// 基础变换通过ZVR检查器
    /// </summary>
    [CustomEditor(typeof(BaseTransformByZVR), true)]
    public class BaseTransformByZVRInspector : BaseTransformByZVRInspector<BaseTransformByZVR>
    {
    }

    /// <summary>
    /// 基础变换通过ZVR检查器泛型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseTransformByZVRInspector<T> : MBInspector<T>
        where T : BaseTransformByZVR
    {
        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            EditorZVRHelper.DrawSelectZVRManager();
        }
    }
}
