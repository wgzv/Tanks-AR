﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;

namespace XCSJ.PluginTools.SelectionUtils
{
    /// <summary>
    /// 选择集边界渲染器
    /// </summary>
    [DisallowMultipleComponent]
    [Name("选择集边界渲染器")]
    public class SelectionBoundsRenderer : SelectionRender<SelectionBoundsRenderer>
    {
        /// <summary>
        /// 颜色
        /// </summary>
        [Name("颜色")]
        [FormerlySerializedAs(nameof(color))]
        public Color _color = Color.green;

        /// <summary>
        /// 颜色
        /// </summary>
        public Color color => _color;

        /// <summary>
        /// 宽度
        /// </summary>
        [Name("宽度")]
        [Range(0, 10)]
        [FormerlySerializedAs(nameof(width))]
        public float _width = 0;

        /// <summary>
        /// 宽度
        /// </summary>
        public float width => _width;

        /// <summary>
        /// 遮挡
        /// </summary>
        [Name("遮挡")]
        [FormerlySerializedAs(nameof(occlusion))]
        public bool _occlusion = true;

        /// <summary>
        /// 遮挡
        /// </summary>
        public bool occlusion => _occlusion;

        /// <summary>
        /// 使用主相机
        /// </summary>
        [Name("使用主相机")]
        [FormerlySerializedAs(nameof(useMainCamera))]
        public bool _useMainCamera = true;

        /// <summary>
        /// 使用主相机
        /// </summary>
        public bool useMainCamera => _useMainCamera;

        /// <summary>
        /// 渲染相机
        /// </summary>
        [Name("渲染相机")]
        [HideInSuperInspector(nameof(useMainCamera), EValidityCheckType.True)]
        [FormerlySerializedAs(nameof(_rendererCamera))]
        public Camera _rendererCamera;

        /// <summary>
        /// 渲染相机
        /// </summary>
        public Camera rendererCamera => _rendererCamera;

        /// <summary>
        /// 渲染规则
        /// </summary>
        public enum ERenderRule
        {
            /// <summary>
            /// 渲染器:对选择集中有渲染器组件的每个游戏对象进行各自独立绘制
            /// </summary>
            [Name("渲染器")]
            [Tip("对选择集中有渲染器组件的每个游戏对象进行各自独立绘制")]
            Renderer,

            /// <summary>
            /// 包围盒:对选择集中每个对象获取包围盒后进行各自独立绘制
            /// </summary>
            [Name("包围盒")]
            [Tip("对选择集中每个对象获取包围盒后进行各自独立绘制")]
            Bounds,
        }

        /// <summary>
        /// 渲染规则
        /// </summary>
        [Name("渲染规则")]
        [EnumPopup]
        public ERenderRule _renderRule = ERenderRule.Renderer;

        /// <summary>
        /// 包含子级游戏对象
        /// </summary>
        [Name("包含子级游戏对象")]
        public bool _includeChildren = true;

        /// <summary>
        /// 包含非激活游戏对象
        /// </summary>
        [Name("包含非激活游戏对象")]
        public bool _includeInactiveGO = true;

        /// <summary>
        /// 包含禁用渲染器
        /// </summary>
        [Name("包含禁用渲染器")]
        public bool _includeDisableRenderer = true;

        private List<(GameObject, BoundsProvider)> gameObjectAndBoundsProviders = new List<(GameObject, BoundsProvider)>();

        private List<Renderer> objects = new List<Renderer>();

        /// <summary>
        /// 渲染模式
        /// </summary>
        [Name("渲染模式")]
        [EnumPopup]
        public ERenderMode _RenderMode = ERenderMode.GL;

        /// <summary>
        /// 材质
        /// </summary>
        [Name("材质")]
        public Material _material;

        /// <summary>
        /// 当选择集变更
        /// </summary>
        /// <param name="oldSelections"></param>
        /// <param name="flag"></param>
        protected override void OnSelectionChanged(GameObject[] oldSelections, bool flag)
        {
            switch (_renderRule)
            {
                case ERenderRule.Renderer:
                    {
                        objects.Clear();
                        foreach (var o in Selection.selections)
                        {
                            if (o)
                            {
                                var renderer = o.GetComponent<Renderer>();
                                if (renderer)
                                {
                                    objects.Add(renderer);
                                }
                            }
                        }
                        break;
                    }
                case ERenderRule.Bounds:
                    {
                        gameObjectAndBoundsProviders.Clear();
                        gameObjectAndBoundsProviders.AddRange(Selection.selections.WhereCast(go => go, go => (go, go.GetComponent<BoundsProvider>())));
                        break;
                    }
            }
        }

        List<LineRenderer> lineRenderers = new List<LineRenderer>();

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            lineRenderers.AddRange(GetComponents<LineRenderer>());

            if (!_material)
            {
                _material = CommonGL.commonMaterial;
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            lineRenderers.Clear();
        }

        private void Set(int index, Bounds bounds)
        {
            LineRenderer lineRenderer;
            if (index < lineRenderers.Count)
            {
                lineRenderer = lineRenderers[index];
            }
            else
            {
                lineRenderer = this.XAddComponent<LineRenderer>();
                lineRenderers.Add(lineRenderer);
            }

            List<Vector3> outPoints = new List<Vector3>();
            CommonGL.BoundsToLineStrip(bounds, outPoints);
            lineRenderer.material = _material;
            lineRenderer.positionCount = outPoints.Count;
            lineRenderer.SetPositions(outPoints.ToArray());
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
            lineRenderer.startColor = _color;
            lineRenderer.endColor = _color;
            lineRenderer.allowOcclusionWhenDynamic = occlusion;
            lineRenderer.XSetEnable(true);
        }

        private void Clear(int index)
        {
            for (int i = index; i < lineRenderers.Count; i++)
            {
                lineRenderers[i].XSetEnable(false);
            }
        }

        private void Update()
        {
            if (_RenderMode != ERenderMode.LineRenderer) return;
            switch (_renderRule)
            {
                case ERenderRule.Renderer:
                    {
                        var objCount = objects.Count;
                        for (int i = 0; i < objCount; i++)
                        {
                            Set(i, objects[i].bounds);
                        }
                        Clear(objCount);
                        break;
                    }
                case ERenderRule.Bounds:
                    {
                        int i = 0;
                        foreach (var (go, bp) in gameObjectAndBoundsProviders)
                        {
                            if (bp && bp.TryGetBounds(out var bounds))
                            {
                                Set(i, bounds);
                                i++;
                            }
                            else
                            {
                                if (CommonFun.GetBounds(out Bounds b, go, _includeChildren, _includeInactiveGO, _includeDisableRenderer))
                                {
                                    Set(i, b);
                                    i++;
                                }
                            }
                        }
                        Clear(i);
                        break;
                    }
            }
        }

        /// <summary>
        /// 绘制对象
        /// </summary>
        public void OnRenderObject()
        {
            if (_RenderMode != ERenderMode.GL) return;
            Clear(0);
            var camera = useMainCamera ? Camera.main : rendererCamera;
            if (!camera) camera = Camera.current;
            if (!camera) return;
            switch (_renderRule)
            {
                case ERenderRule.Renderer:
                    {
                        foreach (var o in objects)
                        {
                            CommonGL.Bounds(o, _color, camera, occlusion, width, _material);
                        }
                        break;
                    }
                case ERenderRule.Bounds:
                    {
                        foreach (var (go, bp) in gameObjectAndBoundsProviders)
                        {
                            if (bp && bp.TryGetBounds(out var bounds))
                            {
                                CommonGL.Bounds(bounds, _color, camera, occlusion, width, _material);
                            }
                            else
                            {
                                if (CommonFun.GetBounds(out Bounds b, go, _includeChildren, _includeInactiveGO, _includeDisableRenderer))
                                {
                                    CommonGL.Bounds(b, _color, camera, occlusion, width, _material);
                                }
                            }
                        }
                        break;
                    }
            }
        }
    }

    /// <summary>
    /// 渲染模式
    /// </summary>
    public enum ERenderMode
    {
        /// <summary>
        /// GL
        /// </summary>
        [Name("GL")]
        GL,

        /// <summary>
        /// 线渲染器
        /// </summary>
        [Name("线渲染器")]
        LineRenderer,
    }
}
