using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Views.ScrollViews;

namespace XCSJ.PluginsCameras.UI
{
    /// <summary>
    /// 相机视图项数据
    /// </summary>
    [Serializable]
    [Name("相机视图项数据")]
    public class CameraViewItemData : ViewItemData
    {
        /// <summary>
        /// 相机控制器
        /// </summary>
        [Name("相机控制器")]
        [Readonly(EEditorMode.Runtime)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public BaseCameraMainController _cameraController = null;

        private float _duration = 1f;

        /// <summary>
        /// 标题:
        /// 1、首先获取输入的标题。
        /// 2、1为空时，使用相机控制器拥有者所在游戏对象的名称
        /// 3、2为空时，使用相机控制器所在游戏对象的名称
        /// </summary>
        public override string title
        {
            get
            {
                if (string.IsNullOrEmpty(base.title) && _cameraController)
                {
                    return _cameraController.ownerGameObject ? _cameraController.ownerGameObject.name : _cameraController.name;
                }
                return base.title;
            }
            set => base.title = value;
        }

        /// <summary>
        /// 是否选中
        /// </summary>
        public override bool selected
        {
            get
            {
                if (_cameraController)
                {
                    base.selected = _cameraController.gameObject.activeInHierarchy;
                }
                return base.selected;
            }
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public CameraViewItemData() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cameraController"></param>
        public CameraViewItemData(BaseCameraMainController cameraController)
        {
            this._cameraController = cameraController;
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="viewSize"></param>
        public CameraViewItemData InitData(float duration, Vector2Int viewSize)
        {
            _duration = duration;

            RenderView(viewSize);

            return this;
        }

        /// <summary>
        /// 更新画面渲染
        /// </summary>
        /// <param name="viewSize"></param>
        public void RenderView(Vector2Int viewSize)
        {
            if (!_cameraController || !_cameraController.cameraEntityController) return;

            var cam = _cameraController.cameraEntityController.mainCamera;
            if (!cam) return;

            var recorder = new GameObjectRecorder();
            try
            {
                var parentGameObjects = CommonFun.GetParentsGameObject(cam.gameObject, true);

                recorder.Record(parentGameObjects);
                parentGameObjects.ForEach(go => go.SetActive(true));

                var orgCamEnable = cam.enabled;
                {
                    cam.enabled = true;
                    sprite = cam.Render(viewSize).RenderTextureToSprite();
                }
                cam.enabled = orgCamEnable;
            }
            finally
            {
                recorder.Recover();
            }
        }

        /// <summary>
        /// 点击
        /// </summary>
        public override void OnClick()
        {
            base.OnClick();

            var manager = CameraManager.instance;
            if (manager)
            {
                manager.GetProvider().SwitchCameraController(_cameraController, _duration, null, true);
            }
        }
    }
}
