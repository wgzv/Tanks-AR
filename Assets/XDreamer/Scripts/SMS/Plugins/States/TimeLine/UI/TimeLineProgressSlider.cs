using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.XUnityEngine.XUI;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.MultiMedia;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginSMS.States.TimeLine.UI
{
    [Name("时间轴进度条")]
    [RequireComponent(typeof(Slider))]
    public class TimeLineProgressSlider : TimeLineUIRootGetter
    {
        [Name("设置回调")]
        [Tip("播放器时间轴变更时是否发送事件")]
        public bool sendCallback = true;

        protected Slider slider;

        private EventTrigger trigger = null;

        private Audio[] audios = null;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            slider = GetComponent<Slider>();
        }

        protected void Start()
        {
            AddPlayerListener();
        }

        protected void OnDestroy()
        {
            if (trigger)
            {
                GameObject.Destroy(trigger);
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            // 滑动条值变化
            slider.value = 0;
            slider.onValueChanged.AddListener(OnSliderValueChanged);

            if (!trigger)
            {
                trigger = gameObject.AddComponent<EventTrigger>();

                // 滑动条按下
                XGUIHelper.AddEventTrigger(trigger, EventTriggerType.PointerDown, OnPointerDown);

                // 滑动条弹起
                XGUIHelper.AddEventTrigger(trigger, EventTriggerType.PointerUp, OnPointerUp);

                // 滑动条点击
                XGUIHelper.AddEventTrigger(trigger, EventTriggerType.PointerClick, OnPointerClick);
            }

            trigger.enabled = true;

            AddPlayerListener();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();

            RemovePlayerListener();

            trigger.enabled = false;

            if (slider)
            {
                slider.onValueChanged.RemoveListener(OnSliderValueChanged);
            }
        }

        private void AddPlayerListener()
        {
            TimeLinePlayer.onPlayerPercentChanged += OnPlayerPercentChanged;

            if (timeLinePlayer && timeLinePlayer.playContent)
            {
                audios = (timeLinePlayer.playContent.parent as StateCollection).GetComponentsInChildren<Audio>();
            }
        }

        private void RemovePlayerListener()
        {
            TimeLinePlayer.onPlayerPercentChanged -= OnPlayerPercentChanged;
        }

        private bool playerToSlider = false;

        private Slider_LinkType slider_LinkType = null;

        private void OnPlayerPercentChanged(TimeLinePlayer timeLinePlayer, float percent)
        {
            if (slider)
            {
                playerToSlider = true;
                if (sendCallback)
                {
                    slider.value = percent;
                }
                else
                {
                    if (slider_LinkType == null || slider_LinkType.unityEngineObject != slider)
                    {
                        slider_LinkType = new Slider_LinkType(slider);
                    }
                    slider_LinkType.Set(percent, false);
                }
            }
        }

        private void OnSliderValueChanged(float sliderValue)
        {
            if (slider && timeLinePlayer && !playerToSlider)
            {
                timeLinePlayer.SetPercent(slider.value);
            }

            playerToSlider = false;
        }

        private bool orgIsPlaying = false;
 
        private void OnPointerDown(BaseEventData data)
        {
            SetAudioTriggerPlay(false);
            if (timeLinePlayer)
            {
                orgIsPlaying = timeLinePlayer.isPlaying;
                timeLinePlayer.Pause();
            }
        }

        private void OnPointerUp(BaseEventData data)
        {
            if (timeLinePlayer && orgIsPlaying)
            {
                timeLinePlayer.Resume();
            }
            SetAudioTriggerPlay(true);
        }

        private void OnPointerClick(BaseEventData data)
        {
            if(timeLinePlayer) timeLinePlayer.SetPercent(slider.value);
        }

        private void SetAudioTriggerPlay(bool trigger)
        {
            if (audios != null)
            {
                foreach (var audio in audios)
                {
                    audio.triggerPlayWhenSetPercent = trigger;
                }
            }
        }
    }
}
