using System.Linq;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginRepairman.Devices;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.States;

namespace XCSJ.PluginRepairman.UI
{
    [Name("工具栏")]
    public class GUIToolList : GUIItemList
    {
        [Name("背包")]
        [StateComponentPopup(typeof(Bag))]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Bag bag;

        [Name("只显示工具")]
        [Tip("不显示背包根节点")]
        public bool onlyDisplayTool = true;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            if (!bag)
            {
                bag = SMSHelper.GetStateComponents<Bag>().FirstOrDefault();
            }

            if (bag)
            {
                var toolList = bag.GetChildrenItems().ToList();
                if (onlyDisplayTool)
                {
                    toolList = toolList.Where(i => i is Tool).ToList();
                }
                CreateItemList(toolList);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();

            ClearItemList();
        }
    }
}
