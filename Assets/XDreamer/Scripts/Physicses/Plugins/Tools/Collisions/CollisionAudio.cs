using UnityEngine;
using UnityEngine.Audio;
using XCSJ.Attributes;
using XCSJ.Extension.Audios;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginPhysicses.Tools.Collisions
{
    /// <summary>
    /// 碰撞音频
    /// </summary>
    [DisallowMultipleComponent]
    [Name("碰撞音频")]
    [XCSJ.Attributes.Icon(EIcon.Audio)]
    [Tool(PhysicsManager.Title, rootType = typeof(PhysicsManager))]
    public class CollisionAudio : CollisionObserver
    {
        /// <summary>
        /// 碰撞音频剪辑
        /// </summary>
        [Name("碰撞音频剪辑")]
        public AudioClip _collisionClip;

        /// <summary>
        /// 混合音频
        /// </summary>
        [Name("混合音频")]
        public AudioMixerGroup audioMixer;

        /// <summary>
        /// 音量乘积
        /// </summary>
        [Name("音量乘积")]
        public float _volumeMultiplier = 0.05f;

        /// <summary>
        /// 碰撞进入
        /// </summary>
        /// <param name="collision"></param>
        protected override void OnCollisionEnterWithMiniVelocity(Collision collision)
        {
            if (_collisionClip)
            {
                AudioHelper.PlayTemporaryAudioClip(_collisionClip, collision.GetContact(0).point, new Vector2(5, 50), UnityEngine.Random.Range(.9f, 1.1f), collision.relativeVelocity.magnitude * _volumeMultiplier, audioMixer);
            }
        }
    }
}
