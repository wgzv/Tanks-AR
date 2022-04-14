using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.Extension.Base.Maths;
using XCSJ.Maths;
using XCSJ.PluginTools.SelectionUtils;

namespace XCSJ.PluginTools.Puts.TRSHandles
{
    /// <summary>
    /// 平移旋转缩放工具
    /// </summary>
    [Name("平移旋转缩放工具")]
    [Tool("选择集", disallowMultiple = true, rootType = typeof(ToolsManager))]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Drag)]
    [RequireManager(typeof(ToolsManager))]
    public class TRSHandle : BaseDragger
    {
        /// <summary>
        /// 变换空间
        /// </summary>
        [Name("变换空间")] 
        [EnumPopup]
        public ESpace _space = ESpace.Local;

        /// <summary>
        /// 变换空间
        /// </summary>
        public ESpace space
        {
            get => _space;
            set
            {
                if (_space != value)
                {
                    var old = _space;
                    _space = value;
                    onTRSHandleSpaceChanged?.Invoke(this, old);
                    Rebuild();
                }
            }
        }

        /// <summary>
        /// 变换空间变化回调函数,参数2为上一次的变换空间规则
        /// </summary>
        public static event Action<TRSHandle, ESpace> onTRSHandleSpaceChanged;

        /// <summary>
        /// 单次移动距离
        /// </summary>
        [Name("移动步长")] 
        [Min(0)]
        public float _positionSnapValue = 0.01f;

        /// <summary>
        /// 旋转值
        /// </summary>
        [Name("旋转最小角")]
        [Min(0)]
        public float _rotationSnapValue = 0.2f;

        /// <summary>
        /// 缩放最小倍数
        /// </summary>
        [Name("缩放最小倍数")]
        [Min(0)]
        public float _scaleSnapValue = .01f;

        /// <summary>
        /// 工具类型
        /// </summary>
        public ETRSToolType trsToolType
        {
            get => _trsToolType;
            set
            {
                if (_trsToolType != value)
                {
                    var old = _trsToolType;
                    _trsToolType = value;
                    RebuildGizmoMesh(Vector3.one);

                    onTRSHandleToolTypeChanged?.Invoke(this, old);

                    SetActiveDragger(_trsToolType != ETRSToolType.None);
                }
            }
        }
        private ETRSToolType _trsToolType = ETRSToolType.Position;

        /// <summary>
        /// 控制类型变化回调函数,参数2为上一次工具类型
        /// </summary>
        public static Action<TRSHandle, ETRSToolType> onTRSHandleToolTypeChanged = null;

        private Transform _trs;

        private Transform trs { get { if (_trs == null) _trs = gameObject.GetComponent<Transform>(); return _trs; } }

        /// <summary>
        /// 最长轴距离
        /// </summary>
        public const int MaxDistanceHandle = 15;

        /// <summary>
        /// 拖拽轴数量
        /// </summary>
        private int draggingAxesCount = 0;  

        private HandleTransform handleOrigin = HandleTransform.identity;

        #region 生命周期

        /// <summary>
        /// 唤醒
        /// </summary>
        protected void Awake()
        {
            _trs = null;

            trsToolType = ETRSToolType.Position;
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected void Update()
        {
            if (!cam) return;

            if (draggingHandle)
            {
                DebugDrawOnSceneView();
            }

            OnCameraMove();
        }

        /// <summary>
        /// 属性面板修改后
        /// </summary>
        protected void OnValidate()
        {
            Rebuild();
        }

        #endregion

        #region 拖拽

        [Serializable]
        public class DragOrientation
        {
            public Vector3 origin = Vector3.zero;
            public Vector3 axis = Vector3.zero;
            public Vector3 mouse = Vector3.zero;
            public Vector3 cross = Vector3.zero;
            public Vector3 offset = Vector3.zero;
            public Plane plane = new Plane(Vector3.up, Vector3.zero);
        }

        float axisAngle = 0f;

        public bool draggingHandle => _dragAxis != EAxis.None;

        private EAxis _dragAxis = EAxis.None;

        private Vector2 _lastMouse = Vector2.zero;

        /// <summary>
        /// 拖拽输入
        /// </summary>
        public IGrabAction dragTrigger
        {
            get
            {
                if (_dragTrigger == null)
                {
                    _dragTrigger = new DragByMouse();
                }
                return _dragTrigger;
            }
            set
            {
                _dragTrigger = value;
            }
        }
        private IGrabAction _dragTrigger;

        /// <summary>
        /// 拖拽有效性检测
        /// </summary>
        /// <returns></returns>
        public override bool Grab()
        {
            if (!dragTrigger.Grab())
            {
                return false;
            }
            _dragAxis = PickHandleAxis(Input.mousePosition);
            return draggingHandle;
        }

        public override bool Hold() => dragTrigger.Hold();

        public override bool Release() => dragTrigger.Release();

        private GameObject dragGameObject = null;

        /// <summary>
        /// 开始拖拽
        /// </summary>
        public override void OnGrab(params GameObject[] gameObjects)
        {
            if (gameObjects==null || gameObjects.Length==0 || !gameObjects[0])
            {
                return;
            }
            dragGameObject = gameObjects[0];

            if (!draggingHandle) return;

            drag.offset = Vector3.zero;

            axisAngle = 0f;

            _lastMouse = Input.mousePosition;

            drag.axis = Vector3.zero;
            draggingAxesCount = 0;

            Color xA, yA, zA, xP, yP, zP, selectedAColor, selectedPColor;
            xA = yA = zA = xP = yP = zP = Color.gray;
            xP.a = yP.a = zP.a = HandleMesh.AphlaPlane;
            selectedPColor = selectedAColor = Color.yellow;
            selectedPColor.a = HandleMesh.AphlaPlane;

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if ((_dragAxis & EAxis.X) == EAxis.X)
            {
                draggingAxesCount++;
                drag.axis = trs.right;
                drag.plane.SetNormalAndPosition(trs.right, trs.position);

                xA = selectedAColor;
            }

            if ((_dragAxis & EAxis.Y) == EAxis.Y)
            {
                draggingAxesCount++;
                var n = draggingAxesCount > 1 ? Vector3.Cross(drag.axis, trs.up) : trs.up;
                drag.plane.SetNormalAndPosition(n, trs.position);
                drag.axis += trs.up;

                yA = selectedAColor;
            }

            if ((_dragAxis & EAxis.Z) == EAxis.Z)
            {
                draggingAxesCount++;
                var n = draggingAxesCount > 1 ? Vector3.Cross(drag.axis, trs.forward) : trs.forward;
                drag.plane.SetNormalAndPosition(n, trs.position);
                drag.axis += trs.forward;

                zA = selectedAColor;
            }

            // 拖拽轴为1时
            if (draggingAxesCount == 1)
            {
                if (MathLibrary.ClosestPointsOnTwoRay(new Ray(trs.position, drag.axis), ray, out Vector3 a, out Vector3 b))
                {
                    drag.offset = a - trs.position;
                }
                if (drag.plane.Raycast(ray, out float hit))
                {
                    drag.mouse = (ray.GetPoint(hit) - trs.position).normalized;
                    drag.cross = Vector3.Cross(drag.axis, drag.mouse);
                }
            }
            else if (draggingAxesCount > 1)
            {
                if (drag.plane.Raycast(ray, out float hit))
                {
                    drag.offset = ray.GetPoint(hit) - trs.position;
                    drag.mouse = (ray.GetPoint(hit) - trs.position).normalized;
                    drag.cross = Vector3.Cross(drag.axis, drag.mouse);
                }

                // 设置面颜色
                switch (trsToolType)
                {
                    case ETRSToolType.Position:
                        {
                            if (xA != selectedAColor)
                            {
                                xP = selectedPColor;
                            }

                            if (yA != selectedAColor)
                            {
                                yP = selectedPColor;
                            }

                            if (zA != selectedAColor)
                            {
                                zP = selectedPColor;
                            }
                            break;
                        }
                    case ETRSToolType.Rotate:
                        {
                            break;
                        }
                    case ETRSToolType.Scale:
                        {
                            xA = yA = zA = selectedAColor;
                            xP = yP = zP = selectedPColor;
                            break;
                        }
                }
            }

            // 设定拖拽三轴模型颜色
            HandleMesh.SetColor(xA, yA, zA, xP, yP, zP);
            RebuildGizmoMesh(Vector3.one);

            handleOrigin.SetTRS(trs);
            trsTransformOnBegin = new HandleTransform(trs.position, trs.rotation, dragGameObject.transform.lossyScale);
            dragObjectTransformOnBegin = new HandleTransform(dragGameObject.transform);

            dragAndTrsOffsetOnBegin = dragObjectTransformOnBegin - trsTransformOnBegin;

            dragOrgScale = dragGameObject.transform.localScale;
        }

        private Vector3 dragOrgScale = Vector3.one;

        /// <summary>
        /// 拖拽中
        /// </summary>
        public override void OnHold(params GameObject[] gameObjects)
        {
            if (!draggingHandle || gameObjects==null || gameObjects.Length==0 || !gameObjects[0]) return;

            var point = Vector3.zero;

            var ray = cam.ScreenPointToRay(Input.mousePosition);
            if (draggingAxesCount < 2 && trsToolType != ETRSToolType.Rotate)
            {
                if (!MathLibrary.ClosestPointsOnTwoRay(new Ray(trs.position, drag.axis), ray, out point, out Vector3 b))
                {
                    return;
                }
            }
            else
            {
                if (drag.plane.Raycast(ray, out float hit))
                {
                    point = ray.GetPoint(hit);
                }
                else
                {
                    return;
                }
            }

            drag.origin = trs.position;
            switch (trsToolType)
            {
                case ETRSToolType.Position:
                    {
                        dragGameObject.transform.position = trs.position = HandleHelper.Snap(point - drag.offset, _positionSnapValue);
                        break;
                    }
                case ETRSToolType.Rotate:
                    {
                        var delta = (Vector2)Input.mousePosition - _lastMouse;
                        var deltaMagnitude = delta.magnitude;
                        if (MathX.ApproximatelyZero(deltaMagnitude))
                        {
                            return;
                        }
                        _lastMouse = Input.mousePosition;
                        var angleOffset = deltaMagnitude * HandleHelper.CalcMouseDeltaSignWithAxes(cam, drag.origin, drag.axis, drag.cross, delta);
                        axisAngle += angleOffset;
                        trs.rotation = Quaternion.AngleAxis(HandleHelper.Snap(axisAngle, _rotationSnapValue), drag.axis) * handleOrigin.rotation;

                        if (_space == ESpace.Local)
                        {
                            dragGameObject.transform.rotation = dragAndTrsOffsetOnBegin.rotation * trs.rotation;
                        }
                        else
                        {
                            dragGameObject.transform.Rotate(drag.axis, angleOffset, Space.World);
                        }

                        break;
                    }
                case ETRSToolType.Scale:
                    {
                        var p = point - drag.offset - trs.position;
                        var v = Vector3.one + ((draggingAxesCount > 1) ? GetUniformMagnitude(p) : Quaternion.Inverse(handleOrigin.rotation) * p);

                        var scale = HandleHelper.Snap(v, _scaleSnapValue);
                        RebuildGizmoMesh(scale);
                        var tmpScale = dragOrgScale;
                        tmpScale.Scale(scale);
                        dragGameObject.transform.localScale = tmpScale;
                        break;
                    }
            }

            RebuildGizmoMatrix();
        }

        private HandleTransform trsTransformOnBegin;
        private HandleTransform dragObjectTransformOnBegin;
        private HandleTransform dragAndTrsOffsetOnBegin;

        /// <summary>
        /// 结束拖拽
        /// </summary>
        public override void OnRelease(params GameObject[] gameObjects)
        {
            draggingAxesCount = 0;
            HandleMesh.RecoverColor();
            RebuildGizmoMesh(Vector3.one, true);

            StartCoroutine(DelaySetDraggingFalse());
        }

        /// <summary>
        /// 选择集发生改变
        /// </summary>
        /// <param name="oldSelections"></param>
        /// <param name="flag"></param>
        public override void OnSelectionChanged(GameObject[] oldSelections, bool flag)
        {
            base.OnSelectionChanged(oldSelections, flag);

            dragGameObject = Selection.selection;
            Rebuild();
        }

        public override void OnActive(bool active)
        {
            base.OnActive(active);

            dragGameObject = Selection.selection;
        }

        /// <summary>
        /// 设置所有向量长度为最大长度
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        private Vector3 GetUniformMagnitude(Vector3 a)
        {
            var absX = Mathf.Abs(a.x);
            var absY = Mathf.Abs(a.y);
            var absZ = Mathf.Abs(a.z);
            
            float max = absX > absY && absX > absZ ? a.x : absY > absZ ? a.y : a.z;
            a.x = a.y = a.z = max;
            return a;
        }

        private IEnumerator DelaySetDraggingFalse()
        {
            yield return new WaitForEndOfFrame();
            _dragAxis = EAxis.None;
        }

        #endregion

        #region 拾取轴

        /// <summary>
        /// 检测点击轴或面
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="plane"></param>
        /// <returns></returns>
        private EAxis PickHandleAxis(Vector2 mousePosition)
        {
            switch (trsToolType)
            {
                case ETRSToolType.Position:
                case ETRSToolType.Scale:
                    {
                        return PickPositionScaleLineOrPlane(mousePosition);
                    }
                case ETRSToolType.Rotate:
                    {
                        return PickRotationLine(mousePosition);
                    }
            }

            return EAxis.None;
        }

        private EAxis PickPositionScaleLineOrPlane(Vector2 mousePosition)
        {
            var plane = EAxis.None;

            var currentCamera = cam;
            if (!currentCamera) return plane;

            // 将空间坐标转为平面坐标进行求解
            var sceneHandleSize = (1 + _capSize) * HandleHelper.GetScreenAndWorldRatio(trs.position) * _handleSize;
            var center = currentCamera.WorldToScreenPoint(trs.position);
            var up = currentCamera.WorldToScreenPoint(trs.position + trs.up * sceneHandleSize);
            var right = currentCamera.WorldToScreenPoint(trs.position + trs.right * sceneHandleSize);
            var forward = currentCamera.WorldToScreenPoint(trs.position + trs.forward * sceneHandleSize);

            // 检查平面是否激活
            var cameraMask = HandleHelper.DirectionMask(trs, currentCamera.transform.forward);

            var p_right = center + (right - center) * cameraMask.x * _boxSize;
            var p_up = center + (up - center) * cameraMask.y * _boxSize;
            var p_forward = center + (forward - center) * cameraMask.z * _boxSize;

            if (MathLibrary.PointInPolygon(new Vector2[] { center, p_up, p_up, (p_up + p_forward) - center, (p_up + p_forward) - center, p_forward, p_forward, center }, mousePosition))// x plane
            {
                plane = EAxis.Y | EAxis.Z;
            }
            else if (MathLibrary.PointInPolygon(new Vector2[] { center, p_right, p_right, (p_right + p_forward) - center, (p_right + p_forward) - center, p_forward, p_forward, center }, mousePosition))// y plane
            {
                plane = EAxis.X | EAxis.Z;
            }
            else if (MathLibrary.PointInPolygon(new Vector2[] { center, p_up, p_up, (p_up + p_right) - center, (p_up + p_right) - center, p_right, p_right, center }, mousePosition))// z plane
            {
                plane = EAxis.X | EAxis.Y;
            }
            else if (MathLibrary.DistancePointLineSegment(mousePosition, center, up) < MaxDistanceHandle)
            {
                plane = EAxis.Y;
            }
            else if (MathLibrary.DistancePointLineSegment(mousePosition, center, right) < MaxDistanceHandle)
            {
                plane = EAxis.X;
            }
            else if (MathLibrary.DistancePointLineSegment(mousePosition, center, forward) < MaxDistanceHandle)
            {
                plane = EAxis.Z;
            }
            return plane;
        }

        private EAxis PickRotationLine(Vector2 mousePosition)
        {
            var plane = EAxis.None;

            var currentCamera = cam;
            if (!currentCamera) return plane;

            var best = Mathf.Infinity;
            var vertices = HandleMesh.GetRotationVertices(16, 1f);
            Vector2 cur, prev = Vector2.zero;

            for (int i = 0; i < 3; i++)
            {
                cur = currentCamera.WorldToScreenPoint(vertices[i][0]);

                for (int n = 0; n < vertices[i].Length - 1; n++)
                {
                    prev = cur;
                    cur = currentCamera.WorldToScreenPoint(_handleMatrix.MultiplyPoint3x4(vertices[i][n + 1]));

                    float dist = MathLibrary.DistancePointLineSegment(mousePosition, prev, cur);

                    if (dist < best && dist < MaxDistanceHandle)
                    {
                        var viewDir = (_handleMatrix.MultiplyPoint3x4((vertices[i][n] + vertices[i][n + 1]) * .5f) - currentCamera.transform.position).normalized;
                        var nrm = transform.TransformDirection(vertices[i][n]).normalized;

                        if (Vector3.Dot(nrm, viewDir) > .5f) continue;

                        best = dist;

                        switch (i)
                        {
                            case 0:
                                {
                                    plane = EAxis.Y;
                                    break;
                                }
                            case 1:
                                {
                                    plane = EAxis.Z;
                                    break;
                                }
                            case 2:
                                {
                                    plane = EAxis.X;
                                    break;
                                }
                        }
                    }
                }
            }

            return (best < MaxDistanceHandle + .1f) ? plane : EAxis.None;
        }

        #endregion

        #region 三轴创建与渲染

        /// <summary>
        /// 轴大小
        /// </summary>
        [Group("外观设置")]
        [Name("轴大小")]
        [Min(1)]
        public float _handleSize = 90f;

        /// <summary>
        /// 椎体网格
        /// </summary>
        [Name("轴椎体网格模型")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Mesh _coneMesh;

        /// <summary>
        /// 椎体大小
        /// </summary>
        [Name("椎体大小")]
        [Min(0.01f)]
        public float _capSize = 0.1f;

        /// <summary>
        /// 立方体网格
        /// </summary>
        [Name("缩放立方体网格模型")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Mesh _cubeMesh;

        /// <summary>
        /// 立方体大小
        /// </summary>
        [Name("立方体大小")]
        [Min(0.1f)]
        public float _boxSize = .25f;

        /// <summary>
        /// 平移缩放轴材质
        /// </summary>
        [Name("平移缩放轴材质")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Material _handleOpaqueMaterial = null;

        /// <summary>
        /// 旋转线材质
        /// </summary>
        [Name("旋转线材质")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Material _rotateLineMaterial = null;

        /// <summary>
        /// 平面材质
        /// </summary>
        [Name("平面材质")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Material _handleTransparentMaterial = null;

        private Mesh _handleLineMesh;

        private Mesh _handleSolidMesh;

        /// <summary>
        /// 渲染矩阵
        /// </summary>
        private Matrix4x4 _handleMatrix;

        private void Rebuild()
        {
            SetTRS(dragGameObject);
            RebuildGizmoMesh(Vector3.one, true);// 重建拖拽轴模型
        }

        private void OnCameraMove()
        {
            if (cam.transform.hasChanged)
            {
                cam.transform.hasChanged = false;
                RebuildGizmoMesh(Vector3.one, true);
            }
            
            if(dragGameObject)
            {
                var transform = dragGameObject.transform;
                if (transform.hasChanged)
                {
                    transform.hasChanged = false;
                    SetTRS(dragGameObject);
                }
            }
        }

        /// <summary>
        /// 渲染
        /// </summary>
        private void OnRenderObject()
        {
            if (!dragGameObject || !IsCurrentDragger()) return;

            switch (trsToolType)
            {
                case ETRSToolType.Position:
                case ETRSToolType.Scale:
                    {
                        _handleOpaqueMaterial.SetPass(0);
                        Graphics.DrawMeshNow(_handleLineMesh, _handleMatrix);
                        Graphics.DrawMeshNow(_handleSolidMesh, _handleMatrix, 1);  // Cones

                        // 当拖拽轴不为1时，才显示中间的模型网格
                        if (draggingAxesCount != 1)
                        {
                            _handleTransparentMaterial.SetPass(0);
                            Graphics.DrawMeshNow(_handleSolidMesh, _handleMatrix, 0);  // Box
                        }
                        break;
                    }
                case ETRSToolType.Rotate:
                    {
                        _rotateLineMaterial.SetPass(0);
                        Graphics.DrawMeshNow(_handleLineMesh, _handleMatrix);
                        break;
                    }
            }
        }

        /// <summary>
        /// 构建矩阵
        /// </summary>
        private void RebuildGizmoMatrix()
        {
            var handleSize = HandleHelper.GetScreenAndWorldRatio(trs.position);
            _handleMatrix = transform.localToWorldMatrix * Matrix4x4.Scale(Vector3.one * handleSize * _handleSize);
        }

        /// <summary>
        /// 构建网格模型
        /// </summary>
        /// <param name="scale"></param>
        private void RebuildGizmoMesh(Vector3 scale, bool rebuildMatrix = false)
        {
            var currentCamera = cam;
            if (!currentCamera) return;

            switch (trsToolType)
            {
                case ETRSToolType.Position:
                    {
                        HandleMesh.CreateAxisLineMesh(ref _handleLineMesh, trs, scale, currentCamera);
                        HandleMesh.CreateRectangeAndCapMesh(ref _handleSolidMesh, trs, scale, currentCamera, _coneMesh, _boxSize, _capSize);
                        break;
                    }                
                case ETRSToolType.Scale:
                    {
                        HandleMesh.CreateAxisLineMesh(ref _handleLineMesh, trs, scale, currentCamera);
                        HandleMesh.CreateRectangeAndCapMesh(ref _handleSolidMesh, trs, scale, currentCamera, _cubeMesh, _boxSize, _capSize);
                        break;
                    }
                case ETRSToolType.Rotate:
                    {
                        HandleMesh.CreateRotateMesh(ref _handleLineMesh, 48, 1f);
                        //HandleMesh.CreateDiscMesh(ref _handleSolidMesh, 45, 0.9f);
                        break;
                    }
            }

            if (rebuildMatrix)
            {
                RebuildGizmoMatrix();
            }
        }


        private DragOrientation drag = new DragOrientation();

        private void DebugDrawOnSceneView()
        {
            var dir = drag.axis * 2f;
            var color = new Color(Mathf.Abs(drag.axis.x), Mathf.Abs(drag.axis.y), Mathf.Abs(drag.axis.z), 1f);
            var lineTime = 0f;
            Debug.DrawRay(drag.origin, dir * 1f, color, lineTime, false);
            var orgOffset1 = drag.origin + dir;
            var orgOffset2 = drag.origin + dir * .9f;
            Debug.DrawLine(orgOffset1, orgOffset2 + (trs.up * .1f), color, lineTime, false);
            Debug.DrawLine(orgOffset1, orgOffset2 + (trs.forward * .1f), color, lineTime, false);
            Debug.DrawLine(orgOffset1, orgOffset2 + (trs.right * .1f), color, lineTime, false);
            Debug.DrawLine(orgOffset1, orgOffset2 + (-trs.up * .1f), color, lineTime, false);
            Debug.DrawLine(orgOffset1, orgOffset2 + (-trs.forward * .1f), color, lineTime, false);
            Debug.DrawLine(orgOffset1, orgOffset2 + (-trs.right * .1f), color, lineTime, false);

            Debug.DrawLine(drag.origin, drag.origin + drag.mouse, Color.red, lineTime, false);
            Debug.DrawLine(drag.origin, drag.origin + drag.cross, Color.black, lineTime, false);
        }

        #endregion

        public void SetTRS(GameObject go)
        {
            if (go)
            {
                trs.position = go.transform.position;
                trs.rotation = _space == ESpace.Local ? go.transform.rotation : Quaternion.Euler(0, 0, 0);
                trs.localScale = Vector3.one;

                RebuildGizmoMatrix();
            }
        }

        [System.Flags]
        enum EAxis
        {
            None = 0x0,
            X = 0x1,
            Y = 0x2,
            Z = 0x4
        }

        /// <summary>
        /// 操作类型
        /// </summary>
        public enum ESpace
        {
            [Name("本地")]
            Local,

            [Name("世界")]
            World,
        }
    }

    /// <summary>
    /// 工具操作类型
    /// </summary>
    public enum ETRSToolType
    {
        [Name("无")]
        None,

        [Name("平移")]
        Position,

        [Name("旋转")]
        Rotate,

        [Name("缩放")]
        Scale,
    }
}
