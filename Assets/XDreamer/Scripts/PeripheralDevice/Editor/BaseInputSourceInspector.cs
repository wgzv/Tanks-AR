using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginPeripheralDevice;
using XCSJ.PluginPeripheralDevice.Standalones;

namespace XCSJ.EditorPeripheralDevice
{
    public class BaseInputSourceInspector<T> : BaseInspectorWithLimit<T> where T : BaseInputSource
    {
        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case "_showRay":
                case "_canvas":
                case "_origin":
                case "_isCamera":
                case "_eventCamera":
                    {
                        if (targetObject.useDefinedRay)
                            return true;
                        return false;
                    }

            }
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }
    }

    [CustomEditor(typeof(StandaloneInputSource))]
    public class StandaloneInputSourceInspector : BaseInputSourceInspector<StandaloneInputSource>
    {
        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }
    }

}
