using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginPhysicses.Tools.Collisions
{
    /// <summary>
    /// 碰撞观察器
    /// </summary>
    [RequireManager(typeof(PhysicsManager))]
    public abstract class CollisionObserver : BasePhysicsMB
	{
	    /// <summary>
	    /// 碰撞发生最小力
	    /// </summary>
	    [Name("碰撞触发最小速度值")]
	    public float _minVelocity = 5f;

		/// <summary>
		/// 开始
		/// </summary>
		protected virtual void Start() { }

		/// <summary>
		/// 碰撞进入
		/// </summary>
		/// <param name="collision">与当前对象发生碰撞的对象</param>
		protected virtual void OnCollisionEnter(Collision collision)
	    {
	        if (collision.contacts.Length >0  || collision.relativeVelocity.magnitude > _minVelocity)
            {
				OnCollisionEnterWithMiniVelocity(collision);
			}
	    }

		/// <summary>
		/// 碰撞退出
		/// </summary>
		/// <param name="collision">与当前对象发生碰撞的对象</param>
		protected virtual void OnCollisionExit(Collision collision) { }
	
		/// <summary>
		/// 碰撞进入（最小速度约束）
		/// </summary>
		/// <param name="collision"></param>
	    protected abstract void OnCollisionEnterWithMiniVelocity(Collision collision);
	}
}
