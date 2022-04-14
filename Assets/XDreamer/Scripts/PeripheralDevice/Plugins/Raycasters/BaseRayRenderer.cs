using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;

namespace XCSJ.PluginPeripheralDevice.Raycasters
{
    /// <summary>
    /// 
    /// 外部硬件设备射线Line渲染器
    /// </summary>
    [DisallowMultipleComponent]
    [RequireManager(typeof(PeripheralDeviceInputManager))]
    public class BaseRayRenderer : ToolMB
    {
        /// <summary>
        /// 射线采用LineRender
        /// </summary>
        [Name("射线")]
        [Tooltip("场景中显示虚拟射线的对象")]
        public GameObject rayLine;

        /// <summary>
        /// 涉嫌类型
        /// </summary>
        [Name("射线类型")]
        [Tooltip("用以射线检测的对象类型")]
        [EnumPopup]
        public ELineStyle eLineStyle = ELineStyle.Liner;

        /// <summary>
        /// 线条宽度
        /// </summary>
        [Name("线条宽度")]
        [Tooltip("设置线条宽度")]
        public float width = 0.01f;

        /// <summary>
        /// 线条材质
        /// </summary>
        [Name("线条材质")]
        [Tooltip("设置线条的渲染材质")]
        public Material lineMaterial;

        /// <summary>
        /// 射线采用LineRender,方便修改属性，调整射线
        /// </summary>
        private LineRenderer _lineRenderer;

        /// <summary>
        /// 绘制可见射线
        /// </summary>
        /// <param name="position">射线起点</param>
        /// <param name="direction">射线方向</param>
        /// <param name="distance">射线长度</param>
        public void DrawRayLine(Vector3 position, Vector3 endPonit)
        {
            if (eLineStyle == ELineStyle.Liner)
            {
                if (_lineRenderer)
                {
                    _lineRenderer.positionCount = 2;
                    _lineRenderer.SetPositions(new Vector3[] { position, endPonit });
                }
            }
        }


        /// <summary>
        /// 初始化射线对象
        /// </summary>
        /// <param name="show">是否显示</param>
        public void InitRayLine(bool show)
        {
            if (rayLine == null)
            {
                rayLine = new GameObject("RayLine");
                rayLine.transform.parent = transform;
                rayLine.transform.localPosition = Vector3.zero;
                rayLine.transform.localRotation = Quaternion.identity;
            }
            _lineRenderer = rayLine.GetComponent<LineRenderer>();
            if (_lineRenderer == null) _lineRenderer = rayLine.AddComponent<LineRenderer>();
            _lineRenderer.startWidth = width;
            _lineRenderer.endWidth = width;
            _lineRenderer.material = lineMaterial;
            _lineRenderer.useWorldSpace = false;
            _lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _lineRenderer.receiveShadows = false;
            _lineRenderer.SetPositions(new Vector3[] { Vector3.zero, Vector3.zero });
            rayLine.SetActive(show);
        }

        /// <summary>
        /// 设置线条的属性
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="mat">材质</param>
        public void SetLineProperty(float width, Material mat)
        {
            this.width = width;
            lineMaterial = mat;
            if (_lineRenderer)
            {
                _lineRenderer.startWidth = width;
                _lineRenderer.endWidth = width;
                _lineRenderer.material = lineMaterial;
            }
        }

        /// <summary>
        /// 显示射线
        /// </summary>
        /// <param name="isActive">是否显示</param>
        public void ShowRayLine(bool isActive)
        {
            if(rayLine) rayLine.SetActive(isActive);
        }
    }

    public enum ELineStyle
    {
        [Name("线性")]
        Liner,
    }
}
