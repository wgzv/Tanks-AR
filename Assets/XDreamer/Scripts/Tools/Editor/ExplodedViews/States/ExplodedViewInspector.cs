using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.States.Base;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginTools.ExplodedViews.States;

namespace XCSJ.EditorTools.ExplodedViews.States
{
    [Name("爆炸图检查器")]
    [CustomEditor(typeof(ExplodedView))]
    public class ExplodedViewInspector : WorkClipInspector<ExplodedView>
    {
        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(WorkClip.useInitData):
                    {
                        return false;
                    }
            }
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }
    }
}
