using UnityEditor;
using XCSJ.PluginMMO.NetSyncs;

namespace XCSJ.EditorMMO.NetSyncs
{
    [CustomEditor(typeof(NetTransform), true)]
    public class NetTransformInspector: NetMBInspector<NetTransform>
    {
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();
        }
    }
}
