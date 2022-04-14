using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.States;

namespace XCSJ.PluginRepairman.Exam
{
    /// <summary>
    /// 考试信息界面
    /// </summary>
    [RequireComponent(typeof(Text))]
    [RequireManager(typeof(RepairmanManager))]
    public class UIExamInfo : MB
    {
        [Name("拆装修理考试")]
        [StateComponentPopup(typeof(RepairExam), stateCollectionType = EStateCollectionType.Root)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public RepairExam exam;

        [Name("初始化清空信息")]
        public bool setEmptyInfoOnEnable = true;

        private Text textInfo;

        /// <summary>
        /// 唤醒初始化
        /// </summary>
        protected void Awake()
        {
            textInfo = GetComponent<Text>();

            if (!exam)
            {
                exam = RepairmanHelperExtension.GetStateComponent<RepairExam>();
            }
            if (exam)
            {
                exam.onExamInfoChanged += OnExamInfoChanged;
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            if (setEmptyInfoOnEnable && textInfo)
            {
                textInfo.text = "";
            }
        }

        /// <summary>
        /// 当考试信息变更
        /// </summary>
        /// <param name="info"></param>
        protected void OnExamInfoChanged(string info)
        {
            textInfo.text = info;
        }
    }
}
