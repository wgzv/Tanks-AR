using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.CommonUtils.PluginCharacters;
using XCSJ.EditorCommonUtils;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;

namespace XCSJ.CommonUtils.EditorCharacters
{
    [CustomEditor(typeof(BaseGroundDetection), true)]
    public class BaseGroundDetectionInspector : BaseGroundDetectionInspector<BaseGroundDetection> { }

    public class BaseGroundDetectionInspector<T> : MBInspector<T> where T : BaseGroundDetection
    {
        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
            switch (memberProperty.name)
            {
                case "_" + nameof(BaseGroundDetection.stepOffset):
                case "_" + nameof(BaseGroundDetection.castDistance):
                    {
                        if (targetObject.capsuleCollider && !MathX.Approximately(memberProperty.floatValue, targetObject.capsuleCollider.radius))
                        {
                            if (GUILayout.Button(UICommonOption.Reset, EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                            {
                                memberProperty.floatValue = targetObject.capsuleCollider.radius;
                            }
                        }
                        break;
                    }
            }
        }
    }
}
