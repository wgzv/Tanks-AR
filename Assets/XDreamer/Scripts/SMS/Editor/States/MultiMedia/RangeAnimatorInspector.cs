using System;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.MultiMedia;

namespace XCSJ.EditorSMS.States.MultiMedia
{
    [CustomEditor(typeof(RangeAnimator))]
    public class RangeAnimatorInspector : UnityAnimatorInspector<RangeAnimator>
    {
        private bool updatePlayRange = false;

        protected override void OnAnimatorInfoChanged(SerializedProperty memberProperty)
        {
            UICommonFun.DelayCall(0.01f, this, o =>
            {
                if (o is RangeAnimatorInspector inspector && inspector)
                {
                    inspector.updatePlayRange = true;
                }
            });
            base.OnAnimatorInfoChanged(memberProperty);
        }

        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(workClip.takeRange):
                    {
                        memberProperty.vector2IntValue = GetFrameRange(currentAnimationClip);
                        EditorGUI.BeginDisabledGroup(true);
                        UICommonFun.MinMaxSliderLayout(CommonFun.NameTooltip(workClip, memberProperty.name), memberProperty.vector2IntValue, memberProperty.vector2IntValue);
                        EditorGUI.EndDisabledGroup();
                        return false;
                    }
                case nameof(workClip.playRange):
                    {
                        memberProperty.vector2IntValue = UICommonFun.MinMaxSliderLayout(CommonFun.NameTooltip(workClip, memberProperty.name), memberProperty.vector2IntValue, workClip.takeRange);

                        //当前状态组件内Animator属性信息修改时，自动更新播放区间为Take区间
                        if (updatePlayRange && Event.current.type == EventType.Repaint)
                        {
                            updatePlayRange = false;
                            memberProperty.vector2IntValue = workClip.takeRange;
                        }
                        return false;
                    }
            }
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }

        public override bool OnModifiedPropertyField(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(workClip.animator):
                case nameof(workClip.layerIndex):
                case nameof(workClip.stateName):
                    {
                        OnAnimatorInfoChanged(memberProperty);
                        break;
                    }
            }
            return base.OnModifiedPropertyField(type, memberProperty, arrayElementInfo);
        }

        #region 同步TL

        protected override GUIContent GetSyncTLButtonContent()
        {
            var content = CommonFun.NameTooltip(workClip, nameof(workClip.syncTL), ENameTip.EmptyTextWhenHasImage);
            content.tooltip += string.Format("\n将时长同步为播放区间帧时长");
            return content;
        }

        protected override void OnSyncTL(SerializedProperty workRangeMemberProperty)
        {
            var cac = currentAnimationClip;
            if (GetAssetImporter(cac) is ModelImporter modelImporter)
            {
                var pl = workClip.playRange.y - workClip.playRange.x;
                var tl = pl * cac.length / (workClip.takeRange.y - workClip.takeRange.x);

                SetTLByWorkRange(workRangeMemberProperty, tl);
            }
        }

        #endregion

        public override StringBuilder GetHelpInfo()
        {
            var info = base.GetHelpInfo();

            var pl = workClip.playRange.y - workClip.playRange.x;
            info.AppendFormat("\n播放区间帧长:\t{0}", pl.ToString());
            info.Append("\n播放区间帧时长:\t");

            var cac = currentAnimationClip;
            if (GetAssetImporter(cac) is ModelImporter modelImporter)
            {               
                info.Append((pl * cac.length / (workClip.takeRange.y - workClip.takeRange.x)).ToString());
            }
            else
            {
                info.Append("<color=red>区间动画无法处理当前类型的动画剪辑!</color>");
            }
            return info;
        }
    }
}
