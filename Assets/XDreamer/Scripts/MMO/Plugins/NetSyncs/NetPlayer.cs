using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Interfaces;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Legacies;
using XCSJ.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginMMO.NetSyncs
{
    /// <summary>
    /// 网络玩家
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Authentication)]
    [DisallowMultipleComponent]
    [Name("网络玩家")]
    [Tool(MMOHelper.CategoryName, MMOHelper.ToolPurpose, rootType = typeof(MMOManager))]
    public sealed class NetPlayer : NetProperty, INetPlayer
    {
        #region INetPlayer接口实现

        /// <summary>
        /// 昵称
        /// </summary>
        [Name("昵称")]
        public string nickName
        {
            get => GetProperty(nameof(nickName))?.value;
            set => SetProperty(nameof(nickName), value);
        }

        /// <summary>
        /// 本地玩家
        /// </summary>
        public NetPlayer localPlayer => LocalCache.GetLocalPlayer()?.netPlayer as NetPlayer;

        INetPlayer INetPlayer.localPlayer => LocalCache.GetLocalPlayer()?.netPlayer;

        #endregion

        #region 相机处理

        /// <summary>
        /// 相机版本规则
        /// </summary>
        [Name("相机版本规则")]
        [EnumPopup]
        public ECameraVersionRule _cameraVersionRule = ECameraVersionRule.NewFirst;

        /// <summary>
        /// 通过规则查找相机
        /// </summary>
        private void FindCameraByRule()
        {
            switch (_cameraVersionRule)
            {
                case ECameraVersionRule.NewFirst:
                    {
                        if (!_playerCameraController)
                        {
                            _playerCameraController = GetComponentInChildren<BaseCameraMainController>(true);

                            //如果玩家相机控制器任然无效，则尝试查找相机
                            if (!_playerCameraController && !_playerCamera)
                            {
                                _playerCamera = GetComponentInChildren<Camera>(true);

                            }
                        }
                        break;
                    }
                case ECameraVersionRule.New:
                    {
                        if (!_playerCameraController)
                        {
                            _playerCameraController = GetComponentInChildren<BaseCameraMainController>(true);
                        }
                        break;
                    }
                case ECameraVersionRule.Legacy:
                    {
                        if (!_playerCamera)
                        {
                            _playerCamera = GetComponentInChildren<Camera>(true);
                        }
                        break;
                    }
            }
        }

        private void CameraStartLink()
        {
            switch (_cameraVersionRule)
            {
                case ECameraVersionRule.NewFirst:
                    {
                        if (_playerCameraController)
                        {
                            NewCameraStartLink();
                        }
                        else
                        {
                            LegacyCameraStartLink();
                        }
                        break;
                    }
                case ECameraVersionRule.New:
                    {
                        NewCameraStartLink();
                        break;
                    }
                case ECameraVersionRule.Legacy:
                    {
                        LegacyCameraStartLink();
                        break;
                    }
            }
        }

        private void CameraStopLink()
        {
            switch (_cameraVersionRule)
            {
                case ECameraVersionRule.NewFirst:
                    {
                        if (_playerCameraController)
                        {
                            NewCameraStopLink();
                        }
                        else
                        {
                            LegacyCameraStopLink();
                        }
                        break;
                    }
                case ECameraVersionRule.New:
                    {
                        NewCameraStopLink();
                        break;
                    }
                case ECameraVersionRule.Legacy:
                    {
                        LegacyCameraStopLink();
                        break;
                    }
            }
        }

        #region 新版相机设置

        /// <summary>
        /// 玩家相机控制器
        /// </summary>
        [Group("新版相机设置", defaultIsExpanded = false)]
        [Name("玩家相机控制器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public BaseCameraMainController _playerCameraController;

        /// <summary>
        /// 相机目标处理规则
        /// </summary>
        [Name("相机目标处理规则")]
        [EnumPopup]
        public ECameraTargetHandleRule _cameraTargetHandleRule = ECameraTargetHandleRule.None;

        /// <summary>
        /// 相机目标处理规则
        /// </summary>
        [Name("相机目标处理规则")]
        public enum ECameraTargetHandleRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 无
            /// </summary>
            [Name("主目标")]
            [Tip("仅将主目标设置到玩家相机控制器对应的主目标")]
            MainTarget,

            /// <summary>
            /// 目标列表
            /// </summary>
            [Name("目标列表")]
            [Tip("仅将目标列表设置到玩家相机控制器对应的目标列表")]
            Targets,

            /// <summary>
            /// 二者
            /// </summary>
            [Name("二者")]
            [Tip("将主目标与目标列表设置到玩家相机控制器对应的主目标与目标列表")]
            Both,
        }

        /// <summary>
        /// 相机主目标
        /// </summary>
        [Name("相机主目标")]
        [HideInSuperInspector(nameof(_cameraTargetHandleRule), EValidityCheckType.Equal, ECameraTargetHandleRule.None)]
        public Transform _cameraMainTarget;

        [Name("相机目标列表")]
        [HideInSuperInspector(nameof(_cameraTargetHandleRule), EValidityCheckType.Equal, ECameraTargetHandleRule.None)]
        public List<Transform> _cameraTargets = new List<Transform>();

        private BaseCameraMainController _prveCameraController;
        private Dictionary<ICameraTargetController, Transform> _tmpCameraMainTarget = new Dictionary<ICameraTargetController, Transform>();
        private Dictionary<ICameraTargetController, Transform[]> _tmpCameraTargets = new Dictionary<ICameraTargetController, Transform[]>();

        private void NewCameraStartLink()
        {
            _prveCameraController = null;
            _tmpCameraMainTarget.Clear();
            _tmpCameraTargets.Clear();

            if (isLocalPlayer)
            {
                //启用玩家相机控制器
                if (_playerCameraController)
                {
                    var cameraManager = CameraManager.instance;
                    if (cameraManager)
                    {
                        //缓存当前相机控制器
                        _prveCameraController = cameraManager.GetCurrentCameraController();

                        //切换到当前玩家相机控制器
                        cameraManager.SwitchCameraController(_playerCameraController, 0, null, true);
                    }
                    if (!_prveCameraController)
                    {
                        var camera = Camera.main;
                        if (camera)
                        {
                            _prveCameraController = camera.GetComponentInParent<BaseCameraMainController>();
                        }
                    }
                    //将之前的相机控制器游戏对象强制禁用
                    if (_prveCameraController)
                    {
                        _prveCameraController.gameObject.SetActive(false);
                    }
                    else
                    {
                        Debug.LogWarning("前置相机控制器无效！");
                    }

                    //保证玩家相机控制器的可用性
                    _playerCameraController.enabled = true;
                    _playerCameraController.gameObject.SetActive(true);

                    //设置玩家相机控制器相关信息
                    foreach (var t in _playerCameraController.GetComponentsInChildren<ICameraTargetController>())
                    {
                        _tmpCameraMainTarget[t] = t.mainTarget;
                        _tmpCameraTargets[t] = t.targets;

                        switch (_cameraTargetHandleRule)
                        {
                            case ECameraTargetHandleRule.MainTarget:
                                {
                                    t.mainTarget = _cameraMainTarget;
                                    break;
                                }
                            case ECameraTargetHandleRule.Targets:
                                {
                                    t.targets = _cameraTargets.ToArray();
                                    break;
                                }
                            case ECameraTargetHandleRule.Both:
                                {
                                    t.mainTarget = _cameraMainTarget;
                                    t.targets = _cameraTargets.ToArray();
                                    break;
                                }
                        }
                    }
                }
            }
            else
            {
                //禁用玩家相机控制器
                if (_playerCameraController)
                {
                    var localPlayer = this.localPlayer;
                    if (localPlayer && localPlayer._playerCameraController == _playerCameraController)
                    {
                        //多个玩家共用一个相机控制器，不做处理
                    }
                    if (_playerCameraController.cameraEntityController.mainCamera == Camera.main)
                    {
                        //如果玩家控制器的主相机是当前正在使用相机，不做处理
                    }
                    else
                    {
                        _playerCameraController.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void NewCameraStopLink()
        {
            //还原玩家相机控制器相关信息
            if (_playerCameraController)
            {
                foreach (var kv in _tmpCameraMainTarget)
                {
                    kv.Key.mainTarget = kv.Value;
                }
                _tmpCameraMainTarget.Clear();
                foreach (var kv in _tmpCameraTargets)
                {
                    kv.Key.targets = kv.Value;
                }
                _tmpCameraTargets.Clear();
            }

            //还原之前相机控制器
            if (_prveCameraController)
            {
                var manager = CameraManager.instance;
                if (manager && manager.SwitchCameraController(_prveCameraController, 0, null, true))
                {
                    //切换成功
                }

                //保证之前相机控制器的可用性
                _prveCameraController.gameObject.SetActive(true);
                _prveCameraController.enabled = true;
            }
            else
            {
                //切换为初始相机控制器
                var manager = CameraManager.instance;
                if (manager)
                {
                    manager.SwitchCameraController(manager.GetInitCameraController(), 0, null, true);
                }
            }
        }

        #endregion

        #region 旧版相机设置

        /// <summary>
        /// 玩家相机
        /// </summary>
        [Group("旧版相机设置(不推荐使用，仅用于兼容已编辑的旧版相机场景)", EColor.Red, defaultIsExpanded = false)]
        [Name("玩家相机")]
        [FormerlySerializedAs(nameof(playerCamera))]
        public Camera _playerCamera;

        /// <summary>
        /// 玩家相机
        /// </summary>
        public Camera playerCamera => _playerCamera;

        /// <summary>
        /// 相机目标:如果相机目标无效时，使用当前网络玩家组件的变换对象为相机目标对象
        /// </summary>
        [Name("相机目标")]
        [Tip("如果相机目标无效时，使用当前网络玩家组件的变换对象为相机目标对象")]
        public Transform _cameraTarget;

        /// <summary>
        /// 相机目标
        /// </summary>
        public Transform cameraTarget
        {
            get
            {
                return _cameraTarget ? _cameraTarget : transform;
            }
        }

        private Camera _prveCamera;

        private Dictionary<ITarget, object> _targets = new Dictionary<ITarget, object>();

        /// <summary>
        /// 旧版相机开始关联
        /// </summary>
        private void LegacyCameraStartLink()
        {
            _prveCamera = null;
            _targets.Clear();
            if (isLocalPlayer)
            {
                //启用玩家相机
                if (playerCamera)
                {
                    var cameraManager = CameraManager.instance;
                    if (cameraManager)
                    {
                        //缓存当前相机
                        var current = cameraManager.GetCurrentCamera();
                        if (current)
                        {
                            _prveCamera = current.camera;
                        }
                        cameraManager.SwitchCamera(playerCamera.gameObject, 0, "", true);
                    }
                    if (!_prveCamera)
                    {
                        _prveCamera = Camera.main;
                    }
                    if (_prveCamera)
                    {
                        _prveCamera.gameObject.SetActive(false);
                    }

                    //保证玩家相机的可用性
                    playerCamera.enabled = true;
                    playerCamera.gameObject.SetActive(true);

                    //设置玩家相机相关信息
                    foreach (var t in playerCamera.GetComponents<ITarget>())
                    {
                        _targets[t] = t.target;
                        t.target = cameraTarget;
                    }
                }
            }
            else
            {
                //禁用玩家相机
                if (playerCamera)
                {
                    var localPlayer = this.localPlayer;
                    if (localPlayer && localPlayer.playerCamera == playerCamera)
                    {
                        //多个玩家共用一个相机，不做处理
                    }
                    if (playerCamera == Camera.main)
                    {
                        //如果玩家相机是当前的主相机，不做处理
                    }
                    else
                    {
                        playerCamera.gameObject.SetActive(false);
                    }
                }
            }
        }

        /// <summary>
        /// 旧版相机停止关联
        /// </summary>
        private void LegacyCameraStopLink()
        {
            if (isLocalPlayer)
            {
                //还原玩家相机相关信息
                if (playerCamera)
                {
                    foreach (var kv in _targets)
                    {
                        kv.Key.target = kv.Value;
                    }
                    _targets.Clear();
                }

                //还原之前相机
                if (_prveCamera)
                {
                    if (CameraManager.instance && CameraManager.instance.SwitchCamera(_prveCamera.gameObject, 0, "", true))
                    {
                        //切换成功
                    }

                    //保证之前相机的可用性
                    _prveCamera.gameObject.SetActive(true);
                    _prveCamera.enabled = true;
                }
                else
                {
                    if (CameraManager.instance)
                    {
                        CameraManager.instance.SwitchCamera(CameraManager.instance.GetInitCamera(), 0, "", true);
                    }
                }
            }
        }

        #endregion

        #endregion

        #region MB方法

        /// <summary>
        /// 唤醒初始化
        /// </summary>
        public void Awake()
        {
            FindCameraByRule();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            FindCameraByRule();
            nickName = Product.Name;
        }

        #endregion

        #region INetEvent接口实现

        /// <summary>
        /// 当前对象与网络环境中的玩家产生关联时回调
        /// </summary>
        public override void OnStartPlayerLink()
        {
            base.OnStartPlayerLink();
            CameraStartLink();
        }

        /// <summary>
        /// 当前对象与网络环境中的玩家解除关联时回调
        /// </summary>
        public override void OnStopPlayerLink()
        {
            base.OnStopPlayerLink();
            CameraStopLink();
        }

        #endregion
    }
}
