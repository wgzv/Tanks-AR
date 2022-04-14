using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Notes
{
    /// <summary>
    /// 提示触发类型
    /// </summary>
    public enum ETipTriggerType
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 鼠标移入移出
        /// </summary>
        [Name("鼠标移入移出")]
        MouseOver,

        /// <summary>
        /// 点击
        /// </summary>
        [Name("点击")]
        [Tip("点击模型弹出提示，再次点击在非当前对象上则消失")]
        Click,
    }

    /// <summary>
    /// 弹出提示(UGUI)
    /// 支持鼠标移入移出和点击触发提示
    /// </summary>
    [Name("弹出提示(UGUI)")]
    [RequireManager(typeof(ToolsManager))]
    public abstract class Tip : ToolMB
    {
        /// <summary>
        /// 触发类型
        /// </summary>
        [Name("提示触发类型")]
        [EnumPopup]
        public ETipTriggerType tipTriggerType = ETipTriggerType.MouseOver;

        /// <summary>
        /// 提示UI
        /// </summary>
        [Name("弹出UI")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public RectTransform ugui;

        [Group("高级设置", needBoundBox=true, defaultIsExpanded = true)]
        [Name("UGUI位置随着提示位置变化")]
        [Tip("勾选时，UGUI弹出位置会跟随Tip的位置而变换；不勾选时，UGUI保持原位置")]
        public bool uguiFollowTipPosition = true;

        [Name("点击空白隐藏UI")]
        public bool hiddenUIWhenClickEmpty = true;

        [Name("偏移量")]
        [Tip("偏移坐标,X值向右为正，Y值向上为正")]
        [HideInSuperInspector(nameof(uguiFollowTipPosition), EValidityCheckType.Equal, false)]
        public Vector3 offsetOfScreen = Vector3.zero;

        /// <summary>
        /// 弹出时间
        /// </summary>
        [Name("弹出时间")]
        [Range(0.01f, 5f)]
        public float showTime = 0.2f;

        [Name("隐藏时间")]
        [Range(0.01f, 5f)]
        public float hiddenTime = 0.1f;

        [Name("提示文本")]
        public string text = "";

        public static event Action<Tip> onResetCallback = null;

        /// <summary>
        /// 重置
        /// </summary>
        protected virtual void Reset()
        {
            onResetCallback?.Invoke(this);
        }

        protected virtual void Awake()
        {
            if (ugui)
            {
                ugui.gameObject.SetActive(false);

                // 使用当前存储文本设置Text
                if (!string.IsNullOrEmpty(text) && ugui.GetComponentInChildren<Text>() is Text uText && uText && string.IsNullOrEmpty(uText.text))
                {
                    uText.text = text;
                }
            }
        }

        /// <summary>
        /// 移入
        /// </summary>
        protected virtual void OnEnter()
        {
            if (ugui && tipTriggerType == ETipTriggerType.MouseOver) ShowTip();
        }

        /// <summary>
        /// 移出
        /// </summary>
        protected virtual void OnExit()
        {
            if (ugui && tipTriggerType == ETipTriggerType.MouseOver) HiddenTip();
        }

        protected void OnClick()
        {
            if (ugui && tipTriggerType == ETipTriggerType.Click)
            {
                ShowTip();
            }
        }

        /// <summary>
        /// 显示
        /// </summary>
        protected void ShowTip()
        {
            ugui.gameObject.SetActive(true);

            if (uguiFollowTipPosition)
            {
                SetTipPosition(GetTipPosition() + offsetOfScreen);
            }

            // 弹出
            totalTime = showTime;
            scaleState = EScaleState.Increating;
        }

        // 缩小
        protected void HiddenTip()
        {
            totalTime = hiddenTime;
            scaleState = EScaleState.Decreasing;
        }

        /// <summary>
        /// 基于屏幕坐标
        /// </summary>
        /// <returns></returns>
        protected abstract Vector3 GetTipPosition(); 

        /// <summary>
        /// 设置Tip位置
        /// </summary>
        /// <param name="position"></param>
        protected abstract void SetTipPosition(Vector3 position);

        /// <summary>
        /// 更新检测点击其他地方，隐藏当前提示
        /// </summary>
        protected void Update()
        {
            if (!ugui) return;

            if (scaleState == EScaleState.None && Input.GetMouseButtonUp(0))
            {
                if (hiddenUIWhenClickEmpty && !CommonFun.IsOnUGUI() && ugui.localScale != Vector3.zero)
                {
                    HiddenTip();
                }
            }

            if (scaleState != EScaleState.None)
            {
                timeCounter += Time.deltaTime;

                var percent = Mathf.Clamp(timeCounter / totalTime, 0, 1);
                if (scaleState == EScaleState.Decreasing) percent = 1 - percent;

                SetTipScale(percent);

                if (timeCounter >= showTime)
                {
                    scaleState = EScaleState.None;
                    timeCounter = 0;
                }
            }
        }

        private void SetTipScale(float scaleValue)
        {
            ugui.localScale = Vector3.one * scaleValue;
        }

        /// <summary>
        /// 缩放状态
        /// </summary>
        private EScaleState scaleState = EScaleState.None;
        private float timeCounter = 0;
        private float totalTime = 0;

        /// <summary>
        /// 缩放状态
        /// </summary>
        private enum EScaleState
        {
            /// <summary>
            /// 无
            /// </summary>
            None,

            /// <summary>
            /// 递增
            /// </summary>
            Increating,

            /// <summary>
            /// 递减
            /// </summary>
            Decreasing,
        }
    }
}
