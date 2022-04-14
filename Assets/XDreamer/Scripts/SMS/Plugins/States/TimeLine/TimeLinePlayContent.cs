using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.TimeLine
{
    /// <summary>
    /// 时间轴播放内容:被播放器所播放，用于管理子状态机内的其他具有工作剪辑接口状态
    /// </summary>
    [ComponentMenu("时间轴/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(TimeLinePlayContent))]
    [Tip("被播放器所播放，用于管理子状态机内的其他具有工作剪辑接口状态")]
    [XCSJ.Attributes.Icon(index = 33658)]
    [DisallowMultipleComponent]
    [RequireState(typeof(SubStateMachine))]
    public class TimeLinePlayContent : StateWorkClipSet
    {
        /// <summary>
        /// 标题
        /// </summary>
        public new const string Title = "时间轴播放内容";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib("时间轴", typeof(SMSManager), stateType = EStateType.SubStateMachine)]
        [StateComponentMenu("时间轴/"+ Title, typeof(SMSManager))]
#endif
        [Name(Title, nameof(TimeLinePlayContent))]
        [Tip("时间轴播放内容组件是组织和管理状态机内的工作剪辑的对象。只在状态机上使用，可被播放器播放。状态机退出之后，切换为完成态。")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreateTimeLinePlayContent(IGetStateCollection obj)
        {
            return obj?.CreateSubStateMachine(CommonFun.Name(typeof(TimeLinePlayContent)), null, typeof(TimeLinePlayContent));
        }

        public float GetTimeLength()
        {
            UpdateData();
            return timeLength;
        }

        public override void OnBeforeEntry(StateData data)
        {
            base.OnBeforeEntry(data);

            OnSetPercent(Percent.Zero, data);
        }

        /// <summary>
        /// 当有的新的播放内容元素时回调
        /// </summary>
        public event Action<TimeLinePlayContent,List<State>, State, float> onNewPlayContentElement;

        /// <summary>
        /// 当播放内容元素发生变化时回调
        /// </summary>
        public event Action<TimeLinePlayContent, List<State>, List<State>, float> onPlayContentElementChanged;

        public event Action<TimeLinePlayContent> onPlay;

        public event Action<TimeLinePlayContent> onStop;

        public void OnPlay()
        {
            lastPlayState = new List<State>();

            onPlay?.Invoke(this);
        }

        public void OnStop()
        {
            lastPlayState.ForEach(s => s.workMode = EWorkMode.Default);

            onStop?.Invoke(this);
        }

        public void PlayContentElements(params State[] states)
        {
            if (states == null || states.Length == 0) return;

            List<KeyValuePair<State, float>> list = new List<KeyValuePair<State, float>>();
            foreach (var state in states)
            {
                if(TryGetPercentOfState(state, out float p))
                {
                    list.Add(new KeyValuePair<State, float>(state, p));
                }
            }

            if (list.Count == 0) return;
            list.Sort((x, y) => x.Value.CompareTo(y.Value));

            // 百分比值最高的对象
            var last = list.Last();
            PlayContent(last.Value);

            onNewPlayContentElement?.Invoke(this, lastPlayState, last.Key, last.Value);
        }

        public List<State> lastPlayState { get; private set; } = new List<State>();

        public bool PlayContent(float percent, StateData stateData = null)
        {
            try
            {
                return SetPercentOfState(percent, stateData);
            }
            finally
            {
                List<State> newPlayStates = GetStates(percent);

                // 简单比较新旧元素是否发生变化
                if (lastPlayState.Count != newPlayStates.Count || !lastPlayState.All(newPlayStates.Contains))
                {
                    onPlayContentElementChanged?.Invoke(this, lastPlayState, newPlayStates, percent);

                    lastPlayState.ForEach(s => s.workMode = EWorkMode.Default);
                    lastPlayState = newPlayStates;
                }
                newPlayStates.ForEach(s => s.workMode = EWorkMode.Play);
            }
        }
    }
}
