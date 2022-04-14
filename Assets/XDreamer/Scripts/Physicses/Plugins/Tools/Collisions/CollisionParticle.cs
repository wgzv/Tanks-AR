using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginPhysicses.Tools.Collisions
{
    /// <summary>
    /// 碰撞粒子发生器
    /// </summary>
    [DisallowMultipleComponent]
	[Name("碰撞粒子发生器")]
    [Tool(PhysicsManager.Title, rootType = typeof(PhysicsManager))]
    public class CollisionParticle : CollisionObserver
	{
	    /// <summary>
	    /// 粒子模版
	    /// </summary>
	    [Name("粒子模版")]
		[ValidityCheck(EValidityCheckType.NotNull)]
	    public ParticleSystem particleTmplate = null;
	
	    /// <summary>
	    /// 最大粒子数
	    /// </summary>
	    [Name("最大粒子数")]
	    public int _maxParticleCount = 5;
	
	    private List<ParticleSystem> collisionParticles = new List<ParticleSystem>();
	
	    private const string ParticleRootName = "碰撞粒子组";

        /// <summary>
        /// 唤醒
        /// </summary>
		protected void Awake() => CreateParticles();

        /// <summary>
        /// 销毁
        /// </summary>
		protected void OnDestroy() => DestroyParticles();

        /// <summary>
        /// 碰撞进入
        /// </summary>
        /// <param name="collision"></param>
		protected override void OnCollisionEnterWithMiniVelocity(Collision collision) => PlayParticles(collision.GetContact(0).point);

        private void CreateParticles()
        {
            if (particleTmplate)
            {
                var particleRoot = UnityObjectHelper.CreateGameObject(ParticleRootName);
                particleRoot.XSetParent(transform);

                for (int i = 0; i < _maxParticleCount; i++)
                {
                    var go = (GameObject)Instantiate(particleTmplate.gameObject, transform.position, Quaternion.identity) as GameObject;
                    go.XSetParent(particleRoot.transform);
                    var ps = go.GetComponent<ParticleSystem>();
                    var em = ps.emission;
                    em.enabled = false;

                    collisionParticles.Add(ps);
                }
            }
        }

		private void DestroyParticles()
        {
            foreach (var ps in collisionParticles)
            {
                UnityObjectHelper.XDestoryObject(ps.gameObject);
            }
        }

		private void PlayParticles(Vector3 position)
        {
            foreach (var ps in collisionParticles)
            {
                if (!ps.isPlaying)
                {
                    ps.transform.position = position;
                    var em = ps.emission;
                    em.enabled = true;
                    ps.Play();
                }
            }
        }
	}
}