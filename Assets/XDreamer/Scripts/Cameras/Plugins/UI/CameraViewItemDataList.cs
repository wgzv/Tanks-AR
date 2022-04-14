using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginXGUI.Views.ScrollViews;

namespace XCSJ.PluginsCameras.UI
{
    /// <summary>
    /// 相机列表
    /// </summary>
    [Name("相机列表")]
    [RequireManager(typeof(CameraManager))]
    public class CameraViewItemDataList : ViewItemDataCollectionMB
    {
        /// <summary>
        /// 相机切换时间
        /// </summary>
        [Name("相机切换时间")]
        [Min(0)]
        public float _duration = 1f;

        /// <summary>
        /// 相机视图尺寸
        /// </summary>
        [Name("相机视图尺寸")]
        public Vector2Int _viewSize = new Vector2Int(256, 256);

        /// <summary>
        /// 相机查找规则
        /// </summary>
        [Name("相机查找规则")]
        public ECameraSearchRule _cameraSearchRule = ECameraSearchRule.All;

        /// <summary>
        /// 相机查找规则
        /// </summary>
        public enum ECameraSearchRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 全部
            /// </summary>
            [Name("全部")]
            All,

            /// <summary>
            /// 自定义
            /// </summary>
            [Name("自定义")]
            Custom,

            /// <summary>
            /// 除自定义外全部
            /// </summary>
            [Name("除自定义外全部")]
            AllWithoutCustom,
        }

        /// <summary>
        /// 游戏对象视图列表
        /// </summary>
        [Name("相机列表")]
        public List<CameraViewItemData> _cameraList = new List<CameraViewItemData>();

        private List<CameraViewItemData> _datas = new List<CameraViewItemData>();

        /// <summary>
        /// 视图数据
        /// </summary>
        public override IEnumerable<IViewItemData> datas => _datas.Cast<IViewItemData>();

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            ReloadData();

            base.OnEnable();

            CameraControllerEvent.onEndSwitch += OnEndSwitchCamera;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();

            CameraControllerEvent.onEndSwitch -= OnEndSwitchCamera;
        }

        private void OnEndSwitchCamera(BaseCameraMainController from, BaseCameraMainController to)
        {
            UpdateData();
        }

        // 强行刷新相机列表
        public void ReloadData()
        {
            Clear();

            _datas.Clear();

            switch (_cameraSearchRule)
            {
                case ECameraSearchRule.All:
                    {
                        foreach (var c in ComponentCache.Get(typeof(BaseCameraMainController), true).components)
                        {
                            var cameraController = c as BaseCameraMainController;
                            _datas.Add(new CameraViewItemData(cameraController).InitData(_duration, _viewSize));
                        }
                        break;
                    }
                case ECameraSearchRule.Custom:
                    {
                        foreach (var item in _cameraList)
                        {
                            _datas.Add(item.InitData(_duration, _viewSize));
                        }
                        break;
                    }
                case ECameraSearchRule.AllWithoutCustom:
                    {
                        foreach (var c in ComponentCache.Get(typeof(BaseCameraMainController), true).components)
                        {
                            var cameraController = c as BaseCameraMainController;
                            if (!_cameraList.Any(item => item._cameraController == cameraController))
                            {
                                _datas.Add(new CameraViewItemData(cameraController).InitData(_duration, _viewSize));
                            }
                        }
                        break;
                    }
            }

            Load();
        }

        /// <summary>
        /// 强行更新数据 : 更新相机视图
        /// </summary>
        public override void UpdateData()
        {
            _datas.ForEach(d => d.RenderView(_viewSize));

            viewItemCollection?.UpdateData();
        }
    }
}
