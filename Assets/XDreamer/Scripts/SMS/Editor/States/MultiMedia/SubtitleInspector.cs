using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.States.Base;
using XCSJ.EditorXGUI;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.MultiMedia;

namespace XCSJ.EditorSMS.States.MultiMedia
{
    [CustomEditor(typeof(Subtitle))]
    public class SubtitleInspector : WorkClipInspector<Subtitle>
    {
        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch(memberProperty.name)
            {
                case nameof(Subtitle.subtitleClips):
                    {
                        if(arrayElementInfo.index >= 0)
                        {
                            var time = arrayElementInfo.serializedProperty.FindPropertyRelative(nameof(SubtitleClip.time));
                            var text = arrayElementInfo.serializedProperty.FindPropertyRelative(nameof(SubtitleClip.text));
                            UICommonFun.BeginHorizontal(arrayElementInfo.index);
                            EditorGUILayout.LabelField((arrayElementInfo.index + 1).ToString(), GUILayout.Width(30));
                            time.floatValue = EditorGUILayout.FloatField(time.floatValue, GUILayout.Width(60));
                            if (time.floatValue > stateComponent.timeLength) time.floatValue = stateComponent.timeLength;
                            text.stringValue = EditorGUILayout.TextField(text.stringValue);

                            EditorGUI.BeginDisabledGroup(arrayElementInfo.index <= 0);
                            if (GUILayout.Button(UICommonOption.MovePrevious, EditorStyles.miniButtonLeft, UICommonOption.WH24x16))
                            {
                                GUI.FocusControl("");
                                memberProperty.MoveArrayElement(arrayElementInfo.index, arrayElementInfo.index - 1);
                            }
                            EditorGUI.EndDisabledGroup();

                            EditorGUI.BeginDisabledGroup(arrayElementInfo.index >= (memberProperty.arraySize - 1));
                            if (GUILayout.Button(UICommonOption.MoveNext, EditorStyles.miniButtonMid, UICommonOption.WH24x16))
                            {
                                GUI.FocusControl("");
                                memberProperty.MoveArrayElement(arrayElementInfo.index, arrayElementInfo.index + 1);
                            }
                            EditorGUI.EndDisabledGroup();

                            if (GUILayout.Button(UICommonOption.Insert, EditorStyles.miniButtonMid, UICommonOption.WH24x16))
                            {
                                GUI.FocusControl("");
                                memberProperty.InsertArrayElementAtIndex(arrayElementInfo.index);
                            }

                            if (GUILayout.Button(UICommonOption.Delete, EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                            {
                                GUI.FocusControl("");

                                UICommonFun.DeleteArrayElementAtIndex(memberProperty, arrayElementInfo.serializedProperty, arrayElementInfo.index);
                            }

                            UICommonFun.EndHorizontal();
                            return false;
                        }
                        break;
                    }
            }
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }

        public override void OnBeforeArrayVertical(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(Subtitle.subtitleClips):
                    {

                        UICommonFun.EnumButton<ESubtitleSortRule>(sr => SortssSubtitle(sr, memberProperty), true, true, null, null, null, null, ENameTip.Image, GUILayout.ExpandWidth(true), GUILayout.Height(20));
                        break;
                    }
            }

            base.OnBeforeArrayVertical(type, memberProperty, arrayElementInfo);
        }

        public override void OnBeforeArrayElementVertical(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(Subtitle.subtitleClips):
                    {
                        EditorGUILayout.BeginHorizontal("box");
                        GUILayout.Label("NO.", GUILayout.Width(30));
                        GUILayout.Label("时间", GUILayout.Width(50));
                        GUILayout.Label("字幕");
                        GUILayout.Label("操作", GUILayout.Width(100));
                        EditorGUILayout.EndHorizontal();
                        break;
                    }
            }
            base.OnBeforeArrayElementVertical(type, memberProperty, arrayElementInfo);
        }

        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch(memberProperty.name)
            {
                case nameof(Subtitle.subtitleText):
                    {
                        EditorXGUIHelper.DrawCreateButton(stateComponent.subtitleText, () =>
                        {
                            ToolsMenu.CreateUIInCanvas(() =>
                            {
                                var text = ToolsMenu.CreateUIWithStyle<Text>();
                                memberProperty.objectReferenceValue = text;
                                return text.gameObject;
                            });
                        });
                        break;
                    }
            }
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }

        void SortssSubtitle(ESubtitleSortRule eSubtitleSortRule, SerializedProperty memberProperty)
        {
            switch(eSubtitleSortRule)
            {
                case ESubtitleSortRule.Time:
                    {
                        SerializedObjectHelper.ArrayElementSort(memberProperty, (x,y)=>stateComponent.subtitleClips[x.index].CompareTo(stateComponent.subtitleClips[y.index]));
                        break;
                    }
                case ESubtitleSortRule.Reverse:
                    {
                        SerializedObjectHelper.ArrayElementReverse(memberProperty);
                        break;
                    }
            }
        }
    }

    [Name("排序规则")]
    [Tip("对象列表内的元素进行排序")]
    public enum ESubtitleSortRule
    {
        [Name("时间")]
        [XCSJ.Attributes.Icon(EIcon.NameAscendingOrder)]
        Time,

        [Name("逆序")]
        [XCSJ.Attributes.Icon(EIcon.ReverseOrder)]
        Reverse,
    }
}
