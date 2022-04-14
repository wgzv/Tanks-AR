using UnityEditor;
using UnityEngine;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO.NetSyncs;

namespace XCSJ.EditorMMO.NetSyncs
{
    [CustomEditor(typeof(NetChat), true)]
    public class NetChatInspector : NetMBInspector<NetChat>
    {
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            if (!mb.GetComponent<VoiceChat>())
            {
                if (GUILayout.Button(CommonFun.NameTip(typeof(VoiceChat))))
                {
                    mb.XAddComponent<VoiceChat>();
                }
            }
        }
    }
}
