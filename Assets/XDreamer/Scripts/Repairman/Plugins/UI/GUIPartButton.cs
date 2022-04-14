using XCSJ.PluginRepairman.Devices;
using XCSJ.PluginRepairman.Machines;
using XCSJ.PluginTools.SelectionUtils;

namespace XCSJ.PluginRepairman.UI
{
    public class GUIPartButton : GUIItemButton
    {
        protected Part part => item as Part;

        protected override void Update()
        {
            base.Update();

            if (part && part.state == EPartState.Assembly)
            {
                gameObject.SetActive(false);
            }
        }

        protected override void OnButtonClick()
        {
            if (part && part.go)
            {
                LimitedSelection.SetSelected(part.go);
            }
        }
    }
}
