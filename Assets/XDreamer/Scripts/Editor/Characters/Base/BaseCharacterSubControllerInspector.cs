using UnityEditor;
using XCSJ.EditorExtension.Base.Controllers;
using XCSJ.Extension.Characters.Base;

namespace XCSJ.EditorExtension.Characters.Base
{
    /// <summary>
    /// 基础角色子控制器检查器
    /// </summary>
    [CustomEditor(typeof(BaseCharacterSubController), true)]
    public class BaseCharacterSubControllerInspector : BaseCharacterSubControllerInspector<BaseCharacterSubController>
    {
    }

    /// <summary>
    /// 基础角色子控制器检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseCharacterSubControllerInspector<T> : BaseSubControllerInspector<T>
       where T : BaseCharacterSubController
    {
    }

    /// <summary>
    /// 基础角色核心控制器检查器
    /// </summary>
    [CustomEditor(typeof(BaseCharacterCoreController), true)]
    public class BaseCharacterCoreControllerInspector : BaseCharacterCoreControllerInspector<BaseCharacterCoreController>
    {
    }

    /// <summary>
    /// 基础角色核心控制器检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseCharacterCoreControllerInspector<T> : BaseCharacterSubControllerInspector<T>
       where T : BaseCharacterCoreController
    {
    }

    /// <summary>
    /// 基础角色工具控制器检查器
    /// </summary>
    [CustomEditor(typeof(BaseCharacterToolController), true)]
    public class BaseCharacterToolControllerInspector : BaseCharacterToolControllerInspector<BaseCharacterToolController>
    {
    }

    /// <summary>
    /// 基础角色工具控制器检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseCharacterToolControllerInspector<T> : BaseCharacterSubControllerInspector<T>
       where T : BaseCharacterToolController
    {
    }
}
