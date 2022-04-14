using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.PluginVehicleDrive.Controllers;

namespace XCSJ.EditorVehicleDrive
{
    /// <summary>
    /// 车辆驱动属性面板
    /// </summary>
    [CustomEditor(typeof(WheelDriver))]
    [CanEditMultipleObjects]
    public class WheelDriverInspector : BaseInspectorWithLimit<WheelDriver>
    {
        /// <summary>
        /// 目录列表
        /// </summary>
        public static XObject<CategoryList> categoryList { get; } = new XObject<CategoryList>(cl => cl != null, x => EditorToolsHelper.GetWithPurposes(nameof(WheelDriver)));

        /// <summary>
        /// 横向绘制属性
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberProperty"></param>
        /// <param name="arrayElementInfo"></param>
        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(WheelDriver._wheelModel):
                    {
                        EditorGUI.BeginDisabledGroup(!targetObject._wheelModel);
                        if (GUILayout.Button("与模型对齐", EditorStyles.miniButtonRight, UICommonOption.Width60))
                        {
                            targetObject.AlignWithModel();
                        }
                        EditorGUI.EndDisabledGroup();
                        break;
                    }
                default:
                    break;
            }
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }

        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            CategoryListExtension.DrawVertical(categoryList);
        }
    }
}
