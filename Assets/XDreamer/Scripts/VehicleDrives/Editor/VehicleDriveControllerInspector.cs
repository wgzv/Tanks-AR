using UnityEditor;
using XCSJ.Algorithms;
using XCSJ.EditorCameras.Base;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Controls;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.EditorTools;
using XCSJ.PluginVehicleDrive.Controllers;
using UnityEngine;
using XCSJ.PluginVehicleDrive.Base;
using System.Text;
using XCSJ.PluginVehicleDrive;

namespace XCSJ.EditorVehicleDrive
{
    /// <summary>
    /// 相机移动器检查器
    /// </summary>
    [CustomEditor(typeof(VehicleDriver), true)]
    public class VehicleDriveControllerInspector : BaseInspectorWithLimit<VehicleDriver>
    {
        /// <summary>
        /// 目录列表
        /// </summary>
        public static XObject<CategoryList> categoryList { get; } = new XObject<CategoryList>(cl => cl != null, x => EditorToolsHelper.GetWithPurposes(nameof(VehicleDriver)));

        private IEngine engine = null;

        private IBrake brake = null;

        private IGearBox gearBox = null;

        private ISteer steer = null;

        private IFuel fuel = null;

        private IVehicleLightController vehicleLightController = null;

        private bool coreModuleValid = false;

        public override void OnEnable()
        {
            base.OnEnable();

            FindCoreModules();
        }

        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            DrawCoreModuleList();

            CategoryListExtension.DrawVertical(categoryList);
        }

        /// <summary>
        /// 重绘间隔时间
        /// </summary>
        public override float timeRepaintIntervalTime => 1;

        /// <summary>
        /// 重绘
        /// </summary>
        public override bool timedRepaint => true;

        /// <summary>
        /// 定时重绘回调
        /// </summary>
        public override void OnTimedRepaint()
        {
            base.OnTimedRepaint();

            FindCoreModules();
        }

        private void FindCoreModules()
        {
            engine = targetObject.GetComponentInChildren<IEngine>();
            brake = targetObject.GetComponentInChildren<IBrake>();
            gearBox = targetObject.GetComponentInChildren<IGearBox>();
            steer = targetObject.GetComponentInChildren<ISteer>();
            fuel = targetObject.GetComponentInChildren<IFuel>();
            vehicleLightController = targetObject.GetComponentInChildren<IVehicleLightController>();

            coreModuleValid = engine != null && brake != null && gearBox != null && steer != null && fuel != null && vehicleLightController != null;
        }

        private void DrawCoreModuleList()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("车辆核心模块:");
            DrawCoreModuleComponent(VehicleDriveHelper.EngineName, engine);
            DrawCoreModuleComponent(VehicleDriveHelper.BrakeName, brake);
            DrawCoreModuleComponent(VehicleDriveHelper.GearBoxName, gearBox);
            DrawCoreModuleComponent(VehicleDriveHelper.SteerName, steer);
            DrawCoreModuleComponent(VehicleDriveHelper.FuelName, fuel);
            DrawCoreModuleComponent(VehicleDriveHelper.VehicleLightControllerName, vehicleLightController);
            if (!coreModuleValid)
            {
                EditorGUILayout.HelpBox("请在当前对象下创建缺失对象", MessageType.Error);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawCoreModuleComponent(string name, object obj)
        {
            var component = obj as Component;
            var orgColor = GUI.color;
            if (!component)
            {
                GUI.color = Color.red;
            }
            EditorGUILayout.ObjectField(name, component, typeof(Component), false);
            GUI.color = orgColor;
        }
    }
}
