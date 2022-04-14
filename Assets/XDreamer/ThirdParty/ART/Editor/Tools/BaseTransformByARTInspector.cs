using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginART.Tools;

namespace XCSJ.EditorART.Tools
{
    /// <summary>
    /// 基础变换通过ART检查器
    /// </summary>
    [CustomEditor(typeof(BaseTransformByART), true)]
    public class BaseTransformByARTInspector : BaseTransformByARTInspector<BaseTransformByART>
    {
    }

    /// <summary>
    /// 基础变换通过ART检查器泛型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseTransformByARTInspector<T> : MBInspector<T>
        where T : BaseTransformByART
    {
        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            EditorARTHelper.DrawSelectARTManager();
        }
    }
}
