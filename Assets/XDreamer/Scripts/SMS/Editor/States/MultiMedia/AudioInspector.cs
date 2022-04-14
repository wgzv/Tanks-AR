using System;
using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.EditorSMS.NodeKit;
using XCSJ.EditorSMS.States.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.MultiMedia;

namespace XCSJ.EditorSMS.States.MultiMedia
{
    [CustomEditor(typeof(Audio))]
    public class AudioInspector : WorkClipInspector<Audio>
    {
        #region 同步TL

        protected override GUIContent GetSyncTLButtonContent()
        {
            var content = base.GetSyncTLButtonContent();
            content.tooltip += string.Format("\n将时长同步为音频时长");
            return content;
        }

        protected override bool HasSyncTLButton() => true;

        protected override void OnSyncTL(SerializedProperty workRangeMemberProperty)
        {
            if (workClip && workClip.audioSource && workClip.audioSource.clip)
            {
                SetTLByWorkRange(workRangeMemberProperty, workClip.audioSource.clip.length);
            }
        }

        #endregion

        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(Audio.audioSource):
                    {
                        EditorGUI.BeginDisabledGroup(stateComponent.audioSource);
                        if (GUILayout.Button(CommonFun.NameTip(EIcon.Add), EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                        {
                            memberProperty.objectReferenceValue = CreateAudioSource();
                        }
                        EditorGUI.EndDisabledGroup();
                        break;
                    }
                default:
                    break;
            }
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }

        private const string AudioSourceRootName = "音频源目录";
        private const string AudioSourceName = "音频源";

        private AudioSource CreateAudioSource()
        {
            var root = GameObject.Find(AudioSourceRootName);
            if(!root)
            {
                root = new GameObject(AudioSourceRootName);
                UndoHelper.RegisterCreatedObjectUndo(root);
            }

            var go = new GameObject(GameObjectUtility.GetUniqueNameForSibling(root.transform, AudioSourceName));
            UndoHelper.RegisterCreatedObjectUndo(go);
            UndoHelper.RecordSetTransformParent(go.transform, root.transform);

            var audioSource = Undo.AddComponent(go, typeof(AudioSource)) as AudioSource;

            EditorGUIUtility.PingObject(go);

            return audioSource;
        }

        public override StringBuilder GetHelpInfo()
        {
            var info = base.GetHelpInfo();
            var workClip = this.workClip;
            if (!workClip) return info;

            info.Append("\n音频源:\t");
            if (workClip.audioSource)
            {
                info.Append(CommonFun.GameObjectToStringWithoutAlias(workClip.audioSource.gameObject));
            }
            else
            {
                return info.AppendFormat("<color=red>数据无效</color>");
            }


            info.Append("\n音频剪辑:\t");
            if (workClip.audioSource.clip)
            {
                info.Append(AssetDatabase.GetAssetPath(workClip.audioSource.clip));
            }
            else
            {
                return info.Append("<color=red>数据无效</color>");
            }

            return info.AppendFormat("\n音频时长:\t{0} 秒\n高音:\t{1}", workClip.audioSource.clip.length, workClip.audioSource.pitch);
        }
    }
}
