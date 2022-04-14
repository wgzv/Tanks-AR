using System;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCamera;

namespace XCSJ.PluginsCameras.Legacies.Inspectors
{
    /// <summary>
    /// 相机背景幕布检查器
    /// </summary>
    [CustomEditor(typeof(Backdrop))]
    [Obsolete("产品功能升级，不推荐用户再使用本功能组件，请使用新的功能组件替代！", false)]
    public class BackdropInspector : BaseInspectorWithLimit<Backdrop>
    {
        public override bool OnModifiedPropertyField(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            if (memberProperty.name == "image")
            {
                targetObject.SetImage(memberProperty.objectReferenceValue as Texture2D);
            }
            return base.OnModifiedPropertyField(type, memberProperty, arrayElementInfo);
        }
    }
}
