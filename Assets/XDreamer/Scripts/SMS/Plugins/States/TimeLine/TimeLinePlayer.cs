using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Interfaces;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.TimeLine
{
    /// <summary>
    /// 时间轴播放器:时间轴播放器组件是用于播放状态工作剪辑集合的对象。在设定的时间上播放工作片段剪辑，播放完成后，组件切换为完成态。
    /// </summary>
    [ComponentMenu("时间轴/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(TimeLinePlayer))]
    [XCSJ.Attributes.Icon(EIcon.Play)]
    [DisallowMultipleComponent]
    [Tip("时间轴播放器组件是用于播放状态工作剪辑集合的对象。在设定的时间上播放工作片段剪辑，播放完成后，组件切换为完成态。")]
    public class TimeLinePlayer : StateComponent<TimeLinePlayer>, ITimeClip, IWorkClipPlayer
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "时间轴播放器";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib("时间轴", typeof(SMSManager))]
        [StateComponentMenu("时间轴/"+ Title, typeof(SMSManager))]
#endif
        [Name(Title, nameof(TimeLinePlayer))]
        [Tip("时间轴播放器组件是用于播放状态工作剪辑集合的对象。在设定的时间上播放工作片段剪辑，播放完成后，组件切换为完成态。")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("播放内容")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [StateComponentPopup(typeof(TimeLinePlayContent), stateCollectionType = EStateCollectionType.Root, searchFlags = ESearchFlags.Default | ESearchFlags.FirstComponent | ESearchFlags.OptimizeComponent)]
        public TimeLinePlayContent playContent;

        [Name("进入播放")]
        [Tip("当时间轴播放器所在状态进入的时候，就启动播放")]
        public bool playOnEntry = true;

        [Name("播放时长")]
        [Tip("单位为秒")]
        [Range(0, TimeRange.DefaultMaxTimeLength)]
        public float duration = TimeRange.DefaultTimeLength;

        [Name("同步时长")]
        [Tip("将播放时长与播放内容的时长保持同步")]
        public bool useContentTimeLength = true;

        [Name("播放速度")]
        [Range(0, 100)]
        [SerializeField]
        [FormerlySerializedAs(nameof(speed))]
        private float _speed = 1;

        public float speed
        {
            get => _speed * parent.speed;
            set
            {
                if (value <= 0)
                {
                    _speed = 0;
                    return;
                }
                _speed = MathX.Scale(value, parent.speed, value);
            }
        }

        [Name("循环")]
        public bool isLoop = false;

        [Name("播放完成回调函数")]
        [UserDefineFun()]
        public string finishUserScriptCallback;

        [Name("退出时设置百分比")]
        [Tip("勾选,在当前状态组件退出时设置百分比进度为1;不勾选,不做处理;")]
        public bool setPercentOnExit = true;

        [Name("退出时百分比")]
        [Tip("当状态组件退出后,将当前状态组件逻辑数据保持在本百分比进度;值为0,可将数据尽量还原到初始化/进入的状态;")]
        [HideInSuperInspector(nameof(setPercentOnExit), EValidityCheckType.False)]
        [Range(0,1)]
        public float percentOnExit = 1;

        public bool isPlaying => _playerState == EPlayerState.Playing;

        public bool isPause => _playerState == EPlayerState.Pause;

        private EPlayerState _playerState = EPlayerState.None;

        public EPlayerState playerState
        {
            get => _playerState;
            set
            {
                if (_playerState != value)
                {
                    var lastPlayerState = _playerState;
                    _playerState = value;

                    if (playContent) playContent.OnPlayerStateChanged(this, lastPlayerState);

                    onPlayerStateChanged?.Invoke(this, lastPlayerState, _playerState);
                }
            }
        }

        /// <summary>
        /// 任意一个播放器的播放状态发生变化时均回调
        /// </summary>
        public static event Action<TimeLinePlayer, EPlayerState, EPlayerState> onPlayerStateChanged;

        public static event Action<TimeLinePlayer, float> onPlayerPercentChanged;

        public float curTime => duration * percent;

        protected float timeCounter { get; private set; } = 0;

        public float percent { get; private set; } = 0;

        public override float progress { get => percent; set => SetPercent(value); }

        private EPlayerState _beforeFinishState = EPlayerState.None;

        #region 生命周期函数

        public override bool Init(StateData stateData)
        {
            try
            {
                playerState = EPlayerState.Init;
                //BindPlayContent();
                return base.Init(stateData);
            }
            finally
            {
                playerState = EPlayerState.Free;
            }
        }

        public override void Release(StateData stateData)
        {
            base.Release(stateData);
            playerState = EPlayerState.Release;
        }

        public override void OnBeforeEntry(StateData stateData)
        {
            base.OnBeforeEntry(stateData);

            LoadContent();
        }

        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);

            timeCounter = 0;
            percent = 0;

            InitPlayContent();

            // 进入就播放
            if (playOnEntry)
            {
                Replay();
            }
        }

        public override void OnUpdate(StateData stateData)
        {
            base.OnUpdate(stateData);

            switch (playerState)
            {
                case EPlayerState.Playing:
                    {
                        UpdatePercent(stateData);
                        break;
                    }
                case EPlayerState.Play:
                    {
                        if (playContent) playContent.OnPlay();
                        playerState = EPlayerState.Playing;
                        UpdatePercent(stateData);
                        break;
                    }
                case EPlayerState.Resume:
                    {
                        playerState = EPlayerState.Playing;
                        UpdatePercent(stateData);
                        break;
                    }
                case EPlayerState.Finished:
                    {
                        // 结束时处于播放状态，并且循环，则再播放
                        if (isLoop && _beforeFinishState == EPlayerState.Playing)
                        {
                            Replay();
                        }
                        else
                        {
                            playerState = EPlayerState.Free;
                        }
                        finished = true;
                        break;
                    }
            }
        }

        private void UpdatePercent(StateData stateData)
        {
            //计算进度
            timeCounter += Time.deltaTime * speed;

            //设置进度            
            InternalSetPercent(MathX.Scale(timeCounter, duration, 1), stateData);
        }

        public override void OnExit(StateData data)
        {
            Stop();

            if (setPercentOnExit)
            {
                InternalSetPercent(percentOnExit, data);
            }

            base.OnExit(data);
        }

        public override void OnAfterExit(StateData data)
        {
            UnloadContent();

            base.OnAfterExit(data);
        }

        public override bool DataValidity() => base.DataValidity() && playContent;

        public override string ToFriendlyString() => duration + "秒";

        #endregion

        #region 播放内容

        public void SetTime(float time, StateData stateData = null)
        {
            //所在状态必须正在工作中.
            if (!parent.active) return;

            // 通过当前运行时间
            timeCounter = Mathf.Clamp(time, 0, duration);

            InternalSetPercent(MathX.ApproximatelyZero(duration) ? 0 : timeCounter / duration, stateData);
        }

        public void SetPercent(float percent, StateData stateData = null)
        {
            //所在状态必须正在工作中.
            if (!parent.active) return;

            // 通过百分比修改运行时间
            timeCounter = percent * duration;

            InternalSetPercent(percent, stateData);
        }

        private void InternalSetPercent(float percent, StateData stateData)
        {
            this.percent = MathX.Clamp01(percent);

            if (playContent) playContent.PlayContent(this.percent, StateData.Clone(stateData, EWorkMode.Play, parent));

            //回调进度变化的事件
            onPlayerPercentChanged?.Invoke(this, this.percent);
            onPlayerPercentChanged?.Invoke(this, this.percent);

            //检查是否播放完毕
            if (percent >= 1)
            {
                _beforeFinishState = playerState;

                Stop();

                ScriptManager.CallUDF(finishUserScriptCallback);
                playerState = EPlayerState.Finished;
            }
        }

        protected void OnNewPlayContentElement(TimeLinePlayContent timeLinePlayContent, List<State> lastElements, State newElement, float percent)
        {
            SetPercent(percent);
        }

        protected void LoadContent()
        {
            playerState = EPlayerState.LoadContent;
            if (playContent)
            {
                playContent.onNewPlayContentElement += OnNewPlayContentElement;
            }
            playerState = EPlayerState.LoadedContent;
        }

        protected void UnloadContent()
        {
            playerState = EPlayerState.UnloadContent;
            if (playContent)
            {
                playContent.onNewPlayContentElement -= OnNewPlayContentElement;
            }
            playerState = EPlayerState.UnloadedContent;
        }

        protected void InitPlayContent()
        {
            if (playContent)
            {
                // 时长与播放内容时长同步
                if (useContentTimeLength)
                {
                    UseContentTimeLength();
                }
                // 使用当前时间长度设置播放内容长度
                else
                {
                    playContent.ResetTimeLength(duration);
                }
            }
        }

        public void SetPlayContent(TimeLinePlayContent playContent)
        {
            if (playContent)
            {
                // 停止当前播放内容
                Stop();

                UnloadContent();

                this.playContent = playContent;

                LoadContent();

                InitPlayContent();
            }
        }

        #endregion

        #region IPlayer

        public bool IsPlaying() => isPlaying;

        public void Replay()
        {
            SetPercent(0);
            PlayOrResume();
        }

        public bool PlayOrResume()
        {
            return isPause ? Resume() : Play();
        }

        public bool Play()
        {
            playerState = EPlayerState.Play;

            if (playContent) playContent.OnPlay();

            _beforeFinishState = EPlayerState.None;

            return true;
        }

        public void Stop()
        {
            timeCounter = 0;
            playerState = EPlayerState.Stop;

            if (playContent) playContent.OnStop();
        }

        public bool Pause()
        {
            if (playerState == EPlayerState.Playing)
            {
                playerState = EPlayerState.Pause;
                return true;
            }
            return false;
        }

        public bool Resume()
        {
            if (playerState == EPlayerState.Pause)
            {
                playerState = EPlayerState.Resume;
                return true;
            }
            return false;
        }

        #endregion

        #region ITimeClip

        public float beginTime { get => 0; set { } }
        public float endTime { get => duration; set => duration = value; }
        public float timeLength { get => duration; set => duration = value; }

        public void UseContentTimeLength()
        {
            if (playContent)
            {
                duration = playContent.GetTimeLength();
            }
        }

        #endregion
    }
}
