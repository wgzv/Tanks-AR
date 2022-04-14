using System;
using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO.NetSyncs;

namespace XCSJ.EditorMMO.NetSyncs
{
    [CustomEditor(typeof(NetPlayer), true)]
    public class NetPlayerInspector : NetPropertyInspector<NetPlayer>
    {
        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(NetProperty.propertys):
                    {
                        EditorGUI.BeginChangeCheck();
                        var nickName = EditorGUILayout.DelayedTextField(CommonFun.NameTip(mb.GetType(), nameof(NetPlayer.nickName)), mb.nickName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            mb.nickName = nickName;
                        }
                        break;
                    }
            }
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }
    }
}
