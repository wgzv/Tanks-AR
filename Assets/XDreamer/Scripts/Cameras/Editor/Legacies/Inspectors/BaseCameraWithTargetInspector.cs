using System;
using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCamera;

namespace XCSJ.PluginsCameras.Legacies.Inspectors
{
    /// <summary>
    /// 基础带目标的相机检查器
    /// </summary>
    [CustomEditor(typeof(BaseCameraWithTarget), true)]
    [Obsolete("产品功能升级，不推荐用户再使用本功能组件，请使用新的功能组件替代！", false)]
    public class BaseCameraWithTargetInspector : BaseCameraWithTargetInspector<BaseCameraWithTarget> { }

    /// <summary>
    /// 基础带目标的相机检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Obsolete("产品功能升级，不推荐用户再使用本功能组件，请使用新的功能组件替代！", false)]
    public class BaseCameraWithTargetInspector<T> : BaseCameraInspector<T> where T : BaseCameraWithTarget
    {
        /// <summary>
        /// 当对象的成员属性字段修改时回调
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberProperty"></param>
        /// <param name="arrayElementInfo"></param>
        /// <returns></returns>
        public override bool OnModifiedPropertyField(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            if (memberProperty.name == nameof(targetObject.distance))
            {
                //Debug.Log("memberProperty.floatValue: " + memberProperty.floatValue);
                targetObject.LookAtTarget(memberProperty.floatValue);
            }
            return base.OnModifiedPropertyField(type, memberProperty, arrayElementInfo);
        }
    }
}
