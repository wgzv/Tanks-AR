using UnityEngine.UI;
using XCSJ.Extension.Base.XUnityEngine.XUI;

namespace XCSJ.PluginSMS.States.Dataflows.FieldBinds.Base
{
    /// <summary>
    /// 滑动条<see cref="Slider"/>缓存器
    /// </summary>
    [BindObjectUpdater(typeof(Slider))]
    public class Slider_ObjectUpdater : IObjectUpdater
    {
        /// <summary>
        /// 对象更新
        /// </summary>
        /// <param name="fieldBind"></param>
        /// <param name="objFieldValue"></param>
        /// <param name="args"></param>
        public void ObjectUpdate(IFieldBind fieldBind, object objFieldValue, params object[] args)
        {
            new Slider_LinkType(fieldBind.obj).UpdateVisuals();
        }
    }
}
