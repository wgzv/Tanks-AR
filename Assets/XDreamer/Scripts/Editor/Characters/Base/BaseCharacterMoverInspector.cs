using UnityEditor;
using XCSJ.Extension.Characters.Base;

namespace XCSJ.EditorExtension.Characters.Base
{
    /// <summary>
    /// 基础角色移动器检查器
    /// </summary>
    [CustomEditor(typeof(BaseCharacterMover), true)]
    public class BaseCharacterMoverInspector : BaseCharacterMoverInspector<BaseCharacterMover>
    {
    }

    /// <summary>
    /// 基础角色移动器检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseCharacterMoverInspector<T> : BaseCharacterCoreControllerInspector<T>
       where T : BaseCharacterMover
    {
    }
}

