using UnityEditor;
using UnityEngine;
using XCSJ.EditorCameras.Base;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginOptiTrack;
using XCSJ.PluginOptiTrack.Tools;

namespace XCSJ.EditorOptiTrack.Tools
{
    /// <summary>
    /// 基础变换通过OptiTrack检查器
    /// </summary>
    [CustomEditor(typeof(BaseTransformByOptiTrack), true)]
    public class BaseTransformByOptiTrackInspector : BaseTransformByOptiTrackInspector<BaseTransformByOptiTrack>
    {
    }

    /// <summary>
    /// 基础变换通过OptiTrack检查器泛型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseTransformByOptiTrackInspector<T> : MBInspector<T>
        where T : BaseTransformByOptiTrack
    {
        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            EditorOptiTrackHelper.DrawSelectOptiTrackManager();
        }
    }
}
