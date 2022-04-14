using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginPhysicses.Base.Limits
{
    /// <summary>
    /// 限定接口：使用最小最大值进行数值计算的接口
    /// </summary>
    public interface ILimiter
    {
        /// <summary>
        /// 最小值
        /// </summary>
        float min { get; }

        /// <summary>
        /// 最大值
        /// </summary>
        float max { get; }

        /// <summary>
        /// 当前值
        /// </summary>
        float currentValue { get; }

        /// <summary>
        /// 误差区域
        /// 用于纠正物理对象起始位置的误差。误差原因在于场景启动后物理引擎对游戏对象位置进行设定使其开始位置产生微小偏移;
        /// </summary>
        float deadZone { get; }

        /// <summary>
        /// 初始化
        /// </summary>
        void Init();
    }

    /// <summary>
    /// 限定器：用于提供限定计算数据的类对象
    /// </summary>
    public abstract class Limiter : ILimiter
    {
        /// <summary>
        /// 阈值
        /// </summary>
        [Name("阈值")]
        [Tip("用于衡量最小最大状态的百分比值")]
        [LimitRange(0, 1)]
        public Vector2 _range = new Vector2(0.1f, 0.9f);

        /// <summary>
        /// 误差区域：俗称死区，用于纠正物理对象起始位置的误差。误差原因在于场景启动后物理引擎对游戏对象位置进行设定使其开始位置产生微小偏移;当前值应小于阈值;
        /// </summary>
        [Name("误差区域")]
        [Tip("俗称死区，用于纠正物理对象起始位置的误差。误差原因在于场景启动后物理引擎对游戏对象位置进行设定使其开始位置产生微小偏移;当前值应小于阈值;")]
        [Range(0, 1)]
        public float _deadZone = 0.025f;

        /// <summary>
        /// 最小值
        /// </summary>
        public virtual float min => _range.x;

        /// <summary>
        /// 最大值
        /// </summary>
        public virtual float max => _range.y;

        /// <summary>
        /// 当前值
        /// </summary>
        public abstract float currentValue { get; }

        /// <summary>
        /// 误差区域
        /// </summary>
        public float deadZone => _deadZone;

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init() { }
    }
}
