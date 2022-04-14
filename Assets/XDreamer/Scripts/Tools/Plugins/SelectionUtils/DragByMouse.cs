using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.SelectionUtils
{
    /// <summary>
    /// 鼠标拖拽触发器
    /// </summary>
    public class DragByMouse : IGrabAction
    {
        /// <summary>
        /// 是否开始拖拽
        /// </summary>
        /// <returns></returns>
        public virtual bool Grab()
        {
            return XInput.GetMouseButtonDown(0) && !CommonFun.IsOnUGUI();
        }

        /// <summary>
        /// 是否正在拖拽
        /// </summary>
        /// <returns></returns>
        public virtual bool Hold() => XInput.GetMouseButton(0);

        /// <summary>
        /// 是否结束拖拽
        /// </summary>
        /// <returns></returns>
        public virtual bool Release() => XInput.GetMouseButtonUp(0);
    }
}
