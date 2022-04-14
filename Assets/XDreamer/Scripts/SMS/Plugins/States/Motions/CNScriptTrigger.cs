using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Interfaces;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Motions
{
    public enum ECNScriptTrigger
    {
        [Name("工作剪辑触发")]
        [Tip("在工作剪辑的某个百分比上触发")]
        OnTrigger,
    }

    [Serializable]
    public class CNScriptTriggerSet : BaseScriptEventSet<ECNScriptTrigger>
    {
    }

    /// <summary>
    /// 脚本触发:使用中文脚本编写控制逻辑,并在某个区间中触发
    /// </summary>
    [Serializable]
    [ComponentMenu("动作/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(CNScriptTrigger))]
    [Tip("使用中文脚本编写控制逻辑,并在某个区间中触发")]
    [XCSJ.Attributes.Icon(index = 33616)]
    [KeyNode(nameof(IWorkClip), "工作剪辑")]
    public class CNScriptTrigger : StateScriptComponent<CNScriptTrigger, ECNScriptTrigger, CNScriptTriggerSet>, IWorkClip, ITriggerPoint
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "脚本触发";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("动作", typeof(SMSManager))]
        [StateComponentMenu("动作/"+ Title, typeof(SMSManager))]
        [Name(Title, nameof(CNScriptTrigger))]
        [Tip("使用中文脚本编写控制逻辑,并在某个区间中触发")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [Group("区间触发")]
        [Name("工作区间")]
        [Tip("当前组件在状态生命周期内的工作区间(时间与百分比)信息")]
        [OnlyMemberElements]
        public WorkRange workRange = new WorkRange();

        public bool loop => false;

        public float onceTimeLength => workRange.totalTimeLength;

        public override void OnEntry(StateData data)
        {
            _progress = 0;
            base.OnEntry(data);
        }

        public override void OnUpdate(StateData data)
        {
            base.OnUpdate(data);

            SetTimeOfState(parent.timeLengthWithSpeedSinceEntry);
        }

        public override void OnExit(StateData data)
        {
            base.OnExit(data);
        }

        #region 工作剪辑 && 进度

        public float totalTimeLength
        {
            get { return workRange.totalTimeLength; }
            set { workRange.totalTimeLength = value; }
        }

        protected float _progress = 0;

        public override float progress
        {
            get { return Mathf.Clamp01(_progress); }
            set { SetPercent(value); }
        }

        public virtual float beginTime
        {
            get { return workRange.timeRange.timeRange.x; }
            set { workRange.timeRange.timeRange.x = value; }
        }

        public virtual float endTime
        {
            get { return workRange.timeRange.timeRange.y; }
            set { workRange.timeRange.timeRange.y = value; }
        }

        public virtual float timeLength
        {
            get { return endTime - beginTime; }
            set { endTime = beginTime + value; }
        }

        public virtual float beginPercent
        {
            get { return workRange.percentRange.percentRange.x; }
            set { workRange.percentRange.percentRange.x = value; }
        }

        public virtual float endPercent
        {
            get { return workRange.percentRange.percentRange.y; }
            set { workRange.percentRange.percentRange.y = value; }
        }

        public virtual float percentLength
        {
            get { return endPercent - beginPercent; }
            set { endPercent = Mathf.Clamp01(beginPercent + value); }
        }

        public virtual bool SetTimeOfState(float time) => SetTimeOfState(time, null);

        public virtual bool SetTime(float time) => SetTime(time, null);

        public virtual bool SetPercentOfState(float percent) => SetPercentOfState(percent, null);

        public virtual bool SetPercent(float percent) => SetPercent(percent, null);
        
        protected virtual void CheckFinished()
        {
            if (!finished)
            {
                finished = MathX.Approximately(progress, 1) || MathX.ApproximatelyZero(timeLength);
            }
        }

        public bool Validity(float ttl) => MathX.Approximately(ttl, totalTimeLength, WorkClip.Epsilon) && WorkClip.WorkClipValidity(this);


        public bool SetTime(float time, StateData stateData)
        {
            return SetPercent(workRange.timeRange.NormalizeOfRelativeLeft(time), stateData);
        }

        public bool SetTimeOfState(float time, StateData stateData)
        {
            return SetTime(time - beginTime, stateData);
        }

        public bool SetPercentOfState(float percent, StateData stateData)
        {
            return SetPercent(workRange.percentRange.Normalize(percent), stateData);
        }

        public bool SetPercent(float percent, StateData stateData)
        {
            if (!valid) return false;

            // 初始化过后
            if (lastPercent >= 0)
            {
                switch (direction)
                {
                    case ETriggerDirection.Increase:
                        {
                            if (triggerPercent > lastPercent && triggerPercent < percent)
                            {
                                OnTrigger();
                            }
                            break;
                        }
                    case ETriggerDirection.Descending:
                        {
                            if (triggerPercent < lastPercent && triggerPercent > percent)
                            {
                                OnTrigger();
                            }
                            break;
                        }
                    case ETriggerDirection.Both:
                        {
                            if ((triggerPercent > lastPercent && triggerPercent < percent) ||
                                (triggerPercent < lastPercent && triggerPercent > percent))
                            {
                                OnTrigger();
                            }
                            break;
                        }
                }
            }
            lastPercent = percent;

            _progress = percent;
            CheckFinished();
            return true;
        }

        #endregion

        [Name("触发百分比")]
        [Range(0, 1)]
        public float _triggerPercent = 0.05f;

        [Name("递增方向")]
        [EnumPopup]
        public ETriggerDirection _direction = ETriggerDirection.Increase;

        protected float lastPercent = 0;

        private bool _valid = true;

        protected virtual void OnTrigger()
        {
            RunScriptEventWithNamePath(ECNScriptTrigger.OnTrigger);
        }

        public void OnOutOfBounds(IWorkClipPlayer player, EOutOfBoundsMode outOfBoundsMode, float percent, StateData stateData, float lastPercent, IStateWorkClip stateWorkClip) { }

        public void OnPlayerStateChanged(IWorkClipPlayer player, EPlayerState lastPlayerState) { }

        void IPlayerEvent.OnPlayerStateChanged(IPlayer player, EPlayerState lastPlayerState) => OnPlayerStateChanged(player as IWorkClipPlayer, lastPlayerState);

        public virtual float triggerPercent
        {
            get { return this._triggerPercent; }
            set { this._triggerPercent = value; }
        }

        public virtual ETriggerDirection direction
        {
            get { return this._direction; }
            set { this._direction = value; }
        }

        public virtual bool valid
        {
            get { return _valid; }
            set { _valid = value; }
        }
    }
}
