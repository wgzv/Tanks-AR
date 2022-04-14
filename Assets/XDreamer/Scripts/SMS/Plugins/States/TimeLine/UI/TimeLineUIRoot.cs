using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginSMS.States.TimeLine.UI
{
    /// <summary>
    /// 时间轴播放器UI根
    /// </summary>
    [Name("时间轴播放器UI根")]
    public class TimeLineUIRoot : SubWindow
    {
        /// <summary>
        /// 时间轴播放器
        /// </summary>
        [Name("时间轴播放器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [StateComponentPopup(typeof(TimeLinePlayer))]
        public TimeLinePlayer _timeLinePlayer;

        public TimeLinePlayer timeLinePlayer
        {
            get => _timeLinePlayer;
            set
            {
                if (_timeLinePlayer!=value)
                {
                    var old = _timeLinePlayer;
                    _timeLinePlayer = value;
                    onTimeLinePlayerChanged?.Invoke(old, _timeLinePlayer);
                }
            }
        }

        /// <summary>
        /// 时间轴播放器变化回调
        /// </summary>
        public static event Action<TimeLinePlayer, TimeLinePlayer> onTimeLinePlayerChanged = null;

        /// <summary>
        /// 使用当前激活播放器
        /// </summary>
        [Name("使用当前激活播放器")]
        [Tip("当时间轴播放器为空的时候，自动查找当前激活的播放器对象")]
        public bool useCurrentActivePlayer = true;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            FindAndSetActivePlayer();
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected void Update()
        {
            FindAndSetActivePlayer();
        }

        /// <summary>
        /// 查找和设置活跃播放器
        /// </summary>
        protected void FindAndSetActivePlayer()
        {
            if (!useCurrentActivePlayer || (_timeLinePlayer && _timeLinePlayer.parent.active)) return;

            // 查找当前活跃的播放器
            var players = SMSHelper.GetStateComponents<TimeLinePlayer>();
            timeLinePlayer = players.Find(p => p.parent.active);
        }
    }

    /// <summary>
    /// 时间轴播放器根获取器
    /// </summary>
    public abstract class TimeLineUIRootGetter : View
    {
        /// <summary>
        /// 时间轴播放器UI根
        /// </summary>
        [Name("时间轴播放器UI根")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public TimeLineUIRoot _timeLineUIRoot = null;

        /// <summary>
        /// 车辆控制父对象 
        /// </summary>
        public TimeLineUIRoot timeLineUIRoot => this.XGetComponentInParent<TimeLineUIRoot>(ref _timeLineUIRoot);

        /// <summary>
        /// 时间轴播放器
        /// </summary>
        public TimeLinePlayer timeLinePlayer
        {
            get
            {
                if (timeLineUIRoot)
                {
                    return timeLineUIRoot._timeLinePlayer;
                }
                return null;
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        protected virtual void Reset()
        {
            if (timeLineUIRoot) { }
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        protected virtual void Awake()
        {
            if (!timeLineUIRoot)
            {
                Debug.LogErrorFormat("未关联{0}!", CommonFun.Name(typeof(TimeLineUIRoot)));
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            TimeLineUIRoot.onTimeLinePlayerChanged += OnPlayContentChanged;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            TimeLineUIRoot.onTimeLinePlayerChanged -= OnPlayContentChanged;
        }

        /// <summary>
        /// 播放器变化回调
        /// </summary>
        /// <param name="oldPlayer"></param>
        /// <param name="newPlayer"></param>
        protected virtual void OnPlayContentChanged(TimeLinePlayer oldPlayer, TimeLinePlayer newPlayer)
        {

        }
    }
}
