using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.PluginTools;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Maths;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.ExplodedViews.MBs
{
    /// <summary>
    /// 爆炸图:对爆炸目标游戏对象的子级游戏对象执行爆炸图
    /// </summary>
    [Name("爆炸图")]
    [DisallowMultipleComponent]
    [Tool("模型", rootType = typeof(ToolsExtensionManager))]
    [Tip("对爆炸目标游戏对象的子级游戏对象执行爆炸图")]
    [XCSJ.Attributes.Icon(EIcon.ExplodedView)]
    [RequireManager(typeof(ToolsExtensionManager))]
    public class ExplodedView : MB, IPlayer
    {
        #region 爆炸目标

        /// <summary>
        /// 爆炸目标类型
        /// </summary>
        [Name("爆炸目标类型")]
        [EnumPopup]
        public ETargetType _targetType = ETargetType.TargetChildren;

        /// <summary>
        /// 爆炸目标:如果爆炸目标无效，则选择当前游戏对象作为爆炸目标
        /// </summary>
        [Name("爆炸目标")]
        [Tip("如果爆炸目标无效，则选择当前游戏对象作为爆炸目标")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_targetType), EValidityCheckType.NotEqual, ETargetType.TargetChildren)]
        public GameObject _explodedTarget;

        /// <summary>
        /// 爆炸目标
        /// </summary>
        public GameObject explodedTarget => _explodedTarget ? _explodedTarget : _explodedTarget = this.gameObject;

        /// <summary>
        /// 目标类型
        /// </summary>
        public enum ETargetType
        {
            /// <summary>
            /// 目标成员
            /// </summary>
            [Name("目标成员")]
            TargetChildren,

            /// <summary>
            /// 目标列表
            /// </summary>
            [Name("目标列表")]
            TargetList,
        }

        /// <summary>
        /// 爆炸目标列表
        /// </summary>
        [Name("爆炸目标列表")]
        [HideInSuperInspector(nameof(_targetType), EValidityCheckType.NotEqual, ETargetType.TargetList)]
        public List<Transform> _explodedTargets = new List<Transform>();

        /// <summary>
        /// 获取爆炸变换列表
        /// </summary>
        public IEnumerable<Transform> GetExplodedTransforms()
        {
            switch (_targetType)
            {
                case ETargetType.TargetChildren:
                    {
                        foreach (Transform t in explodedTarget.transform)
                        {
                            yield return t;
                        }
                        break;
                    }
                case ETargetType.TargetList:
                    {
                        foreach (Transform t in _explodedTargets)
                        {
                            yield return t;
                        }
                        break;
                    }
            }
        }

        #endregion

        #region 爆炸设置

        /// <summary>
        /// 爆炸数据
        /// </summary>
        [Name("爆炸数据")]
        [Readonly]
        public List<ExplodeData> datas = new List<ExplodeData>();

        /// <summary>
        /// 爆炸视图类型
        /// </summary>
        [Name("爆炸视图类型")]
        [EnumPopup]
        public EExplodeType explodeType = EExplodeType.Point;

        /// <summary>
        /// 排序规则
        /// </summary>
        [Name("排序规则")]
        [EnumPopup]
        public ESortRule _sortRule = ESortRule.DistanceAsc;

        /// <summary>
        /// 中心类型
        /// </summary>
        [Name("中心类型")]
        [EnumPopup]
        public ECenterType centerType = ECenterType.TransformPosition;

        /// <summary>
        /// 中心位置:爆炸中心的世界坐标
        /// </summary>
        [Name("中心位置")]
        [Tip("爆炸中心的世界坐标")]
        [HideInSuperInspector(nameof(centerType), EValidityCheckType.NotEqual, ECenterType.Postion)]
        public Vector3 centerPosition = Vector3.zero;

        /// <summary>
        /// 中心变换:通过中心变换获取爆炸中心的世界坐标
        /// </summary>
        [Name("中心变换")]
        [Tip("通过中心变换获取爆炸中心的世界坐标")]
        [HideInSuperInspector(nameof(centerType), EValidityCheckType.Less | EValidityCheckType.Or, ECenterType.TransformPosition, nameof(centerType), EValidityCheckType.GreaterEqual, ECenterType.BoundsCenter)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform centerTransofrm;

        /// <summary>
        /// 中心偏移量:用于额外纠正爆炸中心的偏移量
        /// </summary>
        [Name("中心偏移量")]
        [Tip("用于额外纠正爆炸中心的偏移量")]
        public Vector3 centerOffset = Vector3.zero;

        /// <summary>
        /// 方向类型
        /// </summary>
        [Name("方向类型")]
        [EnumPopup]
        public EDirectionType directionType = EDirectionType.Vector;

        /// <summary>
        /// 方向向量
        /// </summary>
        [Name("方向向量")]
        [HideInSuperInspector(nameof(directionType), EValidityCheckType.NotEqual, EDirectionType.Vector)]
        public Vector3 directionVector = Vector3.right;

        /// <summary>
        /// 方向变换
        /// </summary>
        [Name("方向变换")]
        [HideInSuperInspector(nameof(directionType), EValidityCheckType.Equal, EDirectionType.Vector)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform directionTransform;

        /// <summary>
        /// 增量间隔值:爆炸计算时，对象包围盒更新爆炸时每次的增量间隔距离值；
        /// </summary>
        [Name("增量间隔值")]
        [Tip("爆炸计算时，对象包围盒更新爆炸时每次的增量间隔距离值；")]
        [Range(0.001f, 1)]
        public float deltaIntervalValue = 0.01f;

        /// <summary>
        /// 最小间隔值:爆炸完成后，任意两个对象包围盒之间的最小间隔距离
        /// </summary>
        [Name("最小间隔值")]
        [Tip("爆炸完成后，任意两个对象包围盒之间的最小间隔距离")]
        [Range(0.001f, 1)]
        public float minIntervalValue = 0.02f;

        /// <summary>
        /// 爆炸倍数:可用于将计算的爆炸结果做额外爆炸的倍数值
        /// </summary>
        [Name("爆炸倍数")]
        [Tip("可用于将计算的爆炸结果做额外爆炸的倍数值")]
        [Range(0.001f, 5)]
        public float explodeMultiple = 1;

        #endregion

        #region 播放设置

        [Group("播放设置")]
        [Name("启用时播放")]
        [Tip("当前组件启用后是否播放爆炸动画")]
        public bool playOnEnable = true;

        [Name("禁用时恢复")]
        [Tip("当前组件禁用后是否恢复到爆炸前的状态")]
        public bool recovryOnDisable = true;

        [Name("持续时间")]
        [Tip("播放爆炸动画的时长；单位为秒；")]
        [Range(0.001f, 30f)]
        public float durtion = 3;

        [Name("启用时更新记录")]
        public bool updateRecordOnEnable = false;

        #endregion

        #region MB

        private float time = 0;

        /// <summary>
        /// 唤醒初始化
        /// </summary>
        public void Awake()
        {
            datas.Clear();
        }

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            if (updateRecordOnEnable)
            {
                Clear();
                Record();
            }
            if (playOnEnable)
            {
                Play();
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            Stop();
            if (recovryOnDisable)
            {
                Clear();
            }
            playerState = EPlayerState.Init;
        }

        public void Update()
        {
            switch (playerState)
            {
                case EPlayerState.Play:
                    {
                        Record();
                        Recovry();

                        datas = ExplodedViewHelper.Explode(explodeType, datas, center, direction, deltaIntervalValue, minIntervalValue, _sortRule);
                        UpdatePercent(0);

                        playerState = EPlayerState.Playing;
                        time = 0;
                        break;
                    }
                case EPlayerState.Playing:
                    {
                        time += Time.deltaTime;
                        var p = time / durtion;
                        if (p >= 1)
                        {
                            playerState = EPlayerState.Finished;
                        }
                        UpdatePercent(Mathf.Clamp01(p) * explodeMultiple);
                        break;
                    }
                case EPlayerState.Finished:
                case EPlayerState.Stop:
                    {
                        playerState = EPlayerState.Init;
                        time = 0;
                        break;
                    }
            }
        }

#if UNITY_EDITOR

        private bool inSimulation = false;
        private Vector3 drawCenter = Vector3.zero;
        private Vector3 drawDirection = Vector3.right;

        public void SetSimulation(bool simulation) => inSimulation = simulation;
        public void SetDrawInfo(Vector3 drawCenter, Vector3 drawDirection)
        {
            this.drawCenter = drawCenter;
            this.drawDirection = drawDirection;
        }

        public void OnDrawGizmosSelected()
        {
            if (!inSimulation) return;

            var color = Gizmos.color;
            {
                //绘制
                foreach (var data in datas)
                {
                    //包围盒
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireCube(data.bounds.center, data.bounds.size);

                    //中心与方向
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(data.center, data.center + data.dir);
                }

                //原始爆炸中心与方向
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(drawCenter, drawCenter + drawDirection);
            }
            Gizmos.color = color;
        }

#endif

        #endregion

        #region 爆炸相关的处理函数

        /// <summary>
        /// 爆炸中心
        /// </summary>
        public Vector3 center
        {
            get
            {
                switch (centerType)
                {
                    case ECenterType.Postion: return centerPosition;
                    case ECenterType.TransformPosition: return centerTransofrm.position;
                    case ECenterType.TransformBoundsCenter:
                        {
                            if (CommonFun.GetBounds(out Bounds bounds, centerTransofrm.gameObject))
                            {
                                return bounds.center;
                            }
                            throw new InvalidProgramException("无效的变换包围盒中心");
                        }
                    case ECenterType.BoundsCenter:
                        {
                            if (CommonFun.GetBounds(out Bounds bounds, explodedTarget))
                            {
                                return bounds.center;
                            }
                            throw new InvalidProgramException("无效的包围盒中心：" + centerType.ToString());
                        }
                    default: throw new InvalidProgramException("无效中心类型：" + centerType.ToString());
                }
            }
        }

        /// <summary>
        /// 获取爆炸中心
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        private Vector3 GetCenter(List<GameObject> objects)
        {
            switch (centerType)
            {
                case ECenterType.Postion: return centerPosition;
                case ECenterType.TransformPosition: return centerTransofrm.position;
                case ECenterType.TransformBoundsCenter:
                    {
                        if (CommonFun.GetBounds(out Bounds bounds, centerTransofrm.gameObject))
                        {
                            return bounds.center;
                        }
                        throw new InvalidProgramException("无效的变换包围盒中心");
                    }
                case ECenterType.BoundsCenter:
                    {
                        if (CommonFun.GetBounds(out Bounds bounds, objects))
                        {
                            return bounds.center;
                        }
                        throw new InvalidProgramException("无效的包围盒中心：" + centerType.ToString());
                    }
                default: throw new InvalidProgramException("无效中心类型：" + centerType.ToString());
            }
        }

        /// <summary>
        /// 爆炸方向
        /// </summary>
        public Vector3 direction
        {
            get
            {
                switch (directionType)
                {
                    case EDirectionType.Vector: return directionVector;
                    case EDirectionType.TransformX: return directionTransform.right;
                    case EDirectionType.TransformY: return directionTransform.up;
                    case EDirectionType.TransformZ: return directionTransform.forward;
                    case EDirectionType.CenterToTransform: return center - directionTransform.position;
                    default: throw new InvalidProgramException("无效方向类型：" + directionType.ToString());
                }
            }
        }

        /// <summary>
        /// 获取爆炸方向
        /// </summary>
        /// <param name="center"></param>
        /// <returns></returns>
        private Vector3 GetDirection(Vector3 center)
        {
            switch (directionType)
            {
                case EDirectionType.Vector: return directionVector;
                case EDirectionType.TransformX: return directionTransform.right;
                case EDirectionType.TransformY: return directionTransform.up;
                case EDirectionType.TransformZ: return directionTransform.forward;
                case EDirectionType.CenterToTransform: return center - directionTransform.position;
                default: throw new InvalidProgramException("无效方向类型：" + directionType.ToString());
            }
        }

        /// <summary>
        /// 记录数据
        /// </summary>
        public void Record()
        {
            if (datas.Count > 0) return;
            foreach (Transform t in GetExplodedTransforms())
            {
                datas.Add(new ExplodeData(t));
            }
        }

        /// <summary>
        /// 恢复
        /// </summary>
        public void Recovry() => datas.ForEach(data => data.Recover());

        /// <summary>
        /// 清除数据；先恢复后清除
        /// </summary>
        public void Clear()
        {
            Recovry();
            datas.Clear();
        }

        /// <summary>
        /// 更新变换
        /// </summary>
        public void UpdateTranforms()
        {
            datas.ForEach(data => data.UpdateTranform());
        }

        /// <summary>
        /// 更新百分比,会同步更新变换
        /// </summary>
        /// <param name="percent"></param>
        public void UpdatePercent(float percent)
        {
            datas.ForEach(data =>
            {
                data.UpatePercent(percent);
                data.UpdateTranform();
            });
        }

        #endregion

        #region IPlayer

        public EPlayerState playerState { get; private set; } = EPlayerState.Init;

        public bool IsPlaying() => playerState == EPlayerState.Playing;

        public bool Play()
        {
            switch (playerState)
            {
                case EPlayerState.Init:
                    {
                        playerState = EPlayerState.Play;
                        return true;
                    }
                default: return false;
            }
        }

        public void Stop()
        {
            switch (playerState)
            {
                case EPlayerState.Play:
                case EPlayerState.Playing:
                case EPlayerState.Pause:
                case EPlayerState.Resume:
                    {
                        playerState = EPlayerState.Stop;
                        break;
                    }
            }
        }

        public bool Pause()
        {
            switch (playerState)
            {
                case EPlayerState.Playing:
                    {
                        playerState = EPlayerState.Pause;
                        return true;
                    }
                default: return false;
            }
        }

        public bool Resume()
        {
            switch (playerState)
            {
                case EPlayerState.Pause:
                    {
                        playerState = EPlayerState.Playing;
                        return true;
                    }
                default: return false;
            }
        }

        #endregion
    }
}
