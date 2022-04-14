using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorCommonUtils.Base.Controls;
using XCSJ.PluginMMO;

namespace XCSJ.EditorMMO
{
    /// <summary>
    /// 网络标识检查器
    /// </summary>
    [CustomEditor(typeof(NetIdentity))]
    public class NetIdentityInspector : MMOMBInspector<NetIdentity>
    {
        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();
            if (Application.isPlaying) return;
            categoryList.DrawVertical();
        }        
    }
}
