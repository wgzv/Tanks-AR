using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;

namespace XCSJ.CommonUtils.PluginCrossSectionShader
{
    /// <summary>
    /// 使用剖面材质类型
    /// </summary>
    public enum EUseCuttingMaterial
    {
        [Name("交集")]
        Intersection = 0,

        [Name("并集")]
        Union
    }

    [Name("剖面控制器")]
    [Tip("剖面控制器将剖面的朝向和位置设置到剖面Shader中")]
    [RequireManager(typeof(ToolsManager), typeof(ToolsExtensionManager))]
    public class PlaneCuttingController : BasePlaneCuttingMB
    {
        [Name("三剖面控制器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public ThreePlanesCuttingController threePlanesCuttingController = null;

        [Group("材质配置")]
        [Name("自动替换剖面Shader")]
        public bool autoChangeShader = true;

        [Name("强制替换剖面Shader")]
        [Tip("当材质已经是三剖面类型的shader也将被替换为预制材质的shader")]
        [HideInSuperInspector(nameof(autoChangeShader), EValidityCheckType.Equal, false)]
        public bool forceChangeShader = true;

        [Name("使用剖面材质类型")]
        [EnumPopup]
        public EUseCuttingMaterial _useCuttingMaterialType = EUseCuttingMaterial.Intersection;

        public EUseCuttingMaterial useCuttingMaterialType
        {
            get => _useCuttingMaterialType;
            set => _useCuttingMaterialType = value;
        }
        public int useCuttingMaterialTypeInt
        {
            get => (int)_useCuttingMaterialType;
            set => _useCuttingMaterialType = (EUseCuttingMaterial)value;
        }

        [Name("交集剖面材质")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(autoChangeShader), EValidityCheckType.Equal, false)]
        public Material cuttingMaterail = null;

        [Name("并集剖面材质")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(autoChangeShader), EValidityCheckType.Equal, false)]
        public Material unionCuttingMaterail = null;

        [Group("剖切对象配置")]
        [Name("包含子对象")]
        public bool includeChildren = true;

        [Name("包含非激活游戏对象")]
        public bool includeInactiveGameObject = false;

        [Name("包含未启用渲染器对象")]
        public bool includeDisableRenderer = false;

        [Name("剖切对象列表")]
        [ValidityCheck(EValidityCheckType.ElementCountGreater, 0)]
        public List<GameObject> cuttedObjects = new List<GameObject>();

        [Group("排除剖切对象配置")]
        [Name("包含子对象")]
        public bool excludeOjbectIncludeChildren = true;

        [Name("对象列表")]
        [Tip("在这个列表的对象，不会被剖切")]
        public List<GameObject> excludeOjbects = new List<GameObject>();

        private EUseCuttingMaterial lastUseCuttingMaterialType = EUseCuttingMaterial.Intersection;

        private HashSet<GameObject> _excludeOjbectMap = new HashSet<GameObject>();

        protected Dictionary<Material, Shader> shaderRecorder = new Dictionary<Material, Shader>();

        /// <summary>
        /// 唤醒
        /// </summary>
        protected void Awake() { }

        /// <summary>
        /// 启动
        /// </summary>
        protected void Start() { InitCuttingShader(); }

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            // 添加剖切面事件
            if (threePlanesCuttingController)
            {
                threePlanesCuttingController.cuttingPlaneActiveChanged += OnCuttingPlaneActiveChanged;
            }

            // 初始化排除对象集合
            _excludeOjbectMap.Clear();
            var objs = excludeOjbects.Distinct();
            _excludeOjbectMap.AddRangeWithDistinct(objs);
            if (excludeOjbectIncludeChildren)
            {
                foreach (var obj in objs)
                {
                    _excludeOjbectMap.AddRangeWithDistinct(CommonFun.GetChildGameObjects(obj));
                }
            }

            lastUseCuttingMaterialType = _useCuttingMaterialType;
            InitCuttingShader();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            if (threePlanesCuttingController)
            {
                threePlanesCuttingController.cuttingPlaneActiveChanged -= OnCuttingPlaneActiveChanged;
            }

            RecoverShader();
            ClearShaderRecorder();
        }

        // 响应平面激活
        protected void OnCuttingPlaneActiveChanged(ThreePlanesCuttingController threePlanesCuttingController, PlaneCuttingInfo planeCuttingInfo)
        {
            if (!threePlanesCuttingController || !planeCuttingInfo) return;

            if (planeCuttingInfo.gameObject.activeSelf)
            {
                InitCuttingShader();
            }
            else
            {
                // 不存在活跃平面，恢复材质Shader
                if (!threePlanesCuttingController.planes.ToList().Any(p => p && p.gameObject.activeSelf))
                {
                    RecoverShader();
                }
            }
        }


        private void Update()
        {
            if (lastUseCuttingMaterialType != _useCuttingMaterialType)
            {
                lastUseCuttingMaterialType = _useCuttingMaterialType;
                InitCuttingShader();
            }
        }

        public void AddCuttedObject(GameObject cuttedObj)
        {
            if (!cuttedObj || cuttedObjects.Contains(cuttedObj)) return;

            cuttedObjects.Add(cuttedObj);
            InitCuttingShader();
        }

        public void RemoveCuttedObject(GameObject cuttedObj)
        {
            if (!cuttedObj) return;

            if(cuttedObjects.Remove(cuttedObj))
            {                
                InitCuttingShader();
            }
        }

        /// <summary>
        /// 应用材质
        /// </summary>
        private Material _applyMaterial
        {
            get
            {
                switch (_useCuttingMaterialType)
                {
                    case EUseCuttingMaterial.Intersection: return cuttingMaterail;
                    case EUseCuttingMaterial.Union: return unionCuttingMaterail;
                    default: throw new NotImplementedException();
                }
            }
        }

        private void InitCuttingShader()
        {
            // 剖面Shader
            Shader cuttingShader = null;
            if (_applyMaterial && ShaderHelper.GenericThreePlanesBSP.Valid(_applyMaterial))
            {
                cuttingShader = _applyMaterial.shader;
            }
            if (!cuttingShader)
            {
                Debug.LogError(CommonFun.Name(typeof(PlaneCuttingController), nameof(cuttingShader)) + "剖面Shader无效！");
                return;
            }

            // 获取剖面Shader渲染器列表
            var materials = GetCuttingShaderMaterials(GetCuttingShaderMaterials);
            if (materials.Count == 0 && !autoChangeShader)
            {
                Debug.LogError(CommonFun.Name(typeof(PlaneCuttingController), nameof(cuttedObjects)) + "无有效的剖面材质！");
                return;
            }

            // 清除剖面记录器
            RecoverShader();
            ClearShaderRecorder();

            // 自动设置剖面Shader
            if(autoChangeShader)
            {
                AddShaderRecored(GetCuttingShaderMaterials(), cuttingShader);
            }

            // 设置三剖面控制器
            if (threePlanesCuttingController)
            {
                threePlanesCuttingController.InitShaderProperty();
                threePlanesCuttingController.SetMaterials(materials.ToArray());
                threePlanesCuttingController.UpdatePlaneCuttingInfos();
            }
            else
            {
                Debug.LogError(CommonFun.Name(typeof(PlaneCuttingController), nameof(threePlanesCuttingController)) + "为空对象！");
                return;
            }
        }

        private List<Material> GetCuttingShaderMaterials(Func<Renderer, List<Material>> compareFun = null)
        {
            if (compareFun == null) compareFun = GetDefaultMaterials;

            // 获取被剖对象渲染器,去除空对象和排除对象
            var validObjects = cuttedObjects.Where(obj => obj && !_excludeOjbectMap.Contains(obj)).Distinct();

            var materials = new List<Material>();
            // 包含子对象
            if (includeChildren)
            {
                validObjects.Foreach(obj => obj.GetComponentsInChildren<Renderer>().Foreach(r =>
                {
                    if (!_excludeOjbectMap.Contains(r.gameObject))
                    {
                        materials.AddRange(compareFun.Invoke(r));
                    }
                }));
            }
            // 不包含子对象
            else
            {
                validObjects.Foreach(obj =>
                {
                    if (obj.GetComponent<Renderer>() is Renderer r && r)
                    {
                        if (!_excludeOjbectMap.Contains(r.gameObject))
                        {
                            materials.AddRange(compareFun.Invoke(r));
                        }
                    }
                });
            }

            return materials;
        }

        private List<Material> GetDefaultMaterials(Renderer renderer)
        {
            return renderer.materials.ToList();
        }

        private List<Material> GetCuttingShaderMaterials(Renderer renderer)
        {
            var materials = new List<Material>();
            foreach (var mat in renderer.materials)
            {
                if (ShaderHelper.GenericThreePlanesBSP.Valid(mat))
                {
                    materials.Add(mat);
                }
            }
            return materials;
        }

        private void AddShaderRecored(List<Material> materials, Shader shader)
        {
            foreach (var mat in materials)
            {
                if (forceChangeShader || !ShaderHelper.GenericThreePlanesBSP.Valid(mat))
                {
                    shaderRecorder.Add(mat, mat.shader);
                    mat.shader = shader;
                }
            }
        }

        private void RecoverShader()
        {
            foreach (var item in shaderRecorder)
            {
                item.Key.shader = item.Value;
            }
        }

        private void ClearShaderRecorder()
        {
            shaderRecorder.Clear();
        }
    }

    /// <summary>
    /// 基础剖面组件
    /// </summary>
    [RequireManager(typeof(ToolsExtensionManager))]
    public abstract class BasePlaneCuttingMB : ToolMB { }
}

