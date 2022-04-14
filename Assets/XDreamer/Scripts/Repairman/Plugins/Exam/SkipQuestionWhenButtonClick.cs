using System.Linq;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginRepairman.Task;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginRepairman.Exam
{
    /// <summary>
    /// 跳过
    /// </summary>
    [ComponentMenu("跳过/按钮点击时跳过修理步骤问题", typeof(RepairmanManager))]
    [Name("按钮点击时跳过修理步骤问题")]
    public class SkipQuestionWhenButtonClick : RepairStepSkipWhenButtonClick
    {
        [Name("拆装修理考试")]
        public RepairExam repairExam = null;

        public override bool Init(StateData data)
        {
            if(repairExam==null)
            {
                repairExam = SMSHelper.GetStateComponents<RepairExam>().FirstOrDefault();
            }
            return base.Init(data);
        }

        protected override void OnSkip()
        {
            if(repairExam)
            {
                repairExam.Skip();
            }

            base.OnSkip();
        }
    }
}
