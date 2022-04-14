using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO;

namespace XCSJ.EditorMMO
{
    [CustomEditor(typeof(MMOPlayerCreater))]
    public class MMOPlayerCreaterInspector:MBInspector<MMOPlayerCreater>
    {
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            if (!mb.GetComponent<MMOPlayerCreaterHUD>())
            {
                if (GUILayout.Button(CommonFun.NameTip(typeof(MMOPlayerCreaterHUD))))
                {
                    Undo.AddComponent<MMOPlayerCreaterHUD>(mb.gameObject);
                }
            }
        }
    }
}
