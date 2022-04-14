using UnityEditor;
using XCSJ.EditorXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.PluginXBox.Tools;

namespace XCSJ.EditorXBox.Tools
{
    /// <summary>
    /// 交互IO通过XBox检查器
    /// </summary>
    [CustomEditor(typeof(InteractIOByXBox), true)]
    public class InteractIOByXBoxInspector: BaseAnalogControllerIOInspector<InteractIOByXBox>
    {
        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            EditorXBoxHelper.DrawSelectXBoxManager();
        }
    }
}
