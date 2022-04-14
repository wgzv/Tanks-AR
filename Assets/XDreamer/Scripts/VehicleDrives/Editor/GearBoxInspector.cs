using System;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginVehicleDrive.Controllers;

namespace XCSJ.EditorVehicleDrive
{
    /// <summary>
    /// 变速箱属性
    /// </summary>
    [CustomEditor(typeof(GearBox))]
    public class GearBoxInspector : BaseInspectorWithLimit<GearBox>
    {
        /// <summary>
        /// 变速箱属性水平后绘制
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberProperty"></param>
        /// <param name="arrayElementInfo"></param>
        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(GearBox._totalGearCount):
                    {
                        if (GUILayout.Button("创建齿轮组", EditorStyles.miniButtonRight, UICommonOption.Width60))
                        {
                            UndoHelper.RegisterCompleteObjectUndo(targetObject);
                            targetObject.InitGears();
                        }
                        break;
                    }
                default:
                    break;
            }
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }
    }
}
