﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginsCameras;
using XCSJ.PluginTools.SelectionUtils;
using XCSJ.PluginXGUI;

namespace XCSJ.PluginTools.GameObjects
{
    /// <summary>
    /// 游戏对象视图信息
    /// </summary>
    [Name("游戏对象视图信息")]
    [Tip("游戏对象生成图像时可使用当前组件提供的信息")]
    [Tool("游戏对象", rootType = typeof(ToolsManager))]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Model)]
    public class GameOjectViewInfoMB : ToolMB
    {
        /// <summary>
        /// 图像规则
        /// </summary>
        [Flags]
        public enum EImageRule
        {
            [Name("使用贴图")]
            [EnumFieldName("使用贴图")]
            Texture = 1 << 0,

            [Name("优先使用子对象视图信息")]
            [EnumFieldName("优先使用子对象视图信息")]
            [Tip("当子对象有当前组件时，优先使用子对象去获取图像")]
            UseChildrenInfo = 1 << 1,

            [Name("拍摄")]
            [EnumFieldName("拍摄")]
            [Tip("使用当前对象配置信息来拍摄")]
            Photograph = 1 << 2,
        }

        [Name("生成图像规则")]
        [EnumPopup]
        public EImageRule _imageRule = EImageRule.Photograph;

        [Name("贴图")]
        public Texture2D _texture;

        #region 拍摄信息

        /// <summary>
        /// 使用包围盒中心：当游戏对象包含粒子系统时，求出的包围盒是个包含粒子系统的大值，此时相机生成的图形可能为空
        /// </summary>
        [Name("使用包围盒中心")]
        [Tip("当游戏对象包含粒子系统时，求出的包围盒是个包含粒子系统的大值，此时相机生成的图形可能为空")]
        public bool _useBoundsCenterAsViewCenter = false;


        [Name("自定义相机详细参数")]
        public bool _constumCameraDetailInfo = true;

        /// <summary>
        /// 距离
        /// </summary>
        [Name("相机与对象间距")]
        [Min(0)]
        [HideInSuperInspector(nameof(_constumCameraDetailInfo), EValidityCheckType.False)]
        public float _viewDistance = 10;

        /// <summary>
        /// 角度
        /// </summary>
        [Name("相机绕目标角度")]
        [HideInSuperInspector(nameof(_constumCameraDetailInfo), EValidityCheckType.False)]
        public Vector3 _viewAngle = Vector3.zero;

        /// <summary>
        /// 相机坐标系下位移量
        /// </summary>
        [Name("相机坐标系下位移量")]
        [HideInSuperInspector(nameof(_constumCameraDetailInfo), EValidityCheckType.False)]
        public Vector3 _positonOffsetBaseOnCamera = Vector3.zero;

        #endregion

        [Name("应用包围盒提供者的忽略规则")]
        public bool _applyBoundsProviderIgnoreGameObject = true;

        /// <summary>
        /// 贴图尺寸
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="textureSize"></param>
        /// <param name="texture"></param>
        /// <returns></returns>
        public bool TryGetTexture(Camera camera, Vector2Int textureSize, out Texture2D texture)
        {
            if ((_imageRule & EImageRule.Texture) == EImageRule.Texture)
            {
                if (_texture)
                {
                    texture = _texture;
                    return true;
                }
            }

            if ((_imageRule & EImageRule.UseChildrenInfo) == EImageRule.UseChildrenInfo)
            {
                var infos = GetComponentsInChildren<GameOjectViewInfoMB>().ToList();
                var current = infos.Find(item => item!=this);
                if (current && current.TryGetTexture(camera, textureSize, out texture))
                {
                    return true;
                }
            }

            if ((_imageRule & EImageRule.Photograph) == EImageRule.Photograph)
            {
                texture = CreateRenderTexture(camera, textureSize);
                if (texture)
                {
                    return true;
                }
            }

            texture = default;
            return false;
        }

        /// <summary>
        /// 创建渲染图
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="textureSize"></param>
        /// <returns></returns>
        public Texture2D CreateRenderTexture(Camera camera, Vector2Int textureSize)
        {
            Texture2D rs = null;
            if (camera && textureSize != Vector2Int.zero)
            {
                var ignoreGameObject = new List<GameObject>();
                var center = gameObject.transform.position;
                if (_useBoundsCenterAsViewCenter)
                {
                    var boundsProvider = gameObject.GetComponent<BoundsProvider>();
                    if ( boundsProvider && boundsProvider.TryGetBounds(out var bounds))
                    {
                        center = bounds.center;
                        ignoreGameObject.AddRange(boundsProvider.GetIgnoreGameObjects());
                    }
                    else if (CommonFun.GetBounds(out bounds, gameObject, true, false))
                    {
                        center = bounds.center;
                    }
                }
                center += _constumCameraDetailInfo ? _positonOffsetBaseOnCamera : Vector3.zero;

                var gameObjectRecorder = new List<(GameObject, bool)>();
                ignoreGameObject.ForEach(go =>
                {
                    gameObjectRecorder.Add((go, go.activeSelf));
                    go.SetActive(false);
                });

                var renderTexture = _constumCameraDetailInfo ? camera.Render(textureSize, center, _viewDistance, _viewAngle, gameObject) : camera.Render(textureSize, gameObject, center);
                rs = XGUIHelper.RenderTextureToTexture2D(renderTexture);
                gameObjectRecorder.ForEach(r => r.Item1.SetActive(r.Item2));
            }

            return rs;
        }

        /// <summary>
        /// 尝试渲染
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="textureSize"></param>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static Texture2D GetTexture(Camera camera, Vector2Int textureSize, GameObject gameObject)
        {
            var viewInfo = gameObject.GetComponent<GameOjectViewInfoMB>();
            if (viewInfo && viewInfo.enabled)
            {
                if(viewInfo.TryGetTexture(camera, textureSize, out var texture))
                {
                    return texture;
                }
            }

            return XGUIHelper.RenderTextureToTexture2D(camera.Render(textureSize, gameObject));
        }
    }
}
