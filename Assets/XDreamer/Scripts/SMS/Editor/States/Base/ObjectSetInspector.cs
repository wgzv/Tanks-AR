using System;
using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.Tools;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.EditorSMS.States.Base
{
    public class ObjectSetInspector<T> : StateComponentInspector<T> where T : StateComponent, IObjectSet
    {
        internal const string ObjectsString = "_" + nameof(IObjectSet.objects);

        public override void OnBeforeArraySizeVertical(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case ObjectsString:
                    {
                        EditorSerializedObjectHelper.DrawUnityObjectArrayHandleRule(memberProperty);
                        break;
                    }
            }
            base.OnBeforeArraySizeVertical(type, memberProperty, arrayElementInfo);
        }

        public override void OnDrawHelpInfo()
        {
            //base.OnDrawHelpInfo();
        }
    }
}
