using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;

namespace XCSJ.PluginsCameras.UI
{
    /// <summary>
    /// 渲染贴图相机：用于渲染游戏对象贴图的相机
    /// </summary>
    [Name(RenderTextureCamera.Title)]
    [Tip("用于渲染游戏对象贴图的相机")]
    [RequireComponent(typeof(Camera))]
    [RequireManager(typeof(CameraManager))]
    public class RenderTextureCamera : ToolMB
    {
        /// <summary>
        /// 渲染贴图相机名称
        /// </summary>
        public const string Title = "渲染贴图相机";

        /// <summary>
        /// 渲染贴图相机层
        /// </summary>
        public const string Layer = Product.Name + nameof(RenderTextureCamera);

        /// <summary>
        /// 渲染贴图相机
        /// </summary>
        public Camera renderCamera
        {
            get
            {
                if (!_renderCamera)
                {
                    _renderCamera = CommonFun.GetOrAddComponent<Camera>(gameObject);
                    _renderCamera.clearFlags = CameraClearFlags.Depth; // 用于渲染对象时，背景透明
                }
                return _renderCamera;
            }
        }
        private Camera _renderCamera = null;

        /// <summary>
        /// 重置
        /// </summary>
        protected void Reset()
        {
            renderCamera.clearFlags = CameraClearFlags.Depth; // 用于渲染对象时，背景透明
            renderCamera.enabled = false; // 初始禁用
        }
    }
}
