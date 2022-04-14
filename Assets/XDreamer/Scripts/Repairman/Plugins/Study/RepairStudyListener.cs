using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.States;
using XCSJ.PluginTools;

namespace XCSJ.PluginRepairman.Study
{
    /// <summary>
    /// 拆装学习监听器
    /// </summary>
    [RequireManager(typeof(RepairmanManager))]
    public abstract class RepairStudyListener : ToolMB
    {
        [Name("拆装修理学习")]
        [StateComponentPopup(typeof(RepairStudy), stateCollectionType = EStateCollectionType.Root)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public RepairStudy study;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            if (!study)
            {
                study = RepairmanHelperExtension.GetStateComponent<RepairStudy>();
            }

            if (study)
            {
                study.onPartSelected += OnPartSelected;
                study.onToolSelected += OnToolSelected;
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            if (study)
            {
                study.onPartSelected -= OnPartSelected;
                study.onToolSelected -= OnToolSelected;
            }
        }

        protected abstract void OnPartSelected(GameObject selectedGO, bool right);

        protected abstract void OnToolSelected(PluginRepairman.Devices.Tool tool, bool right);
    }
}
