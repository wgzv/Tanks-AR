using XCSJ.PluginSMS.States.Dataflows.FieldBinds.Base;

#if XDREAMER_HOLOTOOLKIT_2017_4_3_0_REFRESH
using HoloToolkit.Examples.InteractiveElements;
#endif

namespace XCSJ.PluginHoloLens
{
#if XDREAMER_HOLOTOOLKIT_2017_4_3_0_REFRESH
    [BindObjectUpdater(typeof(SliderGestureControl))]
#endif
    public class SliderGestureControl_ObjectUpdater : IObjectUpdater
    {
        public void ObjectUpdate(IFieldBind fieldBind, object objFieldValue, params object[] args)
        {
#if XDREAMER_HOLOTOOLKIT_2017_4_3_0_REFRESH
            if(fieldBind.obj is SliderGestureControl control && control)
            {
                control.SetSliderValue((float)objFieldValue);
            }
#endif
        }
    }
}
