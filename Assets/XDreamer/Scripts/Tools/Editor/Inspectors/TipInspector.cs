using System;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginTools.Notes;

namespace XCSJ.EditorTools.Inspectors
{
    [CustomEditor(typeof(Tip))]
    [CanEditMultipleObjects]
    public class TipInspector : BaseInspectorWithLimit<Tip>
    {
        [InitializeOnLoadMethod]
        private static void Init()
        {
            Tip.onResetCallback += OnTipReset;
        }

        private static void OnTipReset(Tip tip)
        {
            if (tip && !tip.ugui)
            {
                ToolsMenu.CreateTip(tip);
            }
        }

        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(Tip.ugui):
                    {
                        if (GUILayout.Button(UICommonOption.Insert, UICommonOption.WH24x16))
                        {
                            ToolsMenu.CreateTip(targetObject);
                        }
                        break;
                    }
                default:
                    break;
            }
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }
    }

    [CustomEditor(typeof(ColliderTip))]
    [CanEditMultipleObjects]
    public class ColliderTipInspector : TipInspector
    {

    }

    [CustomEditor(typeof(UGUITip))]
    [CanEditMultipleObjects]
    public class UGUITipInspector : TipInspector
    {

    }
}
