using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;
using XCSJ.Tools;

namespace XCSJ.PluginSMS.States.TimeLine.UI
{
    /// <summary>
    /// 时间轴播放器UI操作
    /// </summary>
    [Name("时间轴播放器UI操作")]
    public class TimeLinePlayerUIOperation : TimeLineUIRootGetter
    {
        /// <summary>
        /// 操作类型
        /// </summary>
        [Name("操作类型")]
        [EnumPopup]
        [Readonly(EEditorMode.Runtime)]
        public EPropertyName _propertyName = EPropertyName.None;

        /// <summary>
        /// 时间轴播放器操作
        /// </summary>
        public enum EPropertyName
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 播放暂停
            /// </summary>
            [Name("播放暂停")]
            PlayOrPause,

            /// <summary>
            /// 循环
            /// </summary>
            [Name("循环")]
            Loop,

            /// <summary>
            /// 当前时间
            /// </summary>
            [Name("当前时间")]
            CurrentTime,

            /// <summary>
            /// 总时间
            /// </summary>
            [Name("总时间")]
            TotalTime,
        }

        /// <summary>
        /// 播放按钮
        /// </summary>
        [Name("播放按钮")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.PlayOrPause)]
        [Readonly(EEditorMode.Runtime)]
        public Toggle _playOrPauseToggle;

        /// <summary>
        /// 循环切换
        /// </summary>
        [Name("循环切换")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Loop)]
        [Readonly(EEditorMode.Runtime)]
        public Toggle _loopToggle;

        [Name("当前时间")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.CurrentTime)]
        [Readonly(EEditorMode.Runtime)]
        public Text _currentTimeText;

        [Name("总时间")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.TotalTime)]
        [Readonly(EEditorMode.Runtime)]
        public Text _totalTimeText;

        [Name("时间格式")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual | EValidityCheckType.And, EPropertyName.CurrentTime, nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.TotalTime)]
        [Readonly(EEditorMode.Runtime)]
        public ETimeFormat _timeFormat = ETimeFormat.mm__ss;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            switch (_propertyName)
            {
                case EPropertyName.PlayOrPause:
                    {
                        if (_playOrPauseToggle)
                        {
                            _playOrPauseToggle.onValueChanged.AddListener(OnPlayOrPauseChanged);

                            TimeLinePlayer.onPlayerStateChanged += OnPlayerStateChanged;
                        }
                        break;
                    }
                case EPropertyName.Loop:
                    {
                        if (_loopToggle)
                        {
                            _loopToggle.onValueChanged.AddListener(OnLoopChanged);
                        }
                        break;
                    }
            }
            
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();

            switch (_propertyName)
            {
                case EPropertyName.PlayOrPause:
                    {
                        if (_playOrPauseToggle)
                        {
                            _playOrPauseToggle.onValueChanged.AddListener(OnPlayOrPauseChanged);

                            TimeLinePlayer.onPlayerStateChanged -= OnPlayerStateChanged;
                        }
                        break;
                    }
                case EPropertyName.Loop:
                    {
                        if (_loopToggle)
                        {
                            _loopToggle.onValueChanged.RemoveListener(OnLoopChanged);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected void Start()
        {
            Init();
        }

        private bool onPlayerState = true;

        private void OnPlayerStateChanged(TimeLinePlayer timeLinePlayer, EPlayerState oldState, EPlayerState newState)
        {
            if (this.timeLinePlayer != timeLinePlayer || !onPlayerState)
            {
                return;
            }
            switch (newState)
            {
                case EPlayerState.Play:
                case EPlayerState.Playing:
                case EPlayerState.Resume:
                    {
                        _playOrPauseToggle.isOn = true;
                        break;
                    }
                case EPlayerState.Init:
                case EPlayerState.Stop:
                case EPlayerState.Pause:
                case EPlayerState.Finished:
                    {
                        _playOrPauseToggle.isOn = false;
                        break;
                    }
            }
        }

        /// <summary>
        /// 时间轴播放器对象变化回调
        /// </summary>
        /// <param name="oldPlayer"></param>
        /// <param name="newPlayer"></param>
        protected override void OnPlayContentChanged(TimeLinePlayer oldPlayer, TimeLinePlayer newPlayer)
        {
            base.OnPlayContentChanged(oldPlayer, newPlayer);

            Init();
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected void Update()
        {
            if (!timeLinePlayer) return;

            switch (_propertyName)
            {
                case EPropertyName.CurrentTime:
                    {
                        if (_currentTimeText)
                        {
                            _currentTimeText.text = ConvertTimeFormat(timeLinePlayer.curTime, _timeFormat);
                        }
                        break;
                    }
            }
        }

        private void OnPlayOrPauseChanged(bool isOn)
        {
            if (timeLinePlayer)
            {
                onPlayerState = false;
                if (isOn)
                {
                    timeLinePlayer.Play();
                }
                else
                {
                    timeLinePlayer.Pause();
                }
                onPlayerState = true;
            }
        }

        private void Init()
        {
            if (!timeLinePlayer) return;

            switch (_propertyName)
            {
                case EPropertyName.Loop:
                    {
                        if (_loopToggle)
                        {
                            _loopToggle.isOn = timeLinePlayer.isLoop;
                        }
                        break;
                    }
                case EPropertyName.TotalTime:
                    {
                        if (_totalTimeText)
                        {
                            _totalTimeText.text = ConvertTimeFormat(timeLinePlayer.duration, _timeFormat);
                        }
                        break;
                    }
            }
        }

        private void OnLoopChanged(bool isOn)
        {
            if (timeLinePlayer)
            {
                timeLinePlayer.isLoop = isOn;
            }
        }

        /// <summary>
        /// 转换函数
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="timeFormat"></param>
        /// <returns></returns>
        public static string ConvertTimeFormat(float seconds, ETimeFormat timeFormat)
        {
            try
            {
                switch (timeFormat)
                {
                    case ETimeFormat.s:
                        {
                            return seconds.ToString();
                        }
                    case ETimeFormat.f:
                        {
                            return (seconds * 1000).ToString();
                        }
                    case ETimeFormat.mm__ss:
                        {
                            return TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss");
                        }
                    case ETimeFormat.hh__mm__ss:
                        {
                            return TimeSpan.FromSeconds(seconds).ToString(@"hh\:mm\:ss");
                        }
                    default: return "";
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return "";
            }
        }
    }

    /// <summary>
    /// 时间格式
    /// </summary>
    public enum ETimeFormat
    {
        [Name("秒")]
        s = 0,

        [Name("分:秒")]
        mm__ss,

        [Name("时:分:秒")]
        hh__mm__ss,

        [Name("毫秒")]
        f,
    }
}
