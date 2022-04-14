using UnityEngine;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;

namespace XCSJ.PluginTools.SelectionUtils
{
    /// <summary>
    /// 选择集鼠标拖拽
    /// </summary>
    public class SelectionDragByMouse : DragByMouse
    {
        public RaycastHit rayHitInfo { get; private set; }

        /// <summary>
        /// 通过鼠标点击判断是否落在有效的选择集对象上, 进而判断拖拽有效性
        /// </summary>
        /// <returns></returns>
        public override bool Grab()
        {
            var cam = Camera.main ? Camera.main : Camera.current;
            if (!cam) return false;

            if (base.Grab() && Physics.Raycast(cam.ScreenPointToRay(XInput.mousePosition), out RaycastHit hitInfo) && hitInfo.transform)
            {
                rayHitInfo = hitInfo;
                foreach (var item in Selection.selections)
                {
                    if (item && (item == hitInfo.transform.gameObject || hitInfo.transform.IsChildOf(item.transform)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
