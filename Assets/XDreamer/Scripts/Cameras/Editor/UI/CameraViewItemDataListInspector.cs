using System;
using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginsCameras.UI;

namespace XCSJ.EditorCameras.UI
{

    /// <summary>
    /// 相机移动器检查器
    /// </summary>
    [CustomEditor(typeof(CameraViewItemDataList), true)]
    public class CameraViewItemDataListInspector : MBInspector<CameraViewItemDataList>
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
                case nameof(CameraViewItemDataList._cameraList):
                    {
                        switch (targetObject._cameraSearchRule)
                        {
                            case CameraViewItemDataList.ECameraSearchRule.None:
                            case CameraViewItemDataList.ECameraSearchRule.All:
                                {
                                    return false;
                                }
                        }
                        break;
                    }
            }
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }
    }
}
