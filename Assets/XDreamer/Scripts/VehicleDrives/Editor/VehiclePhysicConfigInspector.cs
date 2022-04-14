using System;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginVehicleDrive.Controllers;

namespace XCSJ.EditorVehicleDrive
{
    /// <summary>
    /// 车辆控制器属性
    /// </summary>
    [CustomEditor(typeof(VehiclePhysicConfig))]
    public class VehiclePhysicConfigInspector : BaseInspectorWithLimit<VehiclePhysicConfig>
    {
        /// <summary>
        /// 属性垂直后绘制
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberProperty"></param>
        /// <param name="arrayElementInfo"></param>
        public override void OnAfterPropertyFieldVertical(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(VehiclePhysicConfig.COM):
                    {
                        var com = targetObject.COM;
                        if (com && targetObject.vehicleDriver && !com.transform.IsChildOf(targetObject.vehicleDriver.transform))
                        {
                            EditorGUILayout.HelpBox(string.Format("质量中心[{0}]必须是[{1}]的子对象！", com.name, targetObject.vehicleDriver.name), MessageType.Error);
                        }
                        break;
                    }
            }
            base.OnAfterPropertyFieldVertical(type, memberProperty, arrayElementInfo);
        }

    }
}
