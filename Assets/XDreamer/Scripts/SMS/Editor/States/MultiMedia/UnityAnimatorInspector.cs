using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.States.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.MultiMedia;
using XCSJ.Scripts;

namespace XCSJ.EditorSMS.States.MultiMedia
{
    public class UnityAnimatorInspector<T> : WorkClipInspector<T> where T : UnityAnimator<T>
    {
        public Animator animator => workClip.animator;

        public AnimatorController animatorController => animator ? animator.runtimeAnimatorController as AnimatorController : null;

        public int layerCount => animatorController ? animatorController.layers.Length : 0;

        public AnimatorControllerLayer currrentLayer => (animatorController && animatorController.layers != null && workClip.layerIndex >= 0 && workClip.layerIndex < layerCount) ? animatorController.layers[workClip.layerIndex] : null;

        public AnimatorStateMachine currrentStateMachine => currrentLayer == null ? null : currrentLayer.stateMachine;

        public List<string> motionStateNameList => currrentStateMachine ? currrentStateMachine.states.Where(s => s.state.motion).ToList(s => s.state.name) : new List<string>();

        public AnimatorState currentState => currrentStateMachine ? currrentStateMachine.states.FirstOrDefault(s => s.state.name == workClip.stateName).state : null;

        public Motion currentMotion => currentState ? currentState.motion : null;

        public AnimationClip currentAnimationClip => currentMotion as AnimationClip;

        protected virtual void OnAnimatorInfoChanged(SerializedProperty memberProperty) { }

        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(workClip.useInitData):
                    {
                        return false;
                    }
                case nameof(workClip.layerIndex):
                    {
                        var layerCount = this.layerCount;
                        EditorGUI.BeginChangeCheck();
                        memberProperty.intValue = EditorGUILayout.IntSlider(CommonFun.NameTooltip(workClip.GetType(), memberProperty.name), memberProperty.intValue, 0, (layerCount > 0 ? (layerCount - 1) : 0));
                        if (EditorGUI.EndChangeCheck())
                        {
                            OnAnimatorInfoChanged(memberProperty);
                        }
                        return false;
                    }
                case nameof(workClip.stateName):
                    {
                        var list = motionStateNameList;
                        list.Sort();

                        EditorGUI.BeginChangeCheck();
                        memberProperty.stringValue = UICommonFun.Popup(CommonFun.NameTooltip(workClip.GetType(), memberProperty.name), memberProperty.stringValue, list.ToArray(), GUILayout.Width(100));
                        if (EditorGUI.EndChangeCheck())
                        {
                            OnAnimatorInfoChanged(memberProperty);
                        }
                        return false;
                    }
            }
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }

        public static ModelImporterClipAnimation GetModelImporterClipAnimation(AnimationClip animationClip)
        {
            return animationClip ? GetModelImporterClipAnimation(GetAssetImporter(animationClip) as ModelImporter, animationClip.name) : null;
        }

        public static AssetImporter GetAssetImporter(AnimationClip animationClip)
        {
            return animationClip ? AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(animationClip)) : null;
        }

        public static ModelImporterClipAnimation GetModelImporterClipAnimation(ModelImporter modelImporter, string animationClipName)
        {
            if (modelImporter)
            {
                var clip = modelImporter.clipAnimations.FirstOrDefault(ca => ca.name == animationClipName);
                return clip != null ? clip : modelImporter.defaultClipAnimations.FirstOrDefault(ca => ca.name == animationClipName);
            }
            return null;
        }

        public static Vector2Int GetFrameRange(AnimationClip animationClip)
        {
            return GetFrameRange(GetModelImporterClipAnimation(animationClip));
        }

        public static Vector2Int GetFrameRange(ModelImporterClipAnimation modelImporterClipAnimation)
        {
            return modelImporterClipAnimation != null ? new Vector2Int(Mathf.RoundToInt(modelImporterClipAnimation.firstFrame), Mathf.RoundToInt(modelImporterClipAnimation.lastFrame)) : new Vector2Int();
        }

        #region 同步TL

        protected override GUIContent GetSyncTLButtonContent()
        {
            var content = base.GetSyncTLButtonContent();
            content.tooltip += string.Format("\n将时长同步为动画剪辑时长");
            return content;
        }

        protected override bool HasSyncTLButton() => true;

        protected override void OnSyncTL(SerializedProperty workRangeMemberProperty)
        {
            if (currentAnimationClip)
            {
                SetTLByWorkRange(workRangeMemberProperty, currentAnimationClip.length);
            }
        }

        #endregion

        #region 同步OTL

        protected override GUIContent GetSyncOTLButtonContent() => CommonFun.TempContent("动画时长", "将单次时长实时自动同步为动画剪辑时长");

        protected override void OnSyncOTL(SerializedProperty memberProperty)
        {
            if (workClip.syncOTL)
            {
                var cac = currentAnimationClip;
                if (cac) memberProperty.floatValue = cac.length;
            }
            else if (memberProperty.floatValue < 0) memberProperty.floatValue = 0;
        }

        #endregion

        public override StringBuilder GetHelpInfo()
        {
            var info = base.GetHelpInfo();
            info.Append("\nAnimator:\t");
            if (animator)
            {
                info.Append(CommonFun.GameObjectToStringWithoutAlias(animator.gameObject));
            }
            else
            {
                return info.Append("<color=red>数据无效!</color>"); ;
            }

            info.Append("\n动画控制器:\t");
            if (animatorController)
            {
                info.Append(AssetDatabase.GetAssetPath(animatorController));
            }
            else
            {
                return info.Append("<color=red>数据无效!</color>");
            }

            var cac = currentAnimationClip;
            info.Append("\n动画剪辑:");
            if (cac)
            {
                info.AppendFormat("\n\t名称:\t{0}", cac.name);
                info.AppendFormat("\n\t版本:\t{0}", (cac.legacy ? "Legacy" : "Mecanim"));

                var assetImporter = GetAssetImporter(cac);
                if (assetImporter)
                {
                    info.AppendFormat("\n\t路径:\t{0}", assetImporter.assetPath);
                    info.AppendFormat("\n\t导入器:\t{0}", assetImporter.GetType().ToString());
#if CSHARP_7_3_OR_NEWER
                    if (assetImporter is ModelImporter modelImporter)
                    {
#else
                    if (assetImporter is ModelImporter)
                    {
                        var modelImporter = (ModelImporter)assetImporter;
#endif
                        var clip = GetModelImporterClipAnimation(modelImporter, cac.name);
                        if (clip != null)
                        {
                            var range = GetFrameRange(clip);

                            info.AppendFormat("\n\t\t名称:\t{0}", clip.name);
                            info.AppendFormat("\n\t\tTake名:\t{0}", clip.takeName);
                            info.AppendFormat("\n\t\t帧区间:\t[{0}, {1}]", range.x, range.y);
                            info.AppendFormat("\n\t\t帧数:\t{0}", (range.y - range.x));
                        }
                        else
                        {
                            info.Append("\n\t\t<color=red>模型导入器无法识别有效的动画剪辑Take信息!</color>");
                        }
                    }
                    else
                    {
                        //可能是使用 Animation窗口 编辑的动画片段
                        var isAnim = assetImporter.assetPath.EndsWith(".anim", StringComparison.OrdinalIgnoreCase);
                        info.AppendFormat("\n\t\tAnim:\t{0}", isAnim ? "是" : "否");
                    }
                }
                else
                {
                    info.Append("\n\t<color=red>资源导入器无法识别有效的动画剪辑!</color>");
                }

                info.AppendFormat("\n\tFPS:\t{0}", cac.frameRate);
                info.AppendFormat("\n\t时长:\t{0}", cac.length);
            }
            else
            {
                info.AppendFormat("\n\t<color=red>数据无效!</color>");
            }

            return info;
        }
    }
}
