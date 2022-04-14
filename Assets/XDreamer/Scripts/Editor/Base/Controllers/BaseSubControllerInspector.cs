using UnityEditor;
using XCSJ.Extension.Base.Components;

namespace XCSJ.EditorExtension.Base.Controllers
{
    /// <summary>
    /// 基础子控制器检查器
    /// </summary>
    [CustomEditor(typeof(BaseSubController), true)]
    public class BaseSubControllerInspector : BaseSubControllerInspector<BaseSubController>
    {
    }

    /// <summary>
    /// 基础子控制器检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseSubControllerInspector<T> : BaseControllerInspector<T>
       where T : BaseSubController
    {
    }
}
