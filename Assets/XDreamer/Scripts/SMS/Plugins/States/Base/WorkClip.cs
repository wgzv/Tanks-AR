using System;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Interfaces;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Base
{
    [Name("工作剪辑")]
    [KeyNode(nameof(IWorkClip), "工作剪辑")]
    [DisallowMultipleComponent]
    public abstract class WorkClip : StateComponent, ILoopWorkClip
    {
        [Name("工作区间")]
        [Tip("当前组件在状态生命周期内的工作区间(时间与百分比)信息")]
        [OnlyMemberElements]
        public WorkRange workRange = new WorkRange();

        #region 基础设置-变量

        [Group("基础设置", needBoundBox = true, defaultIsExpanded = false)]
        [Name("使用初始化数据")]
        [Tip("为True时，则使用初始化时记录的数据信息进行后续的更新与处理；为False时，则使用进入时(即状态启用或再次启用时)记录的数据信息进行后续的更新与处理")]
        public bool useInitData = true;

        [Name("进入时设置百分比")]
        [Tip("为True时,在当前状态组件进入时设置百分比进度为0;为False时,不做处理;")]
        public bool setPercentOnEntry = true;

        [Name("进入时百分比")]
        [Tip("当状态组件进入后,将当前状态组件逻辑数尝试保持在本百分比进度;")]
        [HideInSuperInspector(nameof(setPercentOnEntry), EValidityCheckType.False)]
        public float percentOnEntry = 0;

        [Name("退出时设置百分比")]
        [Tip("为True时,在当前状态组件退出时设置百分比进度为1;为False时,不做处理;")]
        public bool setPercentOnExit = true;

        [EndGroup(true)]
        [Name("退出时百分比")]
        [Tip("当状态组件退出后,将当前状态组件逻辑数据保持在本百分比进度;值为0,可将数据尽量还原到初始化/进入的状态;")]
        [HideInSuperInspector(nameof(setPercentOnExit), EValidityCheckType.False)]
        public float percentOnExit = 1;

        #endregion

        #region 循环-变量

        [Group("循环设置", needBoundBox = true, defaultIsExpanded = false)]
        [Name("循环类型")]
        [Tip("当前组件在其有效时间区间(百分比区间)内执行逻辑的循环类型;")]
        [EnumPopup]
        public ELoopType loopType = ELoopType.None;

        [Name("单次时长")]
        [Tip("当前状态组件完整执行一次表现逻辑的期望时长")]
        [HideInSuperInspector(nameof(loopType), EValidityCheckType.Equal, ELoopType.None)]
        [FormerlySerializedAs(nameof(onceTimeLength))]
        [SerializeField]
        protected float _onceTimeLength = 3;

        [Name("最少循环次数")]
        [Tip("当已经循环次数大于等于本值时,本状态组件设置为完成态;")]
        [HideInSuperInspector(nameof(loopType), EValidityCheckType.Equal, ELoopType.None)]
        public float leastLoopCount = 0;

        [Name("超出工作区间继续循环")]
        [Tip("当前状态组件所在状态的工作时间(百分比)超出(大于)工作有效时间(百分比)区间右值之后,当前状态组件是否继续执行循环逻辑;为True时,继续执行循环逻辑;为False时,不再继续执行循环逻辑;")]
        [HideInSuperInspector(nameof(loopType), EValidityCheckType.Equal, ELoopType.None)]
        public bool continueLoopAfterWorkRange = true;

        [EndGroup(true)]
        [Name("超出工作区间时百分比")]
        [Tip("当前状态组件所在状态的工作时间(百分比)超出(大于)工作有效时间(百分比)区间右值之后,当前状态组件将保持当前百分比设定的值所对应的状态;")]
        [HideInSuperInspector(nameof(loopType), EValidityCheckType.Equal | EValidityCheckType.Or, ELoopType.None, nameof(continueLoopAfterWorkRange), EValidityCheckType.True)]
        public float percentOnAfterWorkRange = 1;

        #endregion

        #region 工作曲线-变量

        [Group("工作曲线设置", needBoundBox = true, defaultIsExpanded = false)]
        [EndGroup]
        [Name("工作曲线")]
        [Tip("工作曲线仅在当前组件的工作区间的有效百分比区间内生效")]
        public AnimationCurve workCurve = AnimationCurve.Linear(0, 0, 1, 1);

        #endregion

        #region 其它变量

        [Name("锁定比例")]
        [Tip("锁定百分比与时间的比例关系,根据锁定时当前状态组件总时长,对二者进行等比例同步调整;即其中一区间数据发生修改，另一区间数据将同步进行等比例的数据修改;")]
        [XCSJ.Attributes.Icon(EIcon.Lock)]
        [HideInSuperInspector]
        public bool lockRatioOfWorkRange = false;

        /// <summary>
        /// 锁定比例的总时长
        /// </summary>
        [Name("锁定比例的总时长")]
        [HideInSuperInspector]
        public float ttlOfLockRatio = 3f;

        /// <summary>
        /// 同步时长:将时长实时自动同步为当前状态组件中某些有效字段成员所承载的内容时长;同步时保证起始时间不变;TL,即时长(Time Length缩写)
        /// </summary>
        [Name("同步时长")]
        [Tip("将时长实时自动同步为当前状态组件中某些有效字段成员所承载的内容时长;同步时保证起始时间不变;TL,即时长(Time Length缩写)")]
        [HideInSuperInspector]
        [XCSJ.Attributes.Icon(EIcon.Update)]
        public bool syncTL = true;

        /// <summary>
        /// 同步OTL:将单次时长与当前状态组件的有效时长保持同步,即二者修改会互相影响;OTL,即单次时长(Once Time Length缩写)
        /// </summary>
        [Name("同步OTL")]
        [Tip("将单次时长与当前状态组件的有效时长保持同步,即二者;OTL,即单次时长(Once Time Length缩写)")]
        [HideInSuperInspector]
        public bool syncOTL = true;

        /// <summary>
        /// 同步单次时长
        /// </summary>
        public virtual void SyncOTL()
        {
            if (!syncOTL) return;
            onceTimeLength = timeLength;
        }

        #endregion

        #region 基类接口

        public override bool Init(StateData stateData)
        {
            percent.Init(this);
            return base.Init(stateData);
        }

        public override bool Finished() => base.Finished() || MathX.ApproximatelyZero(timeLength);

        public override void OnBeforeEntry(StateData stateData)
        {
            percent.Reset();
            base.OnBeforeEntry(stateData);
        }

        public override void OnAfterEntry(StateData stateData)
        {
            base.OnAfterEntry(stateData);
            if (setPercentOnEntry)
            {
                SetPercent(percentOnEntry, stateData);
            }
        }

        public override void OnUpdate(StateData stateData)
        {
            base.OnUpdate(stateData);

            SetTimeOfState(parent.timeLengthWithSpeedSinceEntry, stateData);
        }

        public override void OnBeforeExit(StateData stateData)
        {
            if (setPercentOnExit)
            {
                SetPercent(percentOnExit, stateData);
            }
            base.OnBeforeExit(stateData);
        }

        #endregion

        #region IProgress && ILoopWorkClip

        /// <summary>
        /// 百分比
        /// </summary>
        public Percent percent { get; private set; } = new Percent();

        /// <summary>
        /// 进度
        /// </summary>
        public override float progress { get => percent.percent01OfWorkCurve; set => SetPercent(value); }

        /// <summary>
        /// 标识是否循环
        /// </summary>
        public bool loop => loopType != ELoopType.None;

        /// <summary>
        /// 循环次数
        /// </summary>
        public float loopCount => loop ? TimeScaleByOTL(timeLength) : 1;

        ELoopType ILoopWorkClip.loopType => loopType;

        AnimationCurve ILoopWorkClip.workCurve => workCurve;

        bool ILoopWorkClip.continueLoopAfterWorkRange => continueLoopAfterWorkRange;

        float ILoopWorkClip.percentOnAfterWorkRange => percentOnAfterWorkRange;

        /// <summary>
        /// 单次时长：简写OTL
        /// </summary>
        public float onceTimeLength
        {
            get => loop ? _onceTimeLength : timeLength;
            set
            {
                if(syncOTL || !loop)
                {
                    timeLength = value;
                }
                else
                {
                    this.XModifyProperty(ref _onceTimeLength, value);
                }
            }
        }

        /// <summary>
        /// 单次百分比长
        /// </summary>
        public float oncePercentLength => loop ? MathX.Scale(percentLength * _onceTimeLength, timeLength) : percentLength;

        /// <summary>
        /// 总时长
        /// </summary>
        public float totalTimeLength { get => workRange.totalTimeLength; set => workRange.totalTimeLength = value; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public float beginTime { get => workRange.beginTime; set => workRange.beginTime = value; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public float endTime { get => workRange.endTime; set => workRange.endTime = value; }

        /// <summary>
        /// 时长
        /// </summary>
        public float timeLength
        {
            get => workRange.timeLength;
            set
            {
                this.XModifyProperty(() =>
                {
                    workRange.timeLength = value;
                    if (syncOTL) _onceTimeLength = value;
                });
            }
        }

        /// <summary>
        /// 开始百分比
        /// </summary>
        public float beginPercent { get => workRange.beginPercent; set => workRange.beginPercent = value; }

        /// <summary>
        /// 结束百分比
        /// </summary>
        public float endPercent { get => workRange.endPercent; set => workRange.endPercent = value; }

        /// <summary>
        /// 百分比长
        /// </summary>
        public float percentLength { get => workRange.percentLength; set => workRange.percentLength = value; }

        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool SetTime(float time) => InternelSetPercentOfState(MathX.Scale(time + beginTime, totalTimeLength), null);

        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="time"></param>
        /// <param name="stateData"></param>
        /// <returns></returns>
        public bool SetTime(float time, StateData stateData) => InternelSetPercentOfState(MathX.Scale(time + beginTime, totalTimeLength), stateData);

        /// <summary>
        /// 设置状态时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool SetTimeOfState(float time) => InternelSetPercentOfState(MathX.Scale(time, totalTimeLength), null);

        /// <summary>
        /// 设置状态时间
        /// </summary>
        /// <param name="time"></param>
        /// <param name="stateData"></param>
        /// <returns></returns>
        public bool SetTimeOfState(float time, StateData stateData) => InternelSetPercentOfState(MathX.Scale(time, totalTimeLength), stateData);

        /// <summary>
        /// 设置百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public bool SetPercent(float percent) => InternelSetPercentOfState(percent + beginPercent, null);

        /// <summary>
        /// 设置百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="stateData"></param>
        /// <returns></returns>
        public bool SetPercent(float percent, StateData stateData) => InternelSetPercentOfState(percent + beginPercent, stateData);

        /// <summary>
        /// 设置状态百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public bool SetPercentOfState(float percent) => InternelSetPercentOfState(percent, null);

        /// <summary>
        /// 设置状态百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="stateData"></param>
        /// <returns></returns>
        public bool SetPercentOfState(float percent, StateData stateData) => InternelSetPercentOfState(percent, stateData);

        private bool InternelSetPercentOfState(float percentOfState, StateData stateData)
        {
            try
            {
                OnSetPercent(percent.Update(percentOfState), stateData);
                return true;
            }
            catch (Exception ex)
            {
                XCSJ.Log.ExceptionFormat("状态组件[{0}]执行[{1}]时异常:{2}", this.GetNamePath(), nameof(OnSetPercent), ex);
                return false;
            }
            finally
            {
                if (!finished && percentOfState >= 0)
                {
                    switch (loopType)
                    {
                        case ELoopType.None:
                            {
                                finished = percent.percent >= 1
                                    || MathX.Approximately(percent.percent01, 1)
                                    || MathX.ApproximatelyZero(timeLength);
                                break;
                            }
                        case ELoopType.Loop:
                        case ELoopType.PingPong:
                            {
                                finished = percent.percent >= leastLoopCount
                                    || MathX.Approximately(percent.percent, leastLoopCount)
                                    || MathX.ApproximatelyZero(timeLength)
                                    || percent.percent * _onceTimeLength >= totalTimeLength;
                                break;
                            }
                    }
                }
            }
        }

        /// <summary>
        /// 当设置百分比时回调
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="stateData"></param>
        protected abstract void OnSetPercent(Percent percent, StateData stateData);

        /// <summary>
        /// 时间缩放通过单次时长（OTL）
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        protected float TimeScaleByOTL(float time) => MathX.Scale(time, _onceTimeLength);

        /// <summary>
        /// 百分比缩放通过单次时长（OTL）
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        protected float PercentScaleByOTL(float percent) => MathX.Scale(percent * timeLength, percentLength * _onceTimeLength);

        /// <summary>
        /// 验证总时长有效性
        /// </summary>
        /// <param name="ttl">期望的总时长</param>
        /// <returns>如期望总时长与计算得到的总是长相等(在允许误差的范围内即认为相等)时返回True；否则返回False</returns>
        public virtual bool Validity(float ttl)
        {
            return MathX.Approximately(ttl, totalTimeLength, Epsilon) && WorkClipValidity(this);
        }

        /// <summary>
        /// 检测工作剪辑的有效性；主要检测时间与百分比的比例关系等；
        /// </summary>
        /// <param name="workClip">待检测的工作剪辑对象</param>
        /// <returns>有效返回True；否则返回False</returns>
        public static bool WorkClipValidity(IWorkClip workClip)
        {
            if (workClip == null) return false;

            //优先检查时长与百分比长
            if (MathX.ApproximatelyZero(workClip.timeLength, Epsilon) || MathX.ApproximatelyZero(workClip.percentLength, Epsilon))
            {
                return false;
            }

            var ttl = workClip.totalTimeLength;
            //需保证起始/结束的时间与百分比相对总时长必须成比例
            return MathX.Approximately(ttl * workClip.beginPercent, workClip.beginTime, Epsilon) && MathX.Approximately(ttl * workClip.endPercent, workClip.endTime, Epsilon);
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
        public virtual void OnOutOfBounds(IWorkClipPlayer player, EOutOfBoundsMode outOfBoundsMode, float percent, StateData stateData, float lastPercent, IStateWorkClip stateWorkClip) {

            switch(outOfBoundsMode)
            {
                case EOutOfBoundsMode.Left:
                    {
                        if (setPercentOnEntry)
                        {
                            SetPercent(percentOnEntry, stateData);
                        }
                        break;
                    }
                case EOutOfBoundsMode.Right:
                    {
                        if (setPercentOnExit)
                        {
                            SetPercent(percentOnExit, stateData);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 工作剪辑播放器对象；进入播放状态对象有效，停止时对象无效；
        /// </summary>
        public IWorkClipPlayer workClipPlayer { get; private set; } = null;

        /// <summary>
        /// 当工作剪辑播放器的播放状态发生变化时回调
        /// </summary>
        /// <param name="player">工作剪辑播放器对象</param>
        /// <param name="lastPlayerState">上次的工作剪辑播放器的播放状态</param>
        public virtual void OnPlayerStateChanged(IWorkClipPlayer player, EPlayerState lastPlayerState)
        {
            switch(player.playerState)
            {
                case EPlayerState.Play:
                    {
                        workClipPlayer = player;
                        break;
                    }
                case EPlayerState.Stop:
                    {
                        workClipPlayer = null;
                        break;
                    }
            }
        }

        void IPlayerEvent.OnPlayerStateChanged(IPlayer player, EPlayerState lastPlayerState) => OnPlayerStateChanged(player as IWorkClipPlayer, lastPlayerState);

        /// <summary>
        /// 误差
        /// </summary>
        public const float Epsilon = SMSHelperExtension.Epsilon;

        #endregion

        #region Unity接口函数

        /// <summary>
        /// 当检查器界面数据变换时调用
        /// </summary>
        public virtual void OnValidate()
        {
            SyncOTL();
        }

        #endregion
    }

    /// <summary>
    /// 工作剪辑模版抽象类
    /// </summary>
    /// <typeparam name="T">子类模版</typeparam>
    [Name("工作剪辑<模版>")]
    public abstract class WorkClip<T> : WorkClip where T : WorkClip<T>
    {
        /// <summary>
        /// 自身对象
        /// </summary>
        public T self => (T)this;

        /// <summary>
        /// 创建携带当前状态组件的普通状态
        /// </summary>
        /// <param name="obj">获取状态集接口对象，即新创建的普通状态会添加在本对象指定的对象集中</param>
        /// <param name="init">初始化方法</param>
        /// <param name="stateComponentTypes">其它需同步添加的状态组件类型</param>
        /// <returns>成功返回新创建的普通状态；否则返回null</returns>
        public static NormalState CreateNormalState(IGetStateCollection obj, Action<NormalState> init = null, params Type[] stateComponentTypes)
        {
            return obj.CreateNormalState<T>(init, stateComponentTypes);
        }

        /// <summary>
        /// 创建携带当前状态组件的子状态机
        /// </summary>
        /// <param name="obj">获取状态集接口对象，即新创建的子状态机会添加在本对象指定的对象集中</param>
        /// <param name="init">初始化方法</param>
        /// <param name="stateComponentTypes">其它需同步添加的状态组件类型</param>
        /// <returns>成功返回新创建的普通状态；否则返回null</returns>
        public static SubStateMachine CreateSubStateMachine(IGetStateCollection obj, Action<SubStateMachine> init = null, params Type[] stateComponentTypes)
        {
            return obj.CreateSubStateMachine<T>(init, stateComponentTypes);
        }
    }
}
