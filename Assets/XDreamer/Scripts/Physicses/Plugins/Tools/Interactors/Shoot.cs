using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginPhysicses.Base;
using XCSJ.PluginTools.Interactions.Base;

namespace XCSJ.PluginPhysicses.Tools.Interactors
{
    /// <summary>
    /// 发射:用于模拟枪械射击或将一个游戏对象发射出去
    /// </summary>
    [Name("发射")]
    [Tip("用于模拟枪械射击或将一个游戏对象发射出去")]
    [XCSJ.Attributes.Icon(EIcon.Target)]
    [Tool(PhysicsManager.Title, rootType = typeof(PhysicsManager))]
    public class Shoot : BasePhysicsMB
    {
        /// <summary>
        /// 自动发射
        /// </summary>
        [Name("自动发射")]
        public bool _autoFire = false;

        /// <summary>
        /// 自动发射时间间隔
        /// </summary>
        [Name("自动发射时间间隔")]
        [Min(0.01f)]
        [HideInSuperInspector(nameof(_autoFire), EValidityCheckType.False)]
        public float _autoFireIntervalTime = 5f;

        private float timeCounter = 0;

        /// <summary>
        /// 发射对象
        /// </summary>
        [Name("发射对象")]
        public ForceTarget _forceTarget = new ForceTarget(EForceTargetRule.Fixed);

        /// <summary>
        /// 施力模式
        /// </summary>
        [Name("施力模式")]
        public ForceModeInfo _forceModeInfo = new ForceModeInfo();

        /// <summary>
        /// 方向位置参考点
        /// </summary>
        [Name("方向位置参考点")]
        public ReferencePoint _referencePoint = new ReferencePoint();

        /// <summary>
        /// 炮弹容量
        /// </summary>
        [Name("炮弹容量")]
        [Tip("炮弹容量")]
        public int _capacity = 100;

        /// <summary>
        /// 当前容量
        /// </summary>
        public int count { get; private set; } = 0;

        /// <summary>
        /// 自动销毁发射对象
        /// </summary>
        [Name("自动销毁发射对象")]
        [Tip("发射对象在运行时动态产生，为了效率可定时销毁该对象")]
        public bool _autoDestoryFireObject = false;

        /// <summary>
        /// 发射对象生命时长
        /// </summary>
        [Name("发射对象生命时长")]
        [Min(1)]
        [HideInSuperInspector(nameof(_autoDestoryFireObject), EValidityCheckType.Equal, false)]
        public float _lifeTime = 100;

        /// <summary>
        /// 开始
        /// </summary>
        protected void Start()
        {
            count = _capacity;
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected void Update()
        {
            if (_autoFire)
            {
                if (timeCounter >= _autoFireIntervalTime)
                {
                    timeCounter = 0;
                    Fire();
                }
                timeCounter += Time.deltaTime;
            }
        }

        /// <summary>
        /// 发射
        /// </summary>
        public void Fire()
        {
            if (!_forceTarget.DataValidity()) return;

            if (count <= 0)
            {
                return;
            }
            else
            {
                --count;
            }

            if (_referencePoint.TryGetPositionAndDirection(out Vector3 pos, out Vector3 dir))
            {
                var rig = _forceTarget.GetObject();
                if (rig)
                {
                    rig = UnityEngine.Object.Instantiate(rig);
                    rig.gameObject.SetActive(true);
                    rig.transform.position = pos;

                    PhysicsManager.instance.AddCloneObject(rig.gameObject, _autoDestoryFireObject ? _lifeTime : -1);

                    rig.AddForce(dir * _forceModeInfo.GetValue(), _forceModeInfo._forceMode);
                }
            }
        }
    }
}
