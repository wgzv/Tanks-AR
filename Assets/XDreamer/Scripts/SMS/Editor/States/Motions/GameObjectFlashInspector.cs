using UnityEditor;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    /// <summary>
    /// 游戏对象闪烁检查器
    /// </summary>
    [CustomEditor(typeof(GameObjectFlash))]
    public class GameObjectFlashInspector : FlashInspector<GameObjectFlash>
    {
    }
}
