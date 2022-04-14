using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.States.Motions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.MultiMedia;
using XCSJ.PluginSMS.States.Motions;
using XCSJ.PluginSMS.Base;

namespace XCSJ.EditorSMS.States.MultiMedia
{
    [CustomEditor(typeof(PathAnimation))]
    public class PathAnimationInspector : PathInspector<PathAnimation>
    {
        public override StringBuilder GetHelpInfo()
        {
            var info = base.GetHelpInfo();

            var count = workClip.transforms.Count;

            var movePath = workClip.GetFullMovePath();
            info.Append("\n移动完美间距:");
            info.AppendFormat("\n\t百分比:\t{0}", workClip.GetPrettySpaceValue(count, PathAnimation.ESpaceType.Perent, movePath));
            info.AppendFormat("\n\t时间:\t{0}", workClip.GetPrettySpaceValue(count, PathAnimation.ESpaceType.Time, movePath));
            info.AppendFormat("\n\t距离:\t{0}", workClip.GetPrettySpaceValue(count, PathAnimation.ESpaceType.Distance, movePath));

            Vector3[] viewPath = null;
            switch (workClip.viewRule)
            {
                case EViewRule.MovePath:
                    {
                        viewPath = movePath;
                        break;
                    }
                case EViewRule.ViewPath:
                    {
                        viewPath = workClip.GetFullViewPath();
                        break;
                    }
                default: return info;
            }
            info.Append("\n视图完美间距:");
            info.AppendFormat("\n\t百分比:\t{0}", workClip.GetPrettySpaceValue(count, PathAnimation.ESpaceType.Perent, viewPath));
            info.AppendFormat("\n\t时间:\t{0}", workClip.GetPrettySpaceValue(count, PathAnimation.ESpaceType.Time, viewPath));
            info.AppendFormat("\n\t距离:\t{0}", workClip.GetPrettySpaceValue(count, PathAnimation.ESpaceType.Distance, viewPath));

            return info;
        }
    }
}
