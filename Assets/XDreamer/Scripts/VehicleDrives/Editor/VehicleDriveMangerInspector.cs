using UnityEditor;
using System.Collections.Generic;
using XCSJ.Algorithms;
using XCSJ.EditorCameras.Base;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Controls;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.EditorTools;
using XCSJ.PluginVehicleDrive.Controllers;
using UnityEngine;
using XCSJ.PluginVehicleDrive;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.EditorExtension.Base;

namespace XCSJ.EditorVehicleDrive
{
    /// <summary>
    /// 相机移动器检查器
    /// </summary>
    [CustomEditor(typeof(VehicleDriveManger), true)]
    public class VehicleDriveMangerInspector : BaseInspectorWithLimit<VehicleDriveManger>
    {
        /// <summary>
        /// 目录列表
        /// </summary>
        public static XObject<CategoryList> categoryList { get; } = new XObject<CategoryList>(cl => cl != null, x => EditorToolsHelper.GetWithPurposes(nameof(VehicleDriveManger)));

        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            CategoryListExtension.DrawVertical(categoryList);

            DrawVehicleControllerList();
        }

        /// <summary>
        /// 车辆控制器列表
        /// </summary>
        [Name("车辆控制器列表")]
        [Tip("当前场景中所有的车辆控制器对象")]
        public bool _display = true;

        private static XGUIStyle _centerLableStyle { get; } = new XGUIStyle(nameof(GUI.skin.label), s =>
          {
              s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(VehicleDriveMangerInspector) + "_centerLableStyle");
              s.alignment = TextAnchor.MiddleCenter;
          });

        private void DrawVehicleControllerList()
        {
            _display = UICommonFun.Foldout(_display, CommonFun.NameTip(GetType(), nameof(_display)));
            if (!_display) return;

            CommonFun.BeginLayout();

            // 标题
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            {
                GUILayout.Label("NO.", UICommonOption.Width32);
                GUILayout.Label(CommonFun.TempContent("车辆控制器", "车辆控制器所在的游戏对象；本项只读；"));
                GUILayout.Label(CommonFun.TempContent("车轮数量"), UICommonOption.Width48);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            // 列表
            var cache = ComponentCache.Get(typeof(VehicleController), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as VehicleController;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //车辆控制器
                EditorGUILayout.ObjectField(component.gameObject, typeof(GameObject), true);

                //车轮数量
                EditorGUILayout.LabelField(component.GetComponentsInChildren<WheelDriver>().Length.ToString(), _centerLableStyle, UICommonOption.Width48);

                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }
    }
}
