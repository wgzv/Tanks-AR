using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.Scripts;

namespace XCSJ.PluginsCameras.Tools.Controllers
{
    /// <summary>
    /// 激活游戏对象通过事件：根据相机控制器的事件触发游戏对象激活或禁用
    /// </summary>
    [Name("激活游戏对象通过事件")]
    [Tip("根据相机控制器的事件触发游戏对象激活或禁用")]
    [Tool(CameraHelperExtension.ControllersCategoryName, nameof(CameraController))]
    [XCSJ.Attributes.Icon(EIcon.Model)]
    [DisallowMultipleComponent]
    public class ActiveGameObjectByEvent : BaseCameraToolController
    {
        /// <summary>
        /// 激活游戏对象信息列表
        /// </summary>
        [Name("激活游戏对象信息列表")]
        public List<ActiveGameObjectInfoList> _infoLists = new List<ActiveGameObjectInfoList>();

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

        /// <summary>
        /// 添加:支持在Unity编辑器中执行撤销与重做；
        /// </summary>
        /// <param name="cameraControllerEvent"></param>
        /// <param name="gameObject"></param>
        /// <param name="active"></param>
        public void Add(ECameraControllerEvent cameraControllerEvent, GameObject gameObject, EBool active)
        {
            if (!gameObject) return;
            this.XModifyProperty(() =>
            {
                var infoList = _infoLists.FirstOrNew(il => il._enumEvent == cameraControllerEvent, il =>
                {
                    il._enumEvent = cameraControllerEvent;
                    _infoLists.Add(il);
                });
                infoList.Add(gameObject, active);
            });
        }

        private void Handle(ECameraControllerEvent cameraControllerEvent)
        {
            foreach(var infos in _infoLists)
            {
                infos.Active(cameraControllerEvent);
            }
        }

        /// <summary>
        /// 激活游戏对象信息列表
        /// </summary>
        [Serializable]
        public class ActiveGameObjectInfoList : ActiveGameObjectInfoList<ECameraControllerEvent>
        {
        }
    }
}
