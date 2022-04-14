using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.LineNotes
{
    /// <summary>
    /// 动画3D批注
    /// </summary>
    [Name("动画3D批注")]
    [RequireManager(typeof(ToolsManager))]
    public class AnimationLineNote3D : BaseLineNote
    {
        /// <summary>
        /// 播放规则
        /// </summary>
        [Name("启用时播放")]
        public bool _playOnEnable = true;

        /// <summary>
        /// 播放规则
        /// </summary>
        [Name("播放规则")]
        [EnumPopup]
        public EPlayRule _playRule = EPlayRule.Sequence;

        /// <summary>
        /// 播放规则
        /// </summary>
        public enum EPlayRule
        {
            [Name("无")]
            None,

            [Name("直接显示")]
            Direct,

            [Name("同步")]
            Sync,

            [Name("顺序")]
            Sequence,
        }

        /// <summary>
        /// 线渲染器动画列表
        /// </summary>
        [Name("线渲染器动画列表")]
        public LineRenderAnimation[] _lineRenderAnimations;

        /// <summary>
        /// 画布动画
        /// </summary>
        [Name("画布动画")]
        public CanvasGroupAnimation _canvasGroupAnimation;

        /// <summary>
        /// 文本动画
        /// </summary>
        [Name("文本动画")]
        public TextAnimation _textAnimation;

        /// <summary>
        /// 显示完成
        /// </summary>
        public static event Action<AnimationLineNote3D> onDisplayFinished;

        private bool displayFinished = false;

        private List<BaseAnimation> animationList = new List<BaseAnimation>();

        /// <summary>
        /// 唤醒
        /// </summary>
        public virtual void Awake()
        {
            // 线渲染器
            foreach (var item in _lineRenderAnimations)
            {
                item.Init();
                if (item.valid)
                {
                    animationList.Add(item);
                }
            }

            // 画布
            _canvasGroupAnimation.Init();
            if (_canvasGroupAnimation.valid)
            {
                animationList.Add(_canvasGroupAnimation);
            }

            // 文字
            _textAnimation.Init();
            if (_textAnimation.valid)
            {
                animationList.Add(_textAnimation);
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            if (_playOnEnable) StartPlay();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();

            StopPlay();
        }

        /// <summary>
        /// 更新
        /// </summary>
        private void Update()
        {
            Playing();
        }

        private bool playing = false;

        private void StartPlay()
        {
            animationList.ForEach(a =>
            {
                a.ResetPlayState();

                switch (_playRule)
                {
                    case EPlayRule.None:
                    case EPlayRule.Direct:
                        {
                            a.Recover();
                            a.playState = EPlayState.End;
                            break;
                        }
                    case EPlayRule.Sync:
                        {
                            a.SetupPlay(true);
                            break;
                        }
                    case EPlayRule.Sequence:
                        {
                            a.SetupPlay(false);
                            break;
                        }
                }
            });
            displayFinished = false;

            playing = true;
        }

        private void Playing()
        {
            if (!playing) return;

            switch (_playRule)
            {
                case EPlayRule.Sync:// 同步
                    {
                        animationList.ForEach(a => a.Play());
                        break;
                    }
                case EPlayRule.Sequence:// 顺序播放
                    {
                        foreach (var item in animationList)
                        {
                            switch (item.playState)
                            {
                                case EPlayState.None:
                                    {
                                        item.StartPlay();
                                        return;
                                    }
                                case EPlayState.Begin:
                                case EPlayState.Playing:
                                    {
                                        item.Play();
                                        return;
                                    }
                            }
                        }
                        break;
                    }
            }

            if (!displayFinished)
            {
                displayFinished = animationList.All(a => a.playState == EPlayState.End);
                if (displayFinished)
                {
                    StopPlay();
                    onDisplayFinished?.Invoke(this);
                }
            }
        }

        private void StopPlay()
        {
            playing = false;
            animationList.ForEach(a => a.Recover());
        }

        /// <summary>
        /// 播放状态
        /// </summary>
        public enum EPlayState
        {
            [Name("无")]
            None,

            [Name("开始")]
            Begin,

            [Name("播放中")]
            Playing,

            [Name("结束")]
            End,
        }

        /// <summary>
        /// 动画
        /// </summary>
        public abstract class BaseAnimation
        {
            /// <summary>
            /// 动画时间
            /// </summary>
            [Name("动画时间")]
            [Min(0.01f)]
            public float _animationTime = 1;

            /// <summary>
            /// 动画曲线
            /// </summary>
            [Name("动画曲线")]
            public AnimationCurve _curve;

            /// <summary>
            /// 有效性
            /// </summary>
            public abstract bool valid { get; }

            /// <summary>
            /// 播放百分比
            /// </summary>
            public float percent { get; private set; } = 0;

            /// <summary>
            /// 是否播放中
            /// </summary>
            public bool isPlaying => percent < 1;

            /// <summary>
            /// 播放状态
            /// </summary>
            public EPlayState playState { get; set; } = EPlayState.None;

            /// <summary>
            /// 开始时间
            /// </summary>
            protected float startPlayTime = 0;

            /// <summary>
            /// 初始化
            /// </summary>
            public abstract void Init();

            /// <summary>
            /// 配置播放
            /// </summary>
            /// <param name="startPlay">开始播放</param>
            public virtual void SetupPlay(bool startPlay)
            {
                if (startPlay)
                {
                    StartPlay();
                }
                else
                {
                    playState = EPlayState.None;
                }
            }

            /// <summary>
            /// 恢复
            /// </summary>
            public abstract void Recover();

            /// <summary>
            /// 重置播放状态
            /// </summary>
            public virtual void ResetPlayState()
            {
                playState = EPlayState.None;
            }

            /// <summary>
            /// 开始播放 
            /// </summary>
            public virtual void StartPlay()
            {
                percent = 0;
                startPlayTime = Time.time;

                playState = EPlayState.Begin;
            }

            /// <summary>
            /// 播放动画
            /// </summary>
            public virtual void Play()
            {
                switch (playState)
                {
                    case EPlayState.Begin:
                        {
                            playState = EPlayState.Playing;
                            break;
                        }
                    case EPlayState.Playing:
                        {
                            if (percent < 1)
                            {
                                percent = _curve.Evaluate((Time.time - startPlayTime) / _animationTime);
                            }
                            else
                            {
                                playState = EPlayState.End;
                            }
                            break;
                        }
                    case EPlayState.End:
                        {
                            break;
                        }
                }                               
            }
        }

        /// <summary>
        /// 线渲染器动画
        /// </summary>
        [Serializable]
        public class LineRenderAnimation : BaseAnimation
        {
            /// <summary>
            /// 线渲染器
            /// </summary>
            [Name("线渲染器")]
            [Tip("使用只有两个点的线段")]
            [ValidityCheck(EValidityCheckType.NotNull)]
            public LineRenderer _line;

            private Vector3[] orgPositions;

            private Vector3[] positions;

            private int count = 0;

            private Vector3 p0p1Offset = Vector3.zero;

            /// <summary>
            /// 有效性
            /// </summary>
            public override bool valid => _line && count == 2;

            /// <summary>
            /// 初始化
            /// </summary>
            public override void Init()
            {
                if (_line)
                {
                    count = _line.positionCount;
                    if (count == 2)
                    {
                        orgPositions = new Vector3[count];
                        positions = new Vector3[count];
                        for (int i = 0; i < count; i++)
                        {
                            positions[i] = orgPositions[i] = _line.GetPosition(i);
                        }
                        if (count == 2)
                        {
                            p0p1Offset = orgPositions[1] - orgPositions[0];
                        }
                    }

                    if (count != 2)
                    {
                        Debug.LogErrorFormat("线渲染器位置数量必须为2!{0}", CommonFun.GameObjectComponentToStringWithoutAlias(_line));
                    }
                }
            }

            /// <summary>
            /// 配置播放
            /// </summary>
            /// <param name="startPlay">开始播放</param>
            public override void SetupPlay(bool startPlay)
            {
                base.SetupPlay(startPlay);
                
                for (int i = 0; i < count; i++)
                {
                    positions[i] = orgPositions[0];
                }
                _line.SetPositions(positions);
            }

            /// <summary>
            /// 播放动画
            /// </summary>
            public override void Play()
            {
                if (valid)
                {
                    base.Play();

                    if (count >= 2)
                    {
                        positions[1] = orgPositions[0] + p0p1Offset * percent;
                        _line.SetPositions(positions);
                    }
                }
            }

            /// <summary>
            /// 恢复
            /// </summary>
            public override void Recover()
            {
                if (valid)
                {
                    _line.SetPositions(orgPositions);
                }
            }
        }

        /// <summary>
        /// 文本动画
        /// </summary>
        [Serializable]
        public class TextAnimation : BaseAnimation
        {
            /// <summary>
            /// 文本
            /// </summary>
            [Name("文本")]
            [ValidityCheck(EValidityCheckType.NotNull)]
            public Text _title;

            /// <summary>
            /// 文本动画类型
            /// </summary>
            [Name("文本动画类型")]
            [EnumPopup]
            public ETextAnimationType _textAnimation = ETextAnimationType.TypeText;

            /// <summary>
            /// 文本动画类型
            /// </summary>
            public enum ETextAnimationType
            {
                [Name("无")]
                None,

                [Name("打字")]
                TypeText,
            }

            private string orgTitleText;

            /// <summary>
            /// 有效性
            /// </summary>
            public override bool valid => _title;

            /// <summary>
            /// 初始化
            /// </summary>
            public override void Init()
            {
                if (valid)
                {
                    orgTitleText = _title.text;
                }
            }

            /// <summary>
            /// 配置播放
            /// </summary>
            /// <param name="startPlay">开始播放</param>
            public override void SetupPlay(bool startPlay)
            {
                base.SetupPlay(startPlay);

                if (valid)
                {
                    switch (_textAnimation)
                    {
                        case ETextAnimationType.TypeText:
                            {
                                _title.text = "";
                                break;
                            }
                    }
                }
            }

            /// <summary>
            /// 播放动画
            /// </summary>
            public override void Play()
            {
                if (valid)
                {
                    base.Play();

                    switch (_textAnimation)
                    {
                        case ETextAnimationType.TypeText:
                            {
                                _title.text = orgTitleText.Substring(0, (int)Mathf.Lerp(0f, orgTitleText.Length, percent));
                                break;
                            }
                    }
                }
            }

            /// <summary>
            /// 恢复
            /// </summary>
            public override void Recover()
            {
                if (valid)
                {
                    _title.text = orgTitleText;
                }
            }
        }

        /// <summary>
        /// 动画
        /// </summary>
        [Serializable]
        public class CanvasGroupAnimation : BaseAnimation
        {
            /// <summary>
            /// 画布组
            /// </summary>
            [Name("画布组")]
            [ValidityCheck(EValidityCheckType.NotNull)]
            public CanvasGroup _contentCanvasGroup;

            private Vector3 canvasStartPos;
            private Vector3 canvasAnimationPos;

            /// <summary>
            /// 有效性
            /// </summary>
            public override bool valid => _contentCanvasGroup;

            /// <summary>
            /// 初始化
            /// </summary>
            public override void Init()
            {
                if (valid)
                {
                    canvasStartPos = _contentCanvasGroup.transform.localPosition;
                    canvasAnimationPos = canvasStartPos - Vector3.up * 0.05f;
                }
            }

            /// <summary>
            /// 配置播放
            /// </summary>
            /// <param name="startPlay">开始播放</param>
            public override void SetupPlay(bool startPlay)
            {
                base.SetupPlay(startPlay);

                if (valid)
                {
                    _contentCanvasGroup.alpha = 0;
                    _contentCanvasGroup.transform.localPosition = canvasAnimationPos;
                }
            }

            /// <summary>
            /// 播放动画
            /// </summary>
            public override void Play()
            {
                if (valid)
                {
                    base.Play();

                    _contentCanvasGroup.alpha = percent;
                    _contentCanvasGroup.transform.localPosition = Vector3.Lerp(canvasAnimationPos, canvasStartPos, percent);
                }
            }

            /// <summary>
            /// 恢复
            /// </summary>
            public override void Recover()
            {
                if (valid)
                {
                    _contentCanvasGroup.transform.localPosition = canvasStartPos;
                }
            }
        }
    }
}
