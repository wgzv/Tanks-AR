using UnityEngine.UI;

namespace XCSJ.PluginSMS.States.Dataflows.FieldBinds.Base
{
    /// <summary>
    /// 文本<see cref="Text"/>更新器
    /// </summary>
    [BindObjectUpdater(typeof(Text))]
    public class Text_ObjectUpdater : IObjectUpdater
    {
        /// <summary>
        /// 对象更新
        /// </summary>
        /// <param name="fieldBind"></param>
        /// <param name="objFieldValue"></param>
        /// <param name="args"></param>
        public void ObjectUpdate(IFieldBind fieldBind, object objFieldValue, params object[] args)
        {
            if (fieldBind.obj is Text text && text)
            {
                text.SetAllDirty();
            }
        }
    }
}
