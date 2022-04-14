using System;
using System.Collections.Generic;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
using XCSJ.PluginCommonUtils.Tools;
using UnityEngine;

namespace XCSJ.PluginsCameras.Tools.Controllers
{
    /// <summary>
    /// 启用组件通过事件
    /// </summary>
    [Name("启用组件通过事件")]
    [Tool(CameraHelperExtension.ControllersCategoryName, nameof(CameraController))]
    [XCSJ.Attributes.Icon(EIcon.Component)]
    [DisallowMultipleComponent]
    public class EnableComponentByEvent : BaseCameraToolController
    {
        /// <summary>
        /// 启用组件信息列表
        /// </summary>
        [Name("启用组件信息列表")]
        public List<EnableComponentInfoList> _infoLists = new List<EnableComponentInfoList>();

        #region 事件

        /// <summary>
        /// 当将要开始切换相机控制器之前回调，即将由旧相机控制器切换到新相机控制器；
        /// </summary>
        /// <param name="from">旧相机控制器(即当前相机控制器)</param>
        /// <param name="to">新相机控制器</param>
        public override void OnBeginSwitch(BaseCameraMainController from, BaseCameraMainController to)
        {
            base.OnBeginSwitch(from, to);
            if (from != cameraController) return;

            Handle(ECameraControllerEvent.OnBeginSwitch);
        }

        /// <summary>
        /// 当将要结束切换之前回调的事件；即旧相机控制器(即当前相机控制器)已经切换到新相机控制器的位置与旋转（如果需要补间）之后回调；
        /// </summary>
        /// <param name="from">旧相机控制器(即当前相机控制器)</param>
        /// <param name="to">新相机控制器</param>
        public override void OnWillEndSwitch(BaseCameraMainController from, BaseCameraMainController to)
        {
            base.OnWillEndSwitch(from, to);
            if (from != cameraController) return;

            Handle(ECameraControllerEvent.OnWillEndSwitch);
        }

        /// <summary>
        /// 当将要切换为上一个相机控制器之前回调的事件；将要切换为非当前相机前回调；
        /// </summary>
        /// <param name="from">旧相机控制器(即当前相机控制器)</param>
        public override void OnWillSwitchToLast(BaseCameraMainController from)
        {
            //base.OnWillSwitchToLast(from);
            if (from != cameraController) return;

            Handle(ECameraControllerEvent.OnWillSwitchToLast);
        }

        /// <summary>
        /// 当已切换为当前相机控制器之后回调的事件；新相机控制器已经被设置为当前相机控制器之后的回调；
        /// </summary>
        /// <param name="to">新相机控制器(即当前相机控制器)</param>
        public override void OnSwitchedToCurrent(BaseCameraMainController to)
        {
            //base.OnSwitchedToCurrent(to);
            if (to != cameraController) return;

            Handle(ECameraControllerEvent.OnSwitchedToCurrent);
        }

        /// <summary>
        /// 当将要已经切换相机控制器之后回调的事件；
        /// </summary>
        /// <param name="from">旧相机控制器</param>
        /// <param name="to">新相机控制器(即当前相机控制器)</param>
        public override void OnEndSwitch(BaseCameraMainController from, BaseCameraMainController to)
        {
            base.OnEndSwitch(from, to);
            if (to != cameraController) return;

            Handle(ECameraControllerEvent.OnEndSwitch);
        }

        /// <summary>
        /// 当相机主控制器组件启用时回调的事件；
        /// </summary>
        /// <param name="cameraController"></param>
        public override void OnEnabled(BaseCameraMainController cameraController)
        {
            //base.OnEnabled(cameraController);
            if (this.cameraController != cameraController) return;

            Handle(ECameraControllerEvent.OnEnabled);
        }

        /// <summary>
        /// 当相机主控制器组件禁用时回调的事件；
        /// </summary>
        /// <param name="cameraController"></param>
        public override void OnDisabled(BaseCameraMainController cameraController)
        {
            //base.OnEnabled(cameraController);
            if (this.cameraController != cameraController) return;

            Handle(ECameraControllerEvent.OnEndSwitch);
        }

        #endregion

        private void Handle(ECameraControllerEvent cameraControllerEvent)
        {
            foreach (var infos in _infoLists)
            {
                infos.Enable(cameraControllerEvent);
            }
        }

        /// <summary>
        /// 启用组件信息列表
        /// </summary>
        [Serializable]
        public class EnableComponentInfoList : EnableComponentInfoList<ECameraControllerEvent>
        {
        }
    }
}
