using UnityEditor;
using XCSJ.EditorXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.PluginART.Tools;

namespace XCSJ.EditorART.Tools
{
    /// <summary>
    /// 交互IO通过ART检查器
    /// </summary>
    [CustomEditor(typeof(InteractIOByART), true)]
    public class InteractIOByARTInspector: BaseAnalogControllerIOInspector<InteractIOByART>
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
