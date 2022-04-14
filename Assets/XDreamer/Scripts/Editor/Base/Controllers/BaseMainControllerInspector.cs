﻿using UnityEditor;
using XCSJ.Extension.Base.Components;

namespace XCSJ.EditorExtension.Base.Controllers
{
    /// <summary>
    /// 基础主控制器检查器
    /// </summary>
    [CustomEditor(typeof(BaseMainController), true)]
    public class BaseMainControllerInspector : BaseMainControllerInspector<BaseMainController>
    {
    }

    /// <summary>
    /// 基础主控制器检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseMainControllerInspector<T> : BaseControllerInspector<T>
       where T : BaseMainController
    {
    }
}
