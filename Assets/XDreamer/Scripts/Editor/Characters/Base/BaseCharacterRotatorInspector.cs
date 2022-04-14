using System;
using UnityEditor;
using XCSJ.Extension.Characters.Base;

namespace XCSJ.EditorExtension.Characters.Base
{
    /// <summary>
    /// 基础角色旋转器检查器
    /// </summary>
    [CustomEditor(typeof(BaseCharacterRotator), true)]
    public class BaseCharacterRotatorInspector : BaseCharacterRotatorInspector<BaseCharacterRotator>
    {
    }

    /// <summary>
    /// 基础角色旋转器检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseCharacterRotatorInspector<T> : BaseCharacterCoreControllerInspector<T>
       where T : BaseCharacterRotator
    {
    }
}

