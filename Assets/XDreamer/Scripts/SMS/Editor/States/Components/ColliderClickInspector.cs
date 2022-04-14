using System;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginSMS.States.Components;

namespace XCSJ.EditorSMS.States.Components
{
    [CustomEditor(typeof(ColliderClick))]
    public class ColliderClickInspector : StateComponentInspector<ColliderClick>
    {
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            var targetGO = stateComponent.go;
            if (targetGO)
            {
                if (!targetGO.GetComponent<Collider>() && !targetGO.GetComponentInChildren<MeshRenderer>())
                {
                    UICommonFun.RichHelpBox("当前游戏对象没有碰撞体与网格渲染器，将自动添加立方碰撞体", MessageType.Warning);
                }
            }
        }
    }
}
