using System;
using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginsCameras.Legacies.Inspectors
{
    /// <summary>
    /// 定点伸缩相机检查器
    /// </summary>
    [CustomEditor(typeof(FixedAroundCamera))]
    [Obsolete("产品功能升级，不推荐用户再使用本功能组件，请使用新的功能组件替代！", false)]
    public class FixedAroundCameraInspector : BaseCameraWithTargetInspector<FixedAroundCamera>
    {
        /// <summary>
        /// 当检测是否需要绘制对象的成员时回调
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberProperty"></param>
        /// <param name="arrayElementInfo"></param>
        /// <returns></returns>
        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case "target":
                case "updateTargetPosition":
                    {
                        return false;
                    }
            }
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }
    }
}
