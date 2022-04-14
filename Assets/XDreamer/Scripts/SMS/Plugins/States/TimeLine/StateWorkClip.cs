using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Interfaces;
using XCSJ.Maths;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.PluginSMS.States.TimeLine
{
    [Serializable]
    public class StateWorkClip : IStateWorkClip
    {
        [Name("状态")]
        [SerializeField]
        public State state = null;

        [Name("工作区间")]
        [Tip("播放控制主要信息保存数据结构")]
        [SerializeField]
        public WorkRange workRange = new WorkRange();

        public string name { get => state.name; set { } }

        [Name("速度")]
        public float speed = 1;

        public float loopCount
        {
            get => MathX.Scale(speed * timeLength, onceTimeLength);
            set => speed = MathX.Scale(onceTimeLength * value, timeLength, 1);
        }

        private StateWorkClipSet _stateWorkClipSet;

        public StateWorkClipSet stateWorkClipSet
        {
            get
            {
                if (!_stateWorkClipSet)
                {
                    _stateWorkClipSet = state.GetComponent<StateWorkClipSet>();
                }
                return _stateWorkClipSet;
            }
        }

        public StateWorkClip(State state)
        {
            this.state = state;
        }

        public bool StateToPercent(State state, ref float percent)
        {
            if (stateWorkClipSet && stateWorkClipSet.StateToPercent(state, ref percent))
            {
                percent = percent * oncePercentLengthWithSpeed + beginPercent;
                return true;
            }
            else if (this.state == state)
            {
                percent = beginPercent;
                return true;
            }
            return false;
        }

        public void PercentToState(float percent, List<State> outStates)
        {
            if (workRange.percentRange.In(percent))
            {
                if (stateWorkClipSet)
                {
                    stateWorkClipSet.PercentToStates(GetNormalizeLocalPercentOfLoop(percent), outStates);
                }
                else
                {
                    outStates.Add(state);
                }
            }
        }

        protected float GetNormalizeLocalPercentOfLoop(float percent)
        {
            var lp = GetLocalPercent(percent);

            return Percent.Loop01(lp);
        }

        protected float GetLocalPercent(float percent)
        {
            var v = percentLength * onceTimeLength;
            return MathX.ApproximatelyZero(v) ? 0 : ((percent - beginPercent) * timeLength * speed / v);
        }

        public void ValidTriggerPoint(bool valid)
        {
            if (state)
            {
                foreach (var c in state.components)
                {
                    if (c is ITriggerPoint triggerPoint)
                    {
                        triggerPoint.valid = valid;
                    }
                }
            }
        }

        #region IStateWorkClip接口实现

        public float totalTimeLength { get { return timeLength; } set { timeLength = value; } }

        public float beginTime { get { return workRange.timeRange.timeRange.x; } set { workRange.timeRange.timeRange.x = value; } }
        public float endTime { get { return workRange.timeRange.timeRange.y; } set { workRange.timeRange.timeRange.y = value; } }
        public float timeLength { get { return workRange.timeRange.length; } set { workRange.timeRange.timeRange.y = workRange.timeRange.timeRange.x + value; } }

        public float beginPercent { get { return workRange.percentRange.percentRange.x; } set { workRange.percentRange.percentRange.x = value; } }
        public float endPercent { get { return workRange.percentRange.percentRange.y; } set { workRange.percentRange.percentRange.y = value; } }
        public float percentLength { get { return workRange.percentRange.length; } set { workRange.percentRange.percentRange.y = workRange.percentRange.percentRange.x + value; } }

        public bool loop => !MathX.Approximately(loopCount, 1);

        public float onceTimeLength => state.onceTimeLength;

        public float onceTimeLengthWithSpeed => state.onceTimeLength / speed;

        public float oncePercentLength
        {
            get
            {
                var tl = timeLength;
                return MathX.ApproximatelyZero(tl) ? 0 : (percentLength * onceTimeLength / tl);
            }
        }

        public float oncePercentLengthWithSpeed
        {
            get
            {
                var tl = timeLength * speed;
                return MathX.ApproximatelyZero(tl) ? 0 : (percentLength * onceTimeLength / tl);
            }
        }

        float ISpeed.speed { get => speed; set => speed = value; }
        State IStateWorkClip.state { get => state; set => state = value; }

        public void SetTimeLength(float timeLength,float ttl)
        {
            this.timeLength = timeLength;
            this.endPercent = this.endTime / ttl;
        }

        public bool SetTimeOfState(float time) => SetTime(time - beginTime);
        public bool SetTimeOfState(float time, StateData stateData) => SetTime(time - beginTime, stateData);

        public bool SetTime(float time)
        {
            var otl = onceTimeLength;
            return SetPercent(MathX.ApproximatelyZero(otl) ? 0 : (time * speed / otl));
        }
        public bool SetTime(float time, StateData stateData)
        {
            var otl = onceTimeLength;
            return SetPercent(MathX.ApproximatelyZero(otl) ? 0 : (time * speed / otl), stateData);
        }

        public bool SetPercentOfState(float percent) => SetPercent(GetLocalPercent(percent));
        public bool SetPercentOfState(float percent, StateData stateData) => SetPercent(GetLocalPercent(percent), stateData);

        public bool SetPercent(float percent) => state.SetProgress(percent);
        public bool SetPercent(float percent, StateData stateData) => state.SetPercent(percent, stateData);

        public void OnEntrySetPercent(float percent, StateData stateData)
        {
            state.OnEntry(stateData);
            SetPercentOfState(percent, stateData);
        }

        public void OnExitSetPercent(float percent, StateData stateData)
        {
            SetPercentOfState(percent, stateData);
            state.OnExit(stateData);
        }

        /// <summary>
        /// 当越界发生时回调；
        /// </summary>
        /// <param name="player">工作剪辑播放器对象</param>
        /// <param name="outOfBoundsMode">越界模式</param>
        /// <param name="percent">当前百分比</param>
        /// <param name="stateData">状态数据对象</param>
        /// <param name="lastPercent">上一次的百分比</param>
        /// <param name="stateWorkClip">状态工作剪辑对象</param>
        public void OnOutOfBounds(IWorkClipPlayer player, EOutOfBoundsMode outOfBoundsMode, float percent, StateData stateData, float lastPercent, IStateWorkClip stateWorkClip)
        {
            state.OnOutOfBounds(player, outOfBoundsMode, percent, stateData, lastPercent, stateWorkClip);
        }

        /// <summary>
        /// 当工作剪辑播放器的播放状态发生变化时回调
        /// </summary>
        /// <param name="player">工作剪辑播放器对象</param>
        /// <param name="lastPlayerState">上次的工作剪辑播放器的播放状态</param>
        public void OnPlayerStateChanged(IWorkClipPlayer player, EPlayerState lastPlayerState)
        {
            state.OnPlayerStateChanged(player, lastPlayerState);
        }

        void IPlayerEvent.OnPlayerStateChanged(IPlayer player, EPlayerState lastPlayerState) => OnPlayerStateChanged(player as IWorkClipPlayer, lastPlayerState);

        public bool Validity(float ttl) => MathX.Approximately(ttl, totalTimeLength, WorkClip.Epsilon) && WorkClip.WorkClipValidity(this);

        #endregion       

        public void ResetTimeLength(float parentTimeLength)
        {
            beginTime = parentTimeLength * beginPercent;
            endTime = parentTimeLength * endPercent;
        }
    }
}
