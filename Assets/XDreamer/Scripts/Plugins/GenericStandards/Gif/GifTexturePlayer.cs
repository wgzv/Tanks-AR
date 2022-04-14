using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;
using XCSJ.Tools;

namespace XCSJ.Extension.GenericStandards.Gif
{
    /// <summary>
    /// GIF纹理播放器
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [RequireManager(typeof(GenericStandardScriptManager), typeof(ToolsManager))]
    public abstract class GifTexturePlayer : MB, IBasePlayer, IPause, IResume, ITool
    {
        /// <summary>
        /// 启动时播放:启动时会尝试直接播放GIF序列帧动画(在组件禁用后停止，启用时播放)
        /// </summary>
        [Name("启动时播放")]
        [Tip("启动时会尝试直接播放GIF序列帧动画")]
        public bool playOnStart = true;

        /// <summary>
        /// 期望的播放循环次数；
        /// 如果 值小于等于 0 认为无限循环;
        /// 大于 0 ，在播放了制定次数后，定位在末一帧纹理上；
        /// </summary>
        [Name("播放循环次数")]
        [Tip("限制GIF序列帧动画的循环播放次数；值小于等于0 认为无限循环;")]
        public int playLoopCount = 0;

        /// <summary>
        /// 播放速度:播放整体速度，每帧间隔时间会除这个系数
        /// </summary>
        [Name("播放速度")]
        [Tip("播放整体速度，每帧间隔时间会除这个系数")]
        [Range(0.01f, 1000)]
        public float playSpeed = 1;

        /// <summary>
        /// 如果期望最开始时暂停播放GIF，可将本项设置为true；
        /// </summary>
        [Name("暂停动画")]
        [Tip("设置GIF序列帧动画暂停；如果期望最开始时暂停播放GIF，可将本项设置为勾选")]
        public bool pauseAnimation = false;

        [Name("Gif纹理")]
        [Tip("Gif纹理资源的存储对象")]
        public GifTexture gifTexture = new GifTexture();

        /// <summary>
        /// 已经播放循环次数
        /// </summary>
        public int alreadyPlayLoopCount { get; private set; }

        /// <summary>
        /// 状态
        /// </summary>
        public EState state { get; private set; }

        /// <summary>
        ///  在播放中，在正常播放或暂停都会返回 true；
        /// </summary>
        public bool inPlaying => (state == EState.Playing || state == EState.Pause);

        /// <summary>
        /// 表示是否准备好播放
        /// </summary>
        public bool readyToPlay => state == EState.Ready;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            //Debug.Log("Start:" + gameObject.name);
            state = EState.Loading;
            gifTexture.Init();
            if (playOnStart) Play();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            Stop();
        }

        private IEnumerator GifLoopCoroutine()
        {
            if (!readyToPlay)
            {
                Debug.LogWarning(string.Format("游戏对象: {0} 的待渲染的GIF纹理状态未准备！", CommonFun.GameObjectToString(gameObject)));
                yield break;
            }
            if (!gifTexture.canUse)
            {
                Debug.LogWarning(string.Format("游戏对象: {0} 的待渲染的GIF纹理无效！", CommonFun.GameObjectToString(gameObject)));
                yield break;
            }
            // play start
            state = EState.Playing;
            alreadyPlayLoopCount = 0;
            do
            {
                foreach (var gifTex in gifTexture.textures)
                {
                    // Change texture;
                    OnUpdateGifTexture(gifTex.texture2d);
                    // Delay
                    //float delayedTime = Time.time + gifTex.delaySec/playSpeed;
                    //while (delayedTime > Time.time)
                    //{
                    //    yield return 0;
                    //}
                    yield return new WaitForSeconds(gifTex.delaySec / playSpeed);
                    // Pause
                    //while (pauseAnimation)
                    //{
                    //    yield return 0;
                    //}
                    yield return new WaitWhile(() => pauseAnimation);
                }
                alreadyPlayLoopCount++;
                // 至少要等一帧？~不然如果所有的gifTex.delaySec<=0 且不暂停时，会无限死循环~
                yield return new WaitForEndOfFrame();
            } while (playLoopCount <= 0 || alreadyPlayLoopCount < playLoopCount);
        }

        /// <summary>
        /// 当更新GIF纹理时
        /// </summary>
        /// <param name="texture2D"></param>
        protected abstract void OnUpdateGifTexture(Texture2D texture2D);

        /// <summary>
        /// 重新加载：如果当前是播放中会取消播放，之后进行重新加载；需要使用者自行调用播放；
        /// </summary>
        /// <param name="obj"></param>
        public bool Reload(TextAsset obj, bool autoPlay = true)
        {
            if (!obj) return false;
            Stop();
            state = EState.Loading;
            if (!GifTexture.UpdateGifTexture(obj, gifTexture)) state = EState.Error;
            if (autoPlay)
            {
                Play();
                return inPlaying;
            }
            return readyToPlay;
        }

        /// <summary>
        /// 重新加载：如果当前是播放中会取消播放，之后进行重新加载；需要使用者自行调用播放；
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="autoPlay"></param>
        /// <param name="mustReload"></param>
        /// <returns></returns>
        public bool Reload(GifTextureAsset obj, bool autoPlay = true, bool mustReload = false)
        {
            if (!obj) return false;
            if (gifTexture == obj.gifTexture)
            {
                if (autoPlay)
                {
                    Play();
                    return inPlaying;
                }
                return true;
            }
            if (!mustReload && gifTexture != null && gifTexture.name == obj.gifTexture.name)
            {
                //可能是同名的资源，替换??
                if (autoPlay)
                {
                    Play();
                    return inPlaying;
                }
                return true;
            }
            Stop();
            state = EState.Loading;
            gifTexture = obj.gifTexture;
            state = EState.Ready;
            if (autoPlay)
            {
                Play();
                return inPlaying;
            }
            return false;
        }

        #region 播放器控制接口

        /// <summary>
        /// 播放; 会对GIF纹理做检测；会检测当前状态是已准备；如果在播放中，不处理；
        /// </summary>
        public bool Play()
        {
            if (inPlaying) return true;
            if (gifTexture.Check()) state = EState.Ready;
            if (!readyToPlay)
            {
                //Debug.LogWarning("GIF状态未准备好！");
                return false;
            }
            pauseAnimation = false;
            StopAllCoroutines();
            StartCoroutine(GifLoopCoroutine());
            return true;
        }

        /// <summary>
        /// 停止
        /// </summary>
        public bool Stop()
        {
            if (!inPlaying)
            {
                //Debug.Log("GIF状态非播放或暂停！");
                return false;
            }
            StopAllCoroutines();
            state = EState.Ready;
            return true;
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public bool Pause()
        {
            if (state != EState.Playing)
            {
                //Debug.Log("GIF状态非播放！");
                return false;
            }
            pauseAnimation = true;
            state = EState.Pause;
            return true;
        }

        /// <summary>
        /// 恢复继续
        /// </summary>
        public bool Resume()
        {
            if (state != EState.Pause)
            {
                //Debug.Log("GIF状态非暂停！");
                return false;
            }
            pauseAnimation = false;
            state = EState.Playing;
            return true;
        }
        
        /// <summary>
        /// 是否正在播放中
        /// </summary>
        /// <returns></returns>
        public bool IsPlaying()
        {
            return inPlaying;
        }

        void IStop.Stop()
        {
            Stop();
        }

        #endregion
    }

    /// <summary>
    /// 状态
    /// </summary>
    public enum EState
    {
        /// <summary>
        /// 无
        /// </summary>
        None,

        /// <summary>
        /// 加载中
        /// </summary>
        Loading,

        /// <summary>
        /// 已准备好
        /// </summary>
        Ready,

        /// <summary>
        /// 播放中
        /// </summary>
        Playing,

        /// <summary>
        /// 暂停
        /// </summary>
        Pause,

        /// <summary>
        /// 错误
        /// </summary>
        Error,
    }
}
