using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginsCameras.Base;

namespace XCSJ.PluginsCameras.Controllers
{
    /// <summary>
    /// 相机拥有者：用于标识相机控制器的拥有者，通常在相机控制器所在游戏对象的父级游戏对象上添加；
    /// </summary>
    [Name("相机拥有者")]
    [Tip("用于标识相机控制器的拥有者，通常在相机控制器所在游戏对象的父级游戏对象上添加；")]
    [Tool(CameraHelperExtension.ControllersCategoryName)]
    [RequireManager(typeof(CameraManager))]
    [XCSJ.Attributes.Icon(EIcon.ManHead)]
    public class CameraOwner : MB, ICameraOwner, IComponentHasOwner
    {
        /// <summary>
        /// 拥有者游戏对象
        /// </summary>
        public GameObject ownerGameObject => gameObject;

        /// <summary>
        /// 拥有者
        /// </summary>
        public IComponentOwner owner => this;

        IOwner IHasOwner.owner => this;
    }
}
